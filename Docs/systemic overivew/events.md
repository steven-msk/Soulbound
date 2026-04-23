## Top overview
- the event system is responsible for hooking up listeners and dispatching events at runtime
- its role is to manage which listeners react to which events, as well as dispatching them in a deterministic order
- `EventBus` is the class that contains the dispatching logic. `IEventListener<T>` and `IEventHandler<T>` can be registered to receive game events specified by their type argument. *(known issue: event identity is too vast)*. A game event is represented by a `IGameEvent` instance which can be published.
- at runtime, whenever an event is published, it first locks the dispatch in order to break some recursiveness. If another dispatch is happening, the event is queued and it will be dispatched after the current chain is finished, otherwise it dispatches it instantly. At dispatch, it first makes a pattern match to get the listeners and/or handlers that respond to the published `IGameEvent` type. It then finishes by calling `OnEvent(T)` for each listener or handler that matches the event type. *(known issue: potential unnecessary complexity with listeners vs. handlers)*
- no dependencies as its a fundemental system
- potential extension with event cancellation/propagation control
- known weaknesses:
    - event identity is too vast
    - event dispatch implementation is too centralized
    - potential unnecessary complexity with listeners vs. handlers
    - poorly designed response call

## Event dispatching
- an event is represented by a `IGameEvent` instance. This instance can be published to `EventBus` with `EventBus.Publish<T>(T)` and based on the generic type it will invoke each listener accordingly.
- publishing can optionally provide a dispatch callback `Action<HashSet<Type>>` which represents the "response" the handlers that listen to this event may retrieve.
- note the important distinction between a plain `IEventListener` and a `IEventHandler`. Only handlers are eligible for a response call, while listeners only receive events. The dispatch callback is called with a set of types of handlers that "responded" to this event. This set cannot be null but can be empty if no handlers have received the event.
- both listeners and handlers can be added and removed using `EventBus.AddListener<T>(IEventListener<T>)`, `EventBus.RemoveListener<T>`, `EventBus.AddHandler<T>(IEventHandler<T>)`, `EventBus.RemoveHandler<T>`. `EventBus` also provides a way to add all interfaces of a type using `AddAllInterfaces(object)`.
- the order in which listeners and handlers are called is all handlers first, all listeners last. Listener and handler call indexing is not determined. The dispatch callback is called after all handlers and listeners have been invoked.
- note that any events published during another dispatch are queued and published after the current chain is finished. Any dispatch calls are delayed until all chains are finished.

## Known issues:
- event identity is too vast. Every event is represented by a `IGameEvent` which represents a single event type, for example `BlockBrokenEvent`, `BlockPlacedEvent`, `ButtonClickedEvent` etc. This does provide type safety and supports event context very cleanly, but multiple game events shouldnt rely on multiple types being defined because of scalability issues. Events that share contexts are forced to split up and define types for each one, and the context could be pulled into a common abstraction, but this will certainly not scale well during prod. *This goes hand-in-hand with the dispatch implementation problem*
- event dispatch implementation is too centralized. There is currently only one global dispatch source for all in-game events and all listeners and handlers, that being the static class `EventBus`. This is too centralized and doesnt support domain-specific or scoped events, forcing all events to become global. Consider making event dispatch an abstraction, and allow multiple domains to make their own way of dispatching the events. *This goes hand-in-hand with the event identity problem.*
- potential unnecessary complexity with listeners vs. handlers. The only purpose of handlers is to create the so called "response" call, which provides a way to dynamically communicate between systems. But this could only become an overhead during prod and may not be necessary either way. Consider removing handlers entirely and only rely on listeners, and also remove the response call entirely to simplify the logic.
- poorly designed response call. Since currently there are no features that use this, the design is made only for a theoretical use. This shouldnt persist for prod.

## Event dispatch implementation and event identity problems
- as stated earlier, there are 2 problems that need fixing before prod:
    - the event identity being too vast, as in every event represents a single type
    - and the event dispatcher being too centralized
- this design was made because of the uncertainty between different types of events, and it was mainly the missing features fault
- in order to fix both, 2 conditions need to be met:
    - first, the dispatch implementation needs to support domain-specific and scoped events. For example there should be a clear separation between which events are dispatched from the UI and which events are dispatched from a world session, as well as the listeners that listen to them.
    - second, the event identity needs to match the dispatcher's supported events. For example there needs to be a clear distinction between world events and UI events. World events cannot be invoked in the UI domain and vice-versa.
- considering that both conditions are met, the final implementation should be something like:
    - an event dispatcher (say `IEventDispatcher<T>`) that is domain-scoped and dispatches events according to the event type `T`, where `T` is an `IGameEvent`
    - a game event definition described by `IGameEvent` identified with an `Identifier`, and can contain data about a specific event (for example a `WorldEvent` would contain `BlockPos`, `BlockState`, `Level` etc)
    - every event can be defined and dispatched separately of other events, allowing a way to define some game events globally or inside registries
    - another possibility would be to register game event builders to registries and pass the data to those instead of having to create an instance every time and repeating the same `Identifier`. Every builder should come with its own identifier, and the data can be treated as a dependency.
- there is still a decision to be taken for whether the event should contain both the identifier and data. The winning choice in this case would be to separate the id from the data and keep the event only a registerable object, or make a hybrid and support both concepts.

## Must be addressed and fixed before prod
- event identity and dispatch implementation
- listeners vs. handlers
- response call design
