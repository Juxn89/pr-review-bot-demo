# pr-review-bot-demo

A small orders API on .NET 10 (`src/Aurora.Orders.Api`) that exists to show
[claude-pr-review-bot](https://github.com/Juxn89/claude-pr-review-bot) reviewing real pull requests.
Open the pull requests tab to see the reviews it left.

- The workflow is [`.github/workflows/claude-review.yml`](.github/workflows/claude-review.yml): the
  consumer workflow from the bot's repository, with `model: claude-haiku-5-5` and a short
  `project-context`.
- It only runs on pull requests opened by the repository owner, so nobody else's pull request can spend
  the API key.
- The code builds (`dotnet build src/Aurora.Orders.Api`), but it is a sample: the repository behind it
  is in memory and the payments processor is fictional.

Case study: <https://www.juangomezb.com/en/projects/claude-pr-review-bot>
