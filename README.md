# Slot Machine Prototype (Unity / C#)

This project is a **slot machine gameplay prototype** built in Unity using C#.
It focuses on **clean architecture, extensibility, and testability**, rather than purely visual polish.

The goal of the project is to demonstrate:

- Separation between domain logic and presentation.
- Scalability of gameplay flow using a state machine.
- Deterministic testing via a cheat system.
- Data-driven configuration using Scriptable Objects.
- Asynchronous control flow using Tasks instead of Unity coroutines.

---

## Architecture Overview

The project is split into clear layers:

- **Domain**
  Core slot machine logic (engine, reels, symbols, pay table, win compilation).
- **Core / Gameplay**
  State machine controlling the game flow (start → spin → end).
- **Presentation**
  Visual reel animation, win line rendering, UI interaction.
- **Configuration / Data**
  Scriptable Objects and static data sources for patterns, and symbols.

This separation allows the slot machine logic to be tested or reused without depending on Unity-specific systems.

---

## State Machine Design

The slot machine flow is implemented using a simple, explicit **state machine**:

- StartState
- SpinState
- EndState

This design was chosen to make the gameplay flow **easy to extend** in the future.
For example, adding new states such as:

- Bonus rounds.
- Free spins.
- Mini-games.
- Gamble.

Can be done without rewriting the core flow logic.

The state machine is intentionally generic and not tied to slot-machine specifics, so it can scale with new features and behaviors.

---

## Cheat System (Debug & Testing)

A **cheat system** was implemented to:

- Validate win compilation logic.
- Force deterministic outcomes.
- Enable reproducible test scenarios.

This is useful both for **manual QA** and **automated testing**.

### Opening the Cheat Panel

To open the cheat panel, press:

Q

Then input cheats using the supported formats:

- C:3 → Force 3 consecutive matches of symbol `C` on any valid pattern
- C:3:0 → Force 3 consecutive matches of symbol `C` on pattern with id `0`

The cheat system internally:

- Validates the symbol.
- Validates that the requested match count produces a valid payout.
- Attempts to build forced reel stops that satisfy the requested win.

This makes it possible to **test specific win scenarios without relying on randomness**.

---

## Win Patterns as Scriptable Objects

Winning line patterns are defined as **Scriptable Objects** (`Pattern`).

This allows:

- Adding or tweaking win patterns directly in the Unity editor.
- Without modifying code.
- Without rebuilding the application.

This design makes the system **data-driven** and easier to extend for designers or non-programmers.

---

## Tasks Instead of Coroutines

Reel animation and win line cycling are implemented using **Task-based async workflows** instead of Unity coroutines.

This was a deliberate choice:

- To demonstrate comfort with modern C# async patterns.
- To avoid relying exclusively on Unity-specific coroutine mechanics.

This approach is closer to how gameplay systems might be implemented in **custom C# engines** or non-Unity runtimes, where async/await is preferred over engine-specific scheduling primitives.

---

## GameManager Lifetime

The GameManager is currently designed to operate **within a single scene**.

If the project were to scale to multiple scenes, the intended design would be:

- A single persistent GameManager instance.
- Living across scene loads (DontDestroyOnLoad).
- Acting as the central gameplay coordinator.

The current implementation reflects a scoped, scene-based setup for simplicity.

---

## XML Documentation

XML documentation comments were added across most core and domain classes to:

- Clarify intent.
- Document assumptions and responsibilities.
- Improve readability for reviewers.

This reflects production-level practices where **maintainability and clarity** are important.

---

## Known Limitations / Tradeoffs

- Visual elements (win lines, payout text) are optimized with simple pooling, but a more complete UI system could further improve performance and flexibility.
- The cheat system is intended for development/debug builds only and would be disabled or removed in a production release.

---

## Summary

This project is not meant to be a fully polished slot game, but rather a **technical prototype** demonstrating:

- Clean separation of concerns,
- Extensible architecture,
- Deterministic testing tools,
- Data-driven configuration,
- Modern C# async patterns.

It is designed to be easy to reason about, extend, and test.
