using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDependencyInitializable<TInstance, TDependency> {
	public TInstance OnGameInit(TDependency dependency);
}
