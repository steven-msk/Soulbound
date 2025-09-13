using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AbstractSerializableStat {
	public abstract IStatDefinitionImpl GetStatDefinition();
	public abstract StatApplicationType GetApplicationType();
	public abstract object GetBoxedValue();
	public abstract string GetFormattedExpression();
	public abstract bool showAsBonus { get; }
	public abstract bool persistent { get; set; }

	public override string ToString() => GetFormattedExpression();

	public abstract string GetInfo();

	public override int GetHashCode() {
		return HashCode.Combine(this.GetFormattedExpression(), persistent);
	}

	public override bool Equals(object other) {
		return other is AbstractSerializableStat stat && stat.GetHashCode() == this.GetHashCode();
	}

	internal abstract object Clone();

	public static bool operator ==(AbstractSerializableStat first, AbstractSerializableStat second) {
		return first.Equals(second);
	}

	public static bool operator !=(AbstractSerializableStat first, AbstractSerializableStat second) {
		return !(first == second);
	}
}
