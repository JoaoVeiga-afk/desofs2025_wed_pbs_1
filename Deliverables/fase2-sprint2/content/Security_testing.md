# Security Testing

This document provides a detailed overview of the security testing practices integrated into the project. These practices aim to ensure the security, integrity, and reliability of the codebase throughout the development lifecycle.

---

## Dynamic Code Analysis (DAST)
MUDAR LUIS
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
