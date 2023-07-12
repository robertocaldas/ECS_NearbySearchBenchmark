public class ValueType<T>
{
    private T _value;

    public ValueType() { }

    public ValueType(T value)
    {
        _value = value;
    }

    public T Value
    {
        get { return _value; }
        set { _value = value; }
    }

    //public static implicit operator ValueType<T>(T value)
    //{
    //    return new ValueType<T>(value);
    //}

    public static implicit operator T(ValueType<T> entry)
    {
        return entry.Value;
    }
}

// why this doesnt work?
//public class Float : ValueType<float> { }

public class Float
{
    private const float Max = 100f;
    private const float Min = 0f;

    private float _value;

    public Float() { }

    public Float(float value)
    {
        _value = value;
    }

    public float Value
    {
        get { return _value; }
        set
        {
            _value = value;
            if(_value > Max)
            {
                _value = Max;
            }
            else if(_value < Min)
            {
                _value = Min;
            }
        }
    }

    public static implicit operator float(Float entry)
    {
        return entry.Value;
    }

    //public static implicit operator Float(float entry)
    //{
    //    return new Float(entry);
    //}

    public override string ToString()
    {
        return _value.ToString();
    }


    // TODO: Overload == and !=, implement IEquatable<T>, override
    // Equals(object), GetHashCode and ToStrin
}