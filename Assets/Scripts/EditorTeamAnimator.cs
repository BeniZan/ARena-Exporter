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
     
    private void Update() {
        if (IsRunning)  
            AnimationTime += Time.deltaTime; 
           
            
        MaxAnimationTime = 0f;
        foreach (var character in _movePlacer.PlacedChars) { 
            MaxAnimationTime = Mathf.Max(MaxAnimationTime, character.Clip == null ? 0f : character.Clip.length);
            if(character && character.Data != null) {
                var anim = character.Data.Animation;
                character.SetAnimationTime(AnimationTime);
            }
        }
        if (AnimationTime >= MaxAnimationTime) {
            AnimationTime = MaxAnimationTime;
            IsRunning = false;
        }
        AnimationTime = Mathf.Min(AnimationTime, MaxAnimationTime); 
    }
}
#endif
