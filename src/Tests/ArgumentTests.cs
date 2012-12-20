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

		[Fact] public void Argument_should_throw_exception_when_reason_null() {
			int field = 0;
			Xception.Because.Invoking(x => x.Argument(() => field, null))
				.ShouldThrow<ArgumentNullException>("because a reason for the exception must be provided")
				.And.ParamName.Should().Be("reason");
		}

		[Fact] public void Argument_should_throw_exception_when_reason_empty() {
			int field = 0;
			Xception.Because.Invoking(x => x.Argument(() => field, ""))
				.ShouldThrow<ArgumentException>("because a reason for the exception must be provided")
				.And.ParamName.Should().Be("reason");
		}
	}
}
