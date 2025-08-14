using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAIState {
	public abstract bool isInterruptable { get; }
	public abstract bool isFinished { get; }

	void OnEnter();
	void OnExit();
	void OnUpdate(float deltaTime);
	void Tick();
}
