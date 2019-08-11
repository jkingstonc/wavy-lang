public enum IntegerSizes
{
    // 16 bit integer
    HALF,
    // 32 bit integer
    FULL
}

public enum FloatingSize
{
    // 32 bit double
    HALF,
    // 64 bit double
    FULL
}

class Primitives
{
    // The current numeric data sizes in use
    public static IntegerSizes int_size = IntegerSizes.FULL;
    public static FloatingSize float_size = FloatingSize.FULL;

    // Return a literal object in its correct data format
    public static object parse_literal(object literal)
    {
        if(literal is int)
        {
            return parse_int((int)literal);
        }
        if(literal is double)
        {
            return parse_float((double)literal);
        }
        return literal;
    }

    // Convert an integer to the correct format
    public static object parse_int(int val)
    {
        switch (int_size)
        {
            case IntegerSizes.FULL:
            {
                return System.Convert.ToInt32(val);
            }
            case IntegerSizes.HALF:
            {
                return System.Convert.ToInt16(val);
            }
            default: return 0;
        }
    }

    // Convert a floating point number to the correct format
    public static object parse_float(double val)
    {
        switch (float_size)
        {
            case FloatingSize.FULL:
                {
                    return System.Convert.ToDouble(val);
                }
            case FloatingSize.HALF:
                {
                    return System.Convert.ToSingle(val);
                }
            default: return 0;
        }
    }
}