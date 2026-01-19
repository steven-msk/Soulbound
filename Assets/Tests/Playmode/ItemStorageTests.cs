using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

//namespace ItemStorageTests {
//	[TestFixture]
//	public class ItemSlotTests {
//		private class TestItem : Item {
//			public override string name => "testItem";
//			public override ItemAspect aspect => new ItemAspect(
//				new ItemIcon(
//					Sprite.Create(new Texture2D(32, 32),
//						new Rect(0, 0, 32, 32),
//						new Vector2(0.5f, 0.5f)), 100),
//				() => new GameObject());
//			public override int maxStackSize => 256;
//			public bool attached { get; private set; }

//			public override void OnAttachedInSlot(IItemSlot slot) {
//				attached = true;
//			}

//			public override void OnDetachedFromSlot(IItemSlot slot) {
//				attached = false;
//			}
//		}

//		public class TestContainer : IItemContainer {
//			public bool down;
//			public bool up;
//			public bool enter;
//			public bool exit;
//			public bool displayAdded;

//			public Transform transform { get; }

//			public IReadOnlyList<IItemSlot> slots => throw new NotImplementedException();

//			public TestContainer() {
//				transform = new GameObject("container").transform;
//			}

//			public void OnPointerDown(IItemSlot slot, PointerEventData e) => down = true;
//			public void OnPointerUp(IItemSlot slot, PointerEventData e) => up = true;
//			public void OnPointerEnter(IItemSlot slot, PointerEventData e) => enter = true;
//			public void OnPointerExit(IItemSlot slot, PointerEventData e) => exit = true;

//			public void OnItemDisplayAdded(ItemDisplay d, IItemSlot slot) {
//				displayAdded = true;
//			}

//			public IItemSlot GetSlotByIndex(int index) {
//				throw new NotImplementedException();
//			}
//		}


//		private class TestSlot : MonoBehaviour, IItemSlot {
//			public ItemDisplay itemDisplay { get; set; }
//			public IItemContainer container { get; private set; }
//			public int index { get; set; }
//			public bool showTooltip { get; set; }

//			public TestSlot Init(IItemContainer container) {
//				this.container = container;
//				return this;
//			}

//			public void Deserialize(SerializedItemSlot serialized) {
//				throw new NotImplementedException();
//			}

//			ItemDisplay IItemSlot.CreateDisplay(ItemStack itemStack) {
//				ItemDisplay display = ItemDisplay.Create(itemStack, () => transform);
//				itemStack.item.OnAttachedInSlot(this);
//				container.OnItemDisplayAdded(display, this);
//				this.itemDisplay = display;
//				return display;
//			}
//		}

//		private TestContainer container;
//		private IItemSlot slot;

//		[SetUp]
//		public void Setup() {
//			this.container = new TestContainer();
//			this.slot = new GameObject("slot").AddComponent<TestSlot>().Init(container);
//		}

//		[Test]
//		public void CreateDisplayIfEmpty_ShouldCreateWhenEmpty() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 100);

//			bool created = slot.CreateDisplayIfEmpty(stack, out var d);

//			Assert.IsTrue(created);
//			Assert.IsNotNull(d);
//			Assert.IsTrue(container.displayAdded);
//			Assert.IsTrue(item.attached);
//		}

//		[Test]
//		public void CreateDisplayIfEmpty_ShouldNotCreateWhenExists() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 1);
//			slot.CreateDisplay(stack);
//			Assert.That(slot.HasItem);

//			bool created = slot.CreateDisplayIfEmpty(stack, out var d);

//			Assert.IsFalse(created);
//			Assert.AreEqual(slot.itemDisplay, d);
//		}


//		[Test]
//		public void TryAddStack_WhenEmpty_CreatesDisplay() {
//			var item = new TestItem();

//			int result = slot.TryAddStack(5, item);

//			Assert.AreEqual(5, result);
//			Assert.IsNotNull(slot.itemDisplay);
//			Assert.IsTrue(item.attached);
//		}

//		[Test]
//		public void TryAddStack_WhenNonEmpty_Increments() {
//			var item = new TestItem();
//			slot.CreateDisplay(new ItemStack(item, 3));

//			int result = slot.TryAddStack(5, item);

//			Assert.AreEqual(5, result);
//			Assert.AreEqual(8, slot.stack!.quantity);
//		}

//		[Test]
//		public void AttachItemDisplay_CallsReleaseAndItemAttached() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 3);
//			var display = ItemDisplay.Create(stack, () => null);

//			((TestSlot)slot).itemDisplay = display;

//			slot.AttachItemDisplay(display);

//			Assert.IsFalse(display.isGrabbed);
//			Assert.IsTrue(item.attached);
//		}

//		[Test]
//		public void DetachItemDisplay_CallsGrabAndDetached() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 3);
//			var display = ItemDisplay.Create(stack, () => null);

//			((TestSlot)slot).itemDisplay = display;

//			slot.DetachItemDisplay(new GameObject().transform);

//			Assert.IsTrue(display.isGrabbed);
//			Assert.IsFalse(item.attached);
//		}

//		[Test]
//		public void PointerEnter_ShowsTooltipWhenEnabled() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 3);
//			var display = ItemDisplay.Create(stack, () => null);
//			((TestSlot)slot).itemDisplay = display;

//			slot.showTooltip = true;

//			var ev = new PointerEventData(EventSystem.current);

//			slot.OnPointerEnter(ev);

//			Assert.IsTrue(display.activeTooltip != null);
//		}

//		[Test]
//		public void PointerExit_DestroysTooltip() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 3);
//			var display = ItemDisplay.Create(stack, () => null);
//			((TestSlot)slot).itemDisplay = display;


