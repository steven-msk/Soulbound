namespace SoulboundBackend.Client.World.EntitySystem.SpawnData {
	public abstract class SpawnDataValue {
		public abstract object GetValue();
		public abstract void SetValue(object value);
	}

	public class SpawnDataValue<TValue> : SpawnDataValue {
		public TValue value { get; set; }

		public SpawnDataValue(TValue value) => this.value = value;

		public override object GetValue() => value;

		public override void SetValue(object value) => this.value = (TValue)value;
	}
}
