# AGENTS.md

Guidance for coding agents (and new developers) working in this repository.
Read it before writing, reviewing or generating any code.

---

## 1. Skills — read BEFORE working

The team's conventions and workflows live in `.agents/skills/`. Do not
improvise: when a task is covered by a skill, follow it as the source of truth.
If two skills ever disagree, the house skills (`conventions`, `commits`) win.

| Skill | When to use |
|-------|-------------|
| **`conventions`** | **Mandatory** for any C# work — how code must be written (see below). |
| **`commits`** | **Mandatory** before writing any commit message or PR title (see below). |
| **`system-design`** | Designing scalable distributed systems: capacity estimation, load balancing, caching, database scaling, message queues. |
| **`architecture-designer`** | Architectural decisions and reviews: design patterns, microservices, ADRs, scalability planning. |

### `conventions` — mandatory for C# code

This is our codebase's coding convention: **how code must be written**, what we
consider **anti-patterns**, the **do's and don'ts**, formatting style and
everything else. Load/read this skill **whenever** you touch C# code —
controllers, handlers, repositories, schemes (DTOs), filters, validators,
mappers, service registration (IoC) or tests.

In short (the skill is the complete reference):

- **Layered architecture** (DDD/Clean): `Domain → Application → Infrastructure (+ Infrastructure.IoC) → WebApi`.
- **Formatting:** 4 spaces, file-scoped `namespace`, `public sealed` classes,
  primary constructors for DI, one type per file. The root `.editorconfig`
  promotes several rules to **error**.
- **Global usings** in a `Usings.cs` per project, in sections — a `using` at the
  top of a file is an anti-pattern.
- **Controllers** hold no logic: only `IDispatcher` + a `switch` with pattern
  matching on the `Result`. Logic lives in the **handlers** (Application),
  which **fail fast** (guard clauses + early return, happy path last).
- **Repositories** only **filter and count**; writes (`Insert`/`Update`/`Delete`)
  come from the base abstractions.
- **Errors** in a static catalog, with a `#NAME-ERROR-XXXXX` code.
- **Tests** both unit **and** integration, named `Given_When_Then`, with
  `arrange/act/assert`.
- **Anti-patterns we reject:** private methods, commenting production code, a
  `using` at the top of a file, logic in controllers, nested `else`/`if`,
  "magic" header/content-type strings, among others (see §11 of the skill).

> When in doubt, mirror an equivalent existing file before inventing something.

### `commits` — mandatory before committing

- One single **inline** line: `<type>(#<id>): this commit <verb> ...`. Never a
  wrapped multi-line body, and never short.
- Always starts with `this commit <verb>`; we **reject the verb "adds"**.
- **No attribution trailers** — no `Co-Authored-By`, no "Generated with ..."
  footer. Just the message.
- `#<id>` is the real issue/task id (GitHub, Azure DevOps, …); use `#00` only
  when there is genuinely no work item.
- The `type` drives the **GitVersion** bump: `feature` / `fix` / `feature!`
  version-bumping; `chore` / `docs` / `refactor` / `test` / `style` / `perf` /
  `ci` do not. See the skill (and `.gitmessage`) for the full table.

### design skills (`system-design`, `architecture-designer`)

Community skills (MIT) for architecture work — distributed system design,
capacity estimation, design patterns, ADRs and design reviews. Use them when
shaping or reviewing a design; they inform the *how* of the design, while the
house `conventions` always governs the *how* of the resulting C# code.

---

## 2. Repository layout

```
.
├── .agents/skills/        # team skills (conventions, commits, design)
├── .github/workflows/     # CI (quality assurance)
├── backend/               # .NET 10 solution (all .NET config lives here)
│   ├── Apta.sln
│   ├── Source/            # production projects (layers + WebApi)
│   ├── Tests/             # xUnit v3 (Microsoft.Testing.Platform)
│   ├── Dockerfile
│   ├── global.json        # SDK pin + test runner (MTP)
│   ├── nuget.config       # feed restricted to nuget.org
│   └── .config/           # tool manifest (gitversion, reportgenerator)
├── frontend/              # front-end application
├── documentation/
└── compose.yml            # local infra (mongo, keycloak, seq)
```
