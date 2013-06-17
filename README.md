# Xception

Xception is a micro-library dedicated entirely to the construction of exceptions.
Xception assists you with the creation of information-rich exceptions while eliminating boilerplate and enhancing both readability and refactorability.

Consider the following Xception as compared to traditional code throwing an exception containing an equivalent level of detail:

```cs
if(0 == someString.Length) throw Xception.Because.Argument(() => someString, "cannot be empty");
```

```cs
if(0 == someString.Length)
	throw new ArgumentException(
		"Argument \"someString\" with value \"" + someString + "\" is invalid: someString cannot be empty",
		"someString"
	);
```

The Xception equivalent is shorter, easier to read, resistant to refactoring, and provides extra information in the exception free of charge.

Using Xception has a small performance impact resulting from the instantiation of compiler-generated *display classes* for lambda expression closures. The C# compiler automatically generates a 'display class' to serve as the closed-over environment for expressions accessing stack-allocated variables. Consider the following code:

```cs
public void MyFunc(object maybeNull) {
  if(null == maybeNull) throw Xception.Because.ArgumentNull(() => maybeNull);
}
```

The lambda expression `() => maybeNull` references the stack-allocated local variable `maybeNull`, which may be popped from the stack before the expression is invoked. To address this problem, the C# compiler automatically rewrites `MyFunc` to resemble the following code, making `maybeNull` behave as though it were heap allocated (because it is):

```cs
public void MyFunc(object maybeNull) {
  val env = new { maybeNull = maybeNull };
  if(null == env.maybeNull) throw Xception.Because.ArgumentNull(() => env.maybeNull);
}
```

Generally speaking, Xception is intended for (and naturally tends to be used in) high-value, high-level code requiring robust error reporting. For the vast majority of such use cases, the performance cost of Xception is negligable, and will be utterly eclipsed by I/O operations such as file or database accesses.

## Usage

```cs
// ArgumentNullException
if(null == argument) throw Xception.Because.ArgumentNull(() => argument);

// ArgumentException
if(0 == someArray.Length) throw Xception.Because.Argument(() => someArray, "cannot be empty");

// ArgumentOutOfRangeException
if(percent > 100) throw Xception.Because.ArgumentOutOfRange(() => percent, "must be less than or equal to 100");

// IndexOutOfRangeException
if(i > items.Length) throw Xception.Because.IndexOutOfRange(() => i, "must be less than the number of items");
```

Where possible, Xception will take an unlimited number of reason arguments, automatically convert them to strings, and insert a space between each one to allow easy creation of detailed error messages.

```cs
var MaxValue = 42;
if(MaxValue < stringValue.Length) throw Xception.Because.Argument(() => stringValue, "can be at most", MaxValue, "characters in length");
```

Results in an exception with a message of the form `Argument "stringValue" with value "..." is invalid: stringValue can be at most 42 characters in length`.

## Integration

Rather than referencing an assembly, Xception is designed to be included directly in your project as a source file.  Xception is defined in the global namespace, so no `usings` are required either.

Consider adding Xception to your project as a submodule:

```
git submodule add https://github.com/jcracknell/xception.git lib/xception
```

If you are developing a multi-project solution, you can add the same source file to multiple projects by manually editing the appropriate `.csproj` files:

```xml
<Project>
  <ItemGroup>
    <Compile Include="..\..\..\lib\xception\src\Xception.cs" />
  </ItemGroup>
</Project>
```

## Extensibility

The behavior of Xception can be extended by creating extension methods on `XceptionBuilder`.
