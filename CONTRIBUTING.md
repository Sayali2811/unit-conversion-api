# Contributing

1. Create a feature branch from `main`.
2. Keep the layering intact: `Domain` has no dependencies; `Application` depends only on `Domain`;
   `Infrastructure` and `Api` depend on `Application`.
3. Add or update tests for every behavioural change. Run `dotnet test` before opening a pull request.
4. Follow `.editorconfig` (file-scoped namespaces, `_camelCase` private fields, nullable reference types enabled).
5. Open a pull request into `main`; CI must pass before merging.
