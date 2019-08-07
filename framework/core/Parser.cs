using System;
using System.Collections.Generic;

public class Parser
{
    bool debug;
    List<Statement> statements;
    List<Token> tokens;
    // Current always points to the next token
    int current;

    public List<Statement> parse(List<Token> tokens)
    {
        debug = false;
        this.statements = new List<Statement>();
        this.tokens = tokens;
        this.current = 0;

        if (debug)
        {
            Console.WriteLine("=== PARSING ===");
        }
        while (!end())
        {
            add_statement(preliminary());
        }
        return this.statements;
    }

    // Check for anything that can be interpreted indipendently
    private Statement preliminary()
    {
        if (expect(Token.Type.NamespaceDefine))
        {
            return namespc();
        }
        else if (expect(Token.Type.ClassDefine))
        {
            return wclass();
        }
        else if (expect(Token.Type.FunctionDefine))
        {
            return function();
        }
        else
        {
            return statement();
        }
    }

    // Parse namespace decleration
    private Statement namespc()
    {
        consume(Token.Type.NamespaceDefine);
        Token name = consume(Token.Type.Identifier, "Expected namespace name identifier after namespace decleration");
        List<Statement> body = stmt_block();
        return new NamespaceStmt(name, body);
    }

    // Parse class statement
    private Statement wclass()
    {
        consume(Token.Type.ClassDefine);
        Token name = consume(Token.Type.Identifier, "Expected class name identifier after class decleration");
        Expression superclass = null;
        List<DefStmt> members = new List<DefStmt>();
        List<FunctionStmt> methods = new List<FunctionStmt>();
        // Check if the class extends
        if (consume(Token.Type.Extends) != null)
        {
            // Get the superclass which can be a namespace value or a variable identifier
            superclass = namespace_value();
        }
        consume(Token.Type.LeftCurly, "Expected '{' for class body decleration");
        // Parse methods here
        while (!expect(Token.Type.RightCurly) && !end())
        {
            if (expect(Token.Type.Define))
            {
                members.Add(define());
            }
            else if (expect(Token.Type.FunctionDefine))
            {
                methods.Add(function());
            }
        }
        consume(Token.Type.RightCurly, "Expected '}' for class body decleration");
        return new ClassStmt(name, superclass, members, methods);
    }

    // Parse function statement
    private FunctionStmt function()
    {
        consume(Token.Type.FunctionDefine);
        Token name = consume(Token.Type.Identifier, "Expected function name identifier after function decleration");
        List<Token> paramaters = new List<Token>();
        if (expect(Token.Type.LeftParenthesis))
        {
            paramaters = get_paramaters();
        }
        List<Statement> function_body = stmt_block();
        return new FunctionStmt(name, paramaters, function_body);
    }

    // Parse any type of standard independent statement
    private Statement statement()
    {
        if(expect(Token.Type.Compile))
        {
            return compile();
        }
        if (expect(Token.Type.Using))
        {
            return using_namespace();
        }
        if (expect(Token.Type.Import))
        {
            return import();
        }
        else if (expect(Token.Type.Return))
        {
            return ret();
        }
        else if (expect(Token.Type.Continue))
        {
            return cont();
        }
        else if (expect(Token.Type.Break))
        {
            return brk();
        }
        else if (expect(Token.Type.Try))
        {
            return trycatch();
        }
        else if (expect(Token.Type.Interrupt))
        {
            return interrupt();
        }
        else if (expect(Token.Type.Define))
        {
            return define();
        }
        else if (expect(Token.Type.Break))
        {
            return breakloop();
        }
        else if (expect(Token.Type.For))
        {
            return forloop();
        }
        else if (expect(Token.Type.While))
        {
            return whileloop();
        }
        else if (expect(Token.Type.If))
        {
            return ifblock();
        }
        else if (expect(Token.Type.LeftCurly))
        {
            return new BlockStmt(stmt_block());
        }
        else
        {
            // This is just a random expression in code
            return expression_stmt();
        }
    }

    private CompileStmt compile()
    {
        if (consume(Token.Type.Compile) != null)
        {
            Expression code = expression();
            // The code can be a variable, or a literal
            if(code is VariableExpr || code is LiteralExpr)
            {
                return new CompileStmt(code);
            }
            throw new ParseExceptionUnexpectedToken("Code to compile must be variable or literal");
        }
        return null;
    }

