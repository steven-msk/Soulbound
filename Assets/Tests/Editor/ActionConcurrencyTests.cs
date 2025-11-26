using NUnit.Framework;
using SoulboundBackend.Client.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace ActionConcurrencyTests {
	[TestFixture]
	public class ActionConcurrencyTests {
		[Test]
		public void ExecuteByPriority_ExecutesHighestPriorityRequest() {
			bool[] executed = { false, false, false };
			var requests = new[] {
				Request.New().Execute(() => executed[0] = true)
					.WithPriority(1).GetRequest(),
				Request.New().Execute(() => executed[1] = true)
					.WithPriority(2).GetRequest(),
				Request.New().Execute(() => executed[2] = true)
					.WithPriority(3).GetRequest()
			};

			new ConcurrentActionResolver().ExecuteByPriority(requests);

			Assert.That(executed.SequenceEqual(new[] { false, false, true }));
		}

		[Test]
		public void ExecuteByPriority_ExecutesInDescendingOrder_ForNonExclusivePriority() {
			var requests = new[] {
				Request.New().Execute(() => Debug.Log("executed 1"))
					.WithPriority(10).NonExclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed 2"))
					.WithPriority(5).NonExclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed 3"))
					.WithPriority(1).NonExclusive()
					.GetRequest()
			};

			LogAssert.Expect("executed 1");
			LogAssert.Expect("executed 2");
			LogAssert.Expect("executed 3");
			new ConcurrentActionResolver().ExecuteByPriority(requests);
		}

		[Test]
		public void ExecuteByPriority_ExecutesInOrderOfSubmission_ForIdenticalNonExclusivePriority() {
			var requests = new[] {
				Request.New().Execute(() => Debug.Log("executed first"))
					.WithPriority(10).NonExclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed second"))
					.WithPriority(10).NonExclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed third"))
					.WithPriority(10).NonExclusive()
					.GetRequest()
			};

			LogAssert.Expect("executed first");
			LogAssert.Expect("executed second");
			LogAssert.Expect("executed third");
			new ConcurrentActionResolver().ExecuteByPriority(requests);
		}

		[Test]
		public void ExecuteByPriority_DiscardsExclusivePriorities() {
			var requests = new[] {
				Request.New().Execute(() => Debug.Log("executed 1"))
					.WithPriority(1).NonExclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed 2"))
					.WithPriority(2).Exclusive()
					.GetRequest(),
				Request.New().Execute(() => Debug.Log("executed 3"))
					.WithPriority(3).NonExclusive()
					.GetRequest()
			};

			LogAssert.Expect("executed 3");
			LogAssert.Expect("executed 1");
			new ConcurrentActionResolver().ExecuteByPriority(requests);
		}

		[Test]
		public void SolveConditions_ReturnsRequests_ForAllWhichConditionsAreTrue() {
			var request1 = Request.New().Execute(() => { })
				.OnCondition(() => true)
				.GetRequest();
			var request2 = Request.New().Execute(() => { })
				.OnCondition(() => false).And(() => true)
				.GetRequest();
			var request3 = Request.New().Execute(() => { })
				.OnCondition(() => true).And(() => true)
				.GetRequest();

			var valid = new ConcurrentActionResolver().SolveConditions(new[] { request1, request2, request3 });

			Assert.That(valid.SequenceEqual(new[] { request1, request3 }));
		}

		[Test]
		public void ProcessSuppressions_RemovesSuppressedRequests() {
			var token = new ActionToken(1);
			var request1 = Request.New()
				.UnderToken(token).Execute(() => { })
				.NonExclusive()
				.GetRequest();
			var request2 = Request.New().Execute(() => { })
				.NonExclusive()
				.Suppress(token, () => false)
				.GetRequest();
			
			var actionResolver = new ConcurrentActionResolver();
			actionResolver.Submit(request1);
			actionResolver.Submit(request2);

			((ILateTickable)actionResolver).LateTick();

			var notSuppressed = actionResolver.ProcessSuppressions(new[] { request1, request2 });
			Assert.That(notSuppressed.SequenceEqual(new[] { request2 }));
		}

		[Test]
		public void UnsuppressedRequests_AreExecuted() {
			var token = new ActionToken(1);
			bool request1Called = false;
			bool suppressed = false;

			var request1 = Request.New()
				.UnderToken(token).Execute(() => request1Called = true)
				.NonExclusive()
				.GetRequest();
			var request2 = Request.New().Execute(() => suppressed = true)
				.WithPriority(1).NonExclusive()
				.Suppress(token, () => !suppressed)
				.GetRequest();

			var actionResolver = new ConcurrentActionResolver();
			actionResolver.Submit(request1);
			actionResolver.Submit(request2);

			((ILateTickable)actionResolver).LateTick();
			Assert.IsTrue(suppressed);
			Assert.IsFalse(request1Called);
			Assert.That(actionResolver.IsSuppressed(token));
			var notSuppressed = actionResolver.ProcessSuppressions(new[] { request1, request2 });
			Assert.That(notSuppressed.SequenceEqual(new[] { request2 }));

			actionResolver.Submit(request1);
			suppressed = false;

			((ILateTickable)actionResolver).LateTick();
			Assert.That(!actionResolver.IsSuppressed(token));
			Assert.IsTrue(request1Called);
			notSuppressed = actionResolver.ProcessSuppressions(new[] { request1, request2 });
			Assert.That(notSuppressed.SequenceEqual(new[] { request1, request2 }));
		}

		[Test]
		public void LateTick_ExecutesRequest() {
			bool executed = false;
			var request = Request.New().Execute(() => executed = true);

			var actionResolver = new ConcurrentActionResolver();
			actionResolver.Submit(request);

			((ILateTickable)actionResolver).LateTick();

			Assert.IsTrue(executed);
		}
	}

	//Request.New().Execute.Execute(() => { }).WithPriority(2);
	//Request.New().Execute.Execute(() => { }).OnCondition(() => true).WithPriority(5);
	//Request.New().Execute.Execute(() => { }).OnCondition(() => true).And(() => false).WithPriority(1);
	//Request.New().Execute.Execute(() => { }).OnCondition(() => true).And(() => false).WithPriority(0).NonExclusive();
	//Request.New().Execute.UnderToken(token).Execute(() => { });
	//Request.New().Execute.Execute(() => { });
	//Request.New().Execute(() => { }).NonExclusive().Suppress(token, () => false)

	[TestFixture]
	public class ActionBinderTests {
		[Test]
		public void ActionBinder_Execute_SetsAction() {

			bool called = false;
			Action testAction = () => called = true;

			var binder = Request.New().Execute(testAction);
			var request = binder.GetRequest();

			Assert.NotNull(request.action);
			request.action.Invoke();
			Assert.IsTrue(called);
		}

		[Test]
		public void ActionBinder_Execute_ThrowsForNull() {
			Assert.Throws<ArgumentNullException>(() => Request.New().Execute(null));
		}

		[Test]
		public void ActionBinder_Execute_ReturnsConditionBinder() {
			var binder = Request.New().Execute(() => { });
			Assert.That(binder is ConditionBinder);
		}
	}

	[TestFixture]
	public class ConditionBinderTests {
		[Test]
		public void ConditionBinder_If_AddsConditionPredicate() {
			Func<bool> predicate = () => true;
			var binder = Request.New().Execute(() => { }).OnCondition(predicate);
			var request = binder.GetRequest();

			Assert.That(request.conditions.Contains(predicate));
		}

		[Test]
		public void ConditionBinder_If_ReturnsConditionChainBinder() {
			var binder = Request.New().Execute(() => { }).OnCondition(() => true);
			Assert.That(binder is ConditionChainBinder);
		}
	}


	[TestFixture]
	public class ConditionChainTests {
		[Test]
		public void ConditionChainBinder_And_ReturnsSameInstance() {
			var binder = Request.New().Execute(() => { }).OnCondition(() => true);
			var chained = binder.And(() => false);
			Assert.That(binder == chained);
		}

		[Test]
		public void ConditionChainBinder_And_ThrowsForNull() {
			Assert.Throws<ArgumentNullException>(() => {
				Request.New().Execute(() => { }).OnCondition(() => true).And(null);
			});	
		}
	}

	[TestFixture]
	public class PriorityBinderTests {
		[Test]
		public void PriorityBinder_WithPriority_SetsPriority() {
			var binder = Request.New().Execute(() => { }).WithPriority(100);
			var request = binder.GetRequest();

			Assert.That(request.priority, Is.EqualTo(100));
		}

		[Test]
		public void PriorityBinder_WithPriority_ReturnsPriorityTypeBinder() {
			var binder = Request.New().Execute(() => { }).WithPriority(1);
			Assert.That(binder is PriorityTypeBinder);
		}

		[Test]
		public void Priority_DefaultsTo0() {
			var binder = Request.New().Execute(() => { });
			var request = binder.GetRequest();
			Assert.That(request.priority == 0);
		}
	}

	[TestFixture]
	public class PriorityTypeTests {
		[Test]
		public void PriorityTypeBinder_Exclusive_SetsPriorityTypeExclusive() {
			var binder = Request.New().Execute(() => { }).Exclusive();
			var request = binder.GetRequest();

			Assert.That(request.priorityType == PriorityType.Exclusive);
		}

		[Test]
		public void PriorityTypeBinder_DefaultsToExclusive() {
			var binder = Request.New().Execute(() => { });
			var request = binder.GetRequest();

			Assert.That(request.priorityType == PriorityType.Exclusive);
		}

		[Test]
		public void PriorityTypeBinder_ExclusiveAndNonExclusive_ReturnAbstractBinder() {
			var exclusiveBinder = Request.New().Execute(() => { }).Exclusive();
			var nonExclusiveBinder = Request.New().Execute(() => { }).NonExclusive();

			Assert.That(exclusiveBinder is AbstractActionRequestBinder);
			Assert.That(nonExclusiveBinder is AbstractActionRequestBinder);
		}
	}

	[TestFixture]
	public class TokenBinderTests {
		[Test]
		public void TokenBinder_UnderToken_SetsActionToken() {
			var token = new ActionToken(10);
			var binder = Request.New().UnderToken(token).Execute(() => { });
			var request = binder.GetRequest();

			Assert.That(request.token, Is.EqualTo(token));
		}
	}

	[TestFixture]
	public class SuppressionBinderTests {
		[Test]
		public void SuppressionBinder_Suppress_AddsTokenToConditionMapping() {
			var token1 = new ActionToken(1);
			var token2 = new ActionToken(2);
			var binder = Request.New().Execute(() => { })
				.Suppress(token1, () => true)
				.Suppress(token2, () => false);
			var request = binder.GetRequest();

			Assert.That(request.suppressions.ContainsKey(token1));
			Assert.That(request.suppressions.ContainsKey(token2));
		}
	}
}
