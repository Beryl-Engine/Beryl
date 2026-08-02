// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)


using Beryl.Common.Resources.DefaultProvider;
using System.Reflection;
using System.Text;

namespace Beryl.Common.Resources;

/// <summary>
/// Contains information and settings for a Beryl project.
/// </summary>
public class ProjectInfo
{
	/// <summary> The name of the project. </summary>
	public string Name { get; init; } = Assembly.GetEntryAssembly()?.GetName().Name ?? "New Project";
	
	/// <summary> Information and settings for rendering/graphics. </summary>
	public GraphicsInfo Graphics { get; init; } = default;
}

public readonly struct GraphicsInfo
{
	public bool VSync { get; init; }
}


[ResourceLoader]
internal sealed class ProjectInfoLoader : ResourceLoader<ProjectInfo>
{
	/// <inheritdoc/>
	public override ProjectInfo? LoadResource(byte[] data, string? name = null)
	{
		string yaml = Encoding.UTF8.GetString(data);
		return null;
	}
}
