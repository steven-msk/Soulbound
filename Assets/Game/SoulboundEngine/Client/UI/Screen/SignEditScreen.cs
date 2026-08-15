using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.Client.Assets;
using SoulboundEngine.Registry;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.UI.Screen {
	public class SignEditScreen : UXMLScreen {
		private static readonly Identifier TEXT_FIELD_ELEMENT = Identifier.Of("soulbound:sign_edit_screen/text_field");
		private static readonly Identifier CANCEL_ELEMENT = Identifier.Of("soulbound:sign_edit_screen/cancel");
		private static readonly Identifier DONE_ELEMENT = Identifier.Of("soulbound:sign_edit_screen/done");
		private readonly SignTileEntity signEntity;
		private readonly string originalText;
		private VisualElement root;
		private TextField textField;

		public override bool CloseOnEsc => false;
		public override bool IsOpaque => false;

		public SignEditScreen(SignTileEntity signEntity) 
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("SignEditScreen"))) {
			this.signEntity = signEntity;
			this.originalText = signEntity.GetText();
		}

		protected override void OnBind(VisualElement root) {
			this.root = root;
			root.RegisterCallback<KeyDownEvent>(this.KeyPressed, TrickleDown.TrickleDown);

			this.textField = root.Get<TextField>(TEXT_FIELD_ELEMENT);
			this.textField.RegisterValueChangedCallback(evt => {
				this.signEntity.SetText(evt.newValue);
			});
			this.textField.value = this.originalText;

			Button cancelButton = root.Get<Button>(CANCEL_ELEMENT);
			cancelButton.clicked += this.Cancel;

			Button doneButton = root.Get<Button>(DONE_ELEMENT);
			doneButton.clicked += this.Done;
		}

		public override void OnShow(IScreenHandle handle) {
			this.signEntity.screenHandle = handle;
		}

		private void KeyPressed(KeyDownEvent evt) {
			if (evt.keyCode == UnityEngine.KeyCode.Escape) {
				this.Cancel();
			}
		}

		private void Cancel() {
			this.signEntity.SetText(this.originalText);
			this.ScreenManager.PopScreen(this.handle);
		}

		private void Done() {
			this.ScreenManager.PopScreen(this.handle);
		}

		public override void OnHide(IScreenHandle handle) {
			this.signEntity.screenHandle = null;
		}

		public override void OnDispose(IScreenHandle handle) {
			base.OnDispose(handle);
			this.root.UnregisterCallback<KeyDownEvent>(this.KeyPressed);
		}
	}
}
