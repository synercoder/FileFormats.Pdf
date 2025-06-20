using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Internal;

[DebuggerDisplay("{ToString(),nq}")]
internal class Either<T1, T2>
{
    private readonly T1? _value1;
    private readonly T2? _value2;

    static Either()
    {
        // Runtime validation that T1 and T2 are different types
        if (typeof(T1) == typeof(T2))
            throw new InvalidOperationException(
                $"Either<{typeof(T1).Name}, {typeof(T2).Name}> cannot have duplicate types. " +
                "Both generic parameters resolve to the same type.");
    }

    private Either(T1 value)
    {
        _value1 = value;
        _value2 = default;
        IsFirst = true;
    }

    private Either(T2 value)
    {
        _value1 = default;
        _value2 = value;
        IsFirst = false;
    }

    public static Either<T1, T2> FromFirst(T1 value) => new Either<T1, T2>(value);
    public static Either<T1, T2> FromSecond(T2 value) => new Either<T1, T2>(value);

    public static implicit operator Either<T1, T2>(T1 value) => FromFirst(value);
    public static implicit operator Either<T1, T2>(T2 value) => FromSecond(value);

    public bool IsFirst { get; }
    public bool IsSecond => !IsFirst;

    public T1? AsFirst() => IsFirst ? _value1 : default;
    public T2? AsSecond() => !IsFirst ? _value2 : default;

    // Try to get specific type
    public bool TryGetFirst([NotNullWhen(true)] out T1? value)
    {
        value = IsFirst ? _value1 : default;
        return IsFirst;
    }

    public bool TryGetSecond([NotNullWhen(true)] out T2? value)
    {
        value = !IsFirst ? _value2 : default;
        return !IsFirst;
    }

    public override string ToString()
    {
        return IsFirst
            ? $"[First] ({_value1})"
            : $"[Second] ({_value2})";
    }
}
