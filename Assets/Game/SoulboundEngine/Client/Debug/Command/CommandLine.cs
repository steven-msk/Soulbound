using Brigadier.NET.Suggestion;
using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.Debug {
	public sealed class CommandLine : UxmlWidget, IDisposable {
		private TextField textField;
		private ListView completionList;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly CompletionManager completionManager = new();
		private readonly SoulboundClient.DebugOverlayManager debugOverlayManager;
		private CommandInputMode currentInputMode;
		private int historyIndex;

		public CommandLine(CommandProcessor commandProcessor, SoulboundClient.DebugOverlayManager debugOverlayManager) {
			this.debugOverlayManager = debugOverlayManager;
			this.commandProcessor = commandProcessor;
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);

			this.textField = root.Q<TextField>("TextField");
			this.RegisterCaretChanged((caret) => {
				string command = this.textField.value;
				this.ShowCompletions(command, caret);
			});
			this.textField.RegisterCallback<KeyDownEvent>(this.HandleKeyEvent, TrickleDown.TrickleDown);

			this.completionList = root.Q<ListView>("CompletionList");
			this.completionList.bindItem = (element, index) => {
				Suggestion suggestion = this.completionManager.Get(index);
				element.Q<Label>("SuggestionText").text = suggestion.Text;
			};
			this.completionList.makeNoneElement = () => new VisualElement();
			this.completionList.itemsChosen += this.OnCompletionChosen;
		}

		private void RegisterCaretChanged(Action<int> callback) {
			int lastCursor = this.textField.cursorIndex;

			void CheckCaret() {
				if (!this.isVisible) return;
				if (this.textField.cursorIndex == lastCursor) return;

				lastCursor = this.textField.cursorIndex;
				callback(lastCursor);
			}
			this.textField.RegisterCallback<KeyDownEvent>(_ => this.textField.schedule.Execute(CheckCaret), TrickleDown.TrickleDown);
			this.textField.RegisterCallback<PointerUpEvent>(_ => this.textField.schedule.Execute(CheckCaret), TrickleDown.TrickleDown);
			this.textField.RegisterCallback<FocusInEvent>(_ => this.textField.schedule.Execute(CheckCaret), TrickleDown.TrickleDown);
			this.textField.RegisterCallback<ChangeEvent<string>>(_ => this.textField.schedule.Execute(CheckCaret), TrickleDown.TrickleDown);
		}

		public override void Show() {
			base.Show();
			this.textField.value = "/";

			this.GrabFocus();
			this.SetCaretToEnd();
			this.currentInputMode = this.history.Any()
				? CommandInputMode.CyclingHistory
				: CommandInputMode.Typing;
		}

		public override void Hide() {
			base.Hide();
			this.textField.value = "/";
			this.debugOverlayManager.Hide(SoulboundClient.DebugOverlayFeature.CommandLine);
		}

		private void HandleKeyEvent(KeyDownEvent evt) {
			if (evt.keyCode is KeyCode.UpArrow or KeyCode.DownArrow or KeyCode.Tab or KeyCode.Return or KeyCode.KeypadEnter) {
				evt.StopImmediatePropagation();
			}
			this.HandleKey(evt.keyCode);
		}

		private void HandleKey(KeyCode key) {
			if (!this.isVisible) return;

			if (key is KeyCode.Escape) {
				this.Hide();
				return;
			}

			if (key is KeyCode.Return or KeyCode.KeypadEnter) {
				string command = this.textField.value;
				this.SubmitCommand(command);
				this.Hide();
				return;
			}

			if (key is not (KeyCode.UpArrow or KeyCode.DownArrow)) { 
				this.currentInputMode = CommandInputMode.Typing;
			}

			switch (this.currentInputMode) {
				case CommandInputMode.Typing: 
					this.HandleTyping(key);
					break;
				case CommandInputMode.CyclingCompletions: 
					this.HandleCompletion(key); 
					break;
				case CommandInputMode.CyclingHistory:
					this.HandleHistory(key);
					break;
				default:
					break;
			}
		}

		private void HandleTyping(KeyCode key) {
			if ((key is KeyCode.Tab or KeyCode.UpArrow or KeyCode.DownArrow)
					&& this.completionManager.GetCompletionCount() > 0) {
				this.currentInputMode = CommandInputMode.CyclingCompletions;
				this.HandleCompletion(key);
			}
		}

		private void HandleCompletion(KeyCode key) {
			if (key is KeyCode.DownArrow) {
				this.HighlightCompletion(this.completionManager.SelectNext());
			} else if (key is KeyCode.UpArrow) {
				this.HighlightCompletion(this.completionManager.SelectPrevious());
			} else if (key is KeyCode.Tab) {
				this.InsertCompletion(this.completionManager.GetSelected());
			}
		}

		private void HandleHistory(KeyCode key) {
			if (key is KeyCode.UpArrow) {
				this.historyIndex--;
				if (this.historyIndex < 0) this.historyIndex = this.history.Count - 1;
				this.InsertHistory();
			} else if (key is KeyCode.DownArrow) {
				this.historyIndex = (this.historyIndex + 1) % this.history.Count;
				this.InsertHistory();
			}
		}

		private void SubmitCommand(string command) {
			this.commandProcessor.SubmitCommand(command);
			this.history.Add(command);
		}

		public void ShowCompletions(string value, int caretPos) {
			this.commandProcessor.GetCompletions(value, caretPos)
				.ContinueWith(suggestions => {
					this.completionManager.SetCompletions(suggestions.List);
					this.completionList.itemsSource = suggestions.List;
					this.completionList.RefreshItems();
					this.HighlightCompletion(0);
				})
			.Forget(e => {
				this.completionManager.SetCompletions(Array.Empty<Suggestion>().ToList());
				Logger.LogFatal(e);
			});
		}

		private void OnCompletionChosen(IEnumerable<object> objects) {
			this.InsertCompletion((Suggestion)objects.First());
		}

		private void InsertCompletion(Suggestion suggestion) {
			string withoutLeadingSlash = this.textField.value[1..];
			string completed = $"/{suggestion.Apply(withoutLeadingSlash)}";
			this.textField.SetValueWithoutNotify(completed);

			int newCaret = suggestion.Range.Start + 1 + suggestion.Text.Length;
			this.textField.schedule.Execute(() => {
				this.textField.Focus();
				this.textField.selectIndex = newCaret;
				this.textField.cursorIndex = newCaret;
			});
		}

		private void HighlightCompletion(int index) {
			this.completionList.selectedIndex = index;
			this.completionList.ScrollToItem(index);
		}

		private void InsertHistory() {
			this.textField.value = this.history[this.historyIndex];
			this.SetCaretToEnd();
		}

		private void GrabFocus() {
			this.textField.schedule.Execute(this.textField.Focus);
		}

		private void SetCaretToEnd() {
			int end = this.textField.value.Length;
			this.textField.cursorIndex = end;
			this.textField.selectIndex = end;
		}

		public void Dispose() {
			this.textField.UnregisterCallback<KeyDownEvent>(this.HandleKeyEvent, TrickleDown.TrickleDown);
			this.completionList.itemsChosen -= this.OnCompletionChosen;
		}
	}
}
