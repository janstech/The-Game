# The Game – C# Scripts

This folder contains the gameplay scripts used in **The Game**, a Unity 3D action project.  
All scripts are written in C# for Unity (Unity 2023, new Input System).

For an overview of the whole project, see the main [README](../README.md).

---

## Player & Camera

- **CharacterControls.cs**  
  First-person player controller. Handles movement, jumping, sprinting, gravity and input.

- **CameraManager.cs**  
  Manages active camera(s) in the scene.

- **FollowCamera.cs**  
  Keeps the camera following the player/target with a smooth offset.

- **WeaponController.cs**  
  Controls the player’s weapon – firing logic, projectiles / raycasts and hit detection.

- **PlayerHealth.cs**  
  Tracks player health, damage, death events and notifies other systems (UI, GameManager).

---

## Enemies, AI & Spawning

- **Enemy.cs**  
  Base logic for an enemy: movement, taking damage and simple behaviour.

- **Monster.cs**  
  Specific monster enemy implementation (animations / stats / behaviour tuning).

- **EnemySpawnManager.cs**  
  Spawns enemies into the level according to the game rules.

- **MonsterSpawnManager.cs**  
  Spawner logic for the monster-type enemies.

- **OrbSpawner.cs**  
  Spawns collectible orbs into the level.

- **EnergyOrb.cs**  
  Logic for an orb pickup – increases the orb counter and plays effects.

---

## Game Flow & Systems

- **GameManager.cs**  
  Central game state controller. Handles orb goals, win/lose conditions, restart and scene reset.

- **SettingsManager.cs**  
  Stores and applies game settings (volume, difficulty, etc.).

- **PowerUp.cs**  
  Base component for a power-up object (what happens when the player collects it).

- **PowerUpManager.cs**  
  Manages active power-ups and their duration/effects.

- **PowerUpSelfDestruct.cs**  
  Destroys a power-up object after it has been used or after a time limit.

---

## UI, Menus & Feedback

- **StartMenuManager.cs**  
  Controls the main menu UI and starting the game.

- **VictoryUI.cs**  
  Handles the victory screen when the player completes the orb objective.

- **Escape_Quit.cs**  
  Simple quit / escape menu behaviour.

- **DamageFlash.cs**  
  Screen flash / feedback when the player takes damage.

---

## Navigation & Effects

- **NavMesh / movement helpers**  
  Several scripts (e.g. Enemy movement) rely on Unity’s **NavMesh** for pathfinding.

- **VFX scripts**  
  Scripts such as `EnergyOrb.cs`, `DamageFlash.cs` and the power-up scripts trigger particle and visual effects when events happen (pickups, hits, death, etc.).

---

## Notes

- Scripts are designed to be read as examples of **gameplay code**, not as a reusable framework.
- The names used in the Unity project and in this folder match 1:1, so you can easily cross-reference scenes and prefabs with the source code.

