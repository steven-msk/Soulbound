using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Core.Registry;

public class FakeBlock : Block {
	public override string name { get; init; }
	public override int minBreakLevel { get; init; }

	public FakeBlock(Identifier id) : base(id) {
	}

	public override BlockRenderData GetRenderData(BlockState blockState) {
		return default;
	}
}

namespace Commands.Parsing {
	internal class BlockIDParserTests {
		//const string pathKey = "block";
		//private static readonly Identifier id = new(pathKey);
		//private BlockIDParser parser;
		//private RuntimeCommandSource context;
		//private Block block;

		//[SetUp]
		//public void Setup() {
		//	parser = new BlockIDParser();
		//	context = new CommandParsingContext(
		//		new CommandArguments(),
		//		Substitute.For<IRuntimeDataProvider>(),
		//		Substitute.For<IRuntimeExecutionServices>()
		//	);
		//	block = Registry<Block>.Add(new FakeBlock(id));
		//}

		//[TearDown]
		//public void TearDown() {
		//	Registry<Block>.Remove(id);
		//}

		//[Test]
		//public void TryParse_KnownIdentifier_ReturnsSuccess() {
		//	ParseResult<Block> result = parser.TryParse(id.ToString(), context);
		//	Assert.That(result.success, Is.True);
		//	Assert.That(result.value, Is.EqualTo(block));
		//}

		//[Test]
		//public void TryParse_KnownIdentifier_DifferentNamespace_ReturnsFail() {
		//	Identifier identifier = new("some_weird_namespace", new[] { pathKey });

		//	ParseResult<Block> result = parser.TryParse(identifier.ToString(), context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_UnknownIdentifier_ReturnsFail() {
		//	const string otherID = "anotherBlock";

		//	ParseResult<Block> result = parser.TryParse(otherID, context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_EmptyString_ReturnsFail() {
		//	ParseResult<Block> result = parser.TryParse("", context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_CaseSensitive_ReturnsFail() {
		//	ParseResult<Block> result = parser.TryParse("Block", context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_WhitespaceToken_ReturnsFail() {
		//	ParseResult<Block> result = parser.TryParse(" ", context);
		//	Assert.That(result.success, Is.False);
		//}
	}
}
