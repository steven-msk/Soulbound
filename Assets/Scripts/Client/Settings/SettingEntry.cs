using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common.Logging;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public abstract class SettingEntry {
		protected static readonly Logger logger = Logger.CreateInstance();
		public readonly string id;
		public readonly Func<Tooltip> tooltipSupplier;
		public abstract object boxedDefaultValue { get; }
		public abstract object boxedValue { get; }
		public abstract Type valueType { get; }

		protected SettingEntry(string id, Func<Tooltip> tooltipSupplier) {
			this.id = id;
			this.tooltipSupplier = tooltipSupplier;
		}
	}

	public class SettingEntry<T> : SettingEntry {
		public readonly T defaultValue;
		public readonly ValueSet<T> valueSet;
		public event Action<T, T>? valueChanged;
		public T value { get; private set; }

		public override object boxedDefaultValue => defaultValue!;
		public override object boxedValue => value!;
		public override Type valueType => typeof(T);

		public SettingEntry(string id, T defaultValue, ValueSet<T> valueSet, Func<Tooltip> tooltipSupplier)
			: base(id, tooltipSupplier) {
			this.defaultValue = defaultValue;
			this.valueSet = valueSet;
			this.value = defaultValue;
		}

		public SettingEntry(string id, T defaultValue, ValueSet<T> valueSet, Func<Tooltip> tooltipSupplier, Action<T, T> valueChanged)
			: this(id, defaultValue, valueSet, tooltipSupplier) {
			this.valueChanged += valueChanged;
		}

		public void SetValue(T value, bool broadcastChange = true) {
			var oldValue = this.value;
			if (valueSet.IsValid(value)) {
				this.value = value;
				if (broadcastChange) {
					valueChanged?.Invoke(oldValue, this.value);
				}
			} else {
				logger.LogWarning(null, "Attempted to set invalid value {} to setting {}", value!, id);
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

		public override SettingVisual<float> GetVisual(Transform parent) {
			Slider slider = SliderFactory.CreateSlider(parent);
			slider.minValue = minInclusive;
			slider.maxValue = maxInclusive;

			SliderSetting sliderSetting = slider.AddComponent<SliderSetting>();
			sliderSetting.slider = slider;
			sliderSetting.tooltipTrigger = slider.AddComponent<TooltipTrigger>();
			return sliderSetting;
		}
	}
}
