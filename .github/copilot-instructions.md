# GitHub Copilot Coding Agent Instructions

## Project Overview

This is the `gex.cli` project - a command-line interface tool. The project is in its early stages and will be developed following modern software development best practices.

## Repository Structure

- `/` - Root directory containing the main project files
- `README.md` - Project documentation
- `.github/` - GitHub configuration and workflow files

## Coding Standards

### General Guidelines

- Write clean, maintainable, and well-documented code
- Follow language-specific best practices and idioms
- Include meaningful commit messages that describe the changes
- Keep functions and methods focused on a single responsibility
- Use descriptive variable and function names

### Code Style

- Follow the language's standard style guide (e.g., PEP 8 for Python, Effective Go for Go, etc.)
- Use consistent indentation (spaces preferred, typically 2 or 4 spaces depending on language)
- Keep lines at a reasonable length (typically 80-120 characters)
- Add comments for complex logic, but prefer self-documenting code

### Testing

- Write unit tests for new functionality
- Ensure all tests pass before submitting a PR
- Maintain or improve code coverage with new changes
- Test edge cases and error conditions

### Documentation

- Update README.md when adding new features or changing functionality
- Document public APIs and interfaces
- Include usage examples where appropriate
- Keep documentation in sync with code changes

## Pull Request Workflow

### Before Creating a PR

- Ensure all tests pass
- Run linters and formatters
- Update documentation if needed
- Write clear, descriptive commit messages

### PR Requirements

- Provide a clear description of the changes and their purpose
- Reference any related issues (e.g., "Fixes #123")
- Include screenshots or examples for UI changes or new features
- Ensure CI/CD checks pass
- Address review comments promptly

### Code Review

- All PRs require review before merging
- Be responsive to feedback and suggestions
- Make requested changes in a timely manner
- Keep PR scope focused and manageable

## Security Practices

- Never commit secrets, API keys, or credentials
- Validate and sanitize all user inputs
- Follow security best practices for the language/framework used
- Keep dependencies up to date
- Report security vulnerabilities appropriately

## Git Workflow

- Create feature branches from the main branch
- Use descriptive branch names (e.g., `feature/add-command`, `fix/bug-123`)
- Keep commits atomic and focused
- Rebase or merge from main regularly to stay up to date
- Squash commits if necessary before merging

## Issue Hygiene

When creating or working on issues:

- Write clear, concise issue descriptions
- Include acceptance criteria
- Specify expected behavior vs. actual behavior for bugs
- Add relevant labels (bug, enhancement, documentation, etc.)
- Break large tasks into smaller, manageable issues

## Development Environment

- Ensure your development environment is set up correctly
- Use version control for all changes
- Test changes locally before pushing
- Follow the project's setup instructions in README.md

## Additional Notes

- This is a living document - update it as the project evolves
- Suggest improvements to these guidelines via PR
- When in doubt, ask for clarification or guidance
