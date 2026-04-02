using SoulboundEngine.Common.Patterns;
using System;
using System.Linq.Expressions;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class NumericAttributeModifier<T> : IAttributeModifier<T> where T : struct, IComparable<T> {
		private readonly T amount;
		private readonly RefActionProvider<NumericAttributeModifier<T>, T> operationProvider;

		public NumericAttributeModifier(RefActionProvider<NumericAttributeModifier<T>, T> operationProvider, T amount) {
			this.amount = amount;
			this.operationProvider = operationProvider;
		}

		public void Apply(ref T value) {
			operationProvider(this)(ref value);
		}

		public void Add(ref T value) {
			Func<T, T, T> add = GetBinaryExpression(Expression.Add);
			ComputeValue(add, ref value);
		}

		public void Subtract(ref T value) {
			Func<T, T, T> subtract = GetBinaryExpression(Expression.Subtract);
			ComputeValue(subtract, ref value);
		}

		public void Multiply(ref T value) {
			Func<T, T, T> multiply = GetBinaryExpression(Expression.Multiply);
			ComputeValue(multiply, ref value);
		}

		public void Divide(ref T value) {
			Func<T, T, T> divide = GetBinaryExpression(Expression.Divide);
			ComputeValue(divide, ref value);
		}

		private Func<T, T, T> GetBinaryExpression(Func<Expression, Expression, BinaryExpression> expressionSupplier) {
			ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
			ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
			BinaryExpression body = expressionSupplier(paramA, paramB);

			return Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
		}

		private void ComputeValue(Func<T, T, T> operation, ref T value) {
			T wrap = value;
			value = operation(wrap, amount);
		}
	}

	public static class NumericModifiers {
		public static NumericAttributeModifier<T> Add<T>(T amount) where T : struct, IComparable<T> {
			return new NumericAttributeModifier<T>(m => m.Add, amount);
		}

		public static NumericAttributeModifier<T> Subtract<T>(T amount) where T : struct, IComparable<T> {
			return new NumericAttributeModifier<T>(m => m.Subtract, amount);
		}

		public static NumericAttributeModifier<T> Multiply<T>(T amount) where T : struct, IComparable<T> {
			return new NumericAttributeModifier<T>(m => m.Multiply, amount);
		}

		public static NumericAttributeModifier<T> Divide<T>(T amount) where T : struct, IComparable<T> {
			return new NumericAttributeModifier<T>(m => m.Divide, amount);
		}
	}
}
