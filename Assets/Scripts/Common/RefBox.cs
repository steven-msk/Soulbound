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

	[Obsolete("It is not recommended to compare RefBox to null. Compare RefBox.Value instead.", true)]
	public static bool operator ==(RefBox<T> a, object? b) => throw new NotImplementedException();

	[Obsolete("It is not recommended to compare RefBox to null. Compare RefBox.Value instead.", true)]
	public static bool operator !=(RefBox<T> a, object? b) => throw new NotImplementedException();

	public override bool Equals(object? obj) => base.Equals(obj);
	public override int GetHashCode() => base.GetHashCode();
}