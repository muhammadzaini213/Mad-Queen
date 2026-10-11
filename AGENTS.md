# AGENTS.md

Unity 6 (6000.3.25f1) 2D side-scroller. URP, Input System, Cinemachine 3. Repo root = Unity project root (`Assets/`, `Packages/`, `ProjectSettings/`).

## Commands

No CLI test/lint. Verification = Unity Editor compile (open project or batchmode via Unity Hub).

Batch build (from `Assets/Editor/BuildScript.cs`, static entry `BuildScript.Build`):

```
Unity -batchmode -quit -projectPath . -executeMethod BuildScript.Build -buildTarget StandaloneLinux64
```

`-buildTarget` required; only `StandaloneWindows64`, `StandaloneLinux64`, `StandaloneOSX`, `WebGL` supported. Output: `build/<Target>/` (gitignored). Build reads enabled scenes from Build Settings, not a hardcoded list.

## Architecture — Slafurry standard

All gameplay code lives under `Assets/_Game/00_Scripts/`, namespaces prefixed `Slafurry.*`:

- `Core/Abstract/` — `Singleton<T>`, `GameSystem<T>`, `Manager`. Base classes. Read before adding systems.
- `Systems/` — persistent managers: `SceneLoader`+`SceneSystem`, `LoadingSystem`, `AudioSystem`, `PauseSystem`, `SaveSystem`, `VFXSystem`.
- `Game/` — scene-bound content (Player, Dialog, Interaction, Trigger).
- `UI/` — HUD and Menu MonoBehaviours.
- `Utils/` — Pooling, GameFeel, UI helpers, Attributes.

### Boot sequence (critical)

1. Build Settings index 0 = `Assets/_Game/04_Scenes/Menu/_Boot.unity`. Never reorder/remove.
2. `_Boot` hosts all `GameSystem` singletons + `BootstrapLoader`.
3. `BootstrapLoader` waits one frame, then for each system in `systemsToWaitFor` (inspector array): `Initialize()` (coroutine) then `PostInitialize()`. Then `SceneSystem.Load(targetSceneName)` (default `MainMenu`).
4. Every `Singleton<T>.Awake()` auto-calls `LoadingSystem.Instance.Register(this)`. `LoadingSystem` then runs `Initialize`/`PostInitialize` on registered objects ordered by `Priority`. New `GameSystem<T>` subclasses get this for free — do not call `Register` manually.
5. Spawns after boot (e.g. Player in a level scene) are handled by `LoadingSystem` late-batch (one frame later). Order guarantees still hold.

### Scene loading

- Never call `SceneManager.LoadSceneAsync` directly. Use `SceneSystem.Load(string)` or `SceneSystem.Load(SceneReference)`.
- `SceneLoader` blocks concurrent loads (`_isLoading`), fires `OnSceneLoadStarted/Progress/Completed`, and supports `OnBeforeSceneLoad` gate (used by pause/loading screens).
- `SceneReference` (`CreateAssetMenu`) is a ScriptableObject wrapping a scene name string. Asset instances: `Assets/_Game/01_Objects/Data/Scenes/`. Prefer references over magic strings.
- Level scenes (`Level_Casttle`, `Level_Forest`, `Level_Village`, `Core_Managers`, tests) exist under `Assets/_Game/04_Scenes/Level/` but are NOT all in Build Settings. If a scene must load in a build, enable it in `EditorBuildSettings.asset` first.

## Folder layout (`Assets/_Game/`)

`00_Scripts` code, `01_Objects` prefabs/data, `02_Art`, `03_Audio`, `04_Scenes` (`Menu/`, `Level/`, `Dev/`), `05_Settings` (URP, Input Actions: `InputSystem_Actions.inputactions`). Third-party: `Assets/_Vendor/`. Editor-only code: `Assets/Editor/`.

## Conventions

- Single assembly (`Assembly-CSharp`). No `.asmdef` files. No tests, no CI workflows — do not invent test commands.
- Input: new Input System only. Actions asset wired in ProjectSettings (`com.unity.input.settings.actions`).
- Comments in existing code are often Indonesian (see `BootstrapLoader`, `Singleton`). Match file language.
- Commit style: Conventional Commits (`feat:`, `fix:`, `chore:`), PR-based flow on GitHub (`muhammadzaini213/Mad-Queen`).

## Git gotchas

- Dual VCS: git + Plastic SCM. `ignore.conf` = Plastic ignore; `.gitignore` = git. Keep both in sync when ignoring paths.
- Gitignore excludes `*.csproj`, `*.sln`, `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `build/`. `Mad-Queen.slnx`, `Packages/manifest.json`, `packages-lock.json` are tracked — commit lockfile changes with manifest changes.
- `.meta` files: 789 tracked. Any new asset/folder needs its `.meta` committed. Never hand-edit GUIDs.
- `.vscode/settings.json` hides `.meta`/`.asset`/`.prefab`/`.unity` in VS Code explorer (files still on disk). Use terminal/Unity for those.
