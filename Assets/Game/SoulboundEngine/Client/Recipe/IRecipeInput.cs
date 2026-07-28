using SoulboundEngine.Client.Item;

namespace SoulboundEngine.Client.Recipe {
	/// <summary>
	/// A generic slotted input source (e.g. a crafting grid) that recipes can be matched against.
	/// </summary>
	public interface IRecipeInput {
		/// <summary>
		/// The number of slots available in this input.
		/// </summary>
		int Size();

		ItemStack GetStackInSlot(int slot);

		/// <returns><c>True</c> if this input has no slots</returns>
		public bool IsEmpty() => this.Size() <= 0;
	}
}
