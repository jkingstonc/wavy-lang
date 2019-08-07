using System.Collections.Generic;

// A callable is an interface that represents an item in the Wavy enviroment that can be called
public interface Callable
{
    // Number of args for this call
    int args();
    // This is called when this callable is called
    object call(Interpreter interpreter, List<object> args);
}
