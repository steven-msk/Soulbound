using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core;

public class FakeBlock : Block {
	public override string name { get; init; }
	public override int minBreakLevel { get; init; }

	public FakeBlock(string id) : base(id) {
	}

	public override BlockRenderData GetRenderData(BlockState blockState) {
		return default;
	}
}

namespace Commands.Parsing {
	internal class BlockIDParserTests {
		const string id = "block";
		private BlockIDParser parser;
		private CommandParsingContext context;
		private Block block;

		[SetUp]
		public void Setup() {
			parser = new BlockIDParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
			Registry<Block>.SetContract(new Blocks.BlockRegistrationContract());
			block = Registry<Block>.Add(new FakeBlock(id));
		}

		[TearDown]
		public void TearDown() {
			Registry<Block>.Remove(new Block.RegistrationKey(id));
		}

		[Test]
		public void TryParse_KnownBlockID_ReturnsSuccess() {
			ParseResult<Block> result = parser.TryParse(id, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(block));
		}

		[Test]
		public void TryParse_UnknownBlockID_ReturnsFail() {
			const string id = "block";
			const string otherID = "anotherBlock";
			Registry<Block>.Add(new FakeBlock(id));

			ParseResult<Block> result = parser.TryParse(otherID, context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<Block> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_CaseSensitive_ReturnsFail() {
			Registry<Block>.Add(new FakeBlock("block"));

			ParseResult<Block> result = parser.TryParse("Block", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_WhitespaceToken_ReturnsFail() {
			ParseResult<Block> result = parser.TryParse(" ", context);
			Assert.That(result.success, Is.False);
		}
	}
}
