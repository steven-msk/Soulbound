namespace Commands.Parsing {
	internal class EntityDescriptorParserTests {
		//const string pathKey = "entity";
		//private static readonly Identifier id = new(pathKey);
		//private EntityDescriptorParser parser;
		//private RuntimeCommandSource context;
		//private EntityDescriptor descriptor;

		//[SetUp]
		//public void Setup() {
		//	parser = new EntityDescriptorParser();
		//	context = new CommandParsingContext(
		//		new CommandArguments(),
		//		Substitute.For<IRuntimeDataProvider>(),
		//		Substitute.For<IRuntimeExecutionServices>()
		//	);
		//	descriptor = Registry<EntityDescriptor>.Add(new EntityDescriptor(id, _ => null));
		//}

		//[TearDown]
		//public void TearDown() {
		//	Registry<EntityDescriptor>.Remove(id);
		//}

		//[Test]
		//public void TryParse_KnownIdentifier_ReturnsSuccess() {
		//	ParseResult<EntityDescriptor> result = parser.TryParse(id.ToString(), context);
		//	Assert.That(result.success, Is.True);
		//	Assert.That(result.value, Is.EqualTo(descriptor));
		//}

		//[Test]
		//public void TryParse_KnownIdentifier_DifferentNamespace_ReturnsFail() {
		//	Identifier identifier = new("some_weird_namespace", new[] { pathKey });

		//	ParseResult<EntityDescriptor> result = parser.TryParse(identifier.ToString(), context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_UnknownIdentifier_ReturnsFail() {
		//	ParseResult<EntityDescriptor> result = parser.TryParse("unknownEntity", context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_EmptyString_ReturnsFail() {
		//	ParseResult<EntityDescriptor> result = parser.TryParse("", context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_CaseDifference_ReturnsFail() {
		//	ParseResult<EntityDescriptor> result = parser.TryParse("Entity", context);
		//	Assert.That(result.success, Is.False);
		//}
	}
}
