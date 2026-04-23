using SoulboundEngine.Client.SettingSystem.View;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.SettingSystem {
	public abstract class SettingEntry {
		public readonly string displayName;
		public readonly string id;
		public abstract object boxedDefaultValue { get; }
		public abstract object boxedValue { get; }
		public abstract Type valueType { get; }

		protected SettingEntry(string name, string id) {
			this.displayName = name;
			this.id = id;
		}

		public override string ToString() {
			return $"{displayName}={boxedValue}";
		}
	}

	public class SettingEntry<T> : SettingEntry {
		public readonly T defaultValue;
		public readonly ValueSet<T> valueSet;
		public event Action<T, T>? valueChanged;
		public virtual T value { get; protected set; }

		public override object boxedDefaultValue => defaultValue!;
		public override object boxedValue => value!;
		public override Type valueType => typeof(T);

		public SettingEntry(string displayName, string id, T defaultValue, ValueSet<T> valueSet)
			: base(displayName, id) {
			this.defaultValue = defaultValue;
			this.valueSet = valueSet;
			this.value = defaultValue;
		}

		public SettingEntry(string displayName, string id, T defaultValue, ValueSet<T> valueSet, Action<T, T> valueChanged)
			: this(displayName, id, defaultValue, valueSet) {
			this.valueChanged += valueChanged;
		}

		public void SetValue(T value) {
			if (value?.Equals(this.value) ?? true) return;
			var oldValue = this.value;

			if (valueSet.IsValid(value)) {
				this.value = value;
				valueChanged?.Invoke(oldValue, this.value);
			} else {
				Logger.LogWarning("Attempted to set invalid value '{}' to setting '{}'", value!, id);
			}
		}
	}

	public abstract record ValueSet<T> : IStringCodec<T> {
		public abstract bool IsValid(T value);
		public abstract T? Decode(string value);
		public abstract string Encode(T? value);
		public abstract SettingVisual<T> GetVisual(Transform parent);
	}

	public record EnumValueSet<T>(T[] acceptedValues) : ValueSet<T> where T : struct, Enum {
		public override T Decode(string value) => Enum.Parse<T>(value);

		public override string Encode(T value) => value.ToString();

		public override bool IsValid(T value) {
			return acceptedValues.Contains(value);
		}

		public override SettingVisual<T> GetVisual(Transform parent) {
			throw new NotImplementedException();
		}
	}

	public record StringEnum(string[] acceptedValues) : ValueSet<string> {
		public override string Decode(string value) => value;

		public override string Encode(string? value) => value ?? string.Empty;

		public override bool IsValid(string value) {
			return acceptedValues.Contains(value);
		}

		public override SettingVisual<string> GetVisual(Transform parent) {
			throw new NotImplementedException();
		}
	}

	public abstract record SlidableValueSet<T>(T minInclusive, T maxInclusive) : ValueSet<T> where T : struct, IComparable<T> {
		public override bool IsValid(T value) {
			return value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0;
		}
		public override string Encode(T value) => value.ToString();
	}

	public record IntRange : SlidableValueSet<int> {
		public IntRange(int minInclusive, int maxInclusive) 
			: base(minInclusive, maxInclusive) {
		}

		public override int Decode(string value) => int.Parse(value);

		public override SettingVisual<int> GetVisual(Transform parent) {
			throw new NotImplementedException();
		}
	}

	public record DoubleRange : SlidableValueSet<double> {
		public DoubleRange(double minInclusive, double maxInclusive)
			: base(minInclusive, maxInclusive) {
		}

		public override double Decode(string value) => double.Parse(value);

		public override SettingVisual<double> GetVisual(Transform parent) {
			throw new NotImplementedException();
		}
	}

	public record FloatRange : SlidableValueSet<float> {
		public FloatRange(float minInclusive, float maxInclusive)
			: base(minInclusive, maxInclusive) {
		}

		public override float Decode(string value) => float.Parse(value);

		[PROTOTYPICAL]
		public override SettingVisual<float> GetVisual(Transform parent) {
			Slider slider = SliderFactory.CreateSlider(parent);
			slider.minValue = minInclusive;
			slider.maxValue = maxInclusive;

			SliderSetting sliderSetting = slider.gameObject.AddComponent<SliderSetting>();
			sliderSetting.slider = slider;
			return sliderSetting;
		}
	}
}
