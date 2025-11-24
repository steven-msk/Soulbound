using NUnit.Framework;
using SoulboundBackend.Client.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionConcurrencyTests {
	[TestFixture]
	public class ActionConcurrencyTests {
	}

	//Request.Create().Execute(() => { }).WithPriority(2);
	//Request.Create().Execute(() => { }).OnCondition(() => true).WithPriority(5);
	//Request.Create().Execute(() => { }).OnCondition(() => true).And(() => false).WithPriority(1);

	[TestFixture]
	public class ActionBinderTests {
		[Test]
		public void ActionBinder_Execute_SetsAction() {
			bool called = false;
			Action testAction = () => called = true;

			var binder = Request.Create().Execute(testAction);
			var request = binder.GetAction();

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
			var request = binder.GetAction();

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
}