//			var ev = new PointerEventData(EventSystem.current);

//			slot.OnPointerExit(ev);

//			Assert.IsTrue(display.activeTooltip == null);
//		}

//		[Test]
//		public void Handshake_ClickOnEmptyWithNoItem_ReturnsFalse() {
//			Assert.IsFalse(slot.Handshake(null, SlotInteractionMode.Click));
//		}

//		[Test]
//		public void Handshake_ClickWithGrabbed_ReturnsTrue() {
//			var display = ItemDisplay.Create(new ItemStack(new TestItem(), 1), () => null);
//			Assert.IsTrue(slot.Handshake(display, SlotInteractionMode.Click));
//		}

//		[Test]
//		public void Handshake_Drag_AlwaysTrue() {
//			Assert.IsTrue(slot.Handshake(null, SlotInteractionMode.Drag));
//		}
//	}

//	[TestFixture]
//	public class ItemDisplayTests {
//		private class TestItem : Item {
//			public override string name => "MockItem";

//			public override ItemAspect aspect => new ItemAspect(
//				new ItemIcon(
//					Sprite.Create(new Texture2D(32, 32),
//						new Rect(0, 0, 32, 32),
//						new Vector2(0.5f, 0.5f)), 100),
//				() => new GameObject());

//			public override Tooltip RenderTooltip(Vector2 pos, Transform parent) {
//				TooltipData tooltipData = Tooltip.Plain(this.name);
//				TooltipRenderer renderer = new(TooltipNodeStylePresets.PresetProvider());
//				Tooltip tooltip = new TestTooltip(renderer, tooltipData);
//				tooltip.Show(pos, parent);
//				return tooltip;
//			}
//		}

//		private class TestTooltip : Tooltip {
//			public bool hidden = false;

//			public TestTooltip(TooltipRenderer renderer, TooltipData data) 
//				: base(renderer, data) {
//			}

//			public override void Hide() {
//				base.Hide();
//				hidden = true;
//			}
//		}

//		[UnityTest]
//		public IEnumerator Destroy_InvokesEvent_AndDestroysGameObject() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var parent = new GameObject().transform;

//			var display = ItemDisplay.Create(stack, () => parent);

//			bool called = false;
//			display.onDestroy += s => called = true;

//			display.Destroy();
//			yield return null;

//			Assert.IsTrue(called);
//			Assert.Throws<MissingReferenceException>(() => {
//				Assert.IsTrue(display.gameObject == null || display == null);
//			});
//		}

//		[Test]
//		public void ShowTooltip_SetsActiveTooltip() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);

//			var display = ItemDisplay.Create(stack, () => null);
//			var parent = new GameObject().transform;

//			display.ShowTooltip(Vector2.zero, parent);

//			Assert.IsNotNull(display.activeTooltip);
//		}

//		[Test]
//		public void DestroyTooltip_ClearsActiveTooltip_AndCallsHide() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);

//			var display = ItemDisplay.Create(stack, () => null);

//			display.ShowTooltip(Vector2.zero, null);
//			var tooltip = (TestTooltip)display.activeTooltip;

//			display.DestroyTooltip();

//			Assert.IsTrue(tooltip.hidden);
//			Assert.IsNull(display.activeTooltip);
//		}

//		[Test]
//		public void OnGrab_SetsGrabbed_DisablesRaycastAndRemovesTooltip() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var display = ItemDisplay.Create(stack, () => null);

//			display.ShowTooltip(Vector2.zero, null);
//			var grabParent = new GameObject().transform;

//			display.OnGrab(grabParent);

//			Assert.IsTrue(display.isGrabbed);
//			Assert.IsNull(display.activeTooltip);
//			Assert.IsFalse(display.GetComponent<Image>().raycastTarget);
//			Assert.AreEqual(grabParent, display.transform.parent);
//		}

//		[Test]
//		public void OnRelease_UnsetsGrabbed_EnablesRaycast_ResetsAnchoredPosition() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var display = ItemDisplay.Create(stack, () => null);

//			var releaseParent = new GameObject().transform;

//			display.OnRelease(releaseParent);

//			Assert.IsFalse(display.isGrabbed);
//			Assert.IsTrue(display.GetComponent<Image>().raycastTarget);
//			Assert.AreEqual(Vector2.zero, display.GetComponent<RectTransform>().anchoredPosition);
//		}

//		[Test]
//		public void UpdateStackText_ReflectsQuantity() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var display = ItemDisplay.Create(stack, () => null);

//			stack.SetQuantity(42);
//			display.UpdateStackText();

//			var tmp = display.stackText.GetComponent<TextMeshProUGUI>();
//			Assert.AreEqual("42", tmp.text);
//		}

//		[UnityTest]
//		public IEnumerator OnStackQuantityChanged_ZeroOrBelow_DestroysSelf() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var display = ItemDisplay.Create(stack, () => null);

//			display.OnStackQuantityChanged(10, 0);
//			yield return null;

//			Assert.IsTrue(display == null || display.gameObject == null);
//		}

//		[Test]
//		public void OnStackQuantityChanged_AboveZero_UpdatesText() {
//			var item = new TestItem();
//			var stack = new ItemStack(item, 10);
//			var display = ItemDisplay.Create(stack, () => null);

//			stack.SetQuantity(7);
//			display.OnStackQuantityChanged(10, 7);

//			var tmp = display.stackText.GetComponent<TextMeshProUGUI>();
//			Assert.AreEqual("7", tmp.text);
//		}
//	}
//}
