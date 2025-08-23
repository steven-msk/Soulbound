using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBufferedStatImpl {
	public IStatTypeImpl GetStatType();
	public void EnableBuffers(IStatProvider source);
	public void DisableBuffers(IStatProvider source);

	public AbstractSerializableStat Cast() => (AbstractSerializableStat)this;
}
