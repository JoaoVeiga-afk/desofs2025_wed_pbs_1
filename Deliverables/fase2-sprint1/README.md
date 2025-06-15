# Fase 2


This phase focuses on defining and enforcing development standards across the repository.  
To ensure consistency, collaboration, and code quality, we have structured our guidelines and automation into two main areas:

---

## 📄 Repository Contribution Rules

This section outlines the standards all contributors must follow when working in this repository:

- **Branch Naming** – Structured names based on feature type and issue ID
- **Commit Messages** – Clear, consistent messages with type and reference
- **Pull Requests** – Guidelines for creating and reviewing PRs

[View repository rules](content/Repository_rules.md)

---

## ⚙️ CI/CD Pipelines
We use GitHub Actions to automate our development lifecycle, ensuring each change is properly verified.

This section documents:
- **Linting and formatting steps**
- **Build process**
- **SonarQube analysis**
- **GitHub Leaks integration for secret detection**
- **Unit test execution**
- **Workflow orchestration**

[View CI/CD pipelines](content/Pipelines_description.md)

---

## 🔐 Security Testing

We have implemented several security-focused practices as part of our CI/CD pipeline and development process:

- **Static analysis (SAST)** with **SonarQube (SonarCloud)**
- **Dependency vulnerability scanning (SCA)** using **Dependabot**
- **Code scanning alerts** enabled via **GitHub Advanced Security**
- **Unit testing** to validate core application logic

[View detailed security testing](content/Security_testing.md)  


## 🔍 Features Overview

This section provides an overview of the key features implemented in the application, including user authentication, database management, and security measures.
[View features overview](content/Features.md)

## Backup Policy

We establish a backup policy to ensure data integrity and availability. This consists of regular backups of database and application data, with clear retention and restoration procedures.
[View backup policy](content/Backup_Policy.md)

