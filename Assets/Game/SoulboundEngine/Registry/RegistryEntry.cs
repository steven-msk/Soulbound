namespace SoulboundEngine.Registry {
	using SoulboundEngine.Serialization;
	using System;

	public class RegistryEntry<T> where T : class {
		private readonly IRegistryEntryOwner<T> owner;
		private readonly RegistryKey<T> key;
		private readonly T value;

		public RegistryEntry(IRegistryEntryOwner<T> owner, RegistryKey<T> key, T value) {
			this.owner = owner;
			this.key = key;
			this.value = value;
		}

		public static Codec<RegistryEntry<T>> GetCodec(Registry<T> registry) {
			return Identifier.CODEC.FlatXmap(
				decode: id => registry.GetEntry(id) is { } entry
					? DataResult<RegistryEntry<T>>.Success(entry)
					: DataResult<RegistryEntry<T>>.Error($"Unknown {typeof(T).Name}: {id}"),
				encode: entry => entry.GetKey().value
			);
		}

		public static Codec<T> MapCodec(Registry<T> registry) {
			return GetCodec(registry).Xmap(e => e.value, registry.GetEntry);
		}

		public RegistryKey<T> GetKey() => this.key;
		public T GetValue() => this.value;

		public string GetIdAsString() => this.GetKey()?.value.ToString() ?? "null";

		public bool MatchesId(Identifier id) {
			return this.key.value.Equals(id);
		}

		public bool MatchesKey(RegistryKey<T> key) {
			return this.key.value.Equals(key.value) && this.key.registry.Equals(key.registry);
		}

		public bool Matches(RegistryEntry<T> entry) {
			return this.MatchesKey(entry.key) && this.value.Equals(entry.value);
		}

		public override int GetHashCode() => HashCode.Combine(this.owner, this.key, this.value);

		public override string ToString() {
			return $"entry[key={this.key}, value={this.value}]";
		}
	}
}
