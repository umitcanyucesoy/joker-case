# Unity C# Copilot Instructions & Architecture Guidelines

## 1. Core Architecture & Initialization
* **Entry Point:** The game is initialized strictly through a `GameBootstrapper`. This class is responsible for setting up services, building controllers, and injecting dependencies.
* **Service Locator:** Use the Service Locator pattern EXCLUSIVELY for shared, foundational systems (e.g., `GridService`, `InputService`, `PoolService`).
* **Controllers & Logic:** Core game mechanics must be handled by Controllers. Do NOT put gameplay logic inside services.
* **Dependency Injection (ISP):** Controllers should communicate via interfaces. Apply the Interface Segregation Principle (ISP). Pass specific interface references to controllers rather than concrete classes.
* **Design Patterns:** Adhere to SOLID principles. Favor the Strategy Pattern for interchangeable behaviors (e.g., grid interactions, match rules, or tile behaviors).
* **Designer-Friendly:** Code must be designer-friendly. Expose necessary variables to the Unity Inspector with clear tooltips and sensible defaults.

## 2. Data & State Management
* **Grid & Map Generation:** Use JSON structures for generating the game map/grid and handling tile modifications.
* **Game Data:** Use `ScriptableObject` for static data, balancing variables, item definitions, and configurations.

## 3. Communication
* **EventBus:** Use an Event-Driven architecture via EventBus, but maintain STRICT control over it.
* **NO Hidden Flows:** Do not overuse events. Keep the event flow explicit, traceable, and limited to necessary cross-domain communication to avoid hidden logic flows.

## 4. Performance & Collections
* **Update Loop:** Strictly avoid frequent callbacks or heavy logic in `Update()`, `FixedUpdate()`, or `LateUpdate()`.
* **Coroutines:** For tasks that do not require continuous frame-by-frame execution, use `Coroutine` instead of `Update`.
* **Big O Complexity:** Always consider Memory and CPU costs.
* **Data Structures:** Choose collections deliberately. Use `Dictionary` for fast lookups (O(1)), `List` for dynamic arrays, and standard `Array` for fixed sizes. Do not default to `List` if a `HashSet` or `Dictionary` is computationally cheaper for the required operation.

## 5. Animation & UI
* **Native Unity Animations:** Use ONLY native Unity frameworks (`Animator`, `Animation`) for animations and tweens.
* **NO External Tweeners:** Do NOT use DOTween or similar third-party tweening libraries under any circumstances.

## 6. Code Style & Tooling
* Write clean, modular, and reusable C# code following standard Rider/ReSharper naming conventions.
* Keep classes focused on a single responsibility.