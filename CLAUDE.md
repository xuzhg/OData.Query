# CLAUDE.md

Guidance for Claude when working in this repository.

## Project overview

OData.Query is a .NET library (`Microsoft.OData.Query`) that processes OData query options (`$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`, `$apply`, ...) without depending on a heavyweight model. The pipeline has five stages:

1. **Tokenize** the query string.
   - `Lexers/`: turns raw text into expression tokens (string literal, integer literal, open parenthesis, ...).
   - `Tokenizations/`: turns expression tokens into meaningful OData tokens (path token, select token, ...).
2. **Parse** tokens into an abstract syntax tree (`Parser/`, `SyntacticAst/`, `Ast/`, `Nodes/`, `Clauses/`, `Paths/`).
3. **Build** the AST into LINQ expressions (`Builders/`, `Expressions/`).
4. **Bind** the LINQ expression to an `IQueryable` (`Binders/`).
5. **Convert** `IQueryable.Expression` back into an OData query string (`Query/`).

Design goals noted in `src/Microsoft.OData.Query/Readme.md`: DI-friendly, model-less, path support.

Entry points: `IOQueryOptionHandler`, `OQueryOptionHandler` and `OQueryOptionSettings` at the root of `src/Microsoft.OData.Query/`. `OQueryOptionHandler.ApplyTo` is still a stub that throws `NotImplementedException`.

## Repository layout

- `src/Microsoft.OData.Query/`: the library (target `net10.0`, implicit usings on). Internals are visible to the test project.
- `test/Microsoft.OData.Query.Tests/`: xUnit tests. Folders mirror the library (`Lexers`, `Parser`, `Binders`, `Tokenizations`, `SyntacticAst`, `Nodes`, `Query`, `Commons`, `Models`).
- `samples/`: `TokenizationSample`, `ApplySample`, `QueryParserSample`, `QueryComposerSample`.
- `DISample/`: dependency-injection sample.
- `OData.Query.sln`: solution containing all of the above.

## Build and test

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

- Run a single test class: `dotnet test --filter "FullyQualifiedName~ExpressionLexerTests"`.
- CI (`.github/workflows/dotnet_rolling.yml`) runs exactly restore, build and test on pushes and pull requests to `main`. A change should build and pass `dotnet test` for the whole solution before a PR is opened.
- The .NET 10 SDK is required. If `dotnet` is not installed in the working environment, say so rather than claiming the code was built or tested.

## Code conventions

- **File header**: every `.cs` file starts with the standard header, copied from an existing file:
  ```
  //-----------------------------------------------------------------------
  // Copyright (c) .NET Foundation and Contributors. All rights reserved.
  // See License.txt in the project root for license information.
  //-----------------------------------------------------------------------
  ```
- **Namespaces**: file-scoped (`namespace Microsoft.OData.Query.Binders;`), matching the folder path.
- **Line endings and encoding**: `.gitattributes` sets `* -text`, so git never converts line endings. Existing source files are UTF-8 with BOM and CRLF. Preserve each file's existing encoding and line endings when editing, and match them in new `.cs` files. Do not reformat or re-save whole files, since that produces noisy diffs.
- **Argument checks**: public methods validate arguments and throw `ArgumentNullException` with `nameof(...)`.
- **Resource strings**: user-facing error messages live in `Commons/SRResources.resx` (with the generated `SRResources.Designer.cs`). Add messages there rather than inlining string literals.
- **Nullability**: the library enables implicit usings; the test project sets `<Nullable>disable</Nullable>`.
- Keep changes small and focused. Do not touch unrelated files, and leave the stray `NewFolder` / `QueryCompositer.cs` project entries in the library `.csproj` alone unless the task is about them.

## Testing conventions

- xUnit (`[Fact]`, `[Theory]`), with Moq available.
- Test class and namespace mirror the code under test, for example `Microsoft.OData.Query.Tests.Lexers.ExpressionLexerTests`.
- Test method names follow `Subject_Behavior` (for example `QueryTokenizerContext_AlwaysIgnoresWhitespace`).
- Add or update tests next to the code you change, in the matching test folder. Bug fixes should include a test that fails before the fix.

## Git and pull requests

- The default branch is `main`. Work on a feature branch, never push directly to `main`.
- Write commit messages in the imperative mood with a short subject (for example "Honor ignored query options").
- Pull request descriptions should say what changed, why, and how it was tested.
- The repository follows the Microsoft Open Source Code of Conduct (see `README.md`).
