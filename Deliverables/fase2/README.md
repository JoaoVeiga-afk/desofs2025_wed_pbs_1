# Fase 2

## Branch Naming Rules

### \<type>/issue-<issue-ID>

Types:
- **feature** – New feature development
- **fix** – Fixing a bug
- **hotfix** – Urgent fix on production
- **refactor** – Code refactoring, no functional change
- **test** – Adding or updating tests
- **docs** – Documentation changes
- **sys** – Maintenance or minor changes

Example:
- feature/issue-123
- fix/issue-456
- hotfix/issue-789


## Commit Rules

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move button" not "Moves button")
- Limit the message to 72 characters or fewer.
- Reference issues and pull requests liberally after the first line

### Starting text of commit message
### <issue-ID>-<type>: <short description>

Types:
- **feature** – New feature development
- **fix** – Fixing a bug
- **hotfix** – Urgent fix on production
- **refactor** – Code refactoring, no functional change
- **test** – Adding or updating tests
- **docs** – Documentation changes
- **sys** – Maintenance or minor changes


#### Example commit message
- #1234-feature: Add a new user registration form
- #1234-fix: Fix bug in a user login process
- #1234-hotfix: Fix expired ssl certificate
