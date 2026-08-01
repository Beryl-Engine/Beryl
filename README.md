# Beryl Engine
<p align="left">
  <img src="https://raw.githubusercontent.com/Beryl-Engine/Branding/refs/heads/main/logo/banner/banner-full-colored.svg" alt="Beryl Banner" width="400">
</p>

![PreRelease](https://img.shields.io/badge/Version-Pre--Release-orange?logo=git&logoColor=f5f5f5) ![MIT](https://img.shields.io/badge/License-MIT-green?logo=opensourcehardware&logoColor=f5f5f5) [![Commit Activity](https://img.shields.io/github/commit-activity/w/Beryl-Engine/Beryl?label=Commit%20Activity&logo=github)](https://github.com/Beryl-Engine/Beryl)

Free & Open Source Game Engine built for lovers of modern C#.

> [!WARNING]
> Beryl is a Pre-Release engine. It's API will have **breaking changes**.

## About
Beryl is a cross-platform, MIT licensed, 3D-Focused game engine that fully embraces modern C#. It's designed to give developers an engine that they'll enjoy using, no messy abstractions, no interop artifacts, and no licensing concerns.

## Getting Started
Once you have a `.csproj` that depends on all Beryl libraries your game will use, you can start the engine through an entry-point that look a bit like this:
```csharp
static void Main(string[] args)
{
    // Register all Modules our game wants to use
    ModuleManager.RegisterModule<InputModule>();
    ModuleManager.RegisterModule<AssemblyModule>();
    ModuleManager.RegisterModule<PhysicsModule>();
    ModuleManager.RegisterModule<SceneModule>();
    ModuleManager.RegisterModule<AudioModule>();
    ModuleManager.RegisterModule<RenderingModule>();

    Application.OnStart += () =>
    {
        // Setup a scene!
        FlyController flyController = new();
        flyController.Name = "Camera";

        DebugEntity floor = new();
        floor.Name = "Floor";
        floor.Transform.Scale = new Vector3(10, 1, 10);

        DebugEntity cube = new();
        cube.Transform.Position = new Vector3(0, 2, 0);

        ModuleManager.GetModule<SceneModule>()?.CurrentScene.StartAll();
    };

    Application.Initialize();
}
```

## Features
### Modern C#
Beryl fully embraces modern C# and it's features. Extensive use of properties, nullability, attributes, and abstraction means code can be more explicit and less based in guess work.

### Extreme Modularity
All engine subsystems have their lifecycle hooked into the `Module` system. Because of this, subsystems can be arbitrarily disabled, profiled, or singled out for debugging.

```csharp
if (Input.PrimaryHandLeft.IsButtonPressed(VirtualRealityController.Button.ActionUp))
  if (ModuleManager.GetModule<PhysicsModule>()?.World.TryRaycast(Transform.Position, Transform.Forward, 1f, out RaycastResult result) == true)
    EngineConsole.Log(result.Body.Owner?.Name ?? "null");
```

### Entity System
Logic is built around Entities and Components. Components are the reusable building blocks of the engine whilst Entities orchestrate their attached components to make more specific logic occur. For instance, a Button Entity would consist of `MeshRenderer`, `RigidBody`, and `Collider` components and contain logic that fires an output when an interaction ray hits the `Collider`.

### Vulkan Renderer
Rendering is built on a Veldrid-based 'Descriptor Defined' Vulkan-first engine with DirectX12 for compatibility.

### Slang Shaders
```slang
import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    [DefaultFloat3(0, 1, 1)]
    float3 Color;
)


[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Object.ModelMatrix, float4(i.Position, 1));
    float4 view = mul(Camera.ViewMatrix, world);

    o.Position = mul(Camera.ProjectionMatrix, view);

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return float4(Parameters.Color, 1);
}
```

