using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Tooltip;
using SoulboundBackend.Common.Logging;
using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.Settings {
	public abstract class AbstractSettingEntry {
		protected static readonly Logger logger = Logger.CreateInstance();
		public readonly string name;
		public readonly string id;
		public readonly Func<Tooltip> tooltipSupplier;
		public abstract object boxedDefaultValue { get; }
		public abstract object boxedValue { get; }
		public abstract Type valueType { get; }

		protected AbstractSettingEntry(string name, string id, Func<Tooltip> tooltipSupplier) {
			this.name = name;
			this.id = id;
			this.tooltipSupplier = tooltipSupplier;
		}
	}

	public class SettingEntry<T> : AbstractSettingEntry {
		public readonly T defaultValue;
		public readonly ValueSet<T> valueSet;
		public event Action<T, T>? valueChanged;
		public T value { get; private set; }

		public override object boxedDefaultValue => defaultValue!;
		public override object boxedValue => value!;
		public override Type valueType => typeof(T);

		public SettingEntry(string name, string id, T defaultValue, ValueSet<T> valueSet, Func<Tooltip> tooltipSupplier)
			: base(name, id, tooltipSupplier) {
			this.defaultValue = defaultValue;
			this.valueSet = valueSet;
			this.value = defaultValue;
		}

		public SettingEntry(string name, string id, T defaultValue, ValueSet<T> valueSet, Func<Tooltip> tooltipSupplier, Action<T, T> valueChanged)
			: this(name, id, defaultValue, valueSet, tooltipSupplier) {
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
				logger.LogWarning(null, "Attempted to set invalid value '{}' to setting '{}'", value!, id);
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
			const float spacing = 8f;
			const ContentSizeFitter.FitMode fitMode = ContentSizeFitter.FitMode.PreferredSize;

			GameObject settingContainer = new("Setting Container", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
			settingContainer.transform.SetParent(parent, false);
			var layout = settingContainer.GetComponent<HorizontalLayoutGroup>();
			layout.spacing = spacing;
			layout.childControlWidth = layout.childControlHeight = false;
			layout.childForceExpandWidth = layout.childForceExpandHeight = false;
			layout.childScaleWidth = layout.childScaleHeight = true;
			layout.childAlignment = TextAnchor.MiddleLeft;
			var sizeFitter = settingContainer.GetComponent<ContentSizeFitter>();
			sizeFitter.horizontalFit = sizeFitter.verticalFit = fitMode;

			GameObject nameObject = new("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
			nameObject.transform.SetParent(settingContainer.transform, false);
			TextMeshProUGUI name = nameObject.GetComponent<TextMeshProUGUI>();
			name.alignment = TextAlignmentOptions.MidlineRight;
			name.autoSizeTextContainer = true;
			name.SetText("setting_name:");

			Slider slider = SliderFactory.CreateSlider(parent);
			slider.transform.SetParent(settingContainer.transform, false);
			slider.minValue = minInclusive;
			slider.maxValue = maxInclusive;

			SliderSetting sliderSetting = settingContainer.AddComponent<SliderSetting>();
			sliderSetting.slider = slider;
			sliderSetting.tooltipTrigger = settingContainer.AddComponent<TooltipTrigger>();
			return sliderSetting;
		}
	}
}
