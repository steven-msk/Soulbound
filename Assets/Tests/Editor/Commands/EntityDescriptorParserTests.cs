using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Core;

namespace Commands.Parsing {
	internal class EntityDescriptorParserTests {
		const string id = "entity";
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
			Registry<EntityDescriptor>.SetContract(new EntityType.EntityDescriptorRegistrationContract());
			descriptor = Registry<EntityDescriptor>.Add(new EntityDescriptor(id, _ => null));
		}

		[TearDown]
		public void TearDown() {
			Registry<EntityDescriptor>.Remove(new EntityDescriptor.RegistrationKey(id));
		}

		[Test]
		public void TryParse_KnownDescriptorID_ReturnsSuccess() {
			ParseResult<EntityDescriptor> result = parser.TryParse(id, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(descriptor));
		}

		[Test]
		public void TryParse_UnknownID_ReturnsFail() {
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
			Registry<EntityDescriptor>.Add(new EntityDescriptor("entity", _ => null));

			ParseResult<EntityDescriptor> result = parser.TryParse("Entity", context);
			Assert.That(result.success, Is.False);
		}
	}
}
