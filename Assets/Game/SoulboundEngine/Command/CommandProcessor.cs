namespace SoulboundEngine.Command {
	using Brigadier.NET;
	using Brigadier.NET.Exceptions;
	using Brigadier.NET.Suggestion;
	using Brigadier.NET.Tree;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

#nullable enable

	public class CommandProcessor<TContext> where TContext : ICommandContext {
		private readonly List<ICommandProvider<TContext>> providerBuffer = new();
		private readonly Func<TContext> contextSupplier;
		private readonly AmbiguityConsumer<TContext> ambiguityConsumer;
		private CommandDispatcher<TContext> dispatcher = new();
		public event Action<string>? onOutputReceived;

		public CommandProcessor(Func<TContext> contextSupplier, AmbiguityConsumer<TContext> ambiguityConsumer) {
			this.contextSupplier = contextSupplier;
			this.ambiguityConsumer = ambiguityConsumer;
		}

		public string? SubmitCommand(string input) {
			ParseResults<TContext> parseResults = this.Parse(input);
			int code = 0;
			try {
				if (parseResults.Exceptions.Any()) {
					throw parseResults.Exceptions.First().Value;
				}
				code = this.dispatcher.Execute(parseResults);
				return null;
			} catch (CommandSyntaxException e) {
				Logger.LogFatal(e);
				code = -1;
				return e.Message;
			} finally {
				Logger.LogInfo("Command dispatched with exit code {}", code);
			}
		}

		public IDictionary<CommandNode<TContext>, string> GetSmartUsages(CommandNode<TContext> node) {
			TContext context = this.contextSupplier();
			return this.dispatcher.GetSmartUsage(node, context);
		}

		public CommandNode<TContext> GetLastNode(string input) {
			if (input.StartsWith('/')) input = input[1..];
			string[] path = input.Split(this.dispatcher.ArgumentSeparator);
			return this.dispatcher.FindNode(path);
		}

		public ParseResults<TContext> Parse(string input) {
			if (input.StartsWith('/')) input = input[1..];
			TContext context = this.contextSupplier();
			return this.dispatcher.Parse(input, context);
		}

		public async Task<Suggestions> GetCompletions(string input, int caretPos) {
			if (input.StartsWith('/')) {
				input = input[1..];
				caretPos--;
			}
			ParseResults<TContext> parseResults = this.Parse(input);
			Task<Suggestions> task;

			try {
				task = this.dispatcher.GetCompletionSuggestions(parseResults, caretPos);
			} catch (CommandSyntaxException e) {
				Logger.LogError(e);
				task = Suggestions.Empty();
			}
			return await task;
		}

		public void AddProvider(ICommandProvider<TContext> provider) {
			this.providerBuffer.Add(provider);
			this.RebuildDispatcher();
		}

		public void RemoveProvider(ICommandProvider<TContext> provider) {
			this.providerBuffer.Remove(provider);
			this.RebuildDispatcher();
		}

		private void RebuildDispatcher() {
			this.dispatcher = new CommandDispatcher<TContext>();
			foreach (ICommandProvider<TContext> provider in this.providerBuffer) {
				provider.RegisterCommands(this.dispatcher, this.ReceiveOutput);
			}
			this.dispatcher.FindAmbiguities(this.ambiguityConsumer);
		}

		public void ReceiveOutput(string message) {
			onOutputReceived?.Invoke(message);
		}
	}
}
