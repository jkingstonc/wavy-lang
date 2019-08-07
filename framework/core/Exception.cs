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