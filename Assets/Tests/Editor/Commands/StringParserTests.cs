using NSubstitute;
using NUnit.Framework;
using SoulboundBackend.Client.Debug.Commands;
using SoulboundBackend.Client.Runtime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Commands.Parsing {
	internal class StringParserTests {
		private StringParser parser;
		private CommandParsingContext context;

		[SetUp]
		public void Setup() {
			parser = new StringParser();
			context = new CommandParsingContext(
				new CommandArguments(),
				Substitute.For<IRuntimeDataProvider>(),
				Substitute.For<IRuntimeExecutionServices>()
			);
		}

		[Test]
		public void TryParse_AnyString_AlwaysSucceeds([Values("129389045E5490488329e-790590", "abcdefg", "   a b  c\t", "{}`[]-?:=+\";/@!")] string value) {
			ParseResult<string> result = parser.TryParse(value, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(value));
		}

		[Test]
		public void TryParse_EmptyString_SucceedsWithEmptyValue() {
			ParseResult<string> result = parser.TryParse("", context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(""));
		}

		[Test]
		public void TryParse_NumberString_SucceedsAsString([Values(0, -54, 93, int.MinValue, int.MaxValue)] int intValue) {
			ParseResult<string> result = parser.TryParse(intValue.ToString(), context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(intValue.ToString()));
		}

		[Test]
		public void TryParse_SpecialCharacters_SucceedsPreservingValue() {
			const string chars = "!@#$%^&*()_+=-{}[]:'\";/?.>,<|`~";

			ParseResult<string> result = parser.TryParse(chars, context);
			Assert.That(result.success, Is.True);
			Assert.That(result.value, Is.EqualTo(chars));
		}
	}
}
