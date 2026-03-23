using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Runtime.Services;
using SoulboundBackend.Core;

namespace Commands.Parsing {
	internal class ItemParserTests {
		const string id = "item";
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
			Registry<Item>.SetContract(new Items.ItemRegistrationContract());
			item = Registry<Item>.Add(new FakeItem(id));
		}

		[TearDown]
		public void TearDown() {
			Registry<Item>.Remove(new Item.RegistrationKey(id));
		}

		[Test]
		public void TryParse_KnownItemID_ReturnsSuccess() {
			ParseResult<Item> result = parser.TryParse(id, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(item));
		}

		[Test]
		public void TryParse_UnknownItemID_ReturnsFail() {
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
