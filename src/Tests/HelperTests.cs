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

    [Fact] public void LiteralEncode_should_encode_null_char() {
      Xception.Because.Helpers.LiteralEncode("a\0b").Should().Be("\"a\\0b\"");
    }

    [Fact] public void LiteralEncode_should_encode_escape_sequences() {
      Xception.Because.Helpers.LiteralEncode("\b\t\n\f\r\"\\\0").Should().Be("\"\\b\\t\\n\\f\\r\\\"\\\\\\0\"");
    }

    [Fact] public void LiteralEncode_should_not_escape_single_quotes() {
      Xception.Because.Helpers.LiteralEncode("'").Should().Be("\"'\"");
    }

    [Fact] public void LiteralEncode_should_encode_upper_ascii_characters() {
      Xception.Because.Helpers.LiteralEncode("æ").Should().Be("\"\\x00e6\"");
    }

    [Fact] public void LiteralEncode_should_encode_unicode_characters() {
      Xception.Because.Helpers.LiteralEncode("ɷ").Should().Be("\"\\x0277\"");
    }

    [Fact] public void SafeToString_should_work_for_null_value() {
      Xception.Because.Helpers.SafeToString(null).Should().Be("<NULL>");
    }

    [Fact] public void ReasonsToString_should_insert_spaces_between_arguments() {
      Xception.Because.Helpers.ReasonsToString("foo", "bar").Should().Be("foo bar");
    }

    [Fact] public void ReasonsToString_should_use_SafeToString() {
      Xception.Because.Helpers.ReasonsToString(null, null, null).Should().Be("<NULL> <NULL> <NULL>");
    }

    [Fact] public void SafeToString_should_work_for_object_whose_ToString_throws_exception() {
      Xception.Because.Helpers.SafeToString(new BuggyToString()).Should().Be("<TOSTRING_EXCEPTION>");
    }
  }
}