    private UsingStmt using_namespace()
    {
        consume(Token.Type.Using);
        consume(Token.Type.NamespaceDefine, "Expected keyword 'namespace' after 'using'");
        Token identifier = consume(Token.Type.Identifier, "Expected namespace identifier after using");
        return new UsingStmt(identifier);
    }

    private ImportStmt import()
    {
        consume(Token.Type.Import);
        Token file = consume(Token.Type.StringObject, "Expected file path after import");
        return new ImportStmt(file);
    }

    private ReturnStmt ret()
    {
        consume(Token.Type.Return);
        return new ReturnStmt(expression());
    }

    private ContinueStmt cont()
    {
        Token continue_token = consume(Token.Type.Continue);
        return new ContinueStmt(continue_token);
    }

    private BreakStmt brk()
    {
        Token break_token = consume(Token.Type.Break);
        return new BreakStmt(break_token);
    }

    private TryCatchStmt trycatch()
    {
        consume(Token.Type.Try);
        Statement try_body = statement();
        consume(Token.Type.Catch, "Expected 'catch' keyword after 'try'");
        List<Token> paramaters = new List<Token>();
        if (expect(Token.Type.LeftParenthesis))
        {
            paramaters = get_paramaters();
        }
        List<Statement> function_body = new List<Statement>(){ statement() };
        FunctionStmt catch_body = new FunctionStmt(new Token(Token.Type.Catch, "catch"), paramaters, function_body);
        return new TryCatchStmt(try_body, catch_body);
    }

    private InterruptStmt interrupt()
    {
        consume(Token.Type.Interrupt);
        return new InterruptStmt(expression());
    }

    // Define a variable
    private DefStmt define()
    {
        consume(Token.Type.Define);
        Token name = consume(Token.Type.Identifier, "Expected identifier after assignment keyword");
        Expression value = null;
        // The variable doesn't have to be initialized
        if (consume(Token.Type.Assignment) != null)
        {
            value = expression();
        }
        return new DefStmt(name, value);
    }

    private BreakStmt breakloop()
    {
        consume(Token.Type.Break);
        return null;
    }

    private Statement forloop()
    {
        consume(Token.Type.For);
        consume(Token.Type.LeftParenthesis, "Expected '(' for opening for loop specification");
        bool iterator = false;
        DefStmt counter_definition = define();
        Expression iterator_object=null, condition=null, counter_action=null;
        // Check if we have an iterator
        if (consume(Token.Type.In)!=null)
        {
            iterator = true;
            iterator_object = expression();
        }
        else
        {
            iterator = false;
            consume(Token.Type.IdentifierSeperator, "Expected ',' at expression end");
            condition = expression();
            consume(Token.Type.IdentifierSeperator, "Expected ',' at expression end");
            counter_action = expression();
        }
        consume(Token.Type.RightParenthesis, "Expected ')' for closing for loop specification");
        // Create the body, along with the counter action
        Statement body = statement();
        if(iterator)
        {
            return new ForIterStmt(counter_definition, iterator_object, body);
        }
        return new ForStmt(counter_definition, condition, counter_action, body);
    }

    private WhileStmt whileloop()
    {
        consume(Token.Type.While);
        consume(Token.Type.LeftParenthesis, "Expected '(' for opening while sloop condition");
        Expression condition = expression();
        consume(Token.Type.RightParenthesis, "Expected ')' for closing while loop condition");
        Statement body = statement();
        return new WhileStmt(condition, body);
    }

    private IfStmt ifblock()
    {
        consume(Token.Type.If);
        consume(Token.Type.LeftParenthesis, "Expected '(' for opening if condition");
        Tuple<Expression, Statement> if_body;
        Expression if_condition = expression();
        consume(Token.Type.RightParenthesis, "Expected ')' for closing if condition");
        if_body = Tuple.Create<Expression, Statement>(if_condition, statement());
        // Process elif's
        List<Tuple<Expression, Statement>> elif_body = new List<Tuple<Expression, Statement>>();
        while (consume(Token.Type.Elif) != null)
        {
            consume(Token.Type.LeftParenthesis, "Expected '(' for opening elif condition");
            Expression elif_condition = expression();
            consume(Token.Type.RightParenthesis, "Expected ')' for closing elif condition");
            elif_body.Add(Tuple.Create<Expression, Statement>(elif_condition, statement()));
        }
        Statement else_body = null;
        if (consume(Token.Type.Else) != null)
        {
            else_body = statement();
        }
        return new IfStmt(if_body, elif_body, else_body);
    }

