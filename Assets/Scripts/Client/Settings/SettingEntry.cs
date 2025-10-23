using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public abstract class SettingEntry {
		public readonly string id;
		public abstract object boxedDefaultValue { get; }
		public abstract object boxedValue { get; }

		public SettingEntry(string id) {
			this.id = id;
		}
	}

	public class SettingEntry<T> : SettingEntry {
		public readonly T defaultValue;
		public readonly ValueSet<T> valueSet;
		public event Action<T, T>? valueChanged;
		public T value { get; private set; }

		public override object boxedDefaultValue => defaultValue!;
		public override object boxedValue => value!;

		public SettingEntry(string id, T defaultValue, ValueSet<T> valueSet)
			: base(id) {
			this.defaultValue = defaultValue;
			this.valueSet = valueSet;
			this.value = defaultValue;
		}

		public void SetValue(T value, bool broadcastChange = true) {
			var oldValue = this.value;
			if (valueSet.IsValid(value)) {
				this.value = value;
				if (broadcastChange) {
					valueChanged?.Invoke(oldValue, this.value);
				}
			}
		}
	}

	public abstract record ValueSet<T> : IStringCodec<T> {
		public abstract bool IsValid(T value);
		public abstract T? Decode(string value);
		public abstract string Encode(T? value);
	}

	public record EnumValueSet<T>(T[] acceptedValues) : ValueSet<T> where T : struct, Enum {
		public override T Decode(string value) => Enum.Parse<T>(value);

		public override string Encode(T value) => value.ToString();

		public override bool IsValid(T value) {
			return acceptedValues.Contains(value);
		}
	}

	public abstract record NumericValueSet<T>(T minInclusive, T maxInclusive) : ValueSet<T> where T : struct, IComparable<T> {
		public override bool IsValid(T value) {
			return value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) >= 0;
		}

		public override string Encode(T value) => value.ToString();
	}

	public record IntRange : NumericValueSet<int> {
		public IntRange(int minInclusive, int maxInclusive) 
			: base(minInclusive, maxInclusive) {
		}

		public override int Decode(string value) => int.Parse(value);
	}

	public record DoubleRange : NumericValueSet<double> {
		public DoubleRange(double minInclusive, double maxInclusive)
			: base(minInclusive, maxInclusive) {
		}

		public override double Decode(string value) => double.Parse(value);
	}

	public record FloatRange : NumericValueSet<float> {
		public FloatRange(float minInclusive, float maxInclusive)
			: base(minInclusive, maxInclusive) {
		}

		public override float Decode(string value) => float.Parse(value);
	}
}
