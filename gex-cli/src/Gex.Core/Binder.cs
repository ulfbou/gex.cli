namespace Gex.Core;

/// <summary>
/// Represents a symbol in the symbol table.
/// </summary>
public abstract class Symbol
{
    protected Symbol(string name, SymbolKind kind)
    {
        Name = name;
        Kind = kind;
    }

    /// <summary>
    /// Gets the name of the symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the kind of the symbol.
    /// </summary>
    public SymbolKind Kind { get; }

    /// <summary>
    /// Gets the type of the symbol.
    /// </summary>
    public abstract TypeSymbol Type { get; }
}

/// <summary>
/// Represents the kind of symbol.
/// </summary>
public enum SymbolKind
{
    Variable,
    Function,
    Parameter,
    Type,
}

/// <summary>
/// Represents a type symbol.
/// </summary>
public sealed class TypeSymbol : Symbol
{
    private TypeSymbol(string name) : base(name, SymbolKind.Type)
    {
    }

    public override TypeSymbol Type => this;

    public static readonly TypeSymbol Int = new("int");
    public static readonly TypeSymbol Float = new("float");
    public static readonly TypeSymbol String = new("string");
    public static readonly TypeSymbol Bool = new("bool");
    public static readonly TypeSymbol Void = new("void");
    public static readonly TypeSymbol Error = new("error");
}

/// <summary>
/// Represents a variable symbol.
/// </summary>
public sealed class VariableSymbol : Symbol
{
    public VariableSymbol(string name, TypeSymbol type) : base(name, SymbolKind.Variable)
    {
        _type = type;
    }

    private readonly TypeSymbol _type;
    public override TypeSymbol Type => _type;
}

/// <summary>
/// Represents a function symbol.
/// </summary>
public sealed class FunctionSymbol : Symbol
{
    public FunctionSymbol(string name, TypeSymbol returnType, IReadOnlyList<ParameterSymbol> parameters) 
        : base(name, SymbolKind.Function)
    {
        ReturnType = returnType;
        Parameters = parameters;
    }

    public TypeSymbol ReturnType { get; }
    public IReadOnlyList<ParameterSymbol> Parameters { get; }
    public override TypeSymbol Type => ReturnType;
}

/// <summary>
/// Represents a parameter symbol.
/// </summary>
public sealed class ParameterSymbol : Symbol
{
    public ParameterSymbol(string name, TypeSymbol type) : base(name, SymbolKind.Parameter)
    {
        _type = type;
    }

    private readonly TypeSymbol _type;
    public override TypeSymbol Type => _type;
}

/// <summary>
/// Represents a bound node in the semantic tree.
/// </summary>
public abstract record BoundNode
{
    /// <summary>
    /// Gets the type of this node.
    /// </summary>
    public abstract TypeSymbol Type { get; }
}

/// <summary>
/// Represents a bound expression.
/// </summary>
public abstract record BoundExpression : BoundNode;

/// <summary>
/// Represents a bound literal expression.
/// </summary>
public sealed record BoundLiteralExpression(object? Value, TypeSymbol Type) : BoundExpression
{
    public override TypeSymbol Type { get; } = Type;
}

/// <summary>
/// Represents a bound variable expression.
/// </summary>
public sealed record BoundVariableExpression(VariableSymbol Variable) : BoundExpression
{
    public override TypeSymbol Type => Variable.Type;
}

/// <summary>
/// Represents a bound binary expression.
/// </summary>
public sealed record BoundBinaryExpression(BoundExpression Left, BoundBinaryOperator Operator, BoundExpression Right) : BoundExpression
{
    public override TypeSymbol Type => Operator.ResultType;
}

/// <summary>
/// Represents a bound unary expression.
/// </summary>
public sealed record BoundUnaryExpression(BoundUnaryOperator Operator, BoundExpression Operand) : BoundExpression
{
    public override TypeSymbol Type => Operator.ResultType;
}

/// <summary>
/// Represents a bound call expression.
/// </summary>
public sealed record BoundCallExpression(FunctionSymbol Function, IReadOnlyList<BoundExpression> Arguments) : BoundExpression
{
    public override TypeSymbol Type => Function.ReturnType;
}

/// <summary>
/// Represents a bound member access expression.
/// </summary>
public sealed record BoundMemberAccessExpression(BoundExpression Target, Symbol Member) : BoundExpression
{
    public override TypeSymbol Type => Member.Type;
}

