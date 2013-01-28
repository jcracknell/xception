/*

Xception.cs
https://github.com/jcracknell/xception
Copyright (c) 2012 James Cracknell

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

// Suppress CS0436 type T in file conflicts with the imported type 'type'. Using the one in 'file'
// occurring when Xception is added to multiple projects in a single solution
#pragma warning disable 436

/// <summary>
/// Creates information-rich exceptions.
/// </summary>
internal static class Xception {
	private static readonly XceptionBuilder _builder = new XceptionBuilder();
	public static XceptionBuilder Because { get { return _builder; } }
}

/// <summary>
/// Creates information-rich exceptions.
/// </summary>
internal class XceptionBuilder {
	private XceptionHelpers _helpers;

	public XceptionBuilder() {
		_helpers = new XceptionHelpers();
	}

	public XceptionHelpers Helpers { get { return _helpers; } }
}

internal class XceptionHelpers {
	private static readonly char[] LITERALENCODE_ESCAPE_CHARS;
	private static readonly char[] HEX_DIGIT = "0123456789abcdef".ToCharArray();
	private static readonly MethodInfo objectToString = typeof(object).GetMethod("ToString");

	static XceptionHelpers() {
		// Initialize array of characters to be escaped during literal encoding
		// Per http://msdn.microsoft.com/en-us/library/h21280bw.aspx
		LITERALENCODE_ESCAPE_CHARS = new char[93];
		LITERALENCODE_ESCAPE_CHARS['\a'] = 'a';
		LITERALENCODE_ESCAPE_CHARS['\b'] = 'b';
		LITERALENCODE_ESCAPE_CHARS['\f'] = 'f';
		LITERALENCODE_ESCAPE_CHARS['\n'] = 'n';
		LITERALENCODE_ESCAPE_CHARS['\r'] = 'r';
		LITERALENCODE_ESCAPE_CHARS['\t'] = 't';
		LITERALENCODE_ESCAPE_CHARS['"'] = '"';
		LITERALENCODE_ESCAPE_CHARS['\\'] = '\\';
		LITERALENCODE_ESCAPE_CHARS['?'] = '?';
	}

	public MemberInfo GetMemberInfoFromExpression(LambdaExpression expression) {
		if(null == expression) throw Xception.Because.ArgumentNull(() => expression);

		var memberExpression = GetExpressionBody(expression) as MemberExpression;
		if(null == memberExpression)
			throw Xception.Because.Argument(() => expression, "is not a " + typeof(MemberExpression).Name);
	
		return memberExpression.Member;
	}

	public MethodInfo GetMethodInfoFromExpression(LambdaExpression expression) {
		if(null == expression) throw Xception.Because.ArgumentNull(() => expression);

		var callExpression = GetExpressionBody(expression) as MethodCallExpression;
		if(null == callExpression)
			throw Xception.Because.Argument(() => expression, "is not a " + typeof(MethodCallExpression).Name);

		return callExpression.Method;
	}

	private Expression GetExpressionBody(LambdaExpression expression) {
		var body = expression.Body;
		while(ExpressionType.Convert == body.NodeType)
			body = ((UnaryExpression)body).Operand;

		return body;
	}

	/// <summary>
	/// Generates a string representation of the provided object.
	/// </summary>
	public string GetStringRepresentation(object o) {
		if(null == o) return "null";
		
		if(!ImplementsParameterlessToString(o))
			return o.GetType().FullName;

		return LiteralEncode(o.ToString());
	}

	/// <summary>
	/// Returns true if the provided object's type hierarchy overrides the default <code>ToString()</code> implementation.
	/// </summary>
	private bool ImplementsParameterlessToString(object o) {
		return !typeof(object).GetMethod("ToString", new Type[0])
			.Equals(o.GetType().GetMethod("ToString", new Type[0]));
	}

	/// <summary>
	/// Encode the provided string as a C# string literal.
	/// </summary>
	/// <param name="s">The string to be encoded as a C# string literal.</param>
	/// <returns><paramref name="s"/> encoded as a C# string literal.</returns>
	public string LiteralEncode(string s) {
		if(null == s) return "null";

		var sb = new StringBuilder(s.Length * 2).Append('"');
		var rp = 0;
		while(rp < s.Length) {
			var c = s[rp++];
			if(LITERALENCODE_ESCAPE_CHARS.Length > c && 0 != LITERALENCODE_ESCAPE_CHARS[c])
				sb.Append('\\').Append(LITERALENCODE_ESCAPE_CHARS[c]);
			else if(' ' <= c && c <= '~')
				sb.Append(c);
			else
				sb.Append("\\x")
					.Append(HEX_DIGIT[c >> 12 & 0xf])
					.Append(HEX_DIGIT[c >> 8 & 0xf])
					.Append(HEX_DIGIT[c >> 4 & 0xf])
					.Append(HEX_DIGIT[c & 0xf]);
		}

		return sb.Append('"').ToString();
	}

	/// <summary>
	/// Convert the provided reasons arguments to a single space-separated string.
	/// </summary>
	public string ReasonsToString(object reason, params object[] reasonContinuation) {
		var sb = new StringBuilder(SafeToString(reason));

		var enumerator = reasonContinuation.GetEnumerator();
		if(!enumerator.MoveNext())
			return sb.ToString();

		sb.Append(' ');
		for(;;) {
			sb.Append(SafeToString(enumerator.Current));

			if(!enumerator.MoveNext())
				break;

			sb.Append(' ');
		}
		
		return sb.ToString();
	}

	/// <summary>
	/// Converts the provided object <paramref name="o"/> to a string using its ToString() implementation, preventing or catching any exceptions which may occur.
	/// </summary>
	public string SafeToString(object o) {
		if(null == o) return "<NULL>";
		try { return o.ToString(); }
		catch { return "<TOSTRING_EXCEPTION>"; }
	}
	
	/// <summary>
	/// Trys to compile and invoke the provided <paramref name="expression"/>, assigning the result to <paramref name="value"/>.
	/// Returns true if compilation and invocation proceeds without exception, false otherwise.
	/// </summary>
	/// <typeparam name="T">The type of the value returned by the expression.</typeparam>
	/// <param name="expression">The expression to be evaluated.</param>
	/// <param name="value">The variable to which the result of the evaluation of <paramref name="expression"/> should be assigned.</param>
	/// <returns>Returns true if compilation and invocation proceeds without exception, false otherwise.</returns>
	public bool TryGetExpressionValue<T>(Expression<Func<T>> expression, out T value) {
		try {
			value = expression.Compile().Invoke();
			return true;
		} catch(Exception ex) {
			value = default(T);
			return false;
		}
	}

	/// <summary>
	/// Compiles and invokes the provided <paramref name="expression"/>, returning the result.
	/// </summary>
	/// <param name="expression">The expression to be evaluated.</param>
	/// <returns>The value resulting from the evaluation of the <paramref name="expression"/>.</returns>
	public T GetExpressionValue<T>(Expression<Func<T>> expression) {
		if(null == expression) throw Xception.Because.ArgumentNull(() => expression);

		Func<T> compiled;
		try { compiled = expression.Compile(); }
		catch(Exception ex) {
			throw new Exception("An exception occurred while compiling the expression.", ex);
		}

		try {
			return compiled.Invoke();
		} catch(Exception ex) {
			throw new Exception("An exception occurred while evaluating the expression.", ex);
		}
	}
}

/// <summary>
/// Defines extension methods used to construct instances of common <see cref="Exception"/> implementations
/// found in the <code>System</code> namespace.
/// </summary>
internal static class CoreXceptionExtensions {
	/// <summary>
	/// Generate an <see cref="ArgumentException"/> indicating that the <paramref name="argument"/> identified by the
	/// provided <see cref="System.Linq.Expressions.Expression"/> is invalid for the specified <paramref name="reason"/>.
	/// </summary>
	/// <typeparam name="T">The type of the argument identified by the provided <see cref="System.Linq.Expressions.Expression"/>.</typeparam>
	/// <param name="argument">The argument which is invalid.</param>
	/// <param name="reason">Reason why the <paramref name="argument"/> is invalid, to be converted to a string.</param>
	/// <param name="reason">Continued reason why the <paramref name="argument"/> is invalid, to be converted to strings.</param>
	/// <returns>
	/// An <see cref="ArgumentException"/> indicating that the <paramref name="argument"/> identified by the provided
	/// <see cref="System.Linq.Expressions.Expression"/> is invalid for the specified <paramref name="reason"/>.
	/// </returns>
	public static ArgumentException Argument<T>(this XceptionBuilder e, Expression<Func<T>> argument, object reason, params object[] reasonContinuation) {
		if(null == argument) throw Xception.Because.ArgumentNull(() => argument);

		var argumentMember = e.Helpers.GetMemberInfoFromExpression(argument);
		if(MemberTypes.Field != argumentMember.MemberType)
			throw Xception.Because.Argument(() => argument, "does not reference a field");

		var argumentName = argumentMember.Name;
		var argumentValue = e.Helpers.GetExpressionValue(argument);

		// Parameters are backwards on this one for some reason
		var message = new StringBuilder()
			.Append("Argument ").Append(e.Helpers.LiteralEncode(argumentName))
			.Append(" with value ").Append(e.Helpers.GetStringRepresentation(argumentValue))
			.Append(" is invalid: ").Append(argumentName).Append(" ")
			.Append(e.Helpers.ReasonsToString(reason, reasonContinuation));

		return new ArgumentException(message.ToString(), argumentName);
	}

	/// <summary>
	/// Generate an <see cref="ArgumentNullException"/> indicating that the <paramref name="argument"/> identified
	/// by the provided <see cref="System.Linq.Expressions.Expression"/> is null.
	/// </summary>
	/// <typeparam name="T">The type of the argument identified by the provided <see cref="System.Linq.Expressions.Expression"/>.</typeparam>
	/// <param name="argument">An expression identifying the argument which is null.</param>
	/// <returns>
	/// An <see cref="ArgumentNullException"/> indicating that the <paramref name="argument"/> identified by the
	/// provided <see cref="System.Linq.Expressions.Expression"/> is null.
	/// </returns>
	public static ArgumentNullException ArgumentNull<T>(this XceptionBuilder e, Expression<Func<T>> argument) where T : class {
		if(null == argument) throw Xception.Because.ArgumentNull(() => argument);

		var argumentMember = e.Helpers.GetMemberInfoFromExpression(argument);
		if(MemberTypes.Field != argumentMember.MemberType)
			throw Xception.Because.Argument(() => argument, "does not reference a field");

		var argumentName = argumentMember.Name;

		// Throw an exception if referenced field is not null, as it is likely the user has made an error.
		var argumentValue = e.Helpers.GetExpressionValue(argument);
		if(null != argumentValue)
			throw Xception.Because.Argument(() => argument, "does not reference a field whose value is null");

		return new ArgumentNullException(
			argumentName,
			"Argument " + e.Helpers.LiteralEncode(argumentName) + " cannot be null."
		);
	}

	/// <summary>
	/// Generate an <see cref="System.ArgumentOutOfRangeException"/> indicating that the <paramref name="argument"/> identified
	/// by the provided <see cref="System.Linq.Expressions.Expression"/> is out of range for the specified <paramref name="reason"/>.
	/// </summary>
	/// <typeparam name="T">The type of the argument identified by the provided <see cref="System.Linq.Expressions.Expression"/>.</typeparam>
	/// <param name="argument">An expression identifying the argument which is out of range.</param>
	/// <param name="reason">Reason why the <paramref name="argument"/> is out of range, to be converted to a string.</param>
	/// <param name="reasonContinuation">Continued reason the <paramref name="argument"/> is out of range, to be converted to strings.</param>
	/// <returns>
	/// Generate an <see cref="System.ArgumentOutOfRangeException"/> indicating that the <paramref name="argument"/> identified
	/// by the provided <see cref="System.Linq.Expressions.Expression"/> is out of range for the specified <paramref name="reason"/>.
	/// </returns>
	public static ArgumentOutOfRangeException ArgumentOutOfRange<T>(this XceptionBuilder e, Expression<Func<T>> argument, object reason, params object[] reasonContinuation) {
		if(null == argument) throw Xception.Because.ArgumentNull(() => argument);

		var argumentMember = e.Helpers.GetMemberInfoFromExpression(argument);
		if(MemberTypes.Field != argumentMember.MemberType)
			throw Xception.Because.Argument(() => argument, "does not reference a field");

		var argumentName = argumentMember.Name;
		var argumentValue = e.Helpers.GetExpressionValue(argument);

		var message = new StringBuilder()
			.Append("Argument ").Append(e.Helpers.LiteralEncode(argumentName))
			.Append(" with value ").Append(e.Helpers.GetStringRepresentation(argumentValue))
			.Append(" is out of range: ").Append(argumentName).Append(" ")
			.Append(e.Helpers.ReasonsToString(reason, reasonContinuation));

		return new ArgumentOutOfRangeException(argumentName, message.ToString());
	}

	/// <summary>
	/// Generate an <see cref="System.IndexOutOfRangeException"/> indicating that the <paramref name="index"/> identified
	/// by the provided <see cref="System.Linq.Expressions.Expression"/> is out of range for the specified <paramref name="reason"/>.
	/// </summary>
	/// <typeparam name="T">The type of the index identified by the provided <see cref="System.Linq.Expressions.Expression"/>.</typeparam>
	/// <param name="index">An expression identifying the index value which is out of range.</param>
	/// <param name="reason">Reason why the identified <paramref name="index"/> is out of range, to be converted to a string.</param>
	/// <param name="reasonContinuation">Continued reason why the identified <paramref name="index"/> is out of range, to be converted to strings.</param>
	/// <returns>
	/// An <see cref="System.IndexOutOfRangeException"/> indicating that the <paramref name="index"/> identified by the
	/// provided <see cref="System.Linq.Expressions.Expression"/> is out of range for the specified <paramref name="reason"/>.
	/// </returns>
	public static IndexOutOfRangeException IndexOutOfRange<T>(this XceptionBuilder e, Expression<Func<T>> index, object reason, params object[] reasonContinuation) {
		if(null == index) throw Xception.Because.ArgumentNull(() => index);

		var indexName = e.Helpers.GetMemberInfoFromExpression(index).Name;
		var indexValue = e.Helpers.GetExpressionValue(index);

		var message = new StringBuilder()
			.Append("Index ").Append(e.Helpers.LiteralEncode(indexName))
			.Append(" with value ").Append(e.Helpers.GetStringRepresentation(indexValue))
			.Append(" is out of range: ").Append(indexName).Append(" ")
			.Append(e.Helpers.ReasonsToString(reason, reasonContinuation));

		return new IndexOutOfRangeException(message.ToString());
	}
}

#pragma warning restore 436