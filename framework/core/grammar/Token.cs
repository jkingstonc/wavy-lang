using System.Collections;
using System.Collections.Generic;

public class Token
{
    // All types of keyword tokens
    public enum Type
    {
        Null,
        BoolObject,            // Literal bool
        NumericObject,         // Literal numeric
        StringObject,          // Literal string
        ListObject,            // Literal list
        Import,
        Super,
        This,
        Identifier,            // Variable identifier
        Return,
        Continue,

        Compile,
        NamespaceDefine,       // 'namespace'
        NamespaceValue,        // Retreiving a value from a namespace e.g. io::print
        Using,
        ClassDefine,
        Extends,

        Try,
        Catch,
        Interrupt,

        LeftCurly,
        RightCurly,
        LeftParenthesis,
        RightParenthesis,
        LeftBracket,
        RightBracket,
        Callable,              // '.' for callables
        IdentifierSeperator,   // ','

        FunctionDefine,        
        Define,                
        Assignment,

        Break,
        For,
        While,
        In,
        If,
        Elif,
        Else,
        Equals,
        NotEquals,
        Greater,
        Less,
        GreaterEqual,
        LessEqual,

        // These are logical such as !True == False
        ConnectiveAnd,
        ConnectiveOr,
        ConnectiveNot,

        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        BitwiseShiftLeft,
        BitwiseShiftRight,

        Plus,
        Minus,
        Multiply,
        Divide,
        Mod,
        NoRemainder,
        Increment,
        Decrement,

        Terminator,
    }

    public static Dictionary<string, Token.Type> token_map = new Dictionary<string, Token.Type>()
        {
            { "null", Type.Null },
            { "True", Type.BoolObject },
            { "False", Type.BoolObject },
            { "import", Type.Import },
            { "super", Type.Super} ,
            { "this" , Type.This},
            { "return", Type.Return },
            { "continue", Type.Continue },
            { "compile", Type.Compile },
            { "namespace", Type.NamespaceDefine},
            { "using", Type.Using },
            { "class", Type.ClassDefine },
            { "extends", Type.Extends },
            { "try", Type.Try },
            { "catch", Type.Catch },
            { "interrupt", Type.Interrupt },
            { "func", Type.FunctionDefine },
            { "var", Type.Define },
            { "break", Type.Break },
            { "for", Type.For },
            { "while", Type.While },
            { "in", Type.In },
            { "if", Type.If },
            { "elif", Type.Elif },
            { "else", Type.Else },
            { "and", Type.ConnectiveAnd },
            { "or", Type.ConnectiveOr },

        };

    public Type type;
    // If the token is an Object or identifier token, it can contain a value
    public object value;

    public Token(Type type, object value)
    {
        this.type = type;
        this.value = value;
    }
}
