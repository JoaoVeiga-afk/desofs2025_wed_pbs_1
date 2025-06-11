# Fase 2 - Sprint 2 Deliverables


This phase focuses on defining and enforcing development standards across the repository.  
To ensure consistency, collaboration, and code quality, we have structured our guidelines and automation into two main areas:

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
