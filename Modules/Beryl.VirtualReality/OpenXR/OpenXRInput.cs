// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.VirtualReality.OpenXR.Bindings;

using Silk.NET.Core.Native;
using Silk.NET.OpenXR;

using Action = Silk.NET.OpenXR.Action;

namespace Beryl.VirtualReality.OpenXR;

// TODO: Rewrite this class

/// <summary>
/// Low-level bindings for OpenXR input.
/// </summary>
internal static class OpenXRInput
{
	public enum InputPath
	{
		TriggerValue = 0,
		GripValue,
		ActionDown,   // A / X
		ActionUp, // B / Y
		System,     // menu / system button
		Count
	}

	private static readonly List<string>[] PathPriorities =
	{
		new() { "/input/trigger/value", "/input/select/value" },  // TriggerValue
        new() { "/input/squeeze/value", "/input/squeeze/click" }, // GripValue
        new() { "/input/a/click", "/input/x/click" },              // ActionUp
        new() { "/input/b/click", "/input/y/click" },              // ActionDown
        new() { "/input/menu/click", "/input/system/click" },      // System
    };

	private static readonly string[] InteractionProfiles =
	{
		"/interaction_profiles/khr/simple_controller",
		"/interaction_profiles/htc/vive_controller",
		"/interaction_profiles/microsoft/motion_controller",
		"/interaction_profiles/hp/mixed_reality_controller",
		"/interaction_profiles/oculus/touch_controller",
		"/interaction_profiles/valve/index_controller",
		"/interaction_profiles/htc/vive_cosmos_controller",
	};

	private static ActionType GetActionType(InputPath path) => path switch
	{
		InputPath.TriggerValue => ActionType.FloatInput,
		InputPath.GripValue => ActionType.FloatInput,
		_ => ActionType.BooleanInput,
	};

	private static readonly Action[,] Actions = new Action[2, (int)InputPath.Count];
	private static ActionSet actionSet;
	private static bool initialized;

	/// <summary> Creates the action set and actions, and suggests bindings across common interaction profiles. Safe to call multiple times — only runs once. Must be called before <see cref="Attach"/>. </summary>
	public static unsafe void Initialize(XR xr, OpenXRInstance instance)
	{
		if (initialized)
			return;

		initialized = true;

		ActionSetCreateInfo setInfo = new()
		{
			Type = StructureType.ActionSetCreateInfo,
			Priority = 0,
		};

		SilkMarshal.StringIntoSpan("gameplay\0", new Span<byte>(setInfo.ActionSetName, 32));
		SilkMarshal.StringIntoSpan("Gameplay\0", new Span<byte>(setInfo.LocalizedActionSetName, 32));

		ActionSet set = default;
		xr.CreateActionSet(instance.Native, &setInfo, &set);
		actionSet = set;

		for (int path = 0; path < (int)InputPath.Count; path++)
		{
			for (int hand = 0; hand < 2; hand++)
			{
				ActionCreateInfo actionInfo = new()
				{
					Type = StructureType.ActionCreateInfo,
					ActionType = GetActionType((InputPath)path),
				};

				string name = $"{(InputPath)path}_h{hand}".ToLowerInvariant() + '\0';
				string localized = $"{(InputPath)path} ({(hand == 0 ? "Left" : "Right")})\0";
				SilkMarshal.StringIntoSpan(name, new Span<byte>(actionInfo.ActionName, 32));
				SilkMarshal.StringIntoSpan(localized, new Span<byte>(actionInfo.LocalizedActionName, 32));

				fixed (Action* actionPtr = &Actions[hand, path])
					xr.CreateAction(actionSet, &actionInfo, actionPtr);
			}
		}

		SuggestBindings(xr, instance);
	}

	private static unsafe void SuggestBindings(XR xr, OpenXRInstance instance)
	{
		foreach (string profilePath in InteractionProfiles)
		{
			ulong profile = 0;
			xr.StringToPath(instance.Native, profilePath, ref profile);

			var bindings = new List<ActionSuggestedBinding>();

			for (int hand = 0; hand < 2; hand++)
			{
				string handRoot = hand == 0 ? "/user/hand/left" : "/user/hand/right";

				for (int path = 0; path < (int)InputPath.Count; path++)
				{
					foreach (string suffix in PathPriorities[path])
					{
						ulong bindingPath = 0;
						xr.StringToPath(instance.Native, handRoot + suffix, ref bindingPath);

						var candidate = new ActionSuggestedBinding
						{
							Action = Actions[hand, path],
							Binding = bindingPath,
						};

						InteractionProfileSuggestedBinding probe = new()
						{
							Type = StructureType.InteractionProfileSuggestedBinding,
							InteractionProfile = profile,
							CountSuggestedBindings = 1,
							SuggestedBindings = &candidate,
						};

						if (xr.SuggestInteractionProfileBinding(instance.Native, &probe) == Result.Success)
						{
							bindings.Add(candidate);
							break;
						}
					}
				}
			}

			if (bindings.Count == 0)
				continue;

			var final = bindings.ToArray();
			fixed (ActionSuggestedBinding* finalPtr = &final[0])
			{
				InteractionProfileSuggestedBinding suggested = new()
				{
					Type = StructureType.InteractionProfileSuggestedBinding,
					InteractionProfile = profile,
					CountSuggestedBindings = (uint)final.Length,
					SuggestedBindings = finalPtr,
				};

				xr.SuggestInteractionProfileBinding(instance.Native, &suggested);
			}
		}
	}

	/// <summary> Attaches the shared action set to the session. Call once per session, after <see cref="Initialize"/> and before the first <see cref="Sync"/> call. </summary>
	public static unsafe void Attach(XR xr, OpenXRSession session)
	{
		ActionSet set = actionSet;
		SessionActionSetsAttachInfo attachInfo = new()
		{
			Type = StructureType.SessionActionSetsAttachInfo,
			CountActionSets = 1,
			ActionSets = &set,
		};

		xr.AttachSessionActionSets(session.Native, &attachInfo);
	}

	/// <summary> Syncs all actions. Call once per frame, before reading any controller state. </summary>
	public static unsafe void Sync(XR xr, OpenXRSession session)
	{
		ActiveActionSet active = new()
		{
			ActionSet = actionSet,
			SubactionPath = 0, // all subaction paths
		};

		ActionsSyncInfo syncInfo = new()
		{
			Type = StructureType.ActionsSyncInfo,
			CountActiveActionSets = 1,
			ActiveActionSets = &active,
		};

		xr.SyncAction(session.Native, in syncInfo);
	}

	public static bool GetBool(XR xr, OpenXRSession session, int hand, InputPath path)
	{
		ActionStateGetInfo getInfo = new()
		{
			Type = StructureType.ActionStateGetInfo,
			Action = Actions[hand, (int)path],
		};
		ActionStateBoolean state = new() { Type = StructureType.ActionStateBoolean };
		xr.GetActionStateBoolean(session.Native, in getInfo, ref state);
		return state.IsActive != 0 && state.CurrentState == 1;
	}

	public static float GetFloat(XR xr, OpenXRSession session, int hand, InputPath path)
	{
		ActionStateGetInfo getInfo = new()
		{
			Type = StructureType.ActionStateGetInfo,
			Action = Actions[hand, (int)path],
		};
		ActionStateFloat state = new() { Type = StructureType.ActionStateFloat };
		xr.GetActionStateFloat(session.Native, in getInfo, ref state);
		return state.IsActive != 0 ? state.CurrentState : 0f;
	}
}
