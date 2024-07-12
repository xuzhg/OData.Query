## OData.Query

5 parts:

1) Tokenize the OData Query into tokens
    a) Lexer: Convert OData Query to expression token, expression token is a token likes "string literal", "interger literal", "Open parenthesis", etc.
    b) Tokenizer: Consume the expression token and convert to meanlingful OData Token, for example, it's "OData Path token", "Select token", etc.

2) Parse tokens to OData abstract search tree node (AST)
3) Build AST into LINQ Expression
4) Bind LINQ Expression to IQueryable
5) Convert IQueryable.Expresion to OData Query string


1) DI 
2) model-less
3) path....