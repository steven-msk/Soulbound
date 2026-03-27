using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.Debug.Commands;
using SoulboundEngine.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Commands.Parsing {
	internal class GuidParserTests {
		private GuidParser parser;
		private CommandParsingContext context;

		[SetUp]
		public void Setup() {
			parser = new GuidParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
		}

		[Test]
		public void TryParse_ValidGuid_ReturnsSuccess() {
			Guid guid = Guid.NewGuid();

			ParseResult<Guid> result = parser.TryParse(guid.ToString(), context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(guid));
		}

		[Test]
		public void TryParse_InvalidGuid_ReturnsFail() {
			ParseResult<Guid> result = parser.TryParse("unknownGUID", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<Guid> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_GuidWithBraces_ReturnsSuccess() {
			Guid guid = Guid.NewGuid();

			ParseResult<Guid> result = parser.TryParse($"{{{guid}}}", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(guid));
		}

		[Test]
		public void TryParse_GuidWithoutHyphens_ReturnsSuccess() {
			Guid guid = Guid.NewGuid();
			string noHyphen = guid.ToString("N");

			ParseResult<Guid> result = parser.TryParse(noHyphen, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(guid));
		}
	}
}
