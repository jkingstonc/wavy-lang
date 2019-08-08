using System;
using System.Collections.Generic;

public class NativeLoader
{
    public static void load_natives(Interpreter interpreter)
    {
        foreach (Dictionary<string, object> pair in native_loader_funcs)
        {
            load_native_dict(interpreter, pair);
        }
    }

    // Add your native module loader func here
    public static List<Dictionary<string, object>> native_loader_funcs = new List<Dictionary<string, object>>()
    {
        IONative.native_obj,
        MathsNative.native_obj,
        ConvertNative.native_obj,
        ListNative.native_obj,
        TimeNative.native_obj,
    };

    public static void load_native_dict(Interpreter interpreter, Dictionary<string, object> pairs)
    {
        foreach (KeyValuePair<string, object> pair in pairs)
        {
            interpreter.add_native_obj(pair.Key, pair.Value);
        }
    }

    // Generate a callable from within a Native.cs module
    public static object create_native(int args, Func<Interpreter, List<object>, object> lambda)
    {
        return new CallableTemplate(args, lambda);
    }

    // Template for dynamically generating callables
    private class CallableTemplate : Callable
    {
        int arg_count;
        Func<Interpreter, List<object>, object> lambda;

        public CallableTemplate(int args, Func<Interpreter, List<object>, object> lambda)
        {
            this.arg_count = args;
            this.lambda = lambda;
        }

        public int args()
        {
            return this.arg_count;
        }

        public object call(Interpreter interpreter, List<object> args)
        {
            return this.lambda(interpreter, args);
        }
    }
}
