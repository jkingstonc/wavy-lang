public class ExceptionManager
{
    // For use when we want to cause an interrupt from the wavy~ core, such as IndexOutOfBounds
    public static void interrupt_wavy_exception(Interpreter interpreter, string exception_name)
    {
        // First check if the exception is valid
        WavyClass exception = (WavyClass)interpreter.namespaces["exception"].get("Exception");
        // Create a new instance of the exception class
        WavyObject exception_object = (WavyObject)((Callable)interpreter.namespaces["exception"].get(exception_name)).call(interpreter, new System.Collections.Generic.List<object>());
        // First check if it extends 'Exception'
        WavyClass super = exception_object.the_class;
        while (super != exception && super != null)
        {
            super = super.superclass;
        }
        if (super != exception)
        {
            throw new RuntimeException("Class must be an Exception to be able to interrupt");
        }
        // Then throw the interrupt
        throw new InterruptException(null, (string)exception_object.get("message"));
    }
}

public class WavyException : System.Exception
{
    public string errror_msg;
}

public class LexExceptionUnexpectedSequence : WavyException
{
    public LexExceptionUnexpectedSequence(string error_msg)
    {
        this.errror_msg = "Lexing Exception: " + error_msg;
    }
}

public class ParseExceptionUnexpectedToken : WavyException
{
    public ParseExceptionUnexpectedToken(string error_msg)
    {
        this.errror_msg = "Parsing Exception: " + error_msg;
    }
}

public class RuntimeException : WavyException
{
    public RuntimeException(string error_msg)
    {
        this.errror_msg = "Runtime Exception: "+error_msg;
    }
}

public class ReturnException : WavyException
{
    public object value;

    public ReturnException(object value)
    {
        this.value = value;
    }
}

public class ContinueException : WavyException
{
    public ContinueException()
    {   
    }
}

public class InterruptException : WavyException
{
    public WavyObject obj;
    public InterruptException(WavyObject obj, string error_msg)
    {
        this.obj = obj;
        this.errror_msg = error_msg;
    }
}