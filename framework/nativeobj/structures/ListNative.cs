using System;
using System.Collections.Generic;

public static class ListNative
{
    public static Dictionary<string, object> native_obj = new Dictionary<string, object>()
    {
        { "createlist_native", NativeLoader.create_native(1, (interpreter, args) =>
            {
                if(!(args[0] is List<object>))
                {
                    return new List<object>();
                }
                return args[0];
            })
        },
        { "listsize_native", NativeLoader.create_native(1, (interpreter, args) => (int)((List<object>)args[0]).Count ) },
        { "addlist_native", NativeLoader.create_native(2, (interpreter, args) =>
            {
                List<object> list = (List<object>)args[0];
                list.Add(args[1]);
                return list;
            })
        },
        { "indexlist_native", NativeLoader.create_native(2, (interpreter, args) =>
            {
                if((int)Math.Round((double)args[1])< ((List<object>)args[0]).Count)
                {
                    return ((List<object>)args[0])[(int)Math.Round((double)args[1])];
                }
                throw new RuntimeException("Index is out of range");
            })
        },
        { "tostringlist_native", NativeLoader.create_native(1, (interpreter, args) =>
            {
                string str = "[";
                var arr = ((List<object>)args[0]).ToArray();
                for (int i=0;i<arr.Length;i++)
                {
                    var item = arr[i];
                    // Try to call @tostring() on the WavyObject
                    var value = WavyObject.try_to_call(item, WavyObject.at_method_names[AtMethod.TOSTRING], interpreter, args);
                    if (value != null)
                    {
                        str += value;
                    }
                    else
                    {
                        str += item;
                    }
                    if (i < arr.Length - 1)
                    {
                        str += ",";
                    }
                }
                return str + "]";
            })
        },
    };
}