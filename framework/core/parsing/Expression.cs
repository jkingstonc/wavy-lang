using System.Collections.Generic;

// This interface represents a system that visits and evaluates expressions
public interface ExpressionVisitor
{
    object visit_namespace_value(NamespaceValueExpr namespace_value_expr);
    object visit_assign(AssignExpr assign_expression);
    object visit_namespace_assign(AssignNamespaceExpr assign_namespace_expr);
    object visit_literal(LiteralExpr literal_expr);
    object visit_list(ListExpr list_expr);
    object visit_variable(VariableExpr variable_expr);
    object visit_unary(UnaryExpr unary_expr);
    object visit_binary(BinaryExpr binary_expr);
    object visit_connective(ConnectiveExpr connective_expr);
    object visit_super(SuperExpr super_expr);
    object visit_this(ThisExpr this_expr);
    object visit_call(CallExpr call_expr);
    object visit_obj_get(ObjGetExpr obj_get_expr);
    object visit_obj_set(ObjSetExpr obj_set_expr);
    object visit_group(GroupExpr group_expr);
}

public abstract class Expression
{
    public abstract object visit(ExpressionVisitor visitor);
}

public class NamespaceValueExpr : Expression
{
    public Expression namespc;
    public Expression value;

    public NamespaceValueExpr(Expression namespc, Expression value)
    {
        this.namespc = namespc;
        this.value = value;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_namespace_value(this);
    }

}

// Expression for assigning to a non-member variable
public class AssignExpr : Expression
{
    public Token name;
    public Expression value;

    public AssignExpr(Token name, Expression value)
    {
        this.name = name;
        this.value = value;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_assign(this);
    }
}

public class AssignNamespaceExpr : Expression
{
    public NamespaceValueExpr identifier;
    public Expression value;

    public AssignNamespaceExpr(NamespaceValueExpr identifier, Expression value)
    {
        this.identifier = identifier;
        this.value = value;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_namespace_assign(this);
    }
}

// Expression for a literal value
public class LiteralExpr : Expression
{
    public object value;

    public LiteralExpr(object value)
    {
        this.value = value;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_literal(this);
    }
}

public class ListExpr : Expression
{
    public List<Expression> values;

    public ListExpr(List<Expression> values)
    {
        this.values = values;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_list(this);
    }
}


// Expression for a variable reference
public class VariableExpr : Expression
{
    public Token name;

    public VariableExpr(Token name)
    {
        this.name = name;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_variable(this);
    }
}


// For when we have single operator on an expression such as ~ or ! or ++
public class UnaryExpr : Expression
{
    public Token op;
    public Expression right;

    public UnaryExpr(Token op, Expression right)
    {
        this.op = op;
        this.right = right;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_unary(this);
    }
}

// For when we have arithmetic expressions
public class BinaryExpr : Expression
{
    public Expression left;
    public Token op;
    public Expression right;

    public BinaryExpr(Expression left, Token op, Expression right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_binary(this);
    }
}

// For when we have a logical operator such as x and y, for use in an ifstatement
public class ConnectiveExpr : Expression
{
    public Expression left;
    public Token op;
    public Expression right;

    public ConnectiveExpr(Expression left, Token op, Expression right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_connective(this);
    }
}

// For when we are calling a super class
public class SuperExpr : Expression
{
    // The identifier is the variable such as super.meme
    public Token super;
    public Token identifier;

    public SuperExpr(Token super, Token identifier)
    {
        this.super = super;
        this.identifier = identifier;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_super(this);
    }
}

// For when we are referencing this object
public class ThisExpr : Expression
{
    public Token this_token;
    public ThisExpr(Token this_tok)
    {
        this.this_token = this_tok;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_this(this);
    }
}

// For calling a function/method
public class CallExpr : Expression
{

    // The method / object constructor
    public Expression caller;
    public List<Expression> args;

    public CallExpr(Expression caller, List<Expression> args)
    {
        this.caller = caller;
        this.args = args;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_call(this);
    }
}

// For getting a member of an object
public class ObjGetExpr : Expression
{

    public Expression obj;
    public Token identifier;

    public ObjGetExpr(Expression obj, Token identifier)
    {
        this.obj = obj;
        this.identifier = identifier;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_obj_get(this);
    }
}

// When assigning to a member variable of an object
public class ObjSetExpr : Expression
{

    public Expression obj;
    public Token identifier;
    public Expression value;

    public ObjSetExpr(Expression obj, Token identifier, Expression value)
    {
        this.obj = obj;
        this.identifier = identifier;
        this.value = value;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_obj_set(this);
    }
}

public class GroupExpr : Expression
{
    public Expression group;

    public GroupExpr(Expression group)
    {
        this.group = group;
    }

    public override object visit(ExpressionVisitor visitor)
    {
        return visitor.visit_group(this);
    }
}