using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class ObservableRef<T> where T : class {
	public T? value;
	public T? Value { 
		get => value;
		set {
			if (!EqualityComparer<T?>.Default.Equals(value, this.value)) {
				this.value = value;
				OnValueChanged?.Invoke(value);
			}
		}
	}

	public event Action<T?>? OnValueChanged;

	public ObservableRef(T? initialValue) {
		this.value = initialValue;
	}

	[Obsolete("ObservableRef instances cannot be compared to null. Compare ObservableRef.Value instead.", true)]
	public static bool operator ==(ObservableRef<T> a, object? b) => throw new NotImplementedException();

	[Obsolete("ObservableRef instances cannot be compared to null. Compare ObservableRef.Value instead.", true)]
	public static bool operator !=(ObservableRef<T> a, object? b) => throw new NotImplementedException();

	public override bool Equals(object? obj) => base.Equals(obj);
	public override int GetHashCode() => base.GetHashCode();
}
