# Keycard Escape Room Starter

A small Unity 6 starter project for practicing first-person raycast interaction architecture.

The scene is intentionally compact: the player finds a keycard, unlocks a door, presses a button to disable a laser trap, and reaches an exit trigger. The focus is clean object communication, not visual production.

Author: Justin Tolstoy <tolstoyjustin@gmail.com>

## Project

- Unity version: `6000.4.10f1`
- Render pipeline: Universal Render Pipeline
- Input: Unity Input System
- Main scene: `Assets/Scenes/SampleScene.unity`

## Controls

- Move: `WASD` or arrow keys
- Look: mouse
- Interact: `E`
- Unlock mouse: `Esc`
- Restart after escape: `R` or click `Play Again`

## Gameplay Loop

1. Read the hint note.
2. Pick up the keycard.
3. Press the button to disable the laser trap.
4. Use the keycard on the locked door.
5. Walk into the exit zone.
6. Restart from the end screen if needed.

## Architecture

The project keeps interaction logic separated into small scripts:

- `IInteractable.cs` defines the shared interaction contract.
- `PlayerInteractor.cs` raycasts from the player camera and calls `Interact`.
- `Keycard.cs` updates inventory state through `GameManager`.
- `Door.cs` checks game state and opens itself.
- `ButtonSwitch.cs` changes scene state by disabling the trap.
- `Trap.cs` resets the player when active.
- `EscapeZone.cs` validates the win condition.
- `GameManager.cs` stores shared state and handles reset/replay flow.
- `UIManager.cs` owns prompts, messages, inventory text, and end options.

This makes it easy to add another interactable object without changing the player interaction script.

## Editor Tools

The project includes two helper menu actions:

- `Tools/Escape Room/Rebuild Room Layout`
- `Tools/Escape Room/Install Gameplay`

Use these only if you want to regenerate the simple challenge scene. The committed `SampleScene` is already set up.

## Coach Test Notes

How does the player detect interactable objects?

`PlayerInteractor` casts a ray from the player camera each frame. If the ray hits a component implementing `IInteractable`, the UI prompt appears and pressing `E` calls `Interact`.

Why is an `IInteractable` interface useful?

It lets the player talk to any interactable object through one common contract. A keycard, door, button, or note can all be added without rewriting the player script.

Where is `hasKeycard` stored?

Inventory state is stored centrally in `GameManager` as `HasKeycard`.

Does the door know about the player directly?

No. The door checks shared game state through `GameManager`, so it does not need a direct player reference.

Could a new interactable be added quickly?

Yes. Add a component that implements `IInteractable`, return a prompt string, and implement its `Interact` behavior.

## Repository Notes

This repo commits Unity source files only. Generated folders such as `Library`, `Temp`, `Logs`, and `UserSettings` are intentionally ignored.
