using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace Commands.Parsing {
	internal class EntityDescriptorParserTests {
		private static readonly Identifier id = new("soulbound_tests", new[] { "entity" });
		private EntityDescriptorParser parser;
		private CommandParsingContext context;
		private EntityDescriptor descriptor;

		[SetUp]
		public void Setup() {
			parser = new EntityDescriptorParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
			descriptor = Registry<EntityDescriptor>.Add(new EntityDescriptor(id, _ => null));
		}

		[TearDown]
		public void TearDown() {
			Registry<EntityDescriptor>.Remove(id);
		}

		[Test]
		public void TryParse_KnownIdentifier_ReturnsSuccess() {
			ParseResult<EntityDescriptor> result = parser.TryParse(id.ToString(), context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(descriptor));
		}

		[Test]
		public void TryParse_UnknownIdentifier_ReturnsFail() {
			ParseResult<EntityDescriptor> result = parser.TryParse("unknownEntity", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<EntityDescriptor> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_CaseDifference_ReturnsFail() {
			ParseResult<EntityDescriptor> result = parser.TryParse("Entity", context);
			Assert.That(result.success, Is.False);
		}
	}
}
