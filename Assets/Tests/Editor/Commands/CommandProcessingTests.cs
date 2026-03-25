using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Runtime.Services;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Commands {
	internal class CommandProcessingTests {
		private CommandProcessor processor = null!;

		private IRuntimeDataProvider NoData() {
			IRuntimeDataProvider data = Substitute.For<IRuntimeDataProvider>();

			IRuntimePlayerDataProvider playerData = Substitute.For<IRuntimePlayerDataProvider>();
			IRuntimeEntityDataProvider entityData = Substitute.For<IRuntimeEntityDataProvider>();
			data.Player.Returns(playerData);
			data.Entities.Returns(entityData);

			return data;
		}

		private IRuntimeExecutionServices NoExec() {
			IRuntimeExecutionServices exec = Substitute.For<IRuntimeExecutionServices>();

			IPlayerExecutionService player = Substitute.For<IPlayerExecutionService>();
			IInventoryExecutionService inventory = Substitute.For<IInventoryExecutionService>();
			player.Inventory.Returns(inventory);
			exec.Player.Returns(player);

			IEntityExecutionService entity = Substitute.For<IEntityExecutionService>();
			exec.Entity.Returns(entity);

			ILevelExecutionService level = Substitute.For<ILevelExecutionService>();
			exec.Level.Returns(level);

			return exec;
		}

		private void CreateProcessor(params CommandNode[] commands) {
			processor = new CommandProcessor(NoData(), NoExec());

			ICommandProvider commandProvider = Substitute.For<ICommandProvider>();
			commandProvider.GetCommands().Returns(commands);
			processor.RegisterProvider(commandProvider);
		}

		[Test]
		public void SubmitCommand_ValidLiteralCommand_ExecutesHandler() {
			bool executed = false;
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => executed = true).GetRootNode()
			);

			processor.SubmitCommand("/command");
			Assert.That(executed, Is.True);
		}

		[Test]
		public void SubmitCommand_UnknownCommand_ThrowsUnknownCommandException() {
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => { }).GetRootNode()
			);

			Assert.Throws<UnknownOrIncompleteCommandException>(() => processor.SubmitCommand("/comman"));
		}

		[Test]
		public void SubmitCommand_EmptyInput_DoesNotExecute() {
			bool executed = false;
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => executed = true).GetRootNode()
			);

			processor.SubmitCommand("");
			Assert.That(executed, Is.False);
		}

		[Test]
		public void SubmitCommand_InputWithoutLeadingSlash_DoesNotExecute() {
			bool executed = false;
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => executed = true).GetRootNode()
			);

			processor.SubmitCommand("command");
			Assert.That(executed, Is.False);
		}

		[Test]
		public void SubmitCommand_ValidArgumentCommand_ParsesAndExecutesWithArgs() {
			int argValue = 0;
			CreateProcessor(
				CommandBuilder.Literal("command")
					.Then(new ArgumentCommandNode<int>("int", new IntParser()))
						.Executes(context => argValue = context.Args.Get<int>("int"))
				.GetRootNode()
			);

			processor.SubmitCommand("/command 500");
			Assert.That(argValue, Is.EqualTo(500));
		}

		[Test]
		public void SubmitCommand_AmbiguousCommand_ThrowsAmbiguousCommandException() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			builder.Then(new ArgumentCommandNode<int>("int", new IntParser()));
			builder.Then(new LiteralCommandNode("1"));
			CreateProcessor(builder.GetRootNode());

			Assert.Throws<AmbiguousCommandException>(() => processor.SubmitCommand("/command 1"));
		}

		[Test]
		public void SubmitCommand_NonTerminalNode_ThrowsCommandNodeNotTerminalException() {
			CreateProcessor(
				CommandBuilder.Literal("command").GetRootNode()
			);

			Assert.Throws<CommandNodeNotTerminalException>(() => processor.SubmitCommand("/command"));
		}

		[Test]
		public void SubmitCommand_ExtraTokensBeyondTerminal_ThrowsException() {
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => { }).GetRootNode()
			);

			Assert.Throws<UnknownOrIncompleteCommandException>(() => processor.SubmitCommand("/command token"));
		}

		[Test]
		public void SubmitCommand_WhitespaceToken_ThrowsInvalidCommandSyntaxException() {
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => { }).GetRootNode()
			);

			Assert.Throws<InvalidCommandSyntaxException>(() => processor.SubmitCommand("/command "));
		}

		[Test]
		public void GetCompletions_EmptyInput_ReturnsNoCompletions() {
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => { }).GetRootNode()
			);

			Assert.That(processor.GetCompletions("", 0), Is.EqualTo(Array.Empty<CommandCompletionToken>()));
		}

		[Test]
		public void GetCompletions_CaretAtZero_ReturnsNoCompletions() {
			CreateProcessor(
				CommandBuilder.Literal("command").Executes(_ => { }).GetRootNode()
			);

			Assert.That(processor.GetCompletions("/command", 0), Is.EqualTo(Array.Empty<CommandCompletionToken>()));
		}

		[Test]
		public void GetCompletions_PartialLiteralToken_ReturnsMatchingLiterals() {
			CreateProcessor(
				CommandBuilder.Literal("command0").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command1").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command2").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command3").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("anotherCommand").Executes(_ => { }).GetRootNode()
			);

			IEnumerable<string> completions = processor.GetCompletions("/com", 3)
				.Select(t => t.text);
			IEnumerable<string> expected = new[] { "command0", "command1", "command2", "command3" };
			Assert.That(completions, Is.EqualTo(expected));
		}

		[Test]
		public void GetCompletions_FullLiteralMatch_ReturnsChildCompletions() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			builder.Then(new LiteralCommandNode("sub1"));
			builder.Then(new LiteralCommandNode("sub12"));
			builder.Then(new LiteralCommandNode("sub123"));
			builder.Then(new LiteralCommandNode("anotherSub"));
			CreateProcessor(builder.GetRootNode());

			IEnumerable<string> completions = processor.GetCompletions("/command sub1", 13)
				.Select(t => t.text);
			IEnumerable<string> expected = new[] { "sub1", "sub12", "sub123" };
			Assert.That(completions, Is.EqualTo(expected));
		}


		[Test]
		public void GetCompletions_ArgumentNodeWithSupplier_ReturnsSuppliedCompletions() {
			ICommandCompletionSupplier supplier = Substitute.For<ICommandCompletionSupplier>();
			IEnumerable<string> supplied = new string[] { "1", "10", "100", "1000" };
			supplier.GetCompletions(Arg.Is("1"), Arg.Any<CommandParsingContext>()).Returns(supplied);

			CreateProcessor(
				CommandBuilder.Literal("command")
					.Then(new ArgumentCommandNode<int>("int", new IntParser(), supplier))
						.Executes(_ => { })
				.GetRootNode()
			);

			IEnumerable<string> completions = processor.GetCompletions("/command 1", 10)
				.Select(t => t.text);
			Assert.That(completions, Is.EqualTo(supplied));
		}

		[Test]
		public void GetCompletions_ArgumentNodeWithNoSupplier_ReturnsNoCompletions() {
			CreateProcessor(
				CommandBuilder.Literal("command")
					.Then(new ArgumentCommandNode<int>("int", new IntParser()))
						.Executes(_ => { })
				.GetRootNode()
			);

			Assert.That(processor.GetCompletions("/command 3", 10), Is.EqualTo(Array.Empty<CommandCompletionToken>()));
		}

		[Test]
		public void GetCompletions_CaretMidToken_ReturnsCompletionsForSubstring() {
			CreateProcessor(
				CommandBuilder.Literal("command0").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command1").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command2").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command3").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("anotherCommand").Executes(_ => { }).GetRootNode()
			);

			IEnumerable<string> completions = processor.GetCompletions("/command1", 3)
				.Select(t => t.text);
			IEnumerable<string> expected = new[] { "command0", "command1", "command2", "command3" };
			Assert.That(completions, Is.EqualTo(expected));
		}

		[Test]
		public void GetCompletions_CaretPastInputEnd_ReturnsNoCompletions() {
			CreateProcessor(
				CommandBuilder.Literal("command0").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("command1").Executes(_ => { }).GetRootNode()
			);

			Assert.That(processor.GetCompletions("/command0", 20), Is.EqualTo(Array.Empty<CommandCompletionToken>()));
		}

		[Test]
		public void GetCompletions_WhitespaceInPrecedingTokens_ReturnsNoCompletions() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			builder.Then(new LiteralCommandNode("sub1"));
			builder.Then(new LiteralCommandNode("sub12"));
			builder.Then(new LiteralCommandNode("sub123"));
			CreateProcessor(builder.GetRootNode());

			Assert.That(processor.GetCompletions("/command  sub", 13), Is.EqualTo(Array.Empty<CommandCompletionToken>()));
		}

		[Test]
		public void RegisterProvider_MultipleProviders_AllCommandsAccessible() {
			bool[] executed = { false, false, false, false };
			ICommandProvider provider1 = Substitute.For<ICommandProvider>();
			IEnumerable<CommandNode> provider1Commands = new[] {
				CommandBuilder.Literal("command0").Executes(_ => executed[0] = true).GetRootNode(),
				CommandBuilder.Literal("command1").Executes(_ => executed[1] = true).GetRootNode()
			};
			provider1.GetCommands().Returns(provider1Commands);
			ICommandProvider provider2 = Substitute.For<ICommandProvider>();
			IEnumerable<CommandNode> provider2Commands = new[] {
				CommandBuilder.Literal("command2").Executes(_ => executed[2] = true).GetRootNode(),
				CommandBuilder.Literal("command3").Executes(_ => executed[3] = true).GetRootNode()
			};
			provider2.GetCommands().Returns(provider2Commands);

			CreateProcessor();
			processor.RegisterProvider(provider1);
			processor.RegisterProvider(provider2);

			processor.SubmitCommand("/command0");
			processor.SubmitCommand("/command3");
			Assert.That(executed, Is.EqualTo(new bool[] { true, false, false, true }));
		}

		[Test]
		public void UnregisterProvider_RemovesCommandsFromRoot() {
			bool executedFromProvider2 = false;
			ICommandProvider provider1 = Substitute.For<ICommandProvider>();
			IEnumerable<CommandNode> provider1Commands = new[] {
				CommandBuilder.Literal("A").Executes(_ => { }).GetRootNode(),
				CommandBuilder.Literal("B").Executes(_ => { }).GetRootNode()
			};
			provider1.GetCommands().Returns(provider1Commands);
			ICommandProvider provider2 = Substitute.For<ICommandProvider>();
			IEnumerable<CommandNode> provider2Commands = new[] {
				CommandBuilder.Literal("C").Executes(_ => executedFromProvider2 = true).GetRootNode(),
				CommandBuilder.Literal("D").Executes(_ => executedFromProvider2 = true).GetRootNode()
			};
			provider2.GetCommands().Returns(provider2Commands);

			CreateProcessor();
			processor.RegisterProvider(provider1);
			processor.RegisterProvider(provider2);
			processor.SubmitCommand("/C");
			Assert.That(executedFromProvider2, Is.True);

			processor.UnregisterProvider(provider2);
			Assert.Throws<UnknownOrIncompleteCommandException>(() => processor.SubmitCommand("/D"));

		}
	}
}
