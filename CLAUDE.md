# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D game project built with Unity 6000.0.51f1 that integrates with Model Context Protocol (MCP) server functionality. The project uses the Universal Render Pipeline (URP) and includes modern Unity packages like the Input System, UniTask for async operations, and NuGet package management.

## Key Architecture Components

### Unity Project Structure
- **Assets/MyGame/**: Main game source code directory (currently empty - all game scripts should be placed here)
- **Assets/Scenes/**: Contains the main game scene (SampleScene.unity)
- **Assets/Settings/**: Unity project settings and URP configuration
- **Assets/Packages/**: NuGet packages managed by NuGetForUnity
- **ProjectSettings/**: Unity project configuration files

### MCP Integration
The project includes Unity Natural MCP Server (UnityNaturalMCP) integration:
- Server configured on port 56780 (configurable in ProjectSettings/UnityNaturalMCPSetting.asset)
- MCP server log display enabled
- Default MCP tools enabled

### Package Dependencies
Key packages and their purposes:
- **Microsoft.Extensions.AI.Abstractions**: AI service abstractions
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **ModelContextProtocol**: MCP client/server functionality
- **System.Text.Json**: JSON serialization for MCP communication
- **UniTask**: Async/await support for Unity
- **Unity Input System**: Modern input handling

## Development Commands

### Unity Editor
- Open project in Unity Editor using Unity Hub
- Build: File → Build Settings → Build (or Ctrl+Shift+B)
- Play mode: Click Play button or press Ctrl+P

### IDE Integration
- **Visual Studio**: Use all-vibecoding-game-sample.sln
- **JetBrains Rider**: Full support configured
- **Visual Studio Code**: Supported through Unity extensions

### NuGet Package Management
- NuGet packages are managed through NuGetForUnity
- Configuration: Assets/NuGet.config
- Package list: Assets/packages.config
- Use NuGet For Unity window in Unity Editor to manage packages

## Technical Configuration

### Unity Settings
- **Unity Version**: 6000.0.51f1
- **Target Framework**: .NET Framework 4.7.1
- **Language Version**: C# 9.0
- **Render Pipeline**: Universal Render Pipeline (URP)
- **2D Features**: Enabled through Unity 2D feature package

### Input System
- Input actions defined in Assets/InputSystem_Actions.inputactions
- Player actions include: Move, Look, Attack, Interact, Crouch
- Uses Vector2 for movement and look, Button for discrete actions

### MCP Server Configuration
- Default IP: '*' (all interfaces)
- Default Port: 56780
- Log display enabled for debugging
- Default tools enabled

## Code Organization Guidelines

### Script Placement
- All game scripts should be placed in Assets/MyGame/
- Follow Unity namespace conventions
- Use assembly definition files for large projects

### Dependencies
- Use Microsoft.Extensions.DependencyInjection for service registration
- Leverage UniTask for async operations instead of Unity Coroutines
- Use System.Text.Json for JSON operations

### MCP Integration Patterns
- MCP tools should be implemented as services
- Use dependency injection for MCP service registration
- Follow MCP protocol specifications for tool definitions

## Development Notes

### Empty Project State
The project is currently in an initial state with:
- Empty MyGame directory for source code
- Basic Unity scene setup
- MCP server integration ready
- All package dependencies installed

### Next Steps for Development
1. Create main game scripts in Assets/MyGame/
2. Implement MCP tools and services
3. Set up game objects and components in the scene
4. Configure input system actions for gameplay

### Testing
- Use Unity Test Framework for unit tests
- MCP server functionality can be tested through MCP client connections
- Build and test across target platforms as needed