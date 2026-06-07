using Sirenix.OdinInspector;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
  
[ExecuteInEditMode]
[SelectionBase]
public class CharComponent : MonoBehaviour {
    [SerializeField] Animator _anim;
    [SerializeField] EPOOutline.Outlinable _outline;
    public AnimationClip Clip;
    public Animator Animator => _anim;
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
     
    private void Update() {
        SetData(_data);

    }  
    void SetAnim(AnimationClip clip) {
        if (clip == Clip)
            return;
        Clip = clip; 
        var runtimeController = _anim.runtimeAnimatorController;
        runtimeController.animationClips[0] = clip;
        _anim.runtimeAnimatorController = runtimeController; 
    } 

    internal void OnIsSelected(bool isSelected) {
        _outline.enabled = isSelected;
    }

}