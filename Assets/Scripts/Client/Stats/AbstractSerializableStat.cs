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
	public abstract bool applyAsBonus { get; }
}
