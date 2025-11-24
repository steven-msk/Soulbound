using NUnit.Framework;
using SoulboundBackend.Client.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace ActionConcurrencyTests {
	[TestFixture]
	public class ActionConcurrencyTests {
		[Test]
		public void ExecuteByPriority_ExecutesHighestPriorityRequest() {
			bool[] executed = { false, false, false };
			var requests = new[] {
				Request.Execute(() => executed[0] = true).WithPriority(1).GetRequest(),
				Request.Execute(() => executed[1] = true).WithPriority(2).GetRequest(),
				Request.Execute(() => executed[2] = true).WithPriority(3).GetRequest()
			};

			new ConcurrentActionResolver().ExecuteByPriority(requests);

			Assert.That(executed.SequenceEqual(new[] { false, false, true }));
		}

		[Test]
		public void ExecuteByPriority_ExecutesInDescendingOrder_ForNonExclusivePriority() {
			var requests = new[] {
				Request.Execute(() => Debug.Log("executed 1")).WithPriority(10).NonExclusive().GetRequest(),
				Request.Execute(() => Debug.Log("executed 2")).WithPriority(5).NonExclusive().GetRequest(),
				Request.Execute(() => Debug.Log("executed 3")).WithPriority(1).NonExclusive().GetRequest()
			};

			LogAssert.Expect("executed 1");
			LogAssert.Expect("executed 2");
			LogAssert.Expect("executed 3");
			new ConcurrentActionResolver().ExecuteByPriority(requests);
		}

		[Test]
		public void ExecuteByPriority_ExecutesInOrderOfSubmission_ForIdenticalNonExclusivePriority() {
			var requests = new[] {
				Request.Execute(() => Debug.Log("executed first")).WithPriority(10).NonExclusive().GetRequest(),
				Request.Execute(() => Debug.Log("executed second")).WithPriority(10).NonExclusive().GetRequest(),
				Request.Execute(() => Debug.Log("executed third")).WithPriority(10).NonExclusive().GetRequest()
			};

			LogAssert.Expect("executed first");
			LogAssert.Expect("executed second");
			LogAssert.Expect("executed third");
			new ConcurrentActionResolver().ExecuteByPriority(requests);
		}
	}

	//Request.Create().Execute(() => { }).WithPriority(2);
	//Request.Create().Execute(() => { }).OnCondition(() => true).WithPriority(5);
	//Request.Create().Execute(() => { }).OnCondition(() => true).And(() => false).WithPriority(1);
	//Request.Create().Execute(() => { }).OnCondition(() => true).And(() => false).WithPriority(0).NonExclusive();

	[TestFixture]
	public class ActionBinderTests {
		[Test]
		public void ActionBinder_Execute_SetsAction() {
			bool called = false;
			Action testAction = () => called = true;

			var binder = Request.Create().Execute(testAction);
			var request = binder.GetRequest();

			Assert.NotNull(request.action);
			request.action.Invoke();
			Assert.IsTrue(called);
		}

		[Test]
		public void ActionBinder_Execute_ThrowsForNull() {
			Assert.Throws<ArgumentNullException>(() => Request.Create().Execute(null));
		}

		[Test]
		public void ActionBinder_Execute_ReturnsConditionBinder() {
			var binder = Request.Create().Execute(() => { });
			Assert.That(binder is ConditionBinder);
		}
	}

	[TestFixture]
	public class ConditionBinderTests {
		[Test]
		public void ConditionBinder_OnCondition_AddsConditionPredicate() {
			Func<bool> predicate = () => true;
			var binder = Request.Create().Execute(() => { }).OnCondition(predicate);
			var request = binder.GetRequest();

			Assert.That(request.conditions.Contains(predicate));
		}

		[Test]
		public void ConditionBinder_OnCondition_ReturnsConditionChainBinder() {
			var binder = Request.Create().Execute(() => { }).OnCondition(() => true);
			Assert.That(binder is ConditionChainBinder);
		}
	}


	[TestFixture]
	public class ConditionChainTests {
		[Test]
		public void ConditionChainBinder_And_ReturnsSameInstance() {
			var binder = Request.Create().Execute(() => { }).OnCondition(() => true);
			var chained = binder.And(() => false);
			Assert.That(binder == chained);
		}

		[Test]
		public void ConditionChainBinder_And_ThrowsForNull() {
			Assert.Throws<ArgumentNullException>(() => {
				Request.Create().Execute(() => { }).OnCondition(() => true).And(null);
			});
		}
	}

	[TestFixture]
	public class PriorityBinderTests {
		[Test]
		public void PriorityBinder_WithPriority_SetsPriority() {
			var binder = Request.Create().Execute(() => { }).WithPriority(100);
			var request = binder.GetRequest();

			Assert.That(request.priority, Is.EqualTo(100));
		}

		[Test]
		public void PriorityBinder_WithPriority_ReturnsPriorityTypeBinder() {
			var binder = Request.Create().Execute(() => { }).WithPriority(1);
			Assert.That(binder is PriorityTypeBinder);
		}

		[Test]
		public void Priority_DefaultsTo0() {
			var binder = Request.Execute(() => { });
			var request = binder.GetRequest();
			Assert.That(request.priority == 0);
		}
	}

	[TestFixture]
	public class PriorityTypeTests {
		[Test]
		public void PriorityTypeBinder_Exclusive_SetsPriorityTypeExclusive() {
			var binder = Request.Create().Execute(() => { }).Exclusive();
			var request = binder.GetRequest();

			Assert.That(request.priorityType == PriorityType.Exclusive);
		}

		[Test]
		public void PriorityTypeBinder_DefaultsToExclusive() {
			var binder = Request.Create().Execute(() => { });
			var request = binder.GetRequest();

			Assert.That(request.priorityType == PriorityType.Exclusive);
		}

		[Test]
		public void PriorityTypeBinder_ExclusiveAndNonExclusive_ReturnAbstractBinder() {
			var exclusiveBinder = Request.Create().Execute(() => { }).Exclusive();
			var nonExclusiveBinder = Request.Create().Execute(() => { }).NonExclusive();

			Assert.That(exclusiveBinder is AbstractActionRequestBinder);
			Assert.That(nonExclusiveBinder is AbstractActionRequestBinder);
		}
	}
}
