using Brigadier.NET;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;
using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.Debug.Commands {
	public sealed record RuntimeCommandSource(
		IRuntimeDataProvider data,
		IRuntimeExecutionServices execServices
	);

	public sealed class CommandProcessor {
		private readonly List<ICommandProvider> providerBuffer = new();
		private readonly IRuntimeDataProvider dataProvider;
		private readonly IRuntimeExecutionServices execServices;
		private CommandDispatcher<RuntimeCommandSource> dispatcher = new();

		public CommandProcessor(IRuntimeDataProvider dataProvider, IRuntimeExecutionServices execServices) {
			this.dataProvider = dataProvider;
			this.execServices = execServices;
		}

		public void SubmitCommand(string input) {
			if (input.StartsWith('/')) input = input[1..];

			RuntimeCommandSource source = new(dataProvider, execServices);
			ParseResults<RuntimeCommandSource> parseResults = dispatcher.Parse(input, source);

			int code = 0;
			try {
				if (parseResults.Exceptions.Any()) {
					throw parseResults.Exceptions.First().Value;
				}
				code = dispatcher.Execute(parseResults);
			} catch (Exception e) when (e is CommandSyntaxException) {
				Logger.LogFatal(e);
				code = -1;
			} finally {
				Logger.LogInfo("Command dispatched with exit code {}", code);
			}
		}

		public async UniTask<Suggestions> GetCompletions(string input, int caretPos) {
			if (input.StartsWith('/')) {
				input = input[1..];
				caretPos--;
			}
			RuntimeCommandSource source = new(dataProvider, execServices);
			Task<Suggestions> task;
			ParseResults<RuntimeCommandSource> parseResults = dispatcher.Parse(input, source);

			try {
				task = dispatcher.GetCompletionSuggestions(parseResults, caretPos);
			} catch (CommandSyntaxException e) {
				Logger.LogError(e);
				task = Suggestions.Empty();
			}
			await task;

			return task.Result;
		}

		public void RegisterProvider(ICommandProvider provider) {
			providerBuffer.Add(provider);
			RebuildDispatcher();
		}
		public void UnregisterProvider(ICommandProvider provider) {
			providerBuffer.Remove(provider);
			RebuildDispatcher();
		}

		private void RebuildDispatcher() {
			dispatcher = new CommandDispatcher<RuntimeCommandSource>();
			foreach (var provider in providerBuffer) {
				provider.RegisterCommands(dispatcher);
			}
			dispatcher.FindAmbiguities((parent, child, sibling, inputs) => {
				Logger.LogFatal(new AmbiguousCommandException(
					parent.UsageText, child.UsageText, sibling.UsageText, inputs
				));
			});
		}
	}
}
