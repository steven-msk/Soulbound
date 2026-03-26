using SoulboundBackend.Client;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Event;

namespace SoulboundBackend.Core.Audio {
	[PROTOTYPICAL]
	public class AudioEventRouter
		: IEventListener<BlockPlacedEvent>,
		  IEventListener<BlockBrokenEvent>,
		  IEventListener<PlayerJumpedEvent> {

		void IEventListener<BlockPlacedEvent>.OnEvent(BlockPlacedEvent e) {
			AudioManager.Emit(AudioCue.BlockPlace);
		}

		void IEventListener<BlockBrokenEvent>.OnEvent(BlockBrokenEvent e) {
			AudioManager.Emit(AudioCue.BlockBreak);
		}

		void IEventListener<PlayerJumpedEvent>.OnEvent(PlayerJumpedEvent e) {
			AudioManager.Emit(AudioCue.Jump);
		}
	}
}