    private List<Statement> stmt_block()
    {
        consume(Token.Type.LeftCurly, "Expected '{' for body decleration");
        List<Statement> statements = new List<Statement>();
        while (!expect(Token.Type.RightCurly) && !end())
        {
            statements.Add(preliminary());
        }
        consume(Token.Type.RightCurly, "Expected '}' for body decleration");
        return statements;
    }

    private Statement expression_stmt()
    {
        Expression expr = expression();
        return new ExprStmt(expr);
    }

    // An expression that returns a value
    // This goes in order of precidence, from lowest to highest
    private Expression expression()
    {
        return assignment();
    }

    // This is assignment of value, lowest precidence
    private Expression assignment()
    {
        Expression higher_precedence = connective_or();

        if (consume(Token.Type.Assignment) != null)
        {
            Expression assign_value = assignment();

            // If we are assigning to a non-object variable
            if (higher_precedence.GetType() == typeof(VariableExpr))
            {
                VariableExpr var = (VariableExpr)higher_precedence;
                return new AssignExpr(var.name, assign_value);
            }
            // If we are assigning to a namespace value
            else if (higher_precedence.GetType() == typeof(NamespaceValueExpr))
            {
                NamespaceValueExpr var = (NamespaceValueExpr)higher_precedence;
                return new AssignNamespaceExpr(var, assign_value);
            }
            // If we are assigning to a member variable
            else if (higher_precedence.GetType() == typeof(ObjGetExpr))
            {
                ObjGetExpr member = (ObjGetExpr)higher_precedence;
                return new ObjSetExpr(member.obj, member.identifier, assign_value);
            }
        }

        return higher_precedence;
    }

    private Expression connective_or()
    {
        Expression higher_precedence = connective_and();
        while (expect(Token.Type.ConnectiveOr))
        {
            Token op = consume();
            Expression right = connective_and();
            higher_precedence = new ConnectiveExpr(higher_precedence, op, right);
        }
        return higher_precedence;
    }

    private Expression connective_and()
    {
        Expression higher_precedence = bitwise_or();
        while (expect(Token.Type.ConnectiveAnd))
        {
            Token op = consume();
            Expression right = bitwise_or();
            higher_precedence = new ConnectiveExpr(higher_precedence, op, right);
        }
        return higher_precedence;
    }

    private Expression bitwise_or()
    {
        Expression higher_precedence = bitwise_and();
        while (expect(Token.Type.BitwiseOr))
        {
            Token op = consume();
            Expression right = bitwise_and();
            higher_precedence = new BinaryExpr(higher_precedence, op, right);
        }
        return higher_precedence;
    }

    private Expression bitwise_and()
    {
        Expression higher_precedence = equality();
        while (expect(Token.Type.BitwiseAnd))
        {
            Token op = consume();
            Expression right = equality();
            higher_precedence = new BinaryExpr(higher_precedence, op, right);
        }
        return higher_precedence;
    }

    private Expression equality()
    {
        Expression higher_precidence = comparison();
        while (expect(Token.Type.Equals) || expect(Token.Type.NotEquals))
        {
            Token op = consume();
            Expression right = comparison();
            higher_precidence = new BinaryExpr(higher_precidence, op, right);
        }
        return higher_precidence;
    }

    private Expression comparison()
    {
        Expression higher_precidence = bitwise_shift();
        while (expect(Token.Type.Greater) || expect(Token.Type.GreaterEqual) || expect(Token.Type.Less) || expect(Token.Type.LessEqual))
        {
            Token op = consume();
            Expression right = bitwise_shift();
            higher_precidence = new BinaryExpr(higher_precidence, op, right);
        }
        return higher_precidence;
    }

    private Expression bitwise_shift()
    {
        Expression higher_precidence = plus_minus();
        while (expect(Token.Type.BitwiseShiftLeft) || expect(Token.Type.BitwiseShiftRight))
        {
            Token op = consume();
            Expression right = plus_minus();
            higher_precidence = new BinaryExpr(higher_precidence, op, right);
        }
        return higher_precidence;
    }

