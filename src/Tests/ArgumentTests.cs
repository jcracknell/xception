using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Tests.Tests {
	public class ArgumentTests {
		[Fact] public void Argument_should_set_ParamName() {
			int foo = 0;
			Xception.Because.Argument(() => foo, "is somehow invalid")
				.ParamName.Should().Be("foo", "because this is the name of the field identified by the expression");
		}

		[Fact] public void Argument_should_throw_exception_when_argument_expression_null() {
			Xception.Because.Invoking(x => x.Argument((Expression<Func<object>>)null, "reason"))
				.ShouldThrow<ArgumentNullException>("because an expression identifying the argument must be provided")
				.And.ParamName.Should().Be("argument");
		}

		[Fact] public void Argument_should_generate_correct_message_for_single_reason() {
			int foo = 100;
			Xception.Because.Argument(() => foo, "reason1")
			.Message.Should().Be("Argument \"foo\" with value \"100\" is invalid: foo reason1\r\nParameter name: foo");
		}

		[Fact] public void Argument_should_generate_correct_message_for_null_reason() {
			int foo = 100;
			Xception.Because.Argument(() => foo, null)
			.Message.Should().Be("Argument \"foo\" with value \"100\" is invalid: foo <NULL>\r\nParameter name: foo");
		}

		[Fact] public void Argument_should_generate_correct_message_for_multiple_reasons() {
			int foo = 100;
			Xception.Because.Argument(() => foo, "reason1", "reason2")
			.Message.Should().Be("Argument \"foo\" with value \"100\" is invalid: foo reason1reason2\r\nParameter name: foo");
		}
	}
}
