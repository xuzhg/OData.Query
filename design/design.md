
We should build a pipeline.

It takes a context (including the 'IQueryable<T>', other settings.)

1) Tokenization Service middleware

      -- Take the raw string and generate the tokens

2) TokenBinder
      -- Take the tokens and binder to AST (abstract search tree)

3) ApplyBinder
      -- Process the $apply

4) FilterBinder
      -- Process the $filter

5) OrderbyBinder
      -- Process the $orderby

