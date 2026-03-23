using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.Parsing {
	internal class CoordinateParserTests {
		private CoordinateParser parser;
		private CommandParsingContext context;

		[SetUp]
		public void Setup() {
			parser = new CoordinateParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
		}

		[Test]
		public void TryParse_TildeOnly_ReturnsRelativeZeroOffset() {
			ParseResult<Coordinate> result = parser.TryParse("~", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.True);
			Assert.AreEqual(0f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_TildeWithPositiveOffset_NoSign_ReturnsRelativeOffset() {
			ParseResult<Coordinate> result = parser.TryParse("~5.5", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.True);
			Assert.AreEqual(5.5f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_TildeWithPositiveOffset_WithSign_ReturnsRelativeOffset() {
			ParseResult<Coordinate> result = parser.TryParse("~+5.5", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.True);
			Assert.AreEqual(5.5f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_TildeWithNegativeOffset_ReturnsRelativeNegativeOffset() {
			ParseResult<Coordinate> result = parser.TryParse("~-10", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.True);
			Assert.AreEqual(-10f, result.value.value, 0.0001f);
		}


		[Test]
		public void TryParse_TildeWithInvalidSuffix_ReturnsFail() {
			ParseResult<Coordinate> result = parser.TryParse("~abc", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_AbsolutePositiveFloat_ReturnsAbsolute() {
			ParseResult<Coordinate> result = parser.TryParse("50", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.False);
			Assert.AreEqual(50f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_AbsoluteNegativeInt_ReturnsAbsolute() {
			ParseResult<Coordinate> result = parser.TryParse("-7.5", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.False);
			Assert.AreEqual(-7.5f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_AbsoluteZero_ReturnsSuccess() {
			ParseResult<Coordinate> result = parser.TryParse("0", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value.isRelative, Is.False);
			Assert.AreEqual(0f, result.value.value, 0.0001f);
		}

		[Test]
		public void TryParse_PlainText_ReturnsFail() {
			ParseResult<Coordinate> result = parser.TryParse("abcdefg", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<Coordinate> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void GetPos_Relative_AddsOffset() {
			ParseResult<Coordinate> result = parser.TryParse("~3", context);
			Assert.That(result.value.GetPos(10f), Is.EqualTo(13));
		}

		[Test]
		public void GetPos_Absolute_IgnoresRelative() {
			ParseResult<Coordinate> result = parser.TryParse("100", context);
			Assert.That(result.value.GetPos(98764f), Is.EqualTo(100));
		}
	}
}