/// <summary>
/// Represents a binary operator.
/// </summary>
public sealed record BoundBinaryOperator(TokenKind TokenKind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType);

/// <summary>
/// Represents a unary operator.
/// </summary>
public sealed record BoundUnaryOperator(TokenKind TokenKind, TypeSymbol OperandType, TypeSymbol ResultType);

/// <summary>
/// Represents a binding error.
/// </summary>
public sealed record BindingError(string Message, int Position, int Line, int Column);

/// <summary>
/// Binds syntax nodes to semantic nodes with type information.
/// </summary>
public sealed class Binder
{
    private readonly Dictionary<string, Symbol> _symbols;
    private readonly List<BindingError> _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="Binder"/> class.
    /// </summary>
    public Binder()
    {
        _symbols = new Dictionary<string, Symbol>();
        _errors = new List<BindingError>();
        InitializeBuiltInSymbols();
    }

    /// <summary>
    /// Gets the binding errors.
    /// </summary>
    public IReadOnlyList<BindingError> Errors => _errors;

    /// <summary>
    /// Gets a value indicating whether binding was successful.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Binds an expression node.
    /// </summary>
    /// <param name="node">The expression node to bind.</param>
    /// <returns>The bound expression.</returns>
    public BoundExpression? BindExpression(ExpressionNode? node)
    {
        if (node == null)
        {
            return null;
        }

        return node switch
        {
            LiteralExpression literal => BindLiteralExpression(literal),
            IdentifierExpression identifier => BindIdentifierExpression(identifier),
            BinaryExpression binary => BindBinaryExpression(binary),
            UnaryExpression unary => BindUnaryExpression(unary),
            CallExpression call => BindCallExpression(call),
            MemberAccessExpression memberAccess => BindMemberAccessExpression(memberAccess),
            _ => throw new InvalidOperationException($"Unexpected node type: {node.GetType()}")
        };
    }

    /// <summary>
    /// Declares a variable symbol.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="type">The type of the variable.</param>
    /// <returns>True if the variable was declared successfully; otherwise, false.</returns>
    public bool DeclareVariable(string name, TypeSymbol type)
    {
        if (_symbols.ContainsKey(name))
        {
            AddError($"Variable '{name}' is already declared", 0, 0, 0);
            return false;
        }

        _symbols[name] = new VariableSymbol(name, type);
        return true;
    }

    /// <summary>
    /// Declares a function symbol.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="returnType">The return type of the function.</param>
    /// <param name="parameters">The parameters of the function.</param>
    /// <returns>True if the function was declared successfully; otherwise, false.</returns>
    public bool DeclareFunction(string name, TypeSymbol returnType, IReadOnlyList<ParameterSymbol> parameters)
    {
        if (_symbols.ContainsKey(name))
        {
            AddError($"Function '{name}' is already declared", 0, 0, 0);
            return false;
        }

        _symbols[name] = new FunctionSymbol(name, returnType, parameters);
        return true;
    }

    private void InitializeBuiltInSymbols()
    {
        // Built-in functions can be added here
        _symbols["print"] = new FunctionSymbol("print", TypeSymbol.Void, new List<ParameterSymbol>
        {
            new("value", TypeSymbol.String)
        });
    }

    private BoundExpression BindLiteralExpression(LiteralExpression literal)
    {
        var type = literal.Token.Kind switch
        {
            TokenKind.Number => TypeSymbol.Float,
            TokenKind.String => TypeSymbol.String,
            _ => TypeSymbol.Error
        };

        object? value = literal.Token.Kind switch
        {
            TokenKind.Number => double.TryParse(literal.Token.Text, out var num) ? num : 0.0,
            TokenKind.String => literal.Token.Text.Trim('"', '\''),
            _ => null
        };

        return new BoundLiteralExpression(value, type);
    }

