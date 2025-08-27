using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

public class RefBox<T> where T : class {
	public T? value;

	public RefBox(T? value = null) => this.value = value;

	public static explicit operator T?(RefBox<T> refBox) => refBox.value;

	[Obsolete("Refbox instances cannot be compared to null. Compare RefBox.value instead.", true)]
	public static bool operator ==(RefBox<T> a, object? b) => throw new NotImplementedException();

	[Obsolete("Refbox instances cannot be compared to null. Compare RefBox.value instead.", true)]
	public static bool operator !=(RefBox<T> a, object? b) => throw new NotImplementedException();

	public override bool Equals(object? obj) => base.Equals(obj);
	public override int GetHashCode() => base.GetHashCode();
}