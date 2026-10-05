#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class StepAnimationEventsTool
{
    private const string AddLeftRightMenu = "Assets/Step Events/Add Left → Right";
    private const string AddRightLeftMenu = "Assets/Step Events/Add Right → Left";
    private const string RemoveMenu = "Assets/Step Events/Remove";

    [MenuItem(AddLeftRightMenu)]
    private static void AddLeftRight()
    {
        AddStepEvents(Foot.Left, Foot.Right);
    }

    [MenuItem(AddLeftRightMenu, true)]
    private static bool ValidateAddLeftRight()
    {
        return HasAnimationClipSelected();
    }

    [MenuItem(AddRightLeftMenu)]
    private static void AddRightLeft()
    {
        AddStepEvents(Foot.Right, Foot.Left);
    }

    [MenuItem(AddRightLeftMenu, true)]
    private static bool ValidateAddRightLeft()
    {
        return HasAnimationClipSelected();
    }

    [MenuItem(RemoveMenu)]
    private static void Remove()
    {
        foreach (var clip in SelectedClips())
        {
            var events = AnimationUtility.GetAnimationEvents(clip)
                .Where(e => e.functionName != nameof(AnimationStepEventSource.OnStep))
                .ToList();

            Undo.RecordObject(clip, "Remove Step Events");
            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
            EditorUtility.SetDirty(clip);
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem(RemoveMenu, true)]
    private static bool ValidateRemove()
    {
        return HasAnimationClipSelected();
    }

    private static void AddStepEvents(Foot firstFoot, Foot secondFoot)
    {
        foreach (var clip in SelectedClips())
        {
            var cycle = Mathf.Max(0f, clip.length - 1f / clip.frameRate);
            var halfTime = Mathf.Round(cycle * 0.5f * clip.frameRate) / clip.frameRate;

            var events = AnimationUtility.GetAnimationEvents(clip)
                .Where(e => e.functionName != nameof(AnimationStepEventSource.OnStep))
                .ToList();

            events.Add(CreateStepEvent(0f, firstFoot));
            events.Add(CreateStepEvent(halfTime, secondFoot));
            events.Sort((a, b) => a.time.CompareTo(b.time));

            Undo.RecordObject(clip, "Add Step Events");
            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
            EditorUtility.SetDirty(clip);
        }

        AssetDatabase.SaveAssets();
    }

    private static AnimationEvent CreateStepEvent(float time, Foot foot)
    {
        return new AnimationEvent
        {
            time = time,
            functionName = nameof(AnimationStepEventSource.OnStep),
            intParameter = (int)foot
        };
    }

    private static bool HasAnimationClipSelected()
    {
        return Selection.objects.OfType<AnimationClip>().Any();
    }

    private static IEnumerable<AnimationClip> SelectedClips()
    {
        return Selection.objects.OfType<AnimationClip>();
    }
}
#endif
