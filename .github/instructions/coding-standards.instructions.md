---
description: "Use when writing, reviewing, or refactoring C# code. Covers coding standards, naming conventions, and design patterns for this solution."
applyTo: "**/*.cs"
---
# Coding Standards

## General

- Use immutability wherever possible; use `init`-only properties and records where appropriate
- Keep methods small and single-purpose
- Always use braces `{}` for control flow blocks (`if`, `else`, `for`, `foreach`, `while`, `do`) even when the body is a single statement
- Use expression-bodied members for simple properties and methods where it enhances readability
- Use `var` when the type is obvious from the right-hand side of the assignment

## File Structure

- All types (except `IsInternalInit`) should be contained in a `namespace` that matches the project name (e.g. `WS.Matrices`, `WS.Kinematics`) plus the folder structure (e.g. `WS.Matrices.Extensions`)
- Place all `using` directives in a separate `GlobalUsings.cs` file at the root of each project, except for `using` directives that are only relevant to a single file which should be placed at the top of that file ahead of the `namespace` declaration
- Order members in the following sequence: fields, properties, constructors, indexers, methods, explicit interface implementations, operators, extension methods
- For member groups except methods, order by accessibility: private, protected, internal, public
- For methods, order by functionality (e.g. public API methods first, then private helper methods)
- Use alphabetical ordering as a tiebreaker within each accessibility level and functionality group

## Naming Conventions

- Classes and methods: PascalCase
- Private fields: camelCase with no underscore prefix
- Type parameters: descriptive names prefixed with T (e.g. TSize, TRows)

## Error Handling

- When `WS.DomainModelling.Common` is referenced railway-oriented programming (ROP) patterns should be used for error handling instead of exceptions for expected failure cases
- Use Result<T, TError> from WS.DomainModelling.Common instead of throwing exceptions for expected failure cases
- Reserve exceptions for truly exceptional/unrecoverable situations

## Documentation

- All public types, members, properties, and extension methods in `src/` projects must have XML documentation comments (`<summary>`, `<param>`, `<returns>`, `<exception>` where applicable)
- Do not add XML documentation comments to test projects

### WS.Matrices

- All matrix dimensions must be expressed as Dimension subtypes (One, Two, Three, Four, Five)
- Public indexers return Result<double, string> — always unwrap via .Match()

### WS.Kinematics

<!-- Add WS.Kinematics-specific patterns here -->

## Testing

- Always use a TDD approach for new features and bug fixes
- Test file mirrors source path under the test project
- One test class per production class
