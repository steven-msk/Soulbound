using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatTypeImpl {
	abstract string BaseName { get; }

	abstract string GetFormattedName(object value);

	abstract string GetFormattedValue(object value, bool applyAsBonus);

	virtual string GetFormattedExpression(object value, bool applyAsBonus = false) {
		return $"{this.GetFormattedValue(value, applyAsBonus)} {this.GetFormattedName(value)}";
	}
}