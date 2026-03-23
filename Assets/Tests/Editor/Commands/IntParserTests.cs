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
	internal class IntParserTests {
		private IntParser parser;
		private CommandParsingContext context;

		[SetUp]
		public void Setup() {
			parser = new IntParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
		}

		[Test]
		public void TryParse_ValidPositiveInt_ReturnsSuccess() {
			ParseResult<int> result = parser.TryParse("42", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(42));
		}

		[Test]
		public void TryParse_ValidNegativeInt_ReturnsSuccess() {
			ParseResult<int> result = parser.TryParse("-67", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(-67));
		}

		[Test]
		public void TryParse_Zero_ReturnsSuccess() {
			ParseResult<int> result = parser.TryParse("0", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(0));
		}

		[Test]
		public void TryParse_Float_ReturnsFail() {
			ParseResult<int> result = parser.TryParse("3.14", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_Word_ReturnsFail() {
			ParseResult<int> result = parser.TryParse("abcdefg", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_EmptyString_ReturnsFail() {
			ParseResult<int> result = parser.TryParse("", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_Overflow_ReturnsFail() {
			ParseResult<int> result = parser.TryParse("9999999999999999999999", context);
			Assert.That(result.success, Is.False);
		}

		[Test]
		public void TryParse_MaxInt_ReturnsSuccess() {
			ParseResult<int> result = parser.TryParse(int.MaxValue.ToString(), context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(int.MaxValue));
		}
	}
}
