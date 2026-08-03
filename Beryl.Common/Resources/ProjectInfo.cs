// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources.DefaultProvider;
using System.Reflection;

namespace Beryl.Common.Resources;

/// <summary>
/// Contains information and settings for a Beryl project.
/// </summary>
public class ProjectInfo
{
	/// <summary> The name of the project. </summary>
	public string Name { get; init; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "New Project";
	
	/// <summary> Information and settings for the application Window. </summary>
	public WindowInfo Window { get; init; } = new WindowInfo();
}

public readonly struct WindowInfo
{
	public WindowInfo() {}

	public bool VSync { get; init; }
	public int Width { get; init; } = 1280;
	public int Height { get; init; } = 720;
}


[ResourceLoader]
internal sealed class ProjectInfoLoader : ResourceLoader<ProjectInfo>
{
	/// <inheritdoc/>
	public override ProjectInfo? LoadResource(byte[] data, string? name = null) => Serialization.Serialization.Deserialize<ProjectInfo>(data);
}
