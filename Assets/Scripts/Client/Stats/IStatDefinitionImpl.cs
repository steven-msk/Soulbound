using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatDefinitionImpl {
	string baseName { get; }
	Type valueType { get; }
	StatApplicationType validApplications { get; }

	string GetFormattedName(object value);

	string GetFormattedValue(object value, bool applyAsBonus);

	virtual string GetFormattedExpression(object value, bool applyAsBonus = false) {
		return $"{this.GetFormattedValue(value, applyAsBonus)} {this.GetFormattedName(value)}";
	}
}