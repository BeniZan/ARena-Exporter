using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class EditorTeamAnimator : MonoBehaviour {
    [SerializeField] TeamManeuverPlacer _movePlacer;
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
        if(!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();
    }
    private void OnDisable() {
        if(AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }
    private void Update() {
        UpdateCurrentAnimTime();
        UpdatePreviewAnimation();
    }

    void UpdateCurrentAnimTime() {
        if(IsRunning)
            AnimationTime += Time.deltaTime;
        foreach (var character in _movePlacer.PlacedChars) {
            MaxAnimationTime = Mathf.Max(MaxAnimationTime, character.Clip == null ? 0f : character.Clip.length); 
        }
        if (AnimationTime >= MaxAnimationTime) {
            AnimationTime = MaxAnimationTime;
            IsRunning = false;
        }
        AnimationTime = Mathf.Min(AnimationTime, MaxAnimationTime);
    }

    void UpdatePreviewAnimation() {
        var wasInAnimMode = AnimationMode.InAnimationMode();
        if (!wasInAnimMode)
            AnimationMode.StartAnimationMode();

        foreach (var character in _movePlacer.PlacedChars) {
            if (character && character.Data != null) {
                var anim = character.Data.Animation;
                AnimationMode.SampleAnimationClip(character.Animator.gameObject, character.Clip, AnimationTime);
            }
        } 
         
    }

}
#endif
