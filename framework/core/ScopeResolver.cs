using System;
using System.Collections.Generic;

// Used to resolve the scope of local variables, and check the variable references are valid
public class ScopeResolver : StatementVisitor, ExpressionVisitor
{
    public enum FunctionType
    {
        None,
        Function,
        Method,
        Constructor
    };
    public enum ClassType
    {
        None,
        Class,
        SubClass
    };

    public Interpreter interpreter;

    public bool in_loop = false;
    public FunctionType function_type = FunctionType.None;
    public ClassType class_type = ClassType.None;

    // To tell us whether values have been assigned and are accessible
    public Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
    // Scopes of namespaces
    public Dictionary<string, Stack<Dictionary<string, bool>>> namespaces = new Dictionary<string, Stack<Dictionary<string, bool>>>();

    public ScopeResolver(Interpreter interpreter)
    {
        this.interpreter = interpreter;
    }

    // Resolve a list of statements
    public void resolve(List<Statement> statements)
    {

        foreach (Statement statement in statements)
        {
            resolve(statement);
        }
    }

    private void resolve(Statement statement)
    {
        statement.visit(this);
    }

    private void resolve(Expression expression)
    {
        expression.visit(this);
    }

    // If we have assigned to a variable, we need to tell the interpreter the position of the variable
    // We are assigning to
    private void resolve_local_position(Expression expression, Token name)
    {
        resolve_local_position(expression, (string)name.value);
    }

    // If we have assigned to a variable, we need to tell the interpreter the position of the variable
    // We are assigning to
    private void resolve_local_position(Expression expression, string name)
    {
        Dictionary<string, bool>[] scope_arr = scopes.ToArray();
        // Go through each scope from the current
        for (int i = scopes.Count-1; i >= 0; i--)
        {
            if (scope_arr[i].ContainsKey(name))
            {
                // Indicate the position of the variable in the scope in the interpreter
                interpreter.resolve(expression, scopes.Count - 1 - i);
                return;
            }
        }
    }

    private void resolve_function(FunctionStmt function, FunctionType type)
    {
        FunctionType enclosing_function = function_type;
        function_type = type;
        start_scope();
        foreach (Token param in function.paramaters)
        {
            declare(param);
            define(param);
        }
        resolve(function.body);
        end_scope();
        function_type = enclosing_function;
    }

    // Begin a new local scope
    private void start_scope()
    {
        this.scopes.Push(new Dictionary<string, bool>());
    }

    // End a local scope
    private void end_scope()
    {
        this.scopes.Pop();
    }

    // For when we have assigned to a value
    private void define(Token name)
    {
        if (scopes.Count > 0)
        {
            this.scopes.Peek()[(string)name.value] = true;
        }
    }

    // For when we are declaring a new value
    private void declare(Token name)
    {
        if (scopes.Count > 0)
        {
            Dictionary<string, bool> current = this.scopes.Peek();
            if (current.ContainsKey((string)name.value))
            {
                throw new RuntimeException("Variable '" + name.value + "' already exists in this scope");
            }
            current[(string)name.value] = false;
        }
    }

    public object visit_namespace_value(NamespaceValueExpr namespace_value_expr)
    {
        if (this.namespaces.ContainsKey((string)namespace_value_expr.namespc.name.value))
        {
            // Save the scope and set the current to the desired namespace
            Stack<Dictionary<string, bool>> previous_scope = this.scopes;
            this.scopes = this.namespaces[(string)namespace_value_expr.namespc.name.value];
            resolve_local_position(namespace_value_expr, namespace_value_expr.identifier);
            this.scopes = previous_scope;
            return null;
        }
        // This causes errors becuase it doesn't account for imports
        //throw new RuntimeException("Cannot find namespace '" + (string)namespace_value_expr.namespc.name.value + "'");
        return null;
    }

    public object visit_assign(AssignExpr assign_expression)
    {
        resolve(assign_expression.value);
        // Resolve the variable to the interpreter
        resolve_local_position(assign_expression, assign_expression.name);
        return null;
    }

