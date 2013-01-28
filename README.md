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

The Xception equivalent is shorter, easier to read, resistant to refactoring, provides extra information in the exception free of charge, and has a negligable performance impact (as we are already in an error state any time the code is run).

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

Rather than referencing an assembly, Xception is designed to be included directly in your project as a source file.
Xception is defined in the global namespace, so no `usings` are required either.

## Extensibility

The behavior of Xception can be extended by creating extension methods on `XceptionBuilder`.
