using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Tests.Tests {
	public class HelperTests {
		class BuggyToString {
			public override string ToString() {
				throw new Exception("bug");
			}
		}

		[Fact] public void LiteralEncode_should_encode_null() {
			Xception.Because.Helpers.LiteralEncode(null).Should().Be("null");
		}

		[Fact] public void LiteralEncode_should_encode_tab() {
			Xception.Because.Helpers.LiteralEncode("\t").Should().Be("\"\\t\"");
		}

		[Fact] public void LiteralEncode_should_encode_newline() {
			Xception.Because.Helpers.LiteralEncode("\n").Should().Be("\"\\n\"");
		}

		[Fact] public void SafeToString_should_work_for_null_value() {
			Xception.Because.Helpers.SafeToString(null).Should().Be("<NULL>");
		}

		[Fact] public void SafeToString_should_work_for_object_whose_ToString_throws_exception() {
			Xception.Because.Helpers.SafeToString(new BuggyToString()).Should().Be("<TOSTRING_EXCEPTION>");
		}
	}
}
