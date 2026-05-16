using SoulboundEngine.Client.Debug.Commands;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Debug {
	public sealed class CommandLine {
		private VisualElement root;
		private TextField textField;
		private readonly CommandProcessor commandProcessor;
		private readonly List<string> history = new();
		private readonly SoulboundClient.DebugOverlayManager debugOverlayManager;

		public CommandLine(CommandProcessor commandProcessor, SoulboundClient.DebugOverlayManager debugOverlayManager) {
			this.debugOverlayManager = debugOverlayManager;
			this.commandProcessor = commandProcessor;
		}

		public bool isVisible { get; private set; }

		public void OnBind(VisualElement root) {
			this.root = root;
			this.textField = root.Q<TextField>("TextField");
		}

		public void Show() {
			if (this.isVisible) return;
			this.isVisible = true;
			this.root.style.display = DisplayStyle.Flex;
			this.textField.value = "/";
			this.textField.Focus();
		}

		public void Hide() {
			if (!this.isVisible) return;
			this.isVisible = false;
			this.root.style.display = DisplayStyle.None;
			this.textField.value = "/";
		}

		//public override void Toggle() {
		//	if (this.visible) this.node.Destroy();
		//	else this.Show();
		//}

		//private void SubmitCommand(string command) {
		//	this.node.Destroy();
		//	this.commandProcessor.SubmitCommand(command);
		//	this.history.Add(command);
		//	this.debugOverlayManager.Hide(SoulboundClient.DebugOverlayFeature.CommandLine);
		//}

		//private void HandleKey(Key key) => this.node?.handle.HandleKey(key);

		public bool HandleKey(Key key) {
			if (!this.isVisible) return false;

			return true;
			//if (key == Key.Escape) {
			//	shouldCloseCommandLine();
			//	return;
			//}
			//if (key != Key.Tab && key != Key.UpArrow && key != Key.DownArrow) currentInputMode = CommandInputMode.Typing;
			//switch (currentInputMode) {
			//	case CommandInputMode.Typing:
			//		HandleTyping(key);
			//		break;
			//	case CommandInputMode.CyclingCompletions:
			//		HandleCompletion(key);
			//		break;
			//	case CommandInputMode.CyclingHistory:
			//		HandleHistory(key);
			//		break;
			//}
		}

		//protected override UIHandledOverlayNode<ICommandLineHandler> GetNode() {
		//	GameObject obj = new("Command Line", typeof(RectTransform));

		//	// TMP_InputField misses the condition to create the caret object when OnEnable is called
		//	// the object needs to be disabled before the component is added
		//	obj.SetActive(false);

		//	RectTransform rect = obj.GetComponent<RectTransform>();
		//	rect.anchorMin = Vector2.zero;
		//	rect.anchorMax = new Vector2(1f, 0f);
		//	rect.pivot = new Vector2(0.5f, 0f);
		//	rect.sizeDelta = new Vector2(0f, 30f);

		//	Image bg = obj.AddComponent<Image>();
		//	bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

		//	CommandLineInputField inputField = obj.AddComponent<CommandLineInputField>();

		//	GameObject textObj = new("Text", typeof(RectTransform));
		//	textObj.transform.SetParent(obj.transform, false);

		//	TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
		//	text.fontSize = 15f;
		//	text.color = Color.white;

		//	RectTransform textRect = textObj.GetComponent<RectTransform>();
		//	textRect.anchorMin = Vector2.zero;
		//	textRect.anchorMax = Vector2.one;
		//	textRect.offsetMin = new Vector2(10f, 0f);
		//	textRect.offsetMax = new Vector2(-10f, 0f);

		//	CommandLineHandler handler = obj.AddComponent<CommandLineHandler>();
		//	handler.Init(inputField, this.commandProcessor, this.history);
		//	handler.shouldCloseCommandLine += () => {
		//		this.debugOverlayManager.Hide(SoulboundClient.DebugOverlayFeature.CommandLine);
		//		this.Toggle();
		//	};

		//	GameObject viewport = new("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
		//	viewport.transform.SetParent(obj.transform, false);

		//	Mask mask = viewport.GetComponent<Mask>();
		//	mask.showMaskGraphic = false;

		//	RectTransform viewportRect = viewport.GetComponent<RectTransform>();
		//	viewportRect.anchorMin = Vector2.zero;
		//	viewportRect.anchorMax = Vector2.one;
		//	viewportRect.offsetMin = Vector2.zero;
		//	viewportRect.offsetMax = Vector2.zero;

		//	textObj.transform.SetParent(viewportRect.transform, false);

		//	inputField.textComponent = text;
		//	inputField.textViewport = viewportRect;
		//	inputField.lineType = TMP_InputField.LineType.SingleLine;
		//	inputField.onSubmit.AddListener(this.SubmitCommand);
		//	inputField.onFocusSelectAll = false;
		//	inputField.restoreOriginalTextOnEscape = false;
		//	inputField.onDeselect.AddListener(_ => inputField.ActivateInputField());

		//	// now the caret object will be created
		//	obj.SetActive(true);

		//	return new UIHandledOverlayNode<ICommandLineHandler>(obj, handler);
		//}

	}
}
