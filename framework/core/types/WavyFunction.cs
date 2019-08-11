using System.Collections.Generic;

public class WavyFunction : Callable
{

    public FunctionStmt function;
    // The outside of the class/program
    public Scope scope;
    public bool is_constructor;

    public WavyFunction(FunctionStmt function, Scope scope, bool is_constructor)
    {
        this.function = function;
        this.scope = scope;
        this.is_constructor = is_constructor;
    }

    // Bind this function to an object instance
    public WavyFunction bind(WavyObject obj)
    {
        // Inside of the function
        Scope bind_scope = new Scope(this.scope);
        // Define "this" as the object we are binding to
        bind_scope.define("this", obj);
        return new WavyFunction(this.function, bind_scope, this.is_constructor);
    }

    public int args()
    {
        return this.function.paramaters.Count;
    }

    public object call(Interpreter interpreter, List<object> args)
    {
        // We first create a new scope for the function
        Scope call_scope = new Scope(this.scope);
        // Loop over each param and define it in the scope
        for (int i = 0; i < this.function.paramaters.Count; i++)
        {
            call_scope.define((string)this.function.paramaters[i].value, args[i]);
        }
        try
        {
            // Then call the block in the interpreter
            interpreter.execute_block(function.body, call_scope);
        }
        catch(ReturnException ret)
        {
            if (is_constructor)
            {
                // We want to return "this"
                return scope.get(new Token(Token.Type.This,"this"));
            }
            return ret.value;
        }
        if(is_constructor)
        {
            // We want to return "this"
            return scope.get(new Token(Token.Type.This, "this"));
        }
        return null;
    }
}
