using Brigadier.NET.Suggestion;
using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.Debug {
	public sealed class CommandLine : IDisposable {
		private VisualElement root;
		private TextField textField;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly CommandCompletion completionQueue = new();
		private readonly SoulboundClient.DebugOverlayManager debugOverlayManager;
		private CommandInputMode currentInputMode;
		private int historyIndex;
		private bool eligibleForHistoryCycling;
		private bool hasEnteredText;

		public CommandLine(CommandProcessor commandProcessor, SoulboundClient.DebugOverlayManager debugOverlayManager) {
			this.debugOverlayManager = debugOverlayManager;
			this.commandProcessor = commandProcessor;
		}

		public bool isVisible { get; private set; }

		public void OnBind(VisualElement root) {
			this.root = root;

			this.textField = root.Q<TextField>("TextField");
			this.textField.RegisterValueChangedCallback(this.TextChanged);
			this.RegisterCaretChanged((caret) => {
				string command = this.textField.value;
				this.ShowCompletions(command, caret);
			});
			this.textField.RegisterCallback<KeyDownEvent>(this.HandleKeyEvent, TrickleDown.TrickleDown);
		}

		private void RegisterCaretChanged(Action<int> callback) {
			int lastCursor = this.textField.cursorIndex;
			
			void CheckCaret() {
				if (this.textField.cursorIndex == lastCursor) return;

				lastCursor = this.textField.cursorIndex;
				callback(lastCursor);
			}
			this.textField.RegisterCallback<KeyDownEvent>(_ => this.textField.schedule.Execute(CheckCaret));
			this.textField.RegisterCallback<PointerUpEvent>(_ => this.textField.schedule.Execute(CheckCaret));
			this.textField.RegisterCallback<FocusInEvent>(_ => this.textField.schedule.Execute(CheckCaret));
			this.textField.RegisterCallback<ChangeEvent<string>>(_ => this.textField.schedule.Execute(CheckCaret));
		}

		public void Show() {
			if (this.isVisible) return;
			this.isVisible = true;

			this.root.style.display = DisplayStyle.Flex;
			this.textField.value = "/";

			this.textField.schedule.Execute(() => {
				this.textField.Focus();
				this.SetCaretToEnd();
			});
		}

		public void Hide() {
			if (!this.isVisible) return;
			this.isVisible = false;

			this.root.style.display = DisplayStyle.None;
			this.textField.value = "/";
			this.debugOverlayManager.Hide(SoulboundClient.DebugOverlayFeature.CommandLine);
		}

		private void HandleKeyEvent(KeyDownEvent evt) => this.HandleKey(evt.keyCode);

		private bool HandleKey(KeyCode key) {
			if (!this.isVisible) return false;

			if (key == KeyCode.Escape) {
				this.Hide();
				return true;
			}

			if (key == KeyCode.Return) {
				string command = this.textField.value;
				this.SubmitCommand(command);
				this.Hide();
				return true;
			}

			if (key != KeyCode.Tab && key != KeyCode.UpArrow && key != KeyCode.DownArrow) { 
				this.currentInputMode = CommandInputMode.Typing; 
			}

			switch (this.currentInputMode) {
				case CommandInputMode.Typing: return this.HandleTyping(key);
				case CommandInputMode.CyclingCompletions: return this.HandleCompletion(key);
				case CommandInputMode.CyclingHistory: return this.HandleHistory(key);
				default: break;
			}

			return false;
		}

		private bool HandleTyping(KeyCode key) {
			if ((key == KeyCode.Tab || key == KeyCode.UpArrow || key == KeyCode.DownArrow)
					&& this.completionQueue.GetCompletionCount() > 0) {
				this.currentInputMode = CommandInputMode.CyclingCompletions;
				this.HandleCompletion(key);
				return true;
			} else if ((key == KeyCode.UpArrow || key == KeyCode.DownArrow) && this.history.Any() && this.eligibleForHistoryCycling) {
				this.currentInputMode = CommandInputMode.CyclingHistory;
				this.HandleHistory(key);
				return true;
			}
			return false;
		}

		private bool HandleCompletion(KeyCode key) {
			if (key == KeyCode.DownArrow) {
				this.HighlightCompletion(this.completionQueue.SelectNext());
				return true;
			} else if (key == KeyCode.UpArrow) {
				this.HighlightCompletion(this.completionQueue.SelectPrevious());
				return true;
			} else if (key == KeyCode.Tab) {
				this.InsertCompletion();
				return true;
			}
			return false;
		}

		private bool HandleHistory(KeyCode key) {
			if (key == KeyCode.UpArrow) {
				this.historyIndex--;
				if (this.historyIndex < 0) this.historyIndex = this.history.Count - 1;
				this.InsertHistory();
				return true;
			} else if (key == KeyCode.DownArrow) {
				this.historyIndex = (this.historyIndex + 1) % this.history.Count;
				this.InsertHistory();
				return true;
			}
			return false;
		}

		private void TextChanged(ChangeEvent<string> evt) {
			string value = evt.newValue;

			if (!this.hasEnteredText) this.hasEnteredText = value.Length > 1;
			if (value.Equals("")) this.hasEnteredText = false;

			this.eligibleForHistoryCycling = !this.hasEnteredText || this.currentInputMode == CommandInputMode.CyclingHistory;
			if (!this.eligibleForHistoryCycling) this.currentInputMode = CommandInputMode.Typing;
		}

		private void SubmitCommand(string command) {
			this.commandProcessor.SubmitCommand(command);
			this.history.Add(command);
		}

		public void ShowCompletions(string value, int caretPos) {
			this.commandProcessor.GetCompletions(value, caretPos)
				.ContinueWith(suggestions => {
					this.completionQueue.SetCompletions(suggestions.List);
				})
			.Forget(e => {
				this.completionQueue.SetCompletions(Array.Empty<Suggestion>().ToList());
				Logger.LogFatal(e);
			});


			//commandProcessor.GetCompletions(value, caretPos)
			//	.ContinueWith(suggestions => {
			//		if (suggestions.List.Any()) {
			//			completionPanel.gameObject.SetActive(true);
			//			foreach (var component in completionPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
			//				component.gameObject.SetActive(false);
			//			}
			//		} else completionPanel.gameObject.SetActive(false);

			//		currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
			//		completionQueue.SetCompletions(suggestions.List);

			//		int i = 0;
			//		for (; i < currentCompletions.Length && i < suggestions.List.Count; i++) {
			//			currentCompletions[i].gameObject.SetActive(true);
			//			currentCompletions[i].text = suggestions.List[i].Text;
			//			currentCompletions[i].Rebuild(CanvasUpdate.LatePreRender);
			//		}

			//		for (; i < suggestions.List.Count; i++) {
			//			CreateCompletionComponent(suggestions.List[i].Text);
			//		}
			//		currentCompletions = completionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
			//		HighlightCompletion(completionQueue.GetSelectedIndex());
			//	}).Forget(e => {
			//		completionQueue.SetCompletions(Array.Empty<Suggestion>().ToList());
			//		completionPanel.gameObject.SetActive(false);
			//		Logger.LogFatal(e);
			//	});

		}

		private void InsertCompletion() {
			if (this.completionQueue.GetCompletionCount() == 0) return;

			Suggestion suggestion = this.completionQueue.GetSelected();

			string withoutLeadingSlash = this.textField.value[1..];
			this.textField.value = $"/{suggestion.Apply(withoutLeadingSlash)}";

			int newCaret = suggestion.Range.Start + 1 + suggestion.Text.Length;
			this.textField.cursorIndex = newCaret;
			this.textField.selectIndex = newCaret;
		}

		private void HighlightCompletion(int index) {
			//if (InvalidCompletionPanel()) return;
			//for (int i = 0; i < currentCompletions.Length; i++) {
			//	RevokeSelectedLayout(currentCompletions[i]);
			//}
			//if (index != -1) ApplySelectedLayout(currentCompletions[index]);
			//RebuildPanelLayout();
		}

		private void InsertHistory() {
			this.textField.value = this.history[this.historyIndex];
			this.SetCaretToEnd();
		}

		private void SetCaretToEnd() {
			int end = this.textField.value.Length;
			this.textField.cursorIndex = end;
			this.textField.selectIndex = end;
		}

		public void Dispose() {
			this.textField.UnregisterValueChangedCallback(this.TextChanged);
		}
	}
}
