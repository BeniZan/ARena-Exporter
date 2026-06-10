using Sirenix.OdinInspector;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

[ExecuteInEditMode]
[SelectionBase]
public class CharComponent : MonoBehaviour {
    [SerializeField] Animator _anim;
    public AnimationClip Clip;
    [ShowInInspector, NonSerialized] CharData _data;
    public CharData Data => _data;
    public Transform Head;

    public void SetData(CharData data) {
        _data = data;
        if(data != null) { 
            var pos = new Vector3(data.FieldStandardPosition.y,0,data.FieldStandardPosition.x);
            var rot = Quaternion.Euler(0, data.yRotation, 0);
            transform.SetPositionAndRotation(pos, rot);
            SetAnim(data.Animation);
        }
    }

    void UpdateData() {
        if(_data == null)
            return;
        _data.FieldStandardPosition = transform.position;
        _data.yRotation = transform.rotation.eulerAngles.y; 
    }

    PlayableGraph _graph;
    AnimationClipPlayable _clipPlayable;
    private void OnEnable() {
        _graph = PlayableGraph.Create("SingleAnimationGraph");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var playableOutput = AnimationPlayableOutput.Create(_graph, "AnimationOutput", _anim);
        _clipPlayable = AnimationClipPlayable.Create(_graph, null);
        playableOutput.SetSourcePlayable(_clipPlayable);
        _graph.Play();
    }

    private void OnDisable() {
        _graph.Destroy();
    } 

    public void SetAnimationTime(float time) {
        _clipPlayable.SetTime(time);
        _graph.Evaluate();
    }

    private void Update() {
        SetData(_data); 
    }  

    void SetAnim(AnimationClip clip) {
        if (clip == Clip)
            return;
        Clip = clip;
        AnimatorClipInfo[] currentClipInfo = _anim.GetCurrentAnimatorClipInfo(0);

        if (currentClipInfo.Length > 0) {  
            _override[originalClip] = clip;
            _anim.Update(0);
        }
    }  

}