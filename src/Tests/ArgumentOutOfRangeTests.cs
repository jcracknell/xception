using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Tests.Tests {
	public class ArgumentOutOfRangeTests {
		[Fact] public void ArgumentOutOfRange_should_set_ParamName() {
			int max = 100;
			Xception.Because.ArgumentOutOfRange(() => max, "should be at most 50")
			.ParamName.Should().Be("max");
		}

		[Fact] public void ArgumentOutOfRange_should_throw_exception_for_null_argument() {
			Xception.Because.Invoking(x => x.ArgumentOutOfRange((Expression<Func<string>>)null, "some reason"))
			.ShouldThrow<ArgumentNullException>()
			.And.ParamName.Should().Be("argument");
		}

		[Fact] public void ArgumentOutOfRange_should_generate_correct_message_for_single_reason() {
			int max = 100;
			Xception.Because.ArgumentOutOfRange(() => max, "reason1")
			.Message.Should().Be("Argument \"max\" with value \"100\" is out of range: max reason1\r\nParameter name: max");
		}

		[Fact] public void ArgumentOutOfRange_should_generate_correct_message_for_null_reason() {
			int max = 100;
			Xception.Because.ArgumentOutOfRange(() => max, null)
			.Message.Should().Be("Argument \"max\" with value \"100\" is out of range: max <NULL>\r\nParameter name: max");
		}

		[Fact] public void ArgumentOutOfRange_should_generate_correct_message_for_multiple_reasons() {
			int max = 100;
			Xception.Because.ArgumentOutOfRange(() => max, "reason1", "reason2")
			.Message.Should().Be("Argument \"max\" with value \"100\" is out of range: max reason1 reason2\r\nParameter name: max");
		}
	}
}
