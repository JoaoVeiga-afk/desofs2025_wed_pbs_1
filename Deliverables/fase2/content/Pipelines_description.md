## CI/CD Workflows

Our project uses modular GitHub Actions to enforce quality, correctness, and analysis:

| Workflow      | Purpose                    | File                                    |
|---------------|----------------------------|-----------------------------------------|
| Linting       | Analyse the code style     | `.github/workflows/pipe-lint.yml`       |
| Build         | Restore & compile the code | `.github/workflows/pipe-build.yml`      |
| Test          | Run unit tests             | `.github/workflows/pipe-test.yml`       |
| SonarQube     | Static code analysis       | `.github/workflows/pipe-sonar.yml`      |
| Scan Leaks    | Detect secret leaks        | `.github/workflows/pipe-scan-leaks.yml` |
| Main Pipeline | Orchestrates all jobs      | `.github/workflows/pipe-main.yml`       |


---

## `pipe-main.yml` – Main Pipeline

**Description**:  
The central workflow that orchestrates all other workflows in a defined order. It is triggered on every `push` or `pull_request` to any branch.

**Steps executed**:
1. `pipe-lint.yml` – Lint and format code
2. `pipe-build.yml` – Restore and compile the project
3. `pipe-sonar.yml` – Run static code analysis using SonarCloud
4. `pipe-test.yml` – Execute unit tests
5. `pipe-scan-leaks.yml` – Scan for secret leaks using GitHub Leaks
---

---

## `pipe-lint.yml` – Lint Workflow

**Description**:  
Runs `dotnet-format` on the solution to enforce code style consistency. If any issues are found, it attempts to auto-fix and commits the changes with a predefined message.

**Triggered by**:  
`workflow_call` (called by the main pipeline)

**Tools used**:
- [`wearerequired/lint-action`](https://github.com/wearerequired/lint-action)
- `.NET 9.0`
- `dotnet format`

---

---

## `pipe-build.yml` – Build Workflow

**Description**:  
Builds the solution in `Release` mode using the .NET SDK. It ensures dependencies are restored beforehand.

**Triggered by**:  
`workflow_call` (called after the lint job)

**Environment**:  
Ubuntu, .NET 9.0

---


## `pipe-sonar.yml` – SonarQube Analysis

**Description**:  
Performs static code analysis using **SonarCloud**. This step evaluates code quality, maintainability, duplication, and coverage.

**Triggered by**:  
`workflow_call` (called after build)

**Requirements**:
- The repository must be public to use SonarCloud's free plan
- `SONAR_TOKEN` secret must be defined

---

## `pipe-test.yml` – Test Workflow

**Description**:  
Runs all unit tests located in `ShopTex.Tests` to ensure the application's functionality is correct.

**Triggered by**:  
`workflow_call` (called after Sonar analysis)

**Environment**:  
Ubuntu, .NET 9.0

---

## `pipe-scan-leaks.yml` – GitHub Leaks Workflow

**Description**:  
This workflow integrates **GitHub Leaks** to scan for secrets in the repository. It runs automatically with every commit to detect any exposed sensitive information such as API keys, credentials, or tokens.

**Triggered by**:  
`workflow_call` (called after tests)

**Tools used**:
- [`gitleaks/gitleaks-action`](https://github.com/gitleaks/gitleaks-action)
- GitHub Leaks configuration

**Outcome**:  
If any leaks are found, the build will fail,