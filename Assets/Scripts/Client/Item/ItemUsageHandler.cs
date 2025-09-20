using SoulboundBackend.Client;
using SoulboundBackend.Client.World;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.ItemSystem {
	[BootstrappableChildOf(typeof(PlayerController))]
    public sealed class ItemUsageHandler : IBootstrappable {
		private readonly Dictionary<(Type itemCapability, ItemUseTrigger useTrigger), Action<ItemStack>> handlers = new();
		private readonly List<ItemUseTrigger> disabledTriggers = new();
		private readonly PlayerController player;

		public ItemUsageHandler(PlayerController player) {
			this.player = player;
		}

		public void Register<T>(ItemUseTrigger trigger, Action<T, ItemStack> action) where T : IItemCapability {
			handlers[(typeof(T), trigger)] = (itemStack => {
				if (itemStack.item is T item) {
					action.Invoke(item, itemStack);
				}
			});
		}

		public void Enable(params ItemUseTrigger[] triggers) => triggers.ToList().ForEach((trigger) => disabledTriggers.Remove(trigger));

		public void Disable(params ItemUseTrigger[] triggers) => disabledTriggers.AddRange(triggers);

		public void HandleInput(ItemUseTrigger trigger, ItemStack itemStack) {
			if (itemStack == null || IsDisabled(trigger)) {
				return;
			}
			foreach (var (itemCapability, useTrigger) in handlers.Keys) {
				if (!itemCapability.IsInstanceOfType(itemStack.item) || useTrigger != trigger) {
					continue;
				}
				if (handlers.TryGetValue((itemCapability, useTrigger), out var action)) {
					action?.Invoke(itemStack);
				}
				if (itemStack == null) {
					break;
				}
			}
		}

		public bool IsDisabled(ItemUseTrigger trigger) => disabledTriggers.Contains((trigger));

        public void OnBootstrap(DependencyContainer dependencyContainer) {
            Register<IConsumable>(ItemUseTrigger.RightClick, (consumable, stack) => consumable.Consume(stack));
            foreach (ItemUseTrigger trigger in Enum.GetValues(typeof(ItemUseTrigger))) {
                Register<IAttackPerformer>(trigger, (attackPerformer, stack) => {
                    InvocationHelper.If(player.CanAttack, () => attackPerformer.PerformAttack(trigger));
                });
            }
            Register<IPlaceable>(ItemUseTrigger.LeftHold, (placeable, stack) => {
                Level level = GameManager.instance.Level;
                BlockPos blockPos = level.ToBlockPos(player.InputHandler.MouseWorldPosition);

                if (player.CanPlaceBlockAt(blockPos)) {
                    level.SetBlock(blockPos, placeable.Place(stack, blockPos));
                }
            });
        }

        public void OnEarlyBootstrap(DependencyContainer dependencyContainer) {
			dependencyContainer.Register<ItemUsageHandler>(this);
        }
    }
}

