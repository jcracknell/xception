using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Tests.Tests {
	public class IndexOutOfRangeTests {
		private int TestProperty { get { return 42; } }
		private int TestField = 43;

		[Fact] public void IndexOutOfRange_should_work_for_property() {
			Xception.Because.Invoking(x => x.IndexOutOfRange(() => TestProperty, "some reason"))
			.ShouldNotThrow();
		}

		[Fact] public void IndexOutOfRange_should_work_for_field() {
			Xception.Because.Invoking(x => x.IndexOutOfRange(() => TestField, "some reason"))
			.ShouldNotThrow();
		}

		[Fact] public void IndexOutOfRange_should_throw_exception_when_index_null() {
			Xception.Because.Invoking(x => x.IndexOutOfRange((Expression<Func<object>>)null, "some reason"))
			.ShouldThrow<ArgumentNullException>()
			.And.ParamName.Should().Be("index");
		}

		[Fact] public void IndexOutOfRange_should_generate_correct_message_for_single_reason() {
			int max = 100;
			Xception.Because.IndexOutOfRange(() => max, "reason1")
			.Message.Should().Be("Index \"max\" with value \"100\" is out of range: max reason1");
		}

		[Fact] public void IndexOutOfRange_should_generate_correct_message_for_null_reason() {
			int max = 100;
			Xception.Because.IndexOutOfRange(() => max, null)
			.Message.Should().Be("Index \"max\" with value \"100\" is out of range: max <NULL>");
		}

		[Fact] public void IndexOutOfRange_should_generate_correct_message_for_multiple_reasons() {
			int max = 100;
			Xception.Because.IndexOutOfRange(() => max, "reason1", "reason2")
			.Message.Should().Be("Index \"max\" with value \"100\" is out of range: max reason1reason2");
		}
	}
}
