using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Tests {
	public class ArgumentNullTests {
		[Fact] public void ArgumentNull_should_set_ParamName() {
			object field = null;

			Xception.Because.ArgumentNull(() => field)
			.ParamName.Should().Be("field", "because this is the name of the field identified by the expression");
		}

		[Fact] public void ArgumentNull_should_throw_exception_when_argument_value_is_not_null() {
			string foo = "not null";

			Xception.Because
			.Invoking(x => x.ArgumentNull(() => foo))
			.ShouldThrow<ArgumentException>()
			.And.ParamName.Should().Be("argument", "because it does not reference a null value");
		}

		[Fact] public void ArgumentNull_should_throw_exception_when_argument_expression_null() {
			Xception.Because
			.Invoking(x => x.ArgumentNull((Expression<Func<object>>)null))
			.ShouldThrow<ArgumentNullException>();
		}
	}
}
