using System;
using System.Collections.Generic;
using System.Threading;

public static class TimeNative
{
    public static Dictionary<string, object> native_obj = new Dictionary<string, object>()
    {
        { "time_native", NativeLoader.create_native(0, (interpreter, args) => DateTime.Now) },
        { "timediff_native", NativeLoader.create_native(2, (interpreter, args) => (double)((DateTime)args[0]).Subtract(((DateTime)args[1])).TotalSeconds) },
        { "timesleep_native", NativeLoader.create_native(1, (interpreter, args) =>
            {
                Thread.Sleep(Convert.ToInt32(args[0]));
                return null;
            })
        },
    };
}