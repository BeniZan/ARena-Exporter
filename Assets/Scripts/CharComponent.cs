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
    [ShowInInspector, SerializeField] CharData _data;
    public CharData Data => _data;
    public Transform Head; 
    [ShowInInspector]
    public AnimationClip Clip { get; private set; }
    public Animator Animator => _anim;

    public void SetData(CharData data) {
        _data = data;
        if(data != null) { 
            var pos = new Vector3(data.FieldStandardPosition.y,0,data.FieldStandardPosition.x);
            var rot = Quaternion.Euler(0, data.yRotation, 0);
            transform.SetLocalPositionAndRotation(pos, rot);
            SetAnim(data.Animation);
        }
    }

    void UpdateData() {
        if(_data == null)
            return;
        _data.FieldStandardPosition = transform.position;
        _data.yRotation = transform.rotation.eulerAngles.y; 
    }

    private void OnEnable() {
        _anim.fireEvents = true;
        _anim.applyRootMotion = true;
        _anim.StartPlayback();
        SetAnim(null);  
    }

    private void OnDisable() {
        _anim.StopPlayback(); 
    }
    float _lastUpdate = 0;
    public void SetAnimationPreviewTime(float time) { 
    }
    private void Update() {
        SetData(_data); 
    }  

    void SetAnim(AnimationClip clip) {
        if (clip == Clip)
            return;
        Clip = clip; 
    }  

}