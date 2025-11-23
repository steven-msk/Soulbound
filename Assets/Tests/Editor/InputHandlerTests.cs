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
using UnityEngine.InputSystem.LowLevel;
using System.Security.Permissions;
using UnityEngine.InputSystem.Controls;

public class InputHandlerTests {
	private void CreateMapping(out InputActionAsset asset, out InputActionMap map, string actionMapID) {
		asset = ScriptableObject.CreateInstance<InputActionAsset>();
		map = asset.AddActionMap(actionMapID);
	}

	private void CreateMapping(out InputActionAsset asset, out InputActionMap map, out InputAction action, string actionMapId, string actionId) {
		CreateMapping(out asset, out map, actionMapId);
		action = map.AddAction(actionId);
	}
	private void CreateMapping(out InputHandler handler, out InputActionAsset asset, out InputActionMap actionMap, string actionMapId = "ActionMap") {
		asset = ScriptableObject.CreateInstance<InputActionAsset>();
		actionMap = asset.AddActionMap(actionMapId);
		handler = new InputHandler(actionMap);
	}

	private void CreateMapping(out InputHandler handler, out InputActionAsset asset, out InputActionMap map, out InputAction action, string actionMapID, string actionID) {
		CreateMapping(out asset, out map, out action, actionMapID, actionID);
		handler = new InputHandler(map);
	}

	[Test]
	public void LateTick_ExecutesRequest() {
		CreateMapping(out var handler, out var _, out var _);
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
		CreateMapping(out var handler, out var _, out var _);
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
		CreateMapping(out var handler, out var _, out var _);

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
		CreateMapping(out var handler, out var _, out var _);
		handler.BlockContext("ActionContext", () => true);

		((ITickable)handler).Tick();

		Assert.IsFalse(handler.IsContextBlocked("ActionContext"));
	}

	[Test]
	public void Tick_DoesntRemoveBlockedContext_IfPredicateFalse() {
		CreateMapping(out var handler, out var _, out var _);
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