    public object visit_namespace_assign(AssignNamespaceExpr assign_namespace_expr)
    {
        if (this.namespaces.ContainsKey((string)assign_namespace_expr.identifier.namespc.name.value))
        {
            // Save the old scope and set the current to the new namespace scope
            Stack<Dictionary<string, bool>> previous_scope = this.scopes;
            this.scopes = this.namespaces[(string)assign_namespace_expr.identifier.namespc.name.value];
            resolve(assign_namespace_expr.value);
            // Resolve the variable to the interpreter
            resolve_local_position(assign_namespace_expr, assign_namespace_expr.identifier.identifier);
            this.scopes = previous_scope;
            return null;
        }
        throw new RuntimeException("Namespace with name '" + (string)assign_namespace_expr.identifier.namespc.name.value + "' doesn't exist");
    }

    public object visit_literal(LiteralExpr literal_expr)
    {
        // Doesn't need resolving
        return null;
    }
    public object visit_list(ListExpr list_expr)
    {
        foreach(Expression expr in list_expr.values)
        {
            resolve(expr);
        }
        return null;
    }

    public object visit_variable(VariableExpr variable_expr)
    {
        // check if the variable exists, but hasn't been assigned
        if (scopes.Count > 0 && (scopes.Peek().ContainsKey((string)variable_expr.name.value) && scopes.Peek()[(string)variable_expr.name.value] == false))
        {
            throw new RuntimeException("Cannot intialise '" + variable_expr.name + "' variable value with itself");
        }
        // Resolve the variable to the interpreter
        resolve_local_position(variable_expr, variable_expr.name);
        return null;
    }

    public object visit_unary(UnaryExpr unary_expr)
    {
        resolve(unary_expr.right);
        return null;
    }

    public object visit_binary(BinaryExpr binary_expr)
    {
        resolve(binary_expr.left);
        resolve(binary_expr.right);
        return null;
    }

    public object visit_connective(ConnectiveExpr connective_expr)
    {
        resolve(connective_expr.left);
        resolve(connective_expr.right);
        return null;
    }

    public object visit_super(SuperExpr super_expr)
    {
        if (class_type == ClassType.None)
        {
            throw new RuntimeException("Cannot use super outside of a class");
        }
        else if (class_type != ClassType.SubClass)
        {
            throw new RuntimeException("Cannot use super in a class that doesn't extend another");
        }
        // Resolve the super identifier
        // This may need to be changed to identifier
        resolve_local_position(super_expr, "super");
        return null;
    }

    public object visit_this(ThisExpr this_expr)
    {
        if (class_type == ClassType.None)
        {
            throw new RuntimeException("Cannot use this outside of a class");
        }
        // Resolve the this identifier
        resolve_local_position(this_expr, "this");
        return null;
    }

    public object visit_call(CallExpr call_expr)
    {
        resolve(call_expr.caller);
        foreach (Expression arg in call_expr.args)
        {
            resolve(arg);
        }
        return null;
    }

    public object visit_obj_get(ObjGetExpr obj_get_expr)
    {
        resolve(obj_get_expr.obj);
        return null;
    }

    public object visit_obj_set(ObjSetExpr obj_set_expr)
    {
        resolve(obj_set_expr.obj);
        resolve(obj_set_expr.value);
        return null;
    }

    public object visit_group(GroupExpr group_expr)
    {
        resolve(group_expr.group);
        return null;
    }

    public void visit_compile(CompileStmt Compile_stmt)
    {
        resolve(Compile_stmt.code);
    }

    public void visit_using(UsingStmt using_stmt)
    {
        if (this.namespaces.ContainsKey((string)using_stmt.namespace_identifier.value))
        {
            return;
        }
        // This causes errors becuase it doesn't account for imports
        //throw new RuntimeException("Cannot find namespace '" + (string)using_stmt.namespace_identifier.value + "'");
        return;
    }

    public void visit_import(ImportStmt import_stmt)
    {
        // Doesn't need resolving
        return;
    }

    public void visit_return(ReturnStmt return_stmt)
    {
        if (function_type == FunctionType.None)
        {
            throw new RuntimeException("Cannot return from a non-function");
        }
        if (return_stmt.value != null)
        {
            if (function_type == FunctionType.Constructor)
            {
                throw new RuntimeException("Cannot return from a constructor");
            }
            resolve(return_stmt.value);
        }
    }

    public void visit_continue(ContinueStmt continue_stmt)
    {
        if (!in_loop)
        {
            throw new RuntimeException("Cannot continue from a non-loop");
        }
    }

