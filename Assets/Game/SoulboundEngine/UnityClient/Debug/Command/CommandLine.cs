namespace SoulboundEngine.UnityClient.Debug {
	using Brigadier.NET.Suggestion;
	using Cysharp.Threading.Tasks;
	using SoulboundEngine.Registry;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Debug.Commands;
	using SoulboundEngine.UnityClient.Settings;
	using SoulboundEngine.UnityClient.UI;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using UnityEngine.UIElements;
	using Keyboard = Input.Keyboard;

	public sealed class CommandLine : UXMLWidget, IInputFocusable {
		private static readonly Identifier TEXT_FIELD_ELEMENT = Identifier.Of("soulbound:command_line/text_field");
		private static readonly Identifier COMPLETION_LIST_ELEMENT = Identifier.Of("soulbound:command_line/completion_list");
		private static readonly Identifier SUGGESTION_TEXT_ELEMENT = Identifier.Of("soulbound:command_suggestion/suggestion_text");
		private TextField textField;
		private ListView completionList;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly CompletionManager completionManager = new();
		private readonly Keyboard keyboard;
		private readonly SoulboundUnityClient client;
		private int historyIndex;
		private bool isCyclingHistory;
		private bool isCyclingCompletions;
		private bool hasEdited;
		private int lastKnownCaretPos;
		private static readonly HashSet<KeyCode> HANDLED_KEYS = new() {
			KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Tab, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Escape
		};

		public CommandLine(CommandProcessor commandProcessor, SoulboundUnityClient client) {
			this.client = client;
			this.commandProcessor = commandProcessor;
			this.keyboard = client.InputManager.keyboard;
		}

		public static void CreateRoot(VisualElement parent) {
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("CommandLine"));
			asset.CloneTree(parent);
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);
			this.textField = root.Get<TextField>(TEXT_FIELD_ELEMENT);
			this.completionList = root.Get<ListView>(COMPLETION_LIST_ELEMENT);
			this.completionList.bindItem = (element, index) => {
				Suggestion suggestion = this.completionManager.Get(index);
				element.Get<Label>(SUGGESTION_TEXT_ELEMENT).text = suggestion.Text;
			};
			this.textField.RegisterCallback<KeyDownEvent>(this.InterceptHandledKeys, TrickleDown.TrickleDown);
			this.completionList.makeNoneElement = () => new VisualElement();
			this.completionList.itemsChosen += this.OnCompletionChosen;
			this.Hide();
		}

		private void InterceptHandledKeys(KeyDownEvent evt) {
			if (!this.isVisible) return;
			if (HANDLED_KEYS.Contains(evt.keyCode)) {
				evt.StopImmediatePropagation();
			}
		}

		internal void Tick() {
			if (!this.isVisible && GameSettings.keybinds.enterCommand.WasPressed()) {
				this.Show();
			}
			
			this.HandleKeyInput();
			if (this.isVisible) {
				if (this.CheckCaret()) this.CaretChanged(this.GetCurrentCaret());
			}
		}

		private void HandleKeyInput() {
			if (!this.isVisible) return;
			if (this.ShouldCloseOnEsc() && this.keyboard.WasPressed(Keyboard.GetControl(Key.Escape))) {
				this.Hide();
				return;
			}
			if (this.keyboard.WasPressed(Keyboard.GetControl(Key.Enter))) {
				this.SubmitCommand(this.GetCommand());
				this.Hide();
				return;
			}

			if (this.CanCycleHistory() && !this.isCyclingCompletions) {
				if (this.keyboard.WasPressed(Keyboard.GetControl(Key.UpArrow))) {
					this.isCyclingHistory = true;
					this.historyIndex--;
					if (this.historyIndex < 0) this.historyIndex = this.history.Count - 1;
					this.OverwriteWithHistory(this.historyIndex);
				} else if (this.isCyclingHistory && this.keyboard.WasPressed(Keyboard.GetControl(Key.DownArrow))) {
					this.historyIndex = (this.historyIndex + 1) % this.history.Count;
					this.OverwriteWithHistory(this.historyIndex);
				}
			}

			if (this.CanCycleCompletions() || this.isCyclingCompletions) {
				if (this.keyboard.WasPressed(Keyboard.GetControl(Key.UpArrow))) {
					this.isCyclingCompletions = true;
					this.HighlightCompletion(this.completionManager.SelectPrevious());
				} else if (this.keyboard.WasPressed(Keyboard.GetControl(Key.DownArrow))) {
					this.isCyclingCompletions = true;
					this.HighlightCompletion(this.completionManager.SelectNext());
				}
				if (this.keyboard.WasPressed(Keyboard.GetControl(Key.Escape))) {
					this.ClearAndDisableCompletions();
				}
				if (this.completionManager.TryGetSelected(out Suggestion suggestion)) {
					if (this.keyboard.WasPressed(Keyboard.GetControl(Key.Tab))) {
						this.InsertCompletion(suggestion);
					}
				}
			}

		}

		private bool ShouldCloseOnEsc() {
			return !this.CanCycleCompletions() && !this.isCyclingCompletions;
		}

		private bool CheckCaret() {
			if (!this.isVisible) return false;
			int currentCaret = this.GetCurrentCaret();
			if (currentCaret == this.lastKnownCaretPos) return false;
			this.lastKnownCaretPos = currentCaret;
			return true;
		}

		private void CaretChanged(int newCaret) {
			if (newCaret > 0) {
				this.ShowCompletions(this.GetCommand(), newCaret);
			} else {
				this.ClearAndDisableCompletions();
			}
			//ParseResults<RuntimeCommandSource> parseResults = this.commandProcessor.Parse(this.GetCommand());
			//IEnumerable<string> usages = parseResults.Context.Nodes
			//	.SelectMany(c => this.commandProcessor.GetSmartUsages(c.Node))
			//	.Select(kvp => kvp.Value);
			//SoulboundEngine.Logger.LogInfo("all: {}", string.Join(", ", usages));

			//if (parseResults.Context.Child != null) {
			//	IEnumerable<string> childUsages = parseResults.Context.Child.Nodes
			//		.SelectMany(c => this.commandProcessor.GetSmartUsages(c.Node))
			//		.Select(kvp => kvp.Value);
			//	SoulboundEngine.Logger.LogInfo("child: {}", string.Join(", ", usages));
			//}

			//foreach (ParsedCommandNode<RuntimeCommandSource> item in parseResults.Context.Nodes) {
			//	SoulboundEngine.Logger.LogInfo(item.Node.Name);
			//}
			//foreach (KeyValuePair<string, IParsedArgument> item in parseResults.Context.GetArguments()) {
			//	SoulboundEngine.Logger.LogInfo("{}: {}", item.Key, item.Value.Result);
			//}

		}

		private void ClearAndDisableCompletions() {
			this.isCyclingCompletions = false;
			this.completionManager.ClearCompletions();
			this.completionList.itemsSource = Array.Empty<Suggestion>();
		}

		private int GetCurrentCaret() => this.textField.cursorIndex;

		public string GetCommand() => this.textField.value;

		public override void Show() {
			base.Show();
			this.root.style.display = DisplayStyle.Flex;
			this.textField.value = "/";
			this.client.PushInputFocus(this);
			this.hasEdited = false;
			this.isCyclingCompletions = false;
			this.isCyclingHistory = false;

			this.textField.RegisterCallback<ChangeEvent<string>>(FirstEdit, TrickleDown.TrickleDown);
			void FirstEdit(ChangeEvent<string> evt) {
				this.hasEdited = true;
				this.textField.UnregisterCallback<ChangeEvent<string>>(FirstEdit, TrickleDown.TrickleDown);
			}

			this.GrabFocus();
			this.SetCaretToEnd();
			this.textField.schedule.Execute(() => {
				this.ShowCompletions(this.GetCommand(), this.GetCurrentCaret());
			});
		}

		public override void Hide() {
			base.Hide();
			this.textField.value = "/";
			this.client.PopInputFocus(this);
			this.root.style.display = DisplayStyle.None;
		}

		private bool CanCycleHistory() {
			return this.history.Any() && !this.hasEdited;
		}

		private bool CanCycleCompletions() {
			return this.completionManager.GetCompletionCount() > 0;
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
				SoulboundEngine.Logger.LogFatal(e);
			});
		}

		private void OnCompletionChosen(IEnumerable<object> objects) {
			this.InsertCompletion((Suggestion)objects.First());
		}

		private void InsertCompletion(Suggestion suggestion) {
			string withoutLeadingSlash = this.textField.value[1..];
			int caretInStripped = this.GetCurrentCaret() - 1;   // account for leading '/';
			int tokenEnd = withoutLeadingSlash.IndexOf(' ', caretInStripped);
			if (tokenEnd < 0) tokenEnd = withoutLeadingSlash.Length;

			string before = withoutLeadingSlash[..suggestion.Range.Start];
			string after = withoutLeadingSlash[tokenEnd..];
			string completed = $"/{before}{suggestion.Text}{after}";
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

		private void OverwriteWithHistory(int historyIndex) {
			this.textField.value = this.history[historyIndex];
			this.SetCaretToEnd();
		}

		private void GrabFocus() {
			this.textField.schedule.Execute(this.textField.Focus);
		}

		private void SetCaretToEnd() {
			this.textField.schedule.Execute(() => {
				int end = this.textField.value.Length;
				this.textField.cursorIndex = end;
				this.textField.selectIndex = end;
			});
		}

		bool IInputFocusable.HasKeyboardFocus() => true;
		bool IInputFocusable.IsPointerOverUI() => true;
	}
}
