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
| Deployment    | Deploy to production       | `.github/workflows/pipe-deploy.yml`     |
| DCA           | Dynamic Code Analysis      | `.github/workflows/pipe-dca.yml`        |


> Note: An IAST pipeline was not included due to the unavailability of free licenses or non-commercial versions suitable for academic environments. The existing solutions on the market are largely aimed at companies (enterprise-grade), which made it impossible to integrate them into this project.
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

---

## 'pipe-deploy.yml` – Deployment Workflow'

**Description**:
This workflow is responsible for deploying the application to the production environment. It is triggered automatically after a commit to the main branch.

**Triggered by**:
`on push` to the `main` branch

**Tools used**:
- SCP for secure file transfer
- SSH for remote execution
- Service management commands to restart the application

**Server Used**:
- Fedora 38 with .NET 9.0 installed: vs531.dei.ipp.pt

---

## 'pipe-dca.yml` – Dynamic Code Analysis Workflow'

**Description**:
This workflow is responsible for performing a dynamic security analysis on the application after the completion of a deploy workflow. It checks specific vulnerabilities on the application's endpoints to ensure the security of the production environment. Additionally, it includes a health check to ensure the application is online before performing the security scan.

**Triggered by**:
This workflow is automatically triggered once the "Deploy Pipeline" workflow completes successfully.

**Tools used**:
- OWASP ZAP (- [`zaproxy/action-full-scan@v0.12.0`](https://github.com/zaproxy/action-full-scan)): For performing dynamic security analysis of the application’s endpoints, detecting vulnerabilities such as SQL injection, authentication failures, and other security issues.
