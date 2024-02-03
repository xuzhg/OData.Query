## OData.Query

5 parts:
1) Tokenize the OData Query into tokens
2) Parse tokens to OData abstract search tree node (AST)
3) Build AST into LINQ Expression
4) Bind LINQ Expression to IQueryable
5) Convert IQueryable to OData Query string