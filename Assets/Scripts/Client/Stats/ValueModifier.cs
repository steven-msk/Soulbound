using SoulboundBackend.Common.Logging;
using System;
using Unity.Plastic.Newtonsoft.Json;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public class ValueModifier<TValue> : AbstractValueModifier, IStatEntryModifier<TValue> 
			where TValue : struct, IComparable<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
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
				entry.CommitModifier(this, modificationToken);
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

		public override int GetHashCode() {
			return HashCode.Combine(value, keepSign, applicationType);
		}
	}
}
