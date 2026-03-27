using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface ILevelExecutionService {
		void SetBlockState(BlockPos blockPos, BlockState? blockState);
	}
}
