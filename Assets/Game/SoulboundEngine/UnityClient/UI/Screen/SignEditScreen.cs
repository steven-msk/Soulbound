namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.UnityClient.World;
	using SoulboundEngine.World.Block.Entity;
	using UnityEngine.InputSystem;
	using UnityEngine.UIElements;
	using Keyboard = Input.Keyboard;

	public class SignEditScreen : UXMLScreen {
		private static readonly UXMLBinding<TextField> TEXT_FIELD_ELEMENT = new("soulbound:sign_edit_screen/text_field");
		private static readonly UXMLBinding<Button> CANCEL_ELEMENT = new("soulbound:sign_edit_screen/cancel");
		private static readonly UXMLBinding<Button> DONE_ELEMENT = new("soulbound:sign_edit_screen/done");
		private readonly SignTileEntity signEntity;
		private readonly ClientPlayerEntity player;
		private readonly string originalText;
		private TextField textField;

		public override bool CloseOnEsc => false;
		public override bool IsOpaque => false;

		public SignEditScreen(SignTileEntity signEntity, ClientPlayerEntity player) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("SignEditScreen"))) {
			this.signEntity = signEntity;
			this.player = player;
			this.originalText = signEntity.GetText();
		}

		protected override void OnBind(VisualElement root) {
			this.textField = TEXT_FIELD_ELEMENT.Get(root);
			this.textField.RegisterValueChangedCallback(evt => {
				this.signEntity.SetText(evt.newValue);
			});
			this.textField.value = this.originalText;
			this.CaptureFocus(this.textField);

			Button cancelButton = CANCEL_ELEMENT.Get(root);
			cancelButton.clicked += this.Cancel;

			Button doneButton = DONE_ELEMENT.Get(root);
			doneButton.clicked += this.Done;
		}

		public override bool HasKeyboardFocus() => true;

		public override bool IsPointerOverUI() => true;

		public override void Tick() {
			if (SoulboundUnityClient.Instance.InputManager.keyboard.WasPressed(Keyboard.GetControl(Key.Escape))) {
				this.Cancel();
			}
		}

		private void CaptureFocus(VisualElement visualElement) {
			visualElement.schedule.Execute(visualElement.Focus);
		}

		private void Cancel() {
			this.signEntity.SetText(this.originalText);
			this.player.CloseSignEditScreen();
		}

		private void Done() {
			this.player.CloseSignEditScreen();
		}
	}
}
