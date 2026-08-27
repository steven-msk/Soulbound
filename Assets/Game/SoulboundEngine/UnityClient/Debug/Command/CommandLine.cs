namespace SoulboundEngine.UnityClient.Debug {
	using Brigadier.NET;
	using Brigadier.NET.Exceptions;
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
		private static readonly Identifier USAGE_LIST_ELEMENT = Identifier.Of("soulbound:command_line/usage_list");
		private static readonly Identifier USAGE_TEXT_ELEMENT = Identifier.Of("soulbound:command_usage/usage_text");
		private static readonly Identifier EXCEPTION_LIST_ELEMENT = Identifier.Of("soulbound:command_line/exception_list");
		private static readonly Identifier EXCEPTION_TEXT_ELEMENT = Identifier.Of("soulbound:command_exception/exception_text");
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly CompletionManager completionManager = new();
		private readonly Keyboard keyboard;
		private readonly SoulboundUnityClient client;
		private TextField textField;
		private ListView completionList;
		private List<string> currentUsages;
		private ListView usageList;
		private List<CommandSyntaxException> currentExceptions;
		private ListView exceptionList;
		private Color defaultFieldColor;
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
			this.usageList = root.Get<ListView>(USAGE_LIST_ELEMENT);
			this.usageList.bindItem = (element, index) => {
				element.Get<Label>(USAGE_TEXT_ELEMENT).text = this.currentUsages[index];
			};
			this.usageList.itemsSource = this.currentUsages;
			this.exceptionList = root.Get<ListView>(EXCEPTION_LIST_ELEMENT);
			this.exceptionList.bindItem = (element, index) => {
				element.Get<Label>(EXCEPTION_TEXT_ELEMENT).text = this.currentExceptions[index].Message;
			};
			this.exceptionList.itemsSource = this.currentExceptions;
			this.textField.RegisterCallback<KeyDownEvent>(this.InterceptHandledKeys, TrickleDown.TrickleDown);
			this.completionList.makeNoneElement = () => new VisualElement();
			this.completionList.itemsChosen += this.OnCompletionChosen;
			this.defaultFieldColor = Color.black;

			this.textField.RegisterCallback<FocusOutEvent>(evt => this.GrabFocus());
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

			ParseResults<RuntimeCommandSource> parseResults = this.commandProcessor.Parse(this.GetCommand());
			if (parseResults.Context.Nodes.Count > 0) {
				List<string> usages = this.commandProcessor.GetSmartUsages(parseResults.Context.Nodes.Last().Node)
					.Select(kvp => kvp.Value)
					.ToList();
				this.currentUsages = usages;
				this.usageList.itemsSource = this.currentUsages;
				this.usageList.Rebuild();
				this.usageList.style.display = usages.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
			} else {
				this.usageList.style.display = DisplayStyle.None;
			}

			List<CommandSyntaxException> exceptions = parseResults.Exceptions
				.Select(kvp => kvp.Value)
				.ToList();
			if (exceptions.Count > 0 && this.usageList.style.display == DisplayStyle.Flex) {
				this.usageList.style.display = DisplayStyle.None;
			}
			this.currentExceptions = exceptions;
			this.exceptionList.itemsSource = this.currentExceptions;
			this.exceptionList.Rebuild();
			this.exceptionList.style.display = exceptions.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
			this.textField.style.color = exceptions.Count > 0 ? Color.red : this.defaultFieldColor;
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
			this.textField.value = "/";
			this.client.PushInputFocus(this);
			this.hasEdited = false;
			this.isCyclingCompletions = false;
			this.isCyclingHistory = false;

			this.textField.RegisterCallback<ChangeEvent<string>>(FirstEdit);
			void FirstEdit(ChangeEvent<string> evt) {
				this.hasEdited = true;
				this.textField.UnregisterCallback<ChangeEvent<string>>(FirstEdit);
			}
			this.textField.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
			void GeometryChanged(GeometryChangedEvent evt) {
				this.GrabFocus();
				this.textField.UnregisterCallback<GeometryChangedEvent>(GeometryChanged);
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
				this.ClearAndDisableCompletions();
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
