using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Core.Event;

internal class EventBusTests {
	[TearDown]
	public void TearDown() {
		EventBus.Clear();
	}

	public struct FakeEvent : IGameEvent { }
	public struct AnotherFakeEvent : IGameEvent { }
	public struct YetAnotherFakeEvent : IGameEvent { }

	[Test]
	public void AddListener_RegistersListenerForEventType() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);

		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void AddHandler_RegistersHandlerForEventType() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler);

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void AddListener_DoesNotRegisterDuplicateInstance() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddListener(listener);

		EventBus.Publish(new FakeEvent());
		listener.Received(1).OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void AddHandler_DoesNotRegisterDuplicateInstance() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler);
		EventBus.AddHandler(handler);

		EventBus.Publish(new FakeEvent());
		handler.Received(1).OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void AddListener_AllowsMultipleInstancesOfSameType() {
		IEventListener<FakeEvent> listener1 = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<FakeEvent> listener2 = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener1);
		EventBus.AddListener(listener2);

		EventBus.Publish(new FakeEvent());
		listener1.Received().OnEvent(Arg.Any<FakeEvent>());
		listener2.Received().OnEvent(Arg.Any<FakeEvent>());
	}


	[Test]
	public void AddHandler_AllowsMultipleInstancesOfSameType() {
		IEventHandler<FakeEvent> handler1 = Substitute.For<IEventHandler<FakeEvent>>();
		IEventHandler<FakeEvent> handler2 = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler1);
		EventBus.AddHandler(handler2);

		EventBus.Publish(new FakeEvent());
		handler1.Received().OnEvent(Arg.Any<FakeEvent>());
		handler2.Received().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void RemoveListener_RemovesPreviouslyRegisteredListener() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);

		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
		listener.ClearReceivedCalls();

		EventBus.RemoveListener(listener);
		EventBus.Publish(new FakeEvent());
		listener.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void RemoveHandler_RemovesPreviouslyRegisteredHandler() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler);

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
		handler.ClearReceivedCalls();

		EventBus.RemoveHandler(handler);
		EventBus.Publish(new FakeEvent());
		handler.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void RemoveListener_NonExistingListener_NoEffect() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.RemoveListener(listener);

		listener.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void RemoveHandler_NonExistingHandler_NoEffect() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.RemoveHandler(handler);

		handler.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Clear_RemovesAllListenersAndHandlers() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddHandler(handler);

		EventBus.Clear();
		EventBus.Publish(new FakeEvent());
		listener.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
		handler.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Publish_InvokesRegisteredListeners() {
		IEventListener<FakeEvent> listener1 = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<FakeEvent> listener2 = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener1);

		EventBus.Publish(new FakeEvent());
		listener1.Received().OnEvent(Arg.Any<FakeEvent>());
		listener2.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Publish_InvokesRegisteredHandlers() {
		IEventHandler<FakeEvent> handler1 = Substitute.For<IEventHandler<FakeEvent>>();
		IEventHandler<FakeEvent> handler2 = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler1);

		EventBus.Publish(new FakeEvent());
		handler1.Received().OnEvent(Arg.Any<FakeEvent>());
		handler2.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Publish_DoesNothingWhenNoListenersOrHandlers() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();

		EventBus.Publish(new FakeEvent());
		listener.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
		handler.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Publish_OnlyInvokesListenersOfMatchingEventType() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<AnotherFakeEvent> anotherListener = Substitute.For<IEventListener<AnotherFakeEvent>>();
		EventBus.AddListener(listener);

		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
		anotherListener.DidNotReceive().OnEvent(Arg.Any<AnotherFakeEvent>());
	}

	[Test]
	public void Publish_OnlyInvokesHandlersOfMatchingEventType() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		IEventHandler<AnotherFakeEvent> anotherHandler = Substitute.For<IEventHandler<AnotherFakeEvent>>();
		EventBus.AddHandler(handler);

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
		anotherHandler.DidNotReceive().OnEvent(Arg.Any<AnotherFakeEvent>());
	}

	[Test]
	public void Publish_InvokesHandlersBeforeListeners() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddHandler(handler);
		EventBus.AddListener(listener);

		bool listenerReceivedAfterHandler = false;
		bool handlerReceived = false;
		handler.When(h => h.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => handlerReceived = true);
		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => listenerReceivedAfterHandler = handlerReceived);

		EventBus.Publish(new FakeEvent());
		Assert.That(listenerReceivedAfterHandler, Is.True);
	}

	[Test]
	public void Publish_HandlersAndListenersAreBothInvoked() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddHandler(handler);
		EventBus.AddListener(listener);

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Listener_CanPublishNewEvent() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<AnotherFakeEvent> otherListener = Substitute.For<IEventListener<AnotherFakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddListener(otherListener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => EventBus.Publish(new AnotherFakeEvent()));

		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
		otherListener.Received().OnEvent(Arg.Any<AnotherFakeEvent>());
	}

	[Test]
	public void Handler_CanPublishNewEvent() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		IEventHandler<AnotherFakeEvent> otherHandler = Substitute.For<IEventHandler<AnotherFakeEvent>>();
		EventBus.AddHandler(handler);
		EventBus.AddHandler(otherHandler);

		handler.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => EventBus.Publish(new AnotherFakeEvent()));

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
		otherHandler.Received().OnEvent(Arg.Any<AnotherFakeEvent>());
	}

	[Test]
	public void NestedPublish_DoesNotCauseImmediateExecution() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<AnotherFakeEvent> otherListener = Substitute.For<IEventListener<AnotherFakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddListener(otherListener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => {
				EventBus.Publish(new AnotherFakeEvent());
				otherListener.DidNotReceive().OnEvent(Arg.Any<AnotherFakeEvent>());
				otherListener.ClearReceivedCalls();
			}
		);
		EventBus.Publish(new FakeEvent());
	}

	[Test]
	public void NestedPublish_ExecutesAfterCurrentEventFinishes() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<AnotherFakeEvent> otherListener = Substitute.For<IEventListener<AnotherFakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddListener(otherListener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => {
				EventBus.Publish(new AnotherFakeEvent());
				otherListener.DidNotReceive().OnEvent(Arg.Any<AnotherFakeEvent>());
				otherListener.ClearReceivedCalls();
			}
		);
		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
		otherListener.Received().OnEvent(Arg.Any<AnotherFakeEvent>());
	}

	[Test]
	public void MultipleNestedPublishes_AllProcessedInOrder() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		IEventListener<AnotherFakeEvent> nestedListener1 = Substitute.For<IEventListener<AnotherFakeEvent>>();
		IEventListener<YetAnotherFakeEvent> nestedListener2 = Substitute.For<IEventListener<YetAnotherFakeEvent>>();
		EventBus.AddListener(listener);
		EventBus.AddListener(nestedListener1);
		EventBus.AddListener(nestedListener2);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => EventBus.Publish(new AnotherFakeEvent()));
		bool nested1Received = false;
		bool nested2ReceivedAfterNested1 = false;
		nestedListener1.When(l => l.OnEvent(Arg.Any<AnotherFakeEvent>()))
			.Do(_ => {
				nested1Received = true;
				EventBus.Publish(new YetAnotherFakeEvent());
			}
		);
		nestedListener2.When(l => l.OnEvent(Arg.Any<YetAnotherFakeEvent>()))
			.Do(_ => nested2ReceivedAfterNested1 = nested1Received);

		EventBus.Publish(new FakeEvent());
		Assert.That(nested1Received, Is.True);
		Assert.That(nested2ReceivedAfterNested1, Is.True);
	}

	[Test]
	public void IsDispatching_ReturnsTrueDuringDispatch() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => Assert.That(EventBus.IsDispatching(), Is.True));
		EventBus.Publish(new FakeEvent());
	}

	[Test]
	public void IsDispatching_ReturnsFalseAfterDispatchCompletes() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => Assert.That(EventBus.IsDispatching(), Is.True));

		EventBus.Publish(new FakeEvent());
		Assert.That(EventBus.IsDispatching(), Is.False);
	}

	[Test]
	public void Listener_CanUnsubscribeDuringDispatch() {
		IEventListener<FakeEvent> listener = Substitute.For<IEventListener<FakeEvent>>();
		EventBus.AddListener(listener);

		listener.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => EventBus.RemoveListener(listener));

		EventBus.Publish(new FakeEvent());
		listener.Received().OnEvent(Arg.Any<FakeEvent>());
		listener.ClearReceivedCalls();

		EventBus.Publish(new FakeEvent());
		listener.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}

	[Test]
	public void Handler_CanUnsubscribeDuringDispatch() {
		IEventHandler<FakeEvent> handler = Substitute.For<IEventHandler<FakeEvent>>();
		EventBus.AddHandler(handler);

		handler.When(l => l.OnEvent(Arg.Any<FakeEvent>()))
			.Do(_ => EventBus.RemoveHandler(handler));

		EventBus.Publish(new FakeEvent());
		handler.Received().OnEvent(Arg.Any<FakeEvent>());
		handler.ClearReceivedCalls();

		EventBus.Publish(new FakeEvent());
		handler.DidNotReceive().OnEvent(Arg.Any<FakeEvent>());
	}
}
