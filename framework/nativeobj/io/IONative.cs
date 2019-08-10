using System;
using System.Collections.Generic;

public class IONative
{
    public static Dictionary<string, object> native_obj = new Dictionary<string, object>()
    {
        { "print_native", NativeLoader.create_native(1, (interpreter, args) =>
            {
                // Try to call @tostring
                var value = WavyObject.try_to_call(args[0], WavyObject.at_method_names[AtMethod.TOSTRING], interpreter, args);
                if (value != null)
                {
                    Console.Write(value);
                }
                else
                {
                    // If it is an WavyObject, we want to print what type
                    if (args[0] is WavyObject)
                    {
                        Console.Write("WavyObject:" + ((WavyObject)args[0]).the_class.name);
                    }
                    else
                    {
                        Console.Write(args[0]);
                    }
                }
                return null;
            } ) },
        { "println_native", NativeLoader.create_native(1, (interpreter, args) =>
                        {
                // Try to call @tostring
                var value = WavyObject.try_to_call(args[0], WavyObject.at_method_names[AtMethod.TOSTRING], interpreter, args);
                if (value != null)
                {
                    Console.WriteLine(value);
                }
                else
                {
                    // If it is an WavyObject, we want to print what type
                    if (args[0] is WavyObject)
                    {
                        Console.WriteLine("WavyObject:" + ((WavyObject)args[0]).the_class.name);
                    }
                    else
                    {
                        Console.WriteLine(args[0]);
                    }
                }
                return null;
            } ) },
        { "get_native", NativeLoader.create_native(0, (interpreter, args) => Console.ReadLine()) },
    };
}