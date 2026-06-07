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
    }

    private void OnDisable() {
        if(AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }

    private void Update() {
        if (IsRunning)  
            AnimationTime += Time.deltaTime; 

        if (!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();
          
        AnimationMode.BeginSampling();
        try {
            MaxAnimationTime = 0f;
            foreach (var character in _movePlacer.PlacedChars) {
                MaxAnimationTime = Mathf.Max(MaxAnimationTime, character.Clip.length);
                var go = character.Animator.gameObject;
                var anim = character.Data.Animation;
                if (character.Data != null && character.Data.Animation != null)
                    AnimationMode.SampleAnimationClip(go, anim, AnimationTime);
            }
            if (AnimationTime >= MaxAnimationTime) {
                AnimationTime = MaxAnimationTime;
                IsRunning = false;
            }
            AnimationTime = Mathf.Min(AnimationTime, MaxAnimationTime);
        } catch (System.Exception ex) { } 
        finally {
            AnimationMode.EndSampling();
        }
    }
}
#endif
