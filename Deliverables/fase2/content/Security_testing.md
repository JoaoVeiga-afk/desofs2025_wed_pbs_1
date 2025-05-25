# Security Testing

This document provides a detailed overview of the security testing practices integrated into the project. These practices aim to ensure the security, integrity, and reliability of the codebase throughout the development lifecycle.

---

## Static Application Security Testing (SAST)

- **Tool**: SonarQube (SonarCloud)
- **Integration**: Via GitHub Actions in `.github/workflows/pipe-sonar.yml`
- **Purpose**: Analyzes the source code for vulnerabilities, code smells, and insecure patterns
- **Output**: Visual reports and metrics available in the [SonarCloud dashboard](https://sonarcloud.io)

---

## Software Composition Analysis (SCA)

- **Tool**: Dependabot
- **Integration**: Native GitHub Security feature
- **Purpose**: Scans third-party dependencies for known vulnerabilities
- **Scope**: NuGet packages (`.csproj` files), updated via pull requests and alerts

---

## Code Scanning Alerts

- **Platform**: GitHub Advanced Security
- **Purpose**: Detects common coding issues and potential security flaws
- **Scope**: Runs on every push and pull request to catch early issues

---

## Linting and Code Quality

- **Tool**: `dotnet format`
- **Integration**: Via `wearerequired/lint-action` in `.github/workflows/pipe-lint.yml`
- **Purpose**: Automatically formats code and applies fixes for style issues
- **Commit**: Auto-commits fixes with a predefined message if necessary

---

## Unit Testing

- **Framework**: .NET Testing Tools
- **Project**: `ShopTex.Tests`
- **Integration**: Executed via GitHub Actions in `.github/workflows/pipe-test.yml`
- **Purpose**: Verifies functional correctness and detects logic flaws

---

## Secrets Detection

- **Tool**: [Gitleaks](https://github.com/gitleaks/gitleaks)
- **Integration**: Implemented via a dedicated GitHub Actions pipeline (`.github/workflows/pipe-scan-leaks.yml`)
- **Purpose**: Scans the codebase to detect accidentally committed secrets such as API keys, tokens, and passwords
- **Trigger**: Automatically runs on every `push` and `pull_request`
- **Behavior**:
    - Fails the build if secrets are found
    - Displays detailed scan results in the pipeline logs
- **Goal**: Prevent exposure of sensitive information in version control and enforce secret hygiene across the team


---

## Related Documents

- [CI/CD Pipeline Description](Pipelines_description.md)
- [Repository Rules](Repository_rules.md)
