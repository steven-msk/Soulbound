using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands {
	internal class CommandBuilderTests {
		[Test]
		public void Then_SingleChild_ChildAddedToCursor() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));
			CommandNode root = builder.GetRootNode();
			CommandNode child = builder.GetCursorNode();

			Assert.That(root.GetChildren(), Contains.Item(child));
		}

		[Test]
		public void Then_ChainedThen_EachNodeAddedAsChildOfPrevious() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"))
					.Then(new LiteralCommandNode("subCommand_of_subCommand"));
			CommandNode root = builder.GetRootNode();
			CommandNode child1 = root.GetChildren().First();
			CommandNode child2 = child1.GetChildren().First();

			Assert.That(root.GetChildren(), Contains.Item(child1));
			Assert.That(child1.GetChildren(), Contains.Item(child2));
			Assert.That(root.GetChildren(), Does.Not.Contains(child2));
		}

		[Test]
		public void Then_ReturnedBuilder_CursorIsNewChild() {
			CommandBuilder rootBuilder = CommandBuilder.Literal("command");
			CommandBuilder childBuilder = rootBuilder.Then(new LiteralCommandNode("subCommand"));

			CommandNode root = rootBuilder.GetRootNode();
			Assert.That(root.GetChildren(), Contains.Item(childBuilder.GetCursorNode()));
		}

		[Test]
		public void Then_ReturnsDifferentBuilder() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			CommandBuilder childBuilder = builder.Then(new LiteralCommandNode("subCommand"));

			Assert.That(builder, Is.Not.EqualTo(childBuilder));
		}

		[Test]
		public void Then_ReturnedBuilder_RootRemainsUnchanged() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			CommandNode root1 = builder.GetRootNode();
			CommandBuilder childBuilder = builder.Then(new LiteralCommandNode("subCommand"));
			CommandNode root2 = childBuilder.GetRootNode();

			Assert.That(root1, Is.EqualTo(root2));
		}

		[Test]
		public void Then_MultipleChildrenOnSameNode_AllChildrenPresent() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			CommandNode child1 = builder.Then(new LiteralCommandNode("subCommand1")).GetCursorNode();
			CommandNode child2 = builder.Then(new LiteralCommandNode("subCommand2")).GetCursorNode();
			CommandNode child3 = builder.Then(new LiteralCommandNode("subCommand3")).GetCursorNode();
			CommandNode root = builder.GetRootNode();

			Assert.That(root.GetChildren(), Is.EqualTo(new[] { child1, child2, child3 }));
		}

		[Test]
		public void Executes_AfterThen_HandlerSetOnChild() {
			CommandHandler handler = _ => { };
			CommandNode child = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"))
					.Executes(handler)
			.GetCursorNode();

			Assert.That(child.IsTerminalNode(), Is.True);
			Assert.That(child.GetHandler(), Is.EqualTo(handler));
		}

		[Test]
		public void Executes_ReturnsSameBuilder() {
			CommandBuilder builder = CommandBuilder.Literal("command");
			CommandNode root = builder.GetCursorNode();
			CommandBuilder noExecuteBuilder = builder.Then(new LiteralCommandNode("subCommand"));
			CommandNode child = noExecuteBuilder.GetCursorNode();
			CommandBuilder executeBuilder = noExecuteBuilder.Executes(_ => { });

			Assert.That(noExecuteBuilder, Is.EqualTo(executeBuilder));
			Assert.That(noExecuteBuilder.GetRootNode(), Is.EqualTo(root));
			Assert.That(executeBuilder.GetRootNode(), Is.EqualTo(root));
			Assert.That(executeBuilder.GetCursorNode(), Is.EqualTo(child));
		}

		[Test]
		public void Executes_SetsHandlerOnCursor() {
			CommandHandler handler = _ => { };
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"))
					.Executes(handler);

			Assert.That(builder.GetCursorNode().GetHandler(), Is.EqualTo(handler));
			Assert.That(builder.GetRootNode().GetHandler(), Is.Null);
		}

		[Test]
		public void ThenRootOf_AddsRootOfGivenBuilderToCursor() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));
			CommandNode root = builder.GetRootNode();
			CommandNode source = builder.GetCursorNode();

			CommandBuilder add = CommandBuilder.Literal("another command")
				.Then(new LiteralCommandNode("sub_another command"));
			CommandBuilder result = builder.ThenRootOf(add);

			Assert.That(result.GetRootNode(), Is.EqualTo(root));
			Assert.That(source.GetChildren().First(), Is.EqualTo(add.GetRootNode()));
		}

		[Test]
		public void ThenRootOf_ReturnedBuilder_CursorIsPassedBuildersRoot() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));

			CommandBuilder add = CommandBuilder.Literal("add command")
				.Then(new LiteralCommandNode("another command"));
			CommandBuilder result = builder.ThenRootOf(add);

			Assert.That(result.GetCursorNode(), Is.EqualTo(add.GetRootNode()));
		}

		[Test]
		public void ThenRootOf_ReturnedBuilder_RootRemainsTheSame() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));
			CommandNode root = builder.GetRootNode();

			CommandBuilder add = CommandBuilder.Literal("add command")
				.Then(new LiteralCommandNode("another command"));
			CommandBuilder result = builder.ThenRootOf(add);

			Assert.That(result.GetRootNode(), Is.EqualTo(root));
		}

		[Test]
		public void ThenRootOf_PassedBuilderHasChain_FullChainPreservedAsChild() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));

			CommandBuilder add = CommandBuilder.Literal("add");
			CommandBuilder add_step1 = add.Then(new LiteralCommandNode("subAdd_1"));
			CommandBuilder add_step2 = add_step1.Then(new LiteralCommandNode("subAdd_2"));

			CommandNode addRoot = add.GetRootNode();
			CommandNode step1 = add_step1.GetCursorNode();
			CommandNode step2 = add_step2.GetCursorNode();

			builder.ThenRootOf(add);

			Assert.That(builder.GetCursorNode().GetChildren(), Contains.Item(addRoot));
			Assert.That(addRoot.GetChildren(), Contains.Item(step1));
			Assert.That(step1.GetChildren(), Contains.Item(step2));
		}

		[Test]
		public void ThenCursorOf_AddsRootOfGivenBuilderToCursor() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));
			CommandNode root = builder.GetRootNode();
			CommandNode source = builder.GetCursorNode();

			CommandBuilder add = CommandBuilder.Literal("another command")
				.Then(new LiteralCommandNode("sub_another command"));
			CommandBuilder result = builder.ThenCursorOf(add);

			Assert.That(result.GetRootNode(), Is.EqualTo(root));
			Assert.That(source.GetChildren().First(), Is.EqualTo(add.GetRootNode()));
		}

		[Test]
		public void ThenCursorOf_ReturnedBuilder_CursorIsPassedBuildersCursor() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));

			CommandBuilder add = CommandBuilder.Literal("add command")
				.Then(new LiteralCommandNode("another command"));
			CommandBuilder result = builder.ThenCursorOf(add);

			Assert.That(result.GetCursorNode(), Is.EqualTo(add.GetCursorNode()));
		}

		[Test]
		public void ThenCursorOf_ReturnedBuilder_RootRemainsTheSame() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));
			CommandNode root = builder.GetRootNode();

			CommandBuilder add = CommandBuilder.Literal("add command")
				.Then(new LiteralCommandNode("another command"));
			CommandBuilder result = builder.ThenCursorOf(add);

			Assert.That(result.GetRootNode(), Is.EqualTo(root));
		}

		[Test]
		public void ThenCursorOf_PassedBuilderWithChain_CursorPointsToLeaf() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));

			CommandBuilder add = CommandBuilder.Literal("add")
				.Then(new LiteralCommandNode("subAdd_1"))
					.Then(new LiteralCommandNode("subAdd_2"));

			CommandBuilder result = builder.ThenCursorOf(add);

			Assert.That(result.GetCursorNode(), Is.EqualTo(add.GetCursorNode()));
		}

		[Test]
		public void ThenCursorOf_SubsequentThen_ChildAddedToPassedBuildersCursor() {
			CommandBuilder builder = CommandBuilder.Literal("command")
				.Then(new LiteralCommandNode("subCommand"));

			CommandBuilder add = CommandBuilder.Literal("add");
			CommandBuilder add_step1 = add.Then(new LiteralCommandNode("subAdd_1"));
			CommandBuilder add_step2 = add_step1.Then(new LiteralCommandNode("subAdd_2"));

			CommandNode addRoot = add.GetRootNode();
			CommandNode step1 = add_step1.GetCursorNode();
			CommandNode step2 = add_step2.GetCursorNode();

			CommandBuilder result = builder.ThenCursorOf(add)
				.Then(new LiteralCommandNode("sub_of_add"));

			Assert.That(add.GetCursorNode().GetChildren(), Contains.Item(result.GetCursorNode()));
		}

		[Test]
		public void GetRootNode_InitialBuilder_ReturnsConstructedNode() {
			CommandNode root = new LiteralCommandNode("root");
			CommandBuilder builder = new(root);

			Assert.That(builder.GetRootNode(), Is.EqualTo(root));
		}

		[Test]
		public void GetCursorNode_InitialBuilder_SameAsRoot() {
			CommandNode root = new LiteralCommandNode("root");
			CommandBuilder builder = new(root);

			Assert.That(builder.GetCursorNode(), Is.EqualTo(root));
		}

		[Test]
		public void GetCursorNode_AfterThen_ReturnsLastChild() {
			CommandNode root = new LiteralCommandNode("root");
			CommandNode child = new LiteralCommandNode("child");
			CommandBuilder builder = new(root);
			CommandBuilder result = builder.Then(child);

			Assert.That(result.GetCursorNode(), Is.EqualTo(child));
		}

		[Test]
		public void GetRootNode_AfterChain_NeverChanges() {
			CommandNode root = new LiteralCommandNode("root");

			CommandBuilder builder = new(root);
			for (int i = 0;  i < 10; i++) {
				builder.Then(new LiteralCommandNode($"child_{i}"));
				Assert.That(builder.GetRootNode(), Is.EqualTo(root));
			}

			for (int i = 0; i < 50; i++) {
				CommandNode node = new LiteralCommandNode($"child_step{i}");
				builder = builder.Then(node);
				Assert.That(builder.GetRootNode(), Is.EqualTo(root));
			}
		}

	}
}
