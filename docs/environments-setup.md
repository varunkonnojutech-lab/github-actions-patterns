# Setting up environments with approvals

The `deploy-staging.yml` workflow targets an environment named `staging`.
The environment name alone does nothing — the protection rules you attach
to it are the actual gate. This takes about five minutes, once, in the
repository settings.

## Staging: require a human approval

1. Go to **Settings → Environments → New environment**, name it `staging`.
2. Check **Required reviewers** and add yourself (or your team).
3. Run the workflow from the **Actions** tab (it is `workflow_dispatch` only).
4. The deploy job now pauses with "Waiting for approval" until a reviewer
   approves it in the run view. Declining cancels the job.

## Production: add a wait timer and branch restrictions

For a production environment, add two more rules on top of required
reviewers:

- **Wait timer** (e.g. 5 minutes): a cooling-off window. A bad deploy
  noticed within five minutes can be cancelled before it starts.
- **Deployment branches**: restrict deploys to `main` (or to tags matching
  `v*`), so a feature branch can never reach production even if someone
  runs the workflow by hand.

## Environment secrets vs repository secrets

Secrets scoped to an environment (e.g. a staging connection string) are
only visible to jobs that target that environment — and only after the
approval gate passes. That is the safe place for deploy credentials.
Repository secrets, by contrast, are available to any workflow in the
repo. Rule of thumb: build-time values (NuGet feeds, signing keys) live
at the repository level; deploy-time credentials live at the environment
level.

## What this repo does not do

There is no real deploy target here — the deploy step prints what it
*would* deploy. That is deliberate: the pattern being taught is the gate,
not one cloud's deploy action. Swap the placeholder step for
`azure/webapps-deploy`, `azd deploy`, or `kubectl apply` and the
approval behavior is unchanged.