    private BoundExpression? BindIdentifierExpression(IdentifierExpression identifier)
    {
        if (!_symbols.TryGetValue(identifier.Name, out var symbol))
        {
            AddError($"Undefined identifier '{identifier.Name}'", identifier.Position, identifier.Line, identifier.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        if (symbol is VariableSymbol variable)
        {
            return new BoundVariableExpression(variable);
        }

        AddError($"'{identifier.Name}' is not a variable", identifier.Position, identifier.Line, identifier.Column);
        return new BoundLiteralExpression(null, TypeSymbol.Error);
    }

    private BoundExpression? BindBinaryExpression(BinaryExpression binary)
    {
        var left = BindExpression(binary.Left);
        var right = BindExpression(binary.Right);

        if (left == null || right == null)
        {
            return null;
        }

        var op = BindBinaryOperator(binary.Operator.Kind, left.Type, right.Type);
        if (op == null)
        {
            AddError($"Binary operator '{binary.Operator.Text}' is not defined for types '{left.Type.Name}' and '{right.Type.Name}'",
                binary.Operator.Position, binary.Operator.Line, binary.Operator.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        return new BoundBinaryExpression(left, op, right);
    }

    private BoundExpression? BindUnaryExpression(UnaryExpression unary)
    {
        var operand = BindExpression(unary.Operand);
        if (operand == null)
        {
            return null;
        }

        var op = BindUnaryOperator(unary.Operator.Kind, operand.Type);
        if (op == null)
        {
            AddError($"Unary operator '{unary.Operator.Text}' is not defined for type '{operand.Type.Name}'",
                unary.Operator.Position, unary.Operator.Line, unary.Operator.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        return new BoundUnaryExpression(op, operand);
    }

    private BoundExpression? BindCallExpression(CallExpression call)
    {
        if (call.Callee is not IdentifierExpression identifier)
        {
            AddError("Only identifier can be called", call.Position, call.Line, call.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        if (!_symbols.TryGetValue(identifier.Name, out var symbol) || symbol is not FunctionSymbol function)
        {
            AddError($"Undefined function '{identifier.Name}'", identifier.Position, identifier.Line, identifier.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        if (call.Arguments.Count != function.Parameters.Count)
        {
            AddError($"Function '{identifier.Name}' expects {function.Parameters.Count} arguments, but got {call.Arguments.Count}",
                call.Position, call.Line, call.Column);
            return new BoundLiteralExpression(null, TypeSymbol.Error);
        }

        var boundArguments = new List<BoundExpression>();
        for (int i = 0; i < call.Arguments.Count; i++)
        {
            var boundArg = BindExpression(call.Arguments[i]);
            if (boundArg == null)
            {
                return null;
            }
            boundArguments.Add(boundArg);
        }

        return new BoundCallExpression(function, boundArguments);
    }

    private BoundExpression? BindMemberAccessExpression(MemberAccessExpression memberAccess)
    {
        var target = BindExpression(memberAccess.Target);
        if (target == null)
        {
            return null;
        }

        // For simplicity, just return an error type
        AddError($"Member access is not yet supported", memberAccess.Position, memberAccess.Line, memberAccess.Column);
        return new BoundLiteralExpression(null, TypeSymbol.Error);
    }

    private static BoundBinaryOperator? BindBinaryOperator(TokenKind kind, TypeSymbol leftType, TypeSymbol rightType)
    {
        // Simple operator binding for numeric types
        if (leftType == TypeSymbol.Float && rightType == TypeSymbol.Float)
        {
            return kind switch
            {
                TokenKind.Plus => new BoundBinaryOperator(kind, leftType, rightType, TypeSymbol.Float),
                TokenKind.Minus => new BoundBinaryOperator(kind, leftType, rightType, TypeSymbol.Float),
                TokenKind.Star => new BoundBinaryOperator(kind, leftType, rightType, TypeSymbol.Float),
                TokenKind.Slash => new BoundBinaryOperator(kind, leftType, rightType, TypeSymbol.Float),
                TokenKind.Percent => new BoundBinaryOperator(kind, leftType, rightType, TypeSymbol.Float),
                _ => null
            };
        }

        return null;
    }

    private static BoundUnaryOperator? BindUnaryOperator(TokenKind kind, TypeSymbol operandType)
    {
        if (operandType == TypeSymbol.Float)
        {
            return kind switch
            {
                TokenKind.Plus => new BoundUnaryOperator(kind, operandType, TypeSymbol.Float),
                TokenKind.Minus => new BoundUnaryOperator(kind, operandType, TypeSymbol.Float),
                _ => null
            };
        }

        return null;
    }

    private void AddError(string message, int position, int line, int column)
    {
        _errors.Add(new BindingError(message, position, line, column));
    }
}
