using System;

[Obsolete]
public interface IBufferedStatImpl {
	public IStatDefinitionImpl GetStatDefinition();
	public void EnableBuffers(IStatProvider provider);
	public void DisableBuffers(IStatProvider provider);

	public AbstractSerializableStat GetSerializable();
}
