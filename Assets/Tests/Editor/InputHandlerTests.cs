using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using SoulboundBackend.Client.Input;
using UnityEngine;
using Zenject;
using UnityEditor;

public class InputHandlerTests {
	private void CreateHandler(out InputHandler handler, out InputActionAsset asset) {
		asset = ScriptableObject.CreateInstance<InputActionAsset>();
		handler = new InputHandler(asset);
	}

	private void CreateMapping(out InputActionAsset asset, out InputActionMap map, string actionMapID) {
		asset = ScriptableObject.CreateInstance<InputActionAsset>();
		map = asset.AddActionMap(actionMapID);
	}

	private void CreateMapping(out InputActionAsset asset, out InputActionMap map, out InputAction action, string actionMapId, string actionId) {
		CreateMapping(out asset, out map, actionMapId);
		action = map.AddAction(actionId);
	}

	private void CreateMapping(out InputHandler handler, out InputActionAsset asset, out InputActionMap map, out InputAction action, string actionMapID, string actionID) {
		CreateMapping(out asset, out map, out action, actionMapID, actionID);
		handler = new InputHandler(asset);
	}

	[Test]
	public void GetAction_WithCompressedID_ReturnsCorrectAction() {
		CreateMapping(out var asset, out var map, out var action, "Player", "Jump");
		var inputHandler = new InputHandler(asset);

		var result = inputHandler.GetAction("Player/Jump");
		Assert.AreEqual(action, result);
	}

	[Test]
	public void GetAction_WithCompressedID_ReturnsDifferentAction_ForDifferentBindings() {
		CreateMapping(out var asset, out var map, "Player");
		var action1 = map.AddAction("Action1");
		var action2 = map.AddAction("Action2");
		var inputHandler = new InputHandler(asset);
		Assert.That(action1, Is.Not.EqualTo(action2));

		var result1 = inputHandler.GetAction("Player/Action1");
		var result2 = inputHandler.GetAction("Player/Action2");
		Assert.That(result1, Is.Not.EqualTo(result2));
		Assert.That(result1, Is.EqualTo(action1));
		Assert.That(result2, Is.EqualTo(action2));
	}

	[Test]
	public void GetAction_Throws_ForInvalidCompressedId() {
		CreateHandler(out var handler, out var _);
		Assert.Throws<ArgumentException>(() => handler.GetAction("Invalid"));
	}

	[Test]
	public void LateTick_ExecutesRequest() {
		CreateHandler(out var handler, out var _);
		bool invoked = false;

		handler.RequestAction(new InputActionRequest(
			context: "ActionContext",
			priority: 0,
			action: () => invoked = true,
			callback: null
		));

		((ILateTickable)handler).LateTick();

		Assert.That(invoked, Is.True);
	}

	[Test]
	public void LateTick_Executes_HighestPriorityRequest() {
		CreateHandler(out var handler, out var _);
		bool lowCalled = false;
		bool highCalled = false;

		handler.RequestAction(new InputActionRequest(
			context: "ActionContext",
			priority: 1,
			action: () => lowCalled = true,
			callback: null
		));

		handler.RequestAction(new InputActionRequest(
			context: "ActionContext",
			priority: 5,
			action: () => highCalled = true,
			callback: null
		));

		((ILateTickable)handler).LateTick();

		Assert.IsTrue(highCalled);
		Assert.IsFalse(lowCalled);
	}

	[Test]
	public void LateTick_IgnoresBlockedRequests() {
		bool executed = false;
		CreateHandler(out var handler, out var _);

		handler.BlockContext("ActionContext", () => false);

		handler.RequestAction(new InputActionRequest(
			context: "ActionContext",
			priority: 5,
			action: () => executed = true,
			callback: null
		));

		((ILateTickable)handler).LateTick();

		Assert.IsFalse(executed);
	}

	[Test]
	public void Tick_RemovesBlockedContext_IfPredicateTrue() {
		CreateHandler(out var handler, out var _);
		handler.BlockContext("ActionContext", () => true);

		((ITickable)handler).Tick();

		Assert.IsFalse(handler.IsContextBlocked("ActionContext"));
	}

	[Test]
	public void Tick_DoesntRemoveBlockedContext_IfPredicateFalse() {
		CreateHandler(out var handler, out var _);
		handler.BlockContext("ActionContext", () => false);

		((ITickable)handler).Tick();

		Assert.IsTrue(handler.IsContextBlocked("ActionContext"));
	}

	[Test]
	public void PauseInputs_DisablesPausableActions() {
		CreateMapping(out var handler, out var _, out var _, out var action, "ActionMap", "Action");
		handler.RegisterInputEvent(action, true, _ => { });

		handler.PauseInputs(true);

		Assert.That(action.enabled == false);
	}

	[Test]
	public void PauseInputs_EnablesPausableActions() {
		CreateMapping(out var handler, out var _, out var _, out var action, "ActionMap", "Action");
		handler.RegisterInputEvent(action, true, _ => { });

		handler.PauseInputs(false);

		Assert.That(action.enabled == true);
	}

	[Test]
	public void RegisterInputEvent_InvokesBindingCallback_Immediately() {
		CreateMapping(out var handler, out var _, out var _, out var action, "ActionMap", "Action");
		bool invoked = false;

		handler.RegisterInputEvent(action, false, _ => invoked = true);

		Assert.True(invoked);
	}
}
