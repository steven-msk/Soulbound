namespace Commands.Parsing {
	internal class EntityParserTests {
		//private Guid guid;
		//private EntityGUIDParser parser;
		//private RuntimeCommandSource context;

		//[SetUp]
		//public void Setup() {
		//	parser = new EntityGUIDParser();

		//	IRuntimeDataProvider data = Substitute.For<IRuntimeDataProvider>();
		//	IRuntimeEntityDataProvider entityData = Substitute.For<IRuntimeEntityDataProvider>();

		//	guid = Guid.NewGuid();
		//	entityData.TryGetEntity(Arg.Is(guid), out var _).Returns(true);

		//	data.Entities.Returns(entityData);

		//	context = new CommandParsingContext(
		//		new CommandArguments(),
		//		data,
		//		Substitute.For<IRuntimeExecutionServices>()
		//	);
		//}

		//[Test]
		//public void TryParse_ValidGuidAndEntityExists_ReturnsSuccessWithGuid() {
		//	ParseResult<Guid> result = parser.TryParse(guid.ToString(), context);
		//	Assert.That(result.success, Is.True);
		//	Assert.That(result.value, Is.EqualTo(guid));
		//}

		//[Test]
		//public void TryParse_ValidGuidButEntityMissing_ReturnsFail() {
		//	Guid otherGuid = Guid.NewGuid();
		//	ParseResult<Guid> result = parser.TryParse(otherGuid.ToString(), context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_InvalidGuidString_ReturnsFail() {
		//	ParseResult<Guid> result = parser.TryParse("unknownGUID", context);
		//	Assert.That(result.success, Is.False);
		//}

		//[Test]
		//public void TryParse_EmptyString_ReturnsFail() {
		//	ParseResult<Guid> result = parser.TryParse("", context);
		//	Assert.That(result.success, Is.False);
		//}
	}
}
