using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpawnDataKey {
	public string name { get; }

	public SpawnDataKey(string name) => this.name = name;

	public static SpawnDataKey Of(string name) => new SpawnDataKey(name);

	public override bool Equals(object obj) {
		return obj is SpawnDataKey key && string.Equals(this.name, key.name);
	}

	public override int GetHashCode() => name.GetHashCode();
}
