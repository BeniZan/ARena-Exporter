using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode] 
public class DrillAnimator : MonoBehaviour {
    [SerializeField] ExporterDrillActivator _movePlacer;
    public float AnimationTime = 0;
    public bool IsRunning { get; private set; }
    public float MaxAnimationTime { get; private set; }
    [Button]
    public void RestartAnimation() {
        ToggleAnimation(true);
        AnimationTime = 0f;
    } 
    public void ToggleAnimation(bool isRunning) {
        if (IsRunning == isRunning)
            return;

        IsRunning = isRunning;
        enabled = true;
    } 

    private void OnEnable() { 
        RestartAnimation(); 
        ToggleAnimation(false);
    }
     
    private void Update() {
        if (IsRunning)  
            AnimationTime += Time.deltaTime;

        if (!_movePlacer)
            return;

        var drill = _movePlacer.CurrentActive;
        MaxAnimationTime = CalculateMaxAnimationTime(drill);
        AnimationTime = Mathf.Clamp(AnimationTime, 0f, MaxAnimationTime);

        foreach (var character in _movePlacer.PlacedChars) {
            if (!character || character.Data == null)
                continue;
            ResolveSegment(drill, character.Data, AnimationTime, out var clip, out var localTime);
            character.SetSegment(clip, localTime);
        }
    }

    /// <summary>
    /// Picks the segment a character is in at <paramref name="time"/>: its base
    /// animation until a gate opens, then the clip of the last gate whose nominal
    /// fire time has passed.
    /// </summary>
    static void ResolveSegment(DrillData drill, CharData data, float time,
                               out AnimationClip clip, out float localTime) {
        clip = data.Animation;
        localTime = time + data.AnimationTimeOffset;

        foreach (var trigger in data.AnimationTriggers) {
            if (!trigger.TriggeredClip)
                continue;
            if (!TryGetSegmentStart(drill, trigger, out var start) || start > time)
                continue;
            clip = trigger.TriggeredClip;
            localTime = time - start;
        }
    }

    static bool TryGetSegmentStart(DrillData drill, CharData.CharAnimationTrigger trigger, out float start) {
        start = 0f;
        if (!drill || trigger.TriggerIndex < 0 || trigger.TriggerIndex >= drill.Triggers.Count)
            return false;
        start = drill.Triggers[trigger.TriggerIndex].NominalTime + trigger.DelayAfterTrigger;
        return true;
    }

    static float CalculateMaxAnimationTime(DrillData drill) {
        if (!drill)
            return 0f;

        var max = 0f;
        foreach (var data in drill.CharsData) {
            if (data == null)
                continue;

            var baseLength = data.Animation ? data.Animation.length : 0f;
            max = Mathf.Max(max, baseLength + Mathf.Abs(data.AnimationTimeOffset));

            foreach (var trigger in data.AnimationTriggers) {
                if (!trigger.TriggeredClip || !TryGetSegmentStart(drill, trigger, out var start))
                    continue;
                max = Mathf.Max(max, start + trigger.TriggeredClip.length);
            }
        }
        return max;
    }
} 
