# Agent Development Guide (`AGENT.md`)

This document defines the rules, conventions, and workflow for AI agents assisting with the development of the **SolarForecast** (`solarforcast`) project.

---

## 1. Language Conventions

- **Documentation & Code Language:** **English** (mandatory).
  - All documentation files, requirements, implementation plans, architecture notes, code comments, identifiers, and commit messages **must** be written in English.
- **Conversation Language:** Agents may respond to the user in the language used in the user's prompt (e.g., Hungarian), while strictly keeping all repository files and documentation in English.

---

## 2. Documentation Structure

All project documentation resides under the `docs/` directory:

```text
docs/
├── 01-requirement/
│   └── requirement.md       # Single source of truth for functional & non-functional requirements
└── 02-implementation/       # Implementation plans, technical designs, and step-by-step task breakdowns
```

### `docs/01-requirement/requirement.md`
- Contains the collected project requirements, user stories, domain rules, and constraints.
- Before starting any feature design or implementation, **always read** `docs/01-requirement/requirement.md` to ensure full alignment with project goals.
- When the user introduces new requirements or modifies existing ones, update `docs/01-requirement/requirement.md` accordingly.

### `docs/02-implementation/`
- Contains detailed implementation plans (e.g., `01-<feature-name>.md`, architecture plans, or phase breakdowns).
- Every non-trivial feature or architectural change must have a corresponding implementation plan in `docs/02-implementation/` before or during implementation.
- Implementation plans should include:
  - **Context & Goal:** Reference to the relevant section(s) in `docs/01-requirement/requirement.md`.
  - **Technical Design:** Proposed architecture, data models, components, and interfaces.
  - **Task Breakdown:** Actionable, step-by-step checklist of implementation tasks.
  - **Verification Plan:** How the changes will be tested and validated.

---

## 3. Agent Development Workflow

1. **Understand Requirements**
   - Check `docs/01-requirement/requirement.md` for existing requirements.
   - Clarify any ambiguities with the user before making assumptions, and record agreed-upon requirements in `docs/01-requirement/requirement.md`.

2. **Plan Implementation**
   - Review existing plans in `docs/02-implementation/`.
   - Create or update an implementation plan in `docs/02-implementation/` detailing the technical approach and tasks.

3. **Implement & Verify**
   - Follow clean code principles, modular architecture, and the established tech stack conventions of the repository.
   - Write unit/integration tests alongside new functionality.
   - Run builds and tests to verify that changes work as expected and introduce no regressions.

4. **Keep Documentation in Sync**
   - Update task status in the relevant `docs/02-implementation/` plan as work progresses.
   - If implementation reveals new constraints or requirement refinements, reflect them in `docs/01-requirement/requirement.md`.
