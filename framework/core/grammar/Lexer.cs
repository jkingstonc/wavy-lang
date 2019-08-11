using System;
using System.Collections.Generic;

public class Lexer
{
    bool debug;
    int current, line;
    string lines;
    List<Token> tokens;

    public List<Token> lex(string lines)
    {
        debug = false;
        this.tokens = new List<Token>();
        this.lines = lines;
        this.current = -1;
        this.line = 0;
        this.tokens = new List<Token>();

        if (debug)
        {
            Console.WriteLine("=== LEXING ===");
        }
        while (!end())
        {
            // Scan for the next token
            scan_token();
        }
        add_token(Token.Type.Terminator, null);
        return this.tokens;
    }

    // Add a token to the return token list
    private void add_token(Token.Type token, object value)
    {
        if (debug)
        {
            if (value != null)
            {
                Console.WriteLine(token + " [" + value + "]");
            }
            else
            {
                Console.WriteLine(token);
            }
        }
        this.tokens.Add(new Token(token, value));
    }

    // Scan the next character
    private void scan_token()
    {
        char c = advance();
        switch (c)
        {
            case ' ':
            case '\t':
                {
                    break;
                }
            case '\n':
            case '\r':
                {
                    line++;
                    break;
                }
            case '#':
                {
                    // Check for block comment
                    if(expect('#'))
                    {
                        advance();
                        process_comment_block();
                        break;
                    }
                    process_comment();
                    break;
                }
            case '{':
                {
                    add_token(Token.Type.LeftCurly, null);
                    break;
                }
            case '}':
                {
                    add_token(Token.Type.RightCurly, null);
                    break;
                }
            case '[':
                {
                    add_token(Token.Type.LeftBracket, null);
                    break;
                }
            case ']':
                {
                    add_token(Token.Type.RightBracket, null);
                    break;
                }
            case '(':
                {
                    add_token(Token.Type.LeftParenthesis, null);
                    break;
                }
            case ')':
                {
                    add_token(Token.Type.RightParenthesis, null);
                    break;
                }
            case ':':
                {
                    if (expect(':'))
                    {
                        add_token(Token.Type.NamespaceValue, null);
                        advance();
                        break;
                    }
                    throw new LexExceptionUnexpectedSequence("pos: " + current + ", line: " + line + ": Invalid sequence, unexpected character '" + c + "'");
                }
            case '.':
                {
                    add_token(Token.Type.Callable, null);
                    break;
                }
            case ',':
                {
                    add_token(Token.Type.IdentifierSeperator, null);
                    break;
                }
            case '=':
                {
                    if (expect('='))
                    {
                        add_token(Token.Type.Equals, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Assignment, null);
                    }
                    break;
                }
            case '!':
                {
                    if (expect('='))
                    {
                        add_token(Token.Type.NotEquals, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.ConnectiveNot, null);
                    }
                    break;
                }
            case '~':
                {

                    add_token(Token.Type.BitwiseNot, null);
                    break;
                }
            case '>':
                {
                    if (expect('='))
                    {
                        add_token(Token.Type.GreaterEqual, null);
                        advance();
                    }
                    else if (expect('>'))
                    {
                        add_token(Token.Type.BitwiseShiftRight, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Greater, null);
                    }
                    break;
                }
            case '<':
                {
                    if (expect('='))
                    {
                        add_token(Token.Type.LessEqual, null);
                        advance();
                    }
                    else if (expect('<'))
                    {
                        add_token(Token.Type.BitwiseShiftLeft, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Less, null);
                    }
                    break;
                }
            case '&':
                {
                    add_token(Token.Type.BitwiseAnd, null);
                    break;
                }
            case '|':
                {
                    add_token(Token.Type.BitwiseOr, null);
                    break;
                }
            case '+':
                {
                    if (expect('+'))
                    {
                        add_token(Token.Type.Increment, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Plus, null);
                    }
                    break;
                }
            case '-':
                {
                    if (expect('-'))
                    {
                        add_token(Token.Type.Decrement, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Minus, null);
                    }
                    break;
                }
            case '*':
                {
                    add_token(Token.Type.Multiply, null);
                    break;
                }
            case '/':
                {
                    if (expect('/'))
                    {
                        add_token(Token.Type.NoRemainder, null);
                        advance();
                    }
                    else
                    {
                        add_token(Token.Type.Divide, null);
                    }
                    break;
                }
            case '%':
                {
                    add_token(Token.Type.Mod, null);
                    break;
                }
            case '"':
            case '\'':
                {
                    process_string();
                    break;
                }
            default:
                {
                    if (System.Char.IsDigit(c))
                    {
                        process_digit(c);
                    }
                    else if (is_alpha(c))
                    {
                        process_alpha(c);
                    }
                    else
                    {
                        // We have an invalid token
                        throw new LexExceptionUnexpectedSequence("pos: " + current + ", line: " + line + ": Invalid sequence, unexpected character '" + c + "'");
                    }
                    break;
                }
        }
    }

    // Get the next character in the stream
    private char advance()
    {
        current++;
        return lines[current];
    }

    // Tell us whether we should expect a particular character
    private bool expect(char c)
    {
        if (peek() == c)
        {
            return true;
        }
        return false;
    }

    // Process a line comment
    private void process_comment()
    {
        advance();
        while (!(is_newline(peek())))
        {
            advance();
        }
    }

    private void process_comment_block()
    {
        int expect_comment_counter = 0;
        while(expect_comment_counter<2)
        {
            if(peek() == '#')
            {
                expect_comment_counter++;
            }
            advance();
        }
    }

    // Process a digit stream
    private void process_digit(char c)
    {
        bool contains_decimal = false;
        string s = "" + c;

        while (!peek_end() && (System.Char.IsDigit(peek()) || peek() == '.'))
        {
            // We have a number with 2 decimals...
            if (contains_decimal && peek() == '.')
            {
                throw new LexExceptionUnexpectedSequence("pos: " + current + ", line: " + line + ": Number cannot contain more than one '.'");
            }
            else
            {
                if (peek() == '.')
                {
                    contains_decimal = true;
                }
                s += advance();
            }
        }
        add_token(Token.Type.NumericObject, Double.Parse(s));
    }

    // Process an alphabet characterm, which can contain digits if identifier
    private void process_alpha(char c)
    {
        string s = "" + c;
        // Get the next character whilst it is a valid identifier/keyword char
        while ((is_alpha(peek()) || System.Char.IsDigit(peek())) && !(peek_end() || is_whitespace(peek())))
        {
            s += advance();
            // We have found a keyword
            if (Token.token_map.ContainsKey(s) && !(is_alpha(peek()) || System.Char.IsDigit(peek())))
            {
                add_token(Token.token_map[s], s);
                return;
            }
        }
        add_token(Token.Type.Identifier, s);
    }

    // Process a string literal, starting with a " or '
    private void process_string()
    {
        string s = "";
        while (peek() != '"' && peek() != '\'')
        {
            // Check if the string hasn't been terminated
            if (peek_end())
            {
                throw new LexExceptionUnexpectedSequence("pos: " + current + ", line: " + line + ": Strings must be terminated");
            }
            s += advance();
        }
        // Ensure we consume the last '"'
        advance();
        add_token(Token.Type.StringObject, s);

    }

    // Peek the next character
    private char peek()
    {
        return this.lines[current + 1];
    }

    // Check if when we peek, we have reached the end
    private bool peek_end()
    {
        if (current + 1 < this.lines.Length - 1)
        {
            return false;
        }
        return true;
    }

    // Check if we have reached the end
    private bool end()
    {
        if (current < this.lines.Length - 1)
        {
            return false;
        }
        return true;
    }

    private bool is_alpha(char c)
    {
        // Identifiers can have underscores in their name
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '_') || (c == '@');
    }

    private bool is_whitespace(char c)
    {
        return (c == ' ') || is_newline(c);
    }

    private bool is_newline(char c)
    {
        return (c == '\n') || (c == '\r');
    }
}
