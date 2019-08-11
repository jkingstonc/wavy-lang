using System;
using System.Collections.Generic;

public interface StatementVisitor
{
    // These return voids becuase statements cannot be evaluated
    void visit_compile(CompileStmt compile_stmt);
    void visit_using(UsingStmt using_stmt);
    void visit_import(ImportStmt import_stmt);
    void visit_return(ReturnStmt return_stmt);
    void visit_continue(ContinueStmt continue_stmt);
    void visit_namespace(NamespaceStmt namespace_stmt);
    void visit_class(ClassStmt class_stmt);
    void visit_function(FunctionStmt function_stmt);
    void visit_define(DefStmt define_stmt);
    void visit_break(BreakStmt break_stmt);
    void visit_trycatch(TryCatchStmt trycatch_stmt);
    void visit_interrupt(InterruptStmt interrupt_stmt);
    void visit_foriter(ForIterStmt foriter_stmt);
    void visit_for(ForStmt for_stmt);
    void visit_while(WhileStmt while_stmt);
    void visit_if(IfStmt if_stmt);
    void visit_block(BlockStmt block_stmt);
    void visit_expression(ExprStmt expr_stmt);
}

public abstract class Statement
{
    public abstract void visit(StatementVisitor visitor);
}

public class CompileStmt : Statement
{
    public Expression code;

    public CompileStmt(Expression code)
    {
        this.code = code;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_compile(this);
    }
}

public class UsingStmt : Statement
{
    public Expression _namespace;

    public UsingStmt(Expression _namespace)
    {
        this._namespace = _namespace;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_using(this);
    }
}

public class ImportStmt : Statement
{

    public Token file;

    public ImportStmt(Token file)
    {
        this.file = file;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_import(this);
    }
}

public class ReturnStmt : Statement
{

    public Expression value;

    public ReturnStmt(Expression value)
    {
        this.value = value;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_return(this);
    }
}

public class ContinueStmt : Statement
{
    public Token continue_token;

    public ContinueStmt(Token continue_token)
    {
        this.continue_token = continue_token;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_continue(this);
    }
}

public class NamespaceStmt : Statement
{
    public Token name;
    public List<Statement> body;

    public NamespaceStmt(Token name, List<Statement> body)
    {
        this.name = name;
        this.body = body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_namespace(this);
    }
}

public class ClassStmt : Statement
{
    public Token name;
    public Expression superclass;
    List<DefStmt> members;
    public List<FunctionStmt> methods;

    public ClassStmt(Token name, Expression superclass, List<DefStmt> members, List<FunctionStmt> methods)
    {
        this.name = name;
        this.superclass = superclass;
        this.members = members;
        this.methods = methods;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_class(this);
    }
}

public class FunctionStmt : Statement
{
    public Token name;
    public List<Token> paramaters;
    public bool set_param_count;
    public List<Statement> body;

    public FunctionStmt(Token name, List<Token> paramaters, List<Statement> body)
    {
        this.name = name;
        this.paramaters = paramaters;
        this.body = body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_function(this);
    }
}

public class DefStmt : Statement
{
    public Token name;
    public Expression value;

    public DefStmt(Token name, Expression value)
    {
        this.name = name;
        this.value = value;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_define(this);
    }
}

public class BreakStmt : Statement
{
    public Token break_token;

    public BreakStmt(Token break_token)
    {
        this.break_token = break_token;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_break(this);
    }
}

public class TryCatchStmt : Statement
{
    public Statement try_body;
    public FunctionStmt catch_body;

    public TryCatchStmt(Statement try_body, FunctionStmt catch_body)
    {
        this.try_body = try_body;
        this.catch_body = catch_body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_trycatch(this);
    }
}

public class InterruptStmt : Statement
{
    public Expression expression;

    public InterruptStmt(Expression expression)
    {
        this.expression = expression;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_interrupt(this);
    }
}

public class ForIterStmt : Statement
{
    public DefStmt counter_definition;
    public Expression iterator_object;
    public Statement body;
    public ForIterStmt(DefStmt counter_definition, Expression iterator_object, Statement body)
    {
        this.counter_definition = counter_definition;
        this.iterator_object = iterator_object;
        this.body = body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_foriter(this);
    }
}

public class ForStmt : Statement
{
    public DefStmt counter_definition;
    public Expression condition;
    public Expression counter_action;
    public Statement body;

    public ForStmt(DefStmt counter_definition, Expression condition, Expression counter_action, Statement body)
    {
        this.counter_definition = counter_definition;
        this.condition = condition;
        this.counter_action = counter_action;
        this.body = body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_for(this);
    }
}

public class WhileStmt : Statement
{
    public Expression condition;
    public Statement body;

    public WhileStmt(Expression condition, Statement body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_while(this);
    }
}

public class IfStmt : Statement
{
    public Tuple<Expression, Statement> if_body;

    public List<Tuple<Expression, Statement>> elif_body;

    public Statement else_body;

    public IfStmt(Tuple<Expression, Statement> if_body, List<Tuple<Expression, Statement>> elif_body, Statement else_body)
    {
        this.if_body = if_body;
        this.elif_body = elif_body;
        this.else_body = else_body;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_if(this);
    }
}

public class BlockStmt : Statement
{
    public List<Statement> statements;

    public BlockStmt(List<Statement> statements)
    {
        this.statements = statements;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_block(this);
    }
}

public class ExprStmt : Statement
{
    public Expression expression;

    public ExprStmt(Expression expression)
    {
        this.expression = expression;
    }

    public override void visit(StatementVisitor visitor)
    {
        visitor.visit_expression(this);
    }
}