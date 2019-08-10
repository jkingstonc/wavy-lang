using System;
using System.Collections.Generic;

public static class MathsNative
{
    public static Dictionary<string, object> native_obj = new Dictionary<string, object>()
    {
        
        { "rand_native", NativeLoader.create_native(2, (interpreter, args) => new Random().NextDouble()) },
        { "power_native", NativeLoader.create_native(2, (interpreter, args) => Math.Pow(Convert.ToDouble(args[1]), Convert.ToDouble(args[0]))) },
        { "log_native", NativeLoader.create_native(2, (interpreter, args) => Math.Log(Convert.ToDouble(args[1]), Convert.ToDouble(args[0]))) },
    };
}