    private Expression plus_minus()
    {
        Expression higher_precidence = mul_div_mod_rem();
        while (expect(Token.Type.Plus) || expect(Token.Type.Minus))
        {
            Token op = consume();
            // Check for compound assignment
            if (consume(Token.Type.Assignment) != null)
            {
                if(!(higher_precidence is VariableExpr))
                {
                    throw new ParseExceptionUnexpectedToken("Compound assignment can only be applied to an identifier");
                }
                Expression value = mul_div_mod_rem();
                return new AssignExpr(((VariableExpr)higher_precidence).name, new BinaryExpr((VariableExpr)higher_precidence, op, value));
            }
            else
            {
                Expression right = mul_div_mod_rem();
                higher_precidence = new BinaryExpr(higher_precidence, op, right);
            }
        }
        return higher_precidence;
    }

    private Expression mul_div_mod_rem()
    {
        Expression higher_precidence = unary();
        while (expect(Token.Type.Multiply) || expect(Token.Type.Divide) || expect(Token.Type.Mod) || expect(Token.Type.NoRemainder))
        {
            Token op = consume();
            // Check for compound assignment
            if (consume(Token.Type.Assignment) != null)
            {
                if (!(higher_precidence is VariableExpr))
                {
                    throw new ParseExceptionUnexpectedToken("Compound assignment can only be applied to an identifier");
                }
                Expression value = unary();
                return new AssignExpr(((VariableExpr)higher_precidence).name, new BinaryExpr((VariableExpr)higher_precidence, op, value));
            }
            else
            {
                Expression right = unary();
                higher_precidence = new BinaryExpr(higher_precidence, op, right);
            }
        }
        return higher_precidence;
    }

    private Expression unary()
    {
        while (expect(Token.Type.ConnectiveNot) || expect(Token.Type.BitwiseNot) || expect(Token.Type.Increment) || expect(Token.Type.Decrement))
        {
            // If we have an increment, we want an assign expression
            if (expect(Token.Type.Increment) || expect(Token.Type.Decrement))
            {
                Token un_operator = consume();
                Token op = (un_operator.type == Token.Type.Increment) ? new Token(Token.Type.Plus, null) : new Token(Token.Type.Minus, null);
                VariableExpr identifier = (VariableExpr)unary();
                return new AssignExpr(identifier.name, new BinaryExpr(identifier, op, new LiteralExpr(1)));
            }
            else
            {
                Token op = consume();
                Expression right = unary();
                return new UnaryExpr(op, right);
            }
        }
        return call();
    }

    private Expression call()
    {
        Expression higher_precidence = namespace_value();
        while (true)
        {
            // If we are calling a function
            if (expect(Token.Type.LeftParenthesis))
            {
                List<Expression> args = get_args();
                higher_precidence = new CallExpr(higher_precidence, args);
            }
            // If we are getting a member/method from an object
            else if (consume(Token.Type.Callable) != null)
            {
                Token identifier = consume(Token.Type.Identifier, "Expected identifier after '.'");
                higher_precidence = new ObjGetExpr(higher_precidence, identifier);
            }
            else
            {
                break;
            }
        }
        return higher_precidence;
    }

    // Check if we are referencing a value in a specific namespace
    private Expression namespace_value()
    {
        Expression higher_precidence = single_vals();
        if (consume(Token.Type.NamespaceValue) != null)
        {
            // The namespace name must be an identifier
            if (!(higher_precidence is VariableExpr))
            {
                throw new ParseExceptionUnexpectedToken("Expected identifier before namespace value specifier");
            }
            Token identifier = consume(Token.Type.Identifier, "Expected identifier after namespace value specifier");
            return new NamespaceValueExpr((VariableExpr)higher_precidence, identifier);
        }
        return higher_precidence;
    }