    // Visit a namespace definition
    public void visit_namespace(NamespaceStmt namespace_stmt)
    {
        if (this.namespaces.ContainsKey((string)namespace_stmt.name.value))
        {
            throw new RuntimeException("Namespace with name '" + (string)namespace_stmt.name.value + "' already exists");
        }
        // Add a new namespace to the dicionary
        // Do we want namespace variables to be indipendent or have the previous scope? [new Scope(this.scoped);]
        this.namespaces.Add((string)namespace_stmt.name.value, new Stack<Dictionary<string, bool>>());
        // Save the old scope and set the current to the new namespace scope
        Stack<Dictionary<string, bool>> previous_scope = this.scopes;
        this.scopes = this.namespaces[(string)namespace_stmt.name.value];
        start_scope();
        resolve(namespace_stmt.body);
        // Restore the previous scope
        this.scopes = previous_scope;
    }

    public void visit_class(ClassStmt class_stmt)
    {
        ClassType enclosing_class = class_type;
        class_type = ClassType.Class;
        declare(class_stmt.name);
        if (class_stmt.superclass != null)
        {
            resolve(class_stmt.superclass);
        }
        define(class_stmt.name);
        // Add the scope of the superclass
        if (class_stmt.superclass != null)
        {
            class_type = ClassType.SubClass;
            start_scope();
            scopes.Peek()["super"] = true;
        }
        // Start the scope of this class
        start_scope();
        scopes.Peek()["this"] = true;
        foreach (FunctionStmt method in class_stmt.methods)
        {
            FunctionType decleration = FunctionType.Method;
            // Check if we have a constructor
            if ((string)method.name.value == (string)class_stmt.name.value)
            {
                decleration = FunctionType.Constructor;
            }
            resolve_function(method, decleration);
        }
        end_scope();
        if (class_stmt.superclass != null)
        {
            end_scope();
        }
        class_type = enclosing_class;
    }

    public void visit_function(FunctionStmt function_stmt)
    {
        declare(function_stmt.name);
        define(function_stmt.name);
        resolve_function(function_stmt, FunctionType.Function);
    }

    public void visit_define(DefStmt define_stmt)
    {
        declare(define_stmt.name);
        if (define_stmt.value != null)
        {
            resolve(define_stmt.value);
        }
        define(define_stmt.name);
    }

    public void visit_break(BreakStmt break_stmt)
    {
        if (!in_loop)
        {
            throw new RuntimeException("Cannot break out of non-loop");
        }
    }

    public void visit_trycatch(TryCatchStmt trycatch_stmt)
    {
        resolve(trycatch_stmt.try_body);
        resolve_function(trycatch_stmt.catch_body, FunctionType.Function);
    }

    public void visit_interrupt(InterruptStmt interrupt_stmt)
    {
        resolve(interrupt_stmt.expression);
    }

    public void visit_foriter(ForIterStmt foriter_stmt)
    {
        bool enclosing_loop = in_loop;
        in_loop = true;
        resolve(foriter_stmt.counter_definition);
        resolve(foriter_stmt.iterator_object);
        resolve(foriter_stmt.body);
        in_loop = enclosing_loop;
    }

    public void visit_for(ForStmt for_stmt)
    {
        bool enclosing_loop = in_loop;
        in_loop = true;
        resolve(for_stmt.counter_definition);
        resolve(for_stmt.condition);
        resolve(for_stmt.counter_action);
        resolve(for_stmt.body);
        in_loop = enclosing_loop;
    }

    public void visit_while(WhileStmt while_stmt)
    {
        bool enclosing_loop = in_loop;
        in_loop = true;
        resolve(while_stmt.condition);
        resolve(while_stmt.body);
        in_loop = enclosing_loop;
    }
    public void visit_if(IfStmt if_stmt)
    {
        // Resolve the if condition
        resolve(if_stmt.if_body.Item1);
        // Resolve the if body
        resolve(if_stmt.if_body.Item2);
        foreach(Tuple<Expression, Statement> elif in if_stmt.elif_body)
        {
            resolve(elif.Item1);
            resolve(elif.Item2);
        }
        if(if_stmt.else_body!=null)
        {
            resolve(if_stmt.else_body);
        }
    }

    public void visit_block(BlockStmt block_stmt)
    {
        start_scope();
        resolve(block_stmt.statements);
        end_scope();
    }
    public void visit_expression(ExprStmt expr_stmt)
    {
        resolve(expr_stmt.expression);
    }
}