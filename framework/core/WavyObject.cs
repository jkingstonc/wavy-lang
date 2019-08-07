using System.Collections.Generic;

public enum AtMethod
{
    TOSTRING,
    ADD,
    SUB,
    MUL,
    DIV,
    BIT_AND,
    BIT_OR,
    GREATER,
    LESS,
    GREATER_EQUAL,
    LESS_EQUAL,
    ITER,
}

public class WavyObject
{
    public static Dictionary<AtMethod, string> at_method_names = new Dictionary<AtMethod, string>()
    {
        { AtMethod.TOSTRING, "@tostring"},
        { AtMethod.ADD, "@add"},
        { AtMethod.SUB, "@sub"},
        { AtMethod.MUL, "@mul"},
        { AtMethod.DIV, "@div"},
        { AtMethod.BIT_AND, "@bitand"},
        { AtMethod.BIT_OR, "@bitor"},
        { AtMethod.GREATER, "@greater"},
        { AtMethod.LESS, "@less"},
        { AtMethod.GREATER_EQUAL, "@greatereq"},
        { AtMethod.LESS_EQUAL, "@lesseq"},
        { AtMethod.ITER, "@iter"},
    };

    public WavyClass the_class;
    public Dictionary<string, object> members;

    public WavyObject(WavyClass the_class)
    {
        this.members = new Dictionary<string, object>();
        this.the_class = the_class;
    }

    // Get a method or member
    public object get(Token identifier)
    {
        return get((string)identifier.value);
    }

    public object get(string identifier)
    {
        // Check class methods
        WavyFunction method = the_class.find_method(this, identifier);
        if (method != null)
        {
            return method;
        }
        // Check object members
        object member = find_member(identifier);
        if (member != null)
        {
            return member;
        }
        throw new RuntimeException("Class member/method cannot be found '" + identifier + "'");
    }

    // Set a member value
    public object set(Token identifier, object value)
    {
        this.members[(string)identifier.value] = value;
        return members[(string)identifier.value];
    }

    // Get a member object
    public object find_member(string identifier)
    {
        if (this.members.ContainsKey(identifier))
        {
            return members[identifier];
        }
        return null;
    }

    public static object try_to_call(object wavy_object, string method, Interpreter interpreter, List<object> args)
    {
        if (wavy_object is WavyObject)
        {
            // Check if the @tostring method exists
            if (((WavyObject)wavy_object).the_class.find_method((WavyObject)wavy_object, AtMethod.TOSTRING) != null)
            {
                // Call the @tostring method
                return (((Callable)((WavyObject)wavy_object).get(method)).call(interpreter, args));
            }
        }
        return null;
    }
}
