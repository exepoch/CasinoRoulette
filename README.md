# 🎰 CasinoRoulette

A modular and testable roulette game prototype developed in Unity using **SOLID principles**, **MVVM pattern**, and **event-driven architecture**.

---

## 🔹 Gameplay & Controls

### 🎮 How to Play

- **Place Bets**:
  - Select a chip value from the **Chip Selector UI**.
  - Click on a **table anchor** (e.g., number, red/black, even/odd) to place a bet.
  - Multiple chips and bet types can be placed before spinning.

- **Spin the Wheel**:
  - Press the **Spin** button to start the roulette animation and resolve the round.

- **Deterministic Outcome**:
  - Use the **Developer Panel** to input a specific number (0–36) for testing.
  - The wheel will stop on the selected number on the next spin.

- **Undo & Clear Bets**:
  - **Undo** removes the last bet.
  - **Clear** removes all bets.

- **Statistics**:
  - The UI shows your **current balance**, **total bet amount**, and **past results**.
  - Wins and losses update your balance accordingly.

---

## 🧠 Architecture & OOP Principles

Built with clean, scalable design focusing on:

- **Encapsulation**  
  Managers (e.g., `BetManager`, `AnchorManager`) handle their own logic/state internally.  
  Chip stacking is encapsulated inside `ChipStack` for separation of UI, logic, and pooling.

- **Abstraction**  
  Systems like chip spawning, wheel control, and UI are abstracted behind interfaces and composed modularly.

- **Inheritance & Polymorphism**  
  Event base classes enable type-safe polymorphism.  
  ViewModels (e.g., `ChipViewModel`, `BalanceViewModel`) respond polymorphically to events.

- **Dependency Management**  
  Cross-system communication flows through a custom **EventBus**, promoting loose coupling.  
  UI ViewModels subscribe to events instead of holding direct logic references.

---

## 📦 SOLID Principles in Practice

| Principle                  | Implementation Highlights                                     |
|----------------------------|---------------------------------------------------------------|
| **Single Responsibility**  | Classes each manage one role: chip stacking, spin resolution, etc. |
| **Open/Closed**            | New features (e.g., deterministic spin) added without changing core code. |
| **Liskov Substitution**    | ViewModels and event handlers are extendable and replaceable.  |
| **Interface Segregation**  | Narrow interfaces for events, factories, and save/load logic.  |
| **Dependency Inversion**   | Core systems rely on abstractions (`IChipFactory`, `ISaveable<T>`, etc). |

---

## 🔄 Deterministic Outcome Feature

- Developer-only debug panel to force the wheel stop number.
- Fully integrated with spin logic.
- Disabled in production builds for fairness.

---

## 🧱 Event-Driven System

- Central **EventBus** facilitates one-to-many messaging between gameplay, UI, and animations.
- Enables decoupled design and easier unit testing.

---

## 📂 Persistence & Statistics

- Implements a type-safe save/load system via `ISaveable<T>`.
- Saves balance, past results, and wheel state across sessions.
- Game mode (American/European) selection is **not persisted** by design.

---

## 🪙 MVVM Pattern

- **Model:** Game state, chip selection, spin results.
- **ViewModel:** Classes like `ChipSelectorViewModel`, `BalanceViewModel` update views based on events.
- **View:** Unity UI components bound to ViewModels.

Benefits: clean separation, easy testing, instant UI updates.

---

## 🎯 Known Issues & Future Improvements

### Known Issues

- American mode treats `0` and `00` bets the same due to wheel model limitations.
- Game mode selection is not persisted between sessions.
- Visual feedback on bet placement needs stronger animations.
- Chip stacking transitions feel abrupt.
- Minimal visual polish and no particle effects.

### Planned Features

- Chip fly animations from selector to bets.
- Mobile touch/tap support.
- Stats visualization with charts.
- Multiple visual themes.
- Settings menu with audio/debug toggles.
- Multiplayer support.
- In-app purchases and player progression systems.
- Persistent game mode setting.
- Improved visual effects and polish.

---

## 📹 Demo Video

[Watch the demo on YouTube](https://youtu.be/6APdph56X-k)

---

## 🧼 Git & Version Control

- Repository uses a single `main` branch.
- Commit messages follow descriptive patterns:

  ```
  Sound Clips Added
  - Added new sound clips
  - Setup audio manager
  ```

- Future work may use `feature/` and `fix/` branches.

---

## 📝 Setup Instructions

1. Clone the repo:

   ```bash
   git clone https://github.com/exepoch/CasinoRoulette.git
   ```

2. Open with **Unity 2022.3+**

3. Open scene at:

   ```
   Assets/Scenes/SelectionScene.unity
   ```

4. Press **Play**

---

## 📧 Contact

For questions or feedback:

- 📬 Email: mstfkbgl53@gmail.com  
- 🔗 LinkedIn: [www.linkedin.com/in/mstfkbgl](https://www.linkedin.com/in/mstfkbgl)
