## Top overview
- the input system handles everything that the user inputs to the game from hardware sources, i.e mouse, keyboard etc.
- its responsible for keeping input dispatching centralized across the backend and allowing multiple sources to listen to input and react accordingly
- `InputManager` contains the input dispatching logic, `IInputContext`s are registered to an `InputManager` in order to receive `HandleInput` calls. Every input dispatch has a context which describes the input event, characterized by an `InputToken`, a phase (`InputActionPhase`), and the generated `InputAction.CallbackContext`.
- every frame, it dispatches the event to each `IInputContext` ordered by their descending priority. At each dispatch, the listener can choose to consume the input, breaking the dispatch iteration early. When this happens, all other listeners do not receive the consumed event. Every listener must check for the event they target and the phase which they were triggered by. *(known issue: separate token and phase checks may fill up `HandleInput` with too many if statements)*
- depends on Unity's Input System as `InputSystem` requires an `InputActionAsset`.
- possible evolution to support cross-platform inputs, not only desktop
- known issues:
    - current input action asset only has mappings for desktop,
    - *(fixed)* separate token and phase checks may fill up `HandleInput` with too many if statements,
    - friction between UI input handled internally by Unity and input managed by `InputManager`,
    - early break in dispatch iteration from consumed input could be dangerous,
    - *(fixed)* slight naming problem with `IInputContext`, 
    - input token equality checks are based on internal token registries and can create inconsistencies between direct hardware input mappings (`InputTokens.Keyboard` which map directly to keyboard keys) and custom ones defined in the asset (`InputTokens.Player` which map to custom actions like jump, move).
    - *(fixed)* unknown behavior of `waiting` and `disabled` phases
    - poor listener prioritization
    - slight input action integration problems

## InputManager
- when an `InputManager` is created, it subscribes to every `performed`, `started`, `canceled` event of every input action in every action map in the given input asset, and creates an input token for every input action (it is asserted that this token is also registered in the global registry `InputTokens` so that equality checks work, otherwise no instance will match the equality). Every input token retains the guid of its `InputAction` used to identify it.
- every input event is represented by an `InputEvent` instance, created on each invocation of either `performed`, `started`, or `canceled` of the triggered input action. The input event is queued, and on the next frame it will be dispatched to every `IInputContext`, in order of their descending priority. The queue has a maximum of 128 concurrent input events. 
- it is important to note that, in a single frame, an input event cannot be dispatched more than once with the same phase, but multiple input events can be dispatched at the same time. The order in which the events are dispatched are in the order they were triggered. 
- every dispatch creates a different `InputEvent` instance containing an `InputToken`, `InputAction.CallbackContext` and `InputActionPhase` representing the triggered action. Every `IInputContext` receives this `InputEvent` and can handle different actions through an equality check (`inputEvent.token.Equals(InputToken)`). Note that if only the token checked then the handled logic will be invoked on every phase that matches the action (that is a call for each `performed`, `started` and `performed`). Consider checking the phase for phase-specific logic or use any of `inputEvent.Performed(InputToken)`, `inputEvent.Canceled(InputToken)`, `inputEvent.Started(InputToken)`, `inputEvent.Waiting(InputToken)`, or `inputEvent.Disabled(InputToken)`. *(known issue: unknown behavior of `waiting` and `disabled` phases)*
- if an input event matches the conditions for something (say a performed key press action), the `IInputContext` can choose to consume the input, signified by `return true`. This completely stops the dispatch iteration, and any subsequent `IInputContext`s that may listen to the consumed event will not be invoked at all. *(known issue: early break in dispatch iteration from consumed input could be dangerous)*
- if no tokens match the conditions then the listener must return false in order to continue the dispatch iteration.

## Input token registry
- every `InputToken` should be registered inside `InputTokens`, which is used by `IInputContext`s to do the token equality checks.
- `InputManager` always creates an `InputToken` instance identical to one and only one token from the registry. This is guaranteed by the guid assigned to each token from the corresponsive `InputAction`. *(known issue: input token equality checks are based on internal token registries and can create inconsistencies between direct hardware input mappings and custom ones defined in the asset)*

## Input listeners and priority
- a listener is represented by `IInputContext` and its responsible for receiving `InputEvent`s from an `InputManager` *(known issue: slight naming problem with `IInputContext`)*
- at dispatch time a listener can choose to consume the input and stop the dispatch iteration for other listeners.
- input listeners must be registered to an `InputManager` to receive input events. A listener can be registered an unregistered from an `InputManager` at any time.

## Known issues:
- current input action asset only has mappings for desktop. There are no mappings made in the asset for console or other platform inputs.
- *(fixed)* separate token and phase checks may fill up `HandleInput` with too many if statements. Because of the strict equality checks and the various different input tokens, it is possible that listeners that can react to multiple tokens can fill up the method really quick. For example `Player`'s `IInputContext.HandleInput` is filled with if and nested if statements.
    - one fix would be to split up the events in methods based on their trigger phase, so one for perform, one for started, one for canceled. This would remove some clutter from the single `HandleInput` method.
    - another, more recommended long-time fix would be to rework the dispatch design. Instead of a method receiving calls, listeners would directly register callbacks to each token and phase, completely removing the need of cluttered matching logic. The callback registration can be made when the listener is registered to an `InputManager`, and that `InputManager` would map the callbacks to each event.
- friction between UI input handled internally by Unity and input managed by `InputManager`. Unity's UI handles inputs and especially mouse inputs internally, thus there is a friction between events that Unity receives and dispatches vs. what `InputManager` dispatches. As of right now, the fix consists of the `UIHandler` implementing `IInputContext` with a really high priority and consuming the every UI-related input (mouse clicks for example). This doesnt need fixing before prod, but keep this in mind.
    - a possible fix would be to give the ability to listeners to block other events. In the case of UI, this would block any subsequent mouse clicks if the click were done on a UI object. This would also possibly remove the need of early breaks in the dispatch iteration.
- early break in dispatch iteration from consumed input could be dangerous. Early breaks in a dispatch iteration may result in unwanted behavior. Use the input consume logic very carefully as it may hide bugs. **This needs to be fixed before prod**
- *(fixed)* slight naming problem with `IInputContext`. Calling the listener a "context" isnt really the right term. Consider renaming ro `IInputListener` or `IInputHandler`.
- input token equality checks are based on internal token registries and can create inconsistencies between direct hardware input mappings (`InputTokens.Keyboard` which map directly to keyboard keys) and custom ones defined in the asset (`InputTokens.Player` which map to custom actions like jump, move). The input actions from the asset and the ones registered need to be synchronized, otherwise bugs may appear.
    - consider making the registry more centralized, possibly make use of the global registry system.
- *(fixed)* unknown behavior of `waiting` and `disabled` phases. As far as I'm concerned, these can be ignored.
- poor listener prioritization. Each `IInputContext` exposes a priority represented by an int, which is then used by `InputManager` to make dispatching order more deterministic. This does fix one layer of determinism, but not fully. Theres still the risk of identical priorities in which case the order of dispatching would theoretically be in the order in which the listeners were registered, which is highly non-deterministic.
- slight input action integration problems. `InputAction`s in an `InputActionAsset` can map multiple hardware inputs to one action. This simplifies listener logic a bit but introduces complexity for input rebinding. It should be clear that, for simplicity reasons, only actions that have a single hardware input are rebindable, all others are static and shouldnt be changed. This doesnt need an immediate fix but it is something to consider during prod.

## Must be addressed and fixed before prod
- if statement clutter in `HandleInput`
- early break in dispatch iteration
- naming issues
- registry and asset mapping inconsistencies
- listener prioritization
