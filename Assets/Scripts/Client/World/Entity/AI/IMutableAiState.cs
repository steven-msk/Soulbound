using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMutableAIState<TAI> : IAIState where TAI : IAIState{
	public void Mutate(Action<TAI> mutateAction);
}
