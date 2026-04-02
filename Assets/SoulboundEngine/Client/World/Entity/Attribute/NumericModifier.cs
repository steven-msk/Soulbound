using System;
using System.Linq.Expressions;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public abstract class NumericModifier<T> : IAttributeModifier<T> where T : struct, IComparable<T> {
		protected readonly T amount;

		public NumericModifier(T amount) {
			this.amount = amount;
		}

		public virtual void Apply(ref T value) {
			T wrap = value;
			value = ToTypedExpression(GetExpression())(wrap, amount);
		}

		protected abstract Func<Expression, Expression, BinaryExpression> GetExpression();

		protected Func<T, T, T> ToTypedExpression(Func<Expression, Expression, BinaryExpression> expressionSupplier) {
			ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
			ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
			BinaryExpression body = expressionSupplier(paramA, paramB);

			return Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
		}
	}

	public class NumericAddModifier<T> : NumericModifier<T> where T : struct, IComparable<T> {
		public NumericAddModifier(T amount) : base(amount) {
		}

		protected override Func<Expression, Expression, BinaryExpression> GetExpression() => Expression.Add;
	}

	public class NumericSubtractModifier<T> : NumericModifier<T> where T : struct, IComparable<T> {
		public NumericSubtractModifier(T amount) : base(amount) {
		}

		protected override Func<Expression, Expression, BinaryExpression> GetExpression() => Expression.Subtract;
	}

	public class NumericMultiplyModifier<T> : NumericModifier<T> where T : struct, IComparable<T> {
		public NumericMultiplyModifier(T amount) : base(amount) {
		}

		protected override Func<Expression, Expression, BinaryExpression> GetExpression() => Expression.Multiply;
	}


	public class NumericDivideModifier<T> : NumericModifier<T> where T : struct, IComparable<T> {
		public NumericDivideModifier(T amount) : base(amount) {
		}

		protected override Func<Expression, Expression, BinaryExpression> GetExpression() => Expression.Divide;
	}
}
