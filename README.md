# GitHub Actions Patterns for .NET — matrix builds, reusable workflows & approval-gated deploys

[![Build](https://github.com/varunkonnojutech-lab/github-actions-patterns/actions/workflows/ci.yml/badge.svg)](https://github.com/varunkonnojutech-lab/github-actions-patterns/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/varunkonnojutech-lab/github-actions-patterns)

Most GitHub Actions tutorials stop at "build and test on push." Real pipelines need more: build on several OSes without duplicating jobs, share one build definition across repos instead of copy-pasting it five times, and put a human approval in front of production. This repo demonstrates all three patterns with workflows that actually run — against a small, real .NET 8 app, so you can watch each pattern work instead of reading about it.

## Who is this for

.NET developers who have a working `ci.yml` and are starting to feel its limits: the same build pasted into a second repo, a deploy step anyone can trigger, or a "does it build on Windows?" question nobody can answer. If you ship .NET services through GitHub Actions, these are the next three patterns to learn.

## What you'll learn

- **Matrix builds** — one job definition, many combinations (OS × configuration), with `fail-fast: false` so one failure doesn't hide the rest
- **Reusable workflows** — a canonical build owned in one file, called from a thin caller workflow (and, with one change, from other repos)
- **Environments & approvals** — the `environment:` key plus required reviewers, so deploys pause for a human; plus wait timers, branch restrictions, and environment-scoped secrets
- **The supporting habits** — NuGet caching, concurrency groups that cancel stale runs, and tag-triggered releases that keep "release" a deliberate act
- **What not to copy** — where each pattern breaks down (matrix explosion, reusable-workflow limits, the environment name that isn't a gate by itself)

## Quick start

No GitHub account needed to run the app itself:

```bash
dotnet run --project src/ActionsDemo.Api
curl localhost:5000/health        # {"status":"ok"}
curl "localhost:5000/greet?name=Ada"   # {"greeting":"Hello, Ada!"}
```

Run the tests:

```bash
dotnet test
```

To see the patterns work, fork the repo and push a commit: the CI, matrix, and reusable-caller workflows all run on push to `main`. The deploy and release workflows are manual/tag-only by design — see below.

## The app (deliberately boring)

A `GreetingService` library (greet a name, greet in Spanish/French/German, blank names fall back to "world") behind a minimal API (`GET /health`, `GET /greet`, `GET /greet/{language}`). The app is not the lesson — it just needs to be real enough that the workflows build and test something genuine, with tests that can actually fail if you break the code.

## The workflows, pattern by pattern

| Workflow | Pattern | When it runs | What to notice |
|---|---|---|---|
| `ci.yml` | Everyday pipeline | Push / PR to `main` | NuGet cache keyed on `*.csproj` hashes; concurrency group cancels stale runs on the same branch |
| `matrix-build.yml` | Matrix builds | Push / PR to `main` | 2 OSes × 2 configurations = 4 jobs from one definition; `fail-fast: false` |
| `reusable-dotnet.yml` | Reusable workflow | Only when called | `workflow_call` inputs with defaults; owns the steps, callers own nothing |
| `call-reusable.yml` | Calling a reusable workflow | Push / PR to `main` | One job, zero steps — just `uses: ./.github/workflows/reusable-dotnet.yml`; cross-repo callers pin a tag, never a branch |
| `deploy-staging.yml` | Environments & approvals | Manual dispatch only | `environment: staging` job; build once, publish as an artifact, deploy from the artifact — the gate is the required reviewers you configure in Settings (see `docs/environments-setup.md`) |
| `release.yml` | Tag-triggered releases | Tag `v*` or manual dispatch | A release is a deliberate tag, not an accident of pushing to `main`; publishes and uploads the artifact |

## The three patterns, in brief

### 1. Matrix builds: multiply jobs, not YAML

```yaml
strategy:
  fail-fast: false
  matrix:
    os: [ubuntu-latest, windows-latest]
    configuration: [Debug, Release]
runs-on: ${{ matrix.os }}
```

Every axis multiplies your job count, so keep matrices small — OS only, or .NET versions only, rarely both. When combinations are uneven (test on Ubuntu, build everywhere), reach for `include`/`exclude` instead of a second workflow.

### 2. Reusable workflows: one build, many callers

The reusable file declares `on: workflow_call` with typed inputs; the caller is a single job with no steps. This is the fix for "ci.yml copy-pasted into five repos, all drifted apart." Two limits to know: a reusable workflow can't call another reusable workflow more than a few levels deep in older setups, and secrets must be passed explicitly (`secrets: inherit` exists, but passing named secrets is clearer about what a caller actually needs).

### 3. Environments: the name is not the gate

```yaml
jobs:
  deploy:
    environment: staging   # the gate lives in Settings → Environments
```

Writing `environment: staging` without configuring required reviewers changes nothing — the job just runs. The approval comes from the reviewers you add; the wait timer and branch restrictions are the production upgrades. Deploy credentials belong in environment secrets, not repository secrets, so they're only visible to jobs that passed the gate.

## Try it yourself

1. **Matrix**: change `matrix.os` to add `macos-latest` and watch the job count jump — then decide whether that 50% cost increase is worth it. (Usually: no, unless you ship desktop apps.)
2. **Reusable**: add a second caller that passes `run-tests: false` and `configuration: Debug` — same build, different shape, zero duplicated steps.
3. **Approvals**: follow `docs/environments-setup.md` to add a required reviewer to `staging`, run the deploy workflow by hand, and watch it pause for your approval.
4. **Break it on purpose**: change a greeting in `Greeter.cs` without updating the test, push, and watch CI, the matrix, and the reusable caller all go red together — that's the point of running the same build three ways.

## Next steps

- Read the workflow files top to bottom — every non-obvious line has a comment explaining *why*, not just *what*.
- If you deploy to Azure, swap the placeholder deploy step in `deploy-staging.yml` for `azure/webapps-deploy` and keep the approval gate as-is.
- For the companion patterns — Docker layer caching, Terraform for the infrastructure these workflows deploy to — see `docker-dotnet-mastery` and `terraform-azure` in this portfolio.
