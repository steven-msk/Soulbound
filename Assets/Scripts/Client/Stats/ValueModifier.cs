
using SoulboundBackend.Client.ItemSystem;
using System;
using System.Linq.Expressions;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class ValueModifier<TValue> : AbstractValueModifier, IStatEntryModifier<TValue> 
			where TValue : struct, IComparable<TValue> {
		public readonly TValue value;
		public override bool keepSign { get; }

		public ValueModifier(
			TValue value,
			bool keepSign,
			StatApplicationType applicationType = StatApplicationType.Flat
		) 
			: base(applicationType) {
			this.value = value;
			this.keepSign = keepSign;
		}

		public virtual void Apply(StatEntry<TValue> entry, ModificationToken modificationToken) {
			var context = new ValueModificationContext<TValue>(this, entry);

			if (context.IsValid()) {
				entry.CommitModifier(this, modificationToken, new Add());
			}
		}

		public virtual void Remove(StatEntry<TValue> entry, ModificationToken modificationToken) {
			entry.UncommitModifier(this, modificationToken);
		}

		public override object GetBoxedValue() => value;

		public override string ToString() {
			return $"ValueModifier[type: {typeof(TValue)}, " +
				   $"value: {value}, " +
				   $"applicationType: {applicationType}, " +
				   $"keepSign: {keepSign}]";
		}

		public override object Clone() {
			return new ValueModifier<TValue>(
				this.value, 
				this.keepSign,
				this.applicationType
			);
		}

		public abstract class MathProcedure {
			protected Func<TValue, TValue, TValue> GetDelegate(Func<Expression, Expression, BinaryExpression> expression) {
				var paramA = Expression.Parameter(typeof(TValue), "a");
				var paramB = Expression.Parameter(typeof(TValue), "b");
				var body = expression(paramA, paramB);

				return Expression.Lambda<Func<TValue, TValue, TValue>>(body, paramA, paramB).Compile();
			}

			protected TValue GetValue(Func<TValue, TValue, TValue> lambda, TValue currentValue, IStatEntryModifier<TValue> modifier) {
				if (modifier is not ValueModifier<TValue> typed) {
					throw new ArgumentException($"Mistyped math procedure modifier, expected ValueModifier<TValue> but is {modifier.GetType()}");
				}
				return lambda(currentValue, typed.value);
			}
		}

		public class Add : MathProcedure, IModificationProcedure<TValue> {
			public TValue Apply(TValue currentValue, IStatEntryModifier<TValue> modifier, StatEntry<TValue> entry) {
				return base.GetValue(base.GetDelegate(Expression.Add), currentValue, modifier);
			}
		}

		public class Subtract : MathProcedure, IModificationProcedure<TValue> {
			public TValue Apply(TValue currentValue, IStatEntryModifier<TValue> modifier, StatEntry<TValue> entry) {
				return base.GetValue(base.GetDelegate(Expression.Subtract), currentValue, modifier);
			}
		}

		public class Multiply : MathProcedure, IModificationProcedure<TValue> {
			public TValue Apply(TValue currentValue, IStatEntryModifier<TValue> modifier, StatEntry<TValue> entry) {
				return base.GetValue(base.GetDelegate(Expression.Multiply), currentValue, modifier);
			}
		}

		public class Divide : MathProcedure, IModificationProcedure<TValue> {
			public TValue Apply(TValue currentValue, IStatEntryModifier<TValue> modifier, StatEntry<TValue> entry) {
				return base.GetValue(base.GetDelegate(Expression.Divide), currentValue, modifier);
			}
		}
	}
}
