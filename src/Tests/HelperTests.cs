using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Tests.Tests {
	public class HelperTests {
		[Fact] public void LiteralEncode_should_encode_null() {
			Xception.Because.Helpers.LiteralEncode(null).Should().Be("null");
		}

		[Fact] public void LiteralEncode_should_encode_tab() {
			Xception.Because.Helpers.LiteralEncode("\t").Should().Be("\"\\t\"");
		}

		[Fact] public void LiteralEncode_should_encode_newline() {
			Xception.Because.Helpers.LiteralEncode("\n").Should().Be("\"\\n\"");
		}
	}
}
