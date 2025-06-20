﻿# Security Testing

This document provides a detailed overview of the security testing practices integrated into the project. These practices aim to ensure the security, integrity, and reliability of the codebase throughout the development lifecycle.

---

## Static Application Security Testing (SAST)

- **Tool**: SonarQube (SonarCloud)
- **Integration**: Via GitHub Actions in `.github/workflows/pipe-sonar.yml`
- **Purpose**: Analyzes the source code for vulnerabilities, code smells, and insecure patterns
- **Output**: Visual reports and metrics available in the [SonarCloud dashboard](https://sonarcloud.io)

---

## Software Composition Analysis (SCA)

### Dependabot
- **Tool**: Dependabot
- **Integration**: Native GitHub Security feature / GitHub Actions
- **Purpose**: Scans third-party dependencies for known vulnerabilities and provides automated pull requests to update vulnerable packages
- **Scope**: NuGet packages (`.csproj` files), updated via pull requests and alerts, or manually triggered in workflows with Snyk for detailed security analysis

### Snyk

- **Tool**: Snyk
- **Integration**: GitHub Actions
- **Purpose**: Performs security analysis on the project’s dependencies after deployment, detecting vulnerabilities in third-party libraries to ensure the security of the production environment.
- **Triggered by**: Automatically triggered once the "Deploy Pipeline" workflow completes successfully.
- **Tools used**: Snyk (via `snyk/snyk-action@v1`) to perform software composition analysis, checking for vulnerabilities in third-party libraries and suggesting fixes or alternatives.

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

## Dynamic Code Analysis (DAST)
- **Tool**: OWASP ZAP (zaproxy/action-full-scan@v0.12.0)
- **Integration**: Implemented via GitHub Actions in `.github/workflows/pipe-dca.yml`
- **Purpose**: Performs dynamic security analysis of the application’s endpoints to detect vulnerabilities such as SQL injection, authentication failures, and other security issues.
- **Trigger**: Automatically triggered after the "Deploy Pipeline" workflow completes.
- **Behavior**:
  - Conducts a full security scan on specific API endpoints.
  - Performs a health check using the `/api/ping` endpoint to ensure the application is online before proceeding with the security scan.
  - Results are stored as artifacts for further analysis and review.
- **Goal**: Ensure that security vulnerabilities in the live environment are detected and addressed.

---

## Related Documents

- [CI/CD Pipeline Description](Pipelines_description.md)
- [Repository Rules](Repository_rules.md)