    // Check if we have said a single value such as false, true, xyz etc
    private Expression single_vals()
    {
        if (expect(Token.Type.BoolObject))
        {
            bool val = false;
            if ((string)peek().value == "True")
            {
                val = true;
            }
            consume();
            return new LiteralExpr(val);
        }
        else if (expect(Token.Type.Null))
        {
            return new LiteralExpr(consume().value);
        }
        else if (consume(Token.Type.Super) != null)
        {
            Token super = previous();
            consume(Token.Type.Callable, "Expected '.' after super keyword");
            Token identifier = consume(Token.Type.Identifier, "Expected identifier after super '.'");
            return new SuperExpr(super, identifier);
        }
        else if (expect(Token.Type.This))
        {
            return new ThisExpr(consume());
        }
        else if (expect(Token.Type.Identifier))
        {
            return new VariableExpr(consume());
        }
        else if (expect(Token.Type.Minus))
        {
            // If we have a minus, we need to consume the value
            consume(Token.Type.Minus);
            Token value = consume(Token.Type.NumericObject, "Expected numeric literal after '-'");
            return new LiteralExpr((double)value.value * -1);
        }
        else if (expect(Token.Type.NumericObject))
        {
            return new LiteralExpr(consume().value);
        }
        else if (expect(Token.Type.StringObject))
        {
            return new LiteralExpr(consume().value);
        }
        else if (expect(Token.Type.LeftBracket))
        {
            consume(Token.Type.LeftBracket);
            // Create a new list of expressions
            List<Expression> list_vals = new List<Expression>();
            // Iterate through each token and add the values to the list expression
            while (!expect(Token.Type.RightBracket))
            {
                list_vals.Add(expression());
                if (expect(Token.Type.RightBracket))
                {
                    break;
                }
                consume(Token.Type.IdentifierSeperator, "Expected ',' for list seperator");
            }
            consume(Token.Type.RightBracket, "Expected ']' for closing list initialisation");
            return new ListExpr(list_vals);
        }
        else if (consume(Token.Type.LeftParenthesis) != null)
        {
            Expression expr = expression();
            consume(Token.Type.RightParenthesis, "Expected ')' for closing group expression");
            return new GroupExpr(expr);
        }
        return null;
    }

    // Get the paramaters from a function/constructor definitoin
    private List<Token> get_paramaters()
    {
        consume(Token.Type.LeftParenthesis, "Expected '(' for opening paramater specification");
        List<Token> paramaters = new List<Token>();
        while (expect(Token.Type.Identifier))
        {
            paramaters.Add(consume());
            // This indicates we have gone past the last paramater
            if (expect(Token.Type.RightParenthesis))
            {
                break;
            }
            consume(Token.Type.IdentifierSeperator, "Paramater keyword must be followed by seperator ','");
        }
        consume(Token.Type.RightParenthesis, "Expected ')' for closing paramater specification");
        return paramaters;
    }

    // Get the arguments from a function/constructor call
    private List<Expression> get_args()
    {
        consume(Token.Type.LeftParenthesis, "Expected '(' for opening argument specification");
        List<Expression> args = new List<Expression>();
        while (!expect(Token.Type.RightParenthesis))
        {
            args.Add(expression());
            // This indicates we have gone past the last paramater
            if (expect(Token.Type.RightParenthesis))
            {
                break;
            }
            consume(Token.Type.IdentifierSeperator, "Argument value must be followed by seperator ','");
        }
        consume(Token.Type.RightParenthesis, "Expected ')' for closing argument specification");
        return args;
    }

    private void add_statement(Statement statement)
    {
        if (debug)
        {
            Console.WriteLine("adding statement: " + statement.GetType());
        }
        this.statements.Add(statement);
    }

    // Consume the next token
    private Token consume()
    {
        return advance();
    }

    // Consume the expected token, don't panic if we dont get it
    private Token consume(Token.Type type)
    {
        if (expect(type))
        {
            return advance();
        }
        return null;
    }

    // Consume the expected token, panic if we dont get it
    private Token consume(Token.Type type, string error_msg)
    {
        if (expect(type, error_msg))
        {
            return advance();
        }
        throw new ParseExceptionUnexpectedToken(error_msg);
    }

    // Check to see if what we are expecting is correct
    private bool expect(List<Token.Type> types)
    {
        foreach (Token.Type type in types)
        {
            if (peek().type == type)
            {
                return true;
            }
        }
        return false;
    }

    // Check to see if what we are expecting is correct
    private bool expect(Token.Type type)
    {
        if (peek().type == type)
        {
            return true;
        }
        return false;
    }

    // Check to see if what we are expecting is correct, if not throw an error
    private bool expect(Token.Type type, string error_msg)
    {
        if (peek().type == type)
        {
            return true;
        }
        return false;
    }

    // Advance to the next token
    private Token advance()
    {
        Token tok = this.tokens[current];
        this.current++;
        return tok;
    }

    private Token previous()
    {
        return this.tokens[current - 1];
    }

    // Peek the next token
    private Token peek()
    {
        return this.tokens[current];
    }

    // Check if we are at the end of the token list
    private bool end()
    {
        if (expect(Token.Type.Terminator))
        {
            return true;
        }
        return false;
    }
}