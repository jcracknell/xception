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

		[Fact] public void IndexOutOfRange_should_throw_exception_when_reason_null() {
			int foo = 42;
			Xception.Because.Invoking(x => x.IndexOutOfRange(() => foo, null))
			.ShouldThrow<ArgumentNullException>()
			.And.ParamName.Should().Be("reason");
		}

		[Fact] public void IndexOutOfRange_should_throw_exception_when_reason_empty() {
			int foo = 42;
			Xception.Because.Invoking(x => x.IndexOutOfRange(() => foo, ""))
			.ShouldThrow<ArgumentException>()
			.And.ParamName.Should().Be("reason");
		}
	}
}
