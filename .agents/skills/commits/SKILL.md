---
name: commits
description: >-
  Mandatory convention for writing git commit messages (and PR titles) in our
  repositories. Use it whenever you are about to commit, draft a commit message
  or open a pull request. It defines the exact format, the required
  "this commit <verb> ..." voice, the verb vocabulary we accept (and the ones we
  reject, such as "adds"), the type-to-GitVersion bump mapping, and the
  issue/task id that every message must carry. It also fixes our style: a single
  inline message, never short, never broken across multiple lines.
---

# Commit messages

This is how we write every commit message. The point is a history that reads
consistently and drives versioning correctly — **GitVersion** derives the next
version from the commit `type` (see `GitVersion.yml`), so the message is not
cosmetic.

The template lives in `.gitmessage`; this skill is the operational guide for
actually writing the message.

---

## 1. Format

```
<type>(#<id>): this commit <verb> <what changed and, inline, the why/context>
```

- `type` — one of the types in §3.
- `#<id>` — the issue/task id (§4). Required.
- The text **always** starts with `this commit <verb>` (§2).
- Everything is **one single inline line** (§5). No wrapped body, no line breaks.

---

## 2. Always "this commit <verb>" — and never "adds"

Every message starts with `this commit ` followed by a present-tense,
third-person verb describing the effect. Describe the **effect/intent**, not the
implementation, and **never name classes, methods, variables or files**.

**We reject "adds".** It is vague and we dislike it — reach for a verb that says
what actually happened.

Verbs we use (not exhaustive):

`introduces` · `implements` · `removes` · `updates` · `refactors` ·
`simplifies` · `fixes` · `prevents` · `renames` · `moves` · `replaces` ·
`extracts` · `restricts` · `pins` · `configures` · `wires` · `exposes` ·
`enables` · `disables` · `containerizes` · `documents`

✅ `this commit introduces the backend clean architecture layers`
❌ `this commit adds some projects`

---

## 3. Types and the GitVersion bump

The `type` decides the version bump, so pick it deliberately:

| Type       | Meaning                                | Version bump |
|------------|----------------------------------------|--------------|
| `feature`  | new functionality                      | **minor** (0.X.0) |
| `fix`      | bug fix                                 | **patch** (0.0.X) |
| `feature!` | breaking change (the `!` before the `:`) | **major** (X.0.0) |
| `chore`    | tooling / config / meta                 | none |
| `docs`     | documentation only                      | none |
| `refactor` | code change, no behavior change         | none |
| `test`     | tests only                              | none |
| `style`    | formatting only                         | none |
| `perf`     | performance improvement                 | none |
| `ci`       | pipelines / workflows                   | none |

Only `feature` / `fix` / `feature!` move the version.

---

## 4. `#<id>` is the issue/task id — it matters

The `#<id>` is the **real identifier of the work item** — the GitHub issue, the
Azure DevOps task/work item, or whatever tracker drives the change. It is **not
decorative**: it links the commit back to the task, so always use the actual id.

- `feature(#1423): this commit ...` → work item 1423.
- Use `#00` **only** when there is genuinely no associated task.

---

## 5. Style — one inline line, substantial, not huge

- **Inline. One line.** No multi-line body, no hard wrapping, no line breaks.
  Everything the message says, it says on the subject line.
- **Not short.** A terse `this commit updates the dockerfile` is not enough —
  say what changed and, inline, the why/context so the line stands on its own.
- **Not huge either.** Enough to be meaningful; stop before it turns into a
  paragraph crammed onto one line.
- **All lowercase**, English, **no trailing period**.
- **No class/method/variable/file names** in the message — describe the effect,
  not the code.
- **No trailers, no attribution.** Never append a `Co-Authored-By`, a
  "Generated with ..." footer or any similar line — it is ugly, especially on
  agent-made commits. Just the message, nothing else.

---

## 6. Examples

✅ Good (inline, substantial, right verb, real id):

```
chore(#1423): this commit scopes the dotnet build configuration to the backend so the sdk pin, package source and local tools apply only to the dotnet workspace
```

```
chore(#88): this commit introduces the backend clean architecture layers wired with inward-pointing references so the solution carries its layered skeleton
```

```
ci(#204): this commit migrates the quality pipeline to the microsoft testing platform so the backend builds and collects coverage through the supported path
```

```
chore(#311): this commit containerizes the backend web api with a multi-stage linux image that restores, builds and publishes the service on a slim runtime base
```

```
fix(#512): this commit prevents duplicate items from being appended to an order when the same product is submitted twice in quick succession
```

```
feature(#77): this commit introduces table session checkout so a table can be closed and its order settled in a single operation
```

```
feature(#90)!: this commit changes the order contract to split items from modifiers, breaking existing clients that read a flat item list
```

❌ Bad — and why:

```
adds dockerfile
```
→ uses `adds`, no `type`, no id, not `this commit ...`, too short.

```
chore(#00): this commit updates stuff
```
→ vague, no context, and `#00` used when a real task id exists.

```
chore(#12): this commit updates AssetsController and AssetService
```
→ names files/classes; describe the effect instead.

```
chore(#12): this commit introduces the docker setup

it adds a multi-stage image and ignore rules so the api can be
built as a container
```
→ multi-line wrapped body; keep it inline on a single line.

```
chore(#12): this commit containerizes the backend

Co-Authored-By: Some Agent <noreply@example.com>
```
→ never append a co-author/attribution trailer; the message stands alone.
