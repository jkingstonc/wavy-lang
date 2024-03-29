﻿using wavy.core;

public class ExceptionManager
{

    public const string NAMESPACE_LOCATION = "exception";
    public const string BASE_EXCEPTION_CLASS = "Exception";


    public static Interpreter interpreter;

    // For use when we want to cause an interrupt from the wavy~ core, such as IndexOutOfBounds
    public static void interrupt_wavy_exception(string exception_name, System.Collections.Generic.List<object> args)
    {
        // Create a new instance of the exception class
        WavyObject exception_object = (WavyObject)((Callable)WavyNamespace.get_var_in_namespace(interpreter.local_scope, NAMESPACE_LOCATION, exception_name)).call(interpreter, args);
        interrupt_wavy_exception(exception_object);
    }

    public static void interrupt_wavy_exception(WavyObject exception_object)
    {
        // First check if the exception is valid by checking it extends the base Exception WavyClass
        WavyClass exception = (WavyClass)WavyNamespace.get_var_in_namespace(interpreter.local_scope, NAMESPACE_LOCATION, BASE_EXCEPTION_CLASS);
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
        throw new InterruptException(exception_object, (string)exception_object.get("message"));
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