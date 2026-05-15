# unity-xlua-wechat-minigame-demo

A Unity learning project that combines `xLua`, WeChat Mini Game SDK integration, runtime UI, save data, enemy rotation, drops, and lightweight gameplay systems.

## Stack
- Unity 2022.x
- C# / .NET Framework 4.7.1
- `xLua`
- WeChat Mini Game WebGL transform SDK

## Project Highlights
- `LuaGameEntry` as the gameplay entry point
- `LuaManager` for `LuaEnv` lifecycle
- `LuaGameUI` and `LuaWorldHud` for Canvas UI and world HUD
- Critical hit system
- Skill attack system
- Multi-enemy rotation
- Fixed material drops and inventory summary
- Save/load via `LuaSaveBridge`
- WeChat runtime font loading via `WXFontManager`

## Important Directories
- `Assets/Scripts` - C# gameplay and bridge code
- `Assets/Resources/Lua` - Lua gameplay logic
- `Assets/XLua/Gen` - tracked xLua generated bindings
- `ProjectSettings` - Unity project settings
- `Packages` - Unity package manifest

## Open and Run
1. Open the project in Unity.
2. Open the gameplay scene you use for this sample.
3. Press Play.

## xLua Note
This repository tracks `Assets/XLua/Gen` on purpose.

If you change any of the following, regenerate bindings:
- `ILuaGameApp`
- `ILuaGameState`
- `ILuaGameTexts`
- `LuaSaveBridge`
- `LuaGameSaveData`
- `XLuaGameConfig`

Use Unity menu:
- `XLua > Generate Code`

## WeChat Mini Game Note
This project includes WeChat Mini Game related runtime code and editor integration.

Before packaging, verify:
- WeChat SDK settings are configured
- font fallback URL is valid
- performance analysis macros are enabled only when the corresponding SDK feature is available

## Git Tracking
The repository is intended to keep:
- `Assets`
- `Packages`
- `ProjectSettings`
- `Assets/XLua/Gen`

It ignores Unity generated folders such as:
- `Library`
- `Temp`
- `Obj`
- `Logs`
- `UserSettings`
