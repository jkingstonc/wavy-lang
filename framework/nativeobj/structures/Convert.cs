using System;
using System.Collections.Generic;

public static class ConvertNative
{
    public static Dictionary<string, object> native_obj = new Dictionary<string, object>()
    {
        { "bool", NativeLoader.create_native(1, (interpreter, args) =>
            {
                if(!(args[0] is WavyObject))
                {
                    return Convert.ToBoolean(args[0]);
                }
                return args[0] != null;
            })
        },
        { "int", NativeLoader.create_native(1, (interpreter, args) =>
            {
                if(!(args[0] is WavyObject))
                {
                    return Convert.ToInt32(args[0]);
                }
                return null;
            })
        },
        { "double", NativeLoader.create_native(1, (interpreter, args) =>
            {
                if(!(args[0] is WavyObject))
                {
                    return Convert.ToDouble(args[0]);
                }
                return null;
            })
        },
        { "string", NativeLoader.create_native(1, (interpreter, args) =>
            {
                if(!(args[0] is WavyObject))
                {
                    return Convert.ToString(args[0]);
                }
                return WavyObject.try_to_call((WavyObject)args[0], WavyObject.at_method_names[AtMethod.TOSTRING], interpreter, args);
            })
        },
    };
}