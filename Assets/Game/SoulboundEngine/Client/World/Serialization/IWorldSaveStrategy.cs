#nullable enable

namespace SoulboundEngine.Client.World.Serialization {
    public interface IWorldSaveStrategy {
		string GetSavesRoot();

        WorldDump? Load(string world);
        void Save(WorldDump obj, string world);

        byte[]? LoadRaw(string world);
        void SaveRaw(byte[] data, string world);

		void Delete(string world);
    }
}
