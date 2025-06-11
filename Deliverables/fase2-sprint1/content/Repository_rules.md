## Repository Contribution Rules
### Branch Naming Rules

### \<type>/issue-\<issue-ID>

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

### When to Commit

- Commit when you have completed a logical unit of work. For example, if you have added a new feature, fixed a bug, or made a significant change to the codebase.
- Commit when you have written tests for your code or updated existing tests.
- Commit when you have updated documentation or made changes to the project structure.
- Commit when you have made changes that improve the code quality, such as refactoring or cleaning up code.
- Commit when you have made changes that improve performance or security.
- All commits should be atomic, meaning they should represent a single logical change to the codebase. Avoid making multiple unrelated changes in a single commit.
- Commit often, but not too often. Aim for a balance between making small, frequent commits and larger, more substantial commits.
- Commit when you have completed a task or feature, but avoid committing incomplete work. If you need to commit incomplete work, use a temporary branch and make sure to document the state of the work.
- Commit when you have resolved a bug or issue, but avoid committing changes that are not related to the bug or issue.

### Commit Rules

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move button" not "Moves button")
- Limit the message to 72 characters or fewer.
- Reference issues and pull requests liberally after the first line

### Starting text of commit message
### \<issue-ID\>-\<type\>: \<short description\>

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

## Pull Request Rules

### When to Create a Pull Request

- Create a pull request when you have completed a feature or bug fix and are ready to merge it into the main branch.
- Assign the pull request to a reviewer or team member who will review your code that is related to the issue and avoid assigning multiple reviewers unless necessary.
- Include a clear and concise description of the changes you made in the pull request.
- Include any relevant information, such as links to related issues or pull requests, and any specific areas you would like the reviewer to focus on.
- Include any relevant screenshots or examples to help the reviewer understand the changes you made.
- Check for any merge conflicts before creating the pull request and resolve them if necessary.
- Check if the reviewer needs any additional context or information to understand the changes you made.