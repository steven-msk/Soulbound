using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace Commands.Parsing {
	internal class ItemParserTests {
		const string pathKey = "path";
		private static readonly Identifier id = new(pathKey);
		private Item item;
		private ItemParser parser;
		private CommandParsingContext context;

		[SetUp]
		public void SetUp() {
			parser = new ItemParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
			item = Registry<Item>.Add(new FakeItem(id));
		}

		[TearDown]
		public void TearDown() {
			Registry<Item>.Remove(id);
		}

		[Test]
		public void TryParse_KnownIdentifier_ReturnsSuccess() {
			ParseResult<Item> result = parser.TryParse(id.ToString(), context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(item));
		}

		[Test]
		public void TryParse_KnownIdentifier_DifferentNamespace_ReturnsFail() {
			Identifier identifier = new("some_weird_namespace", new[] { pathKey });

			ParseResult<Item> result = parser.TryParse(identifier.ToString(), context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_UnknownIdentifier_ReturnsFail() {
			ParseResult<Item> result = parser.TryParse("unknownItemID", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<Item> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}
	}
}
