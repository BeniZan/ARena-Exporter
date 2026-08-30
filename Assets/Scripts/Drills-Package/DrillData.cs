using Sirenix.OdinInspector; 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Sirenix.OdinInspector.Editor;


#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

[System.Serializable]
public class DrillData : ScriptableObject {
    public enum Category { 
        PickAndRoll,
        Shooting,
        PostPlays
    }
    [BoxGroup("Origin")]
    public Category DrillCategory;
    [BoxGroup("Origin")]
    public Vector3 OriginPoint;
    [BoxGroup("Origin"), PropertyRange(0,360)]
    public float OriginYRotation;
    [BoxGroup("Origin")]
    public bool MirrorLeftRight;

    [BoxGroup("Player Position")]
    public Vector2 _localPlayerStartPosXZ;
    public Vector3 LocalPlayerStartPosition => _localPlayerStartPosXZ.XZToXYZ();
    [ListDrawerSettings(OnBeginListElementGUI = nameof(OnTriggerBeginGUI), 
                        OnEndListElementGUI = nameof(OnTriggerEndGUI) ,ShowFoldout = false)]
    [LabelText(SdfIconType.Compass, Text = "Triggers"), BoxGroup("Player Position"), SerializeField]
    List<Vector2> _triggersXZ = new();

    public IReadOnlyList<Vector2> TriggersXZ => _triggersXZ;


    [PropertyOrder(100), ListDrawerSettings(OnBeginListElementGUI = nameof(OnCharDataGUI), ShowFoldout = false)] 
    public List<CharData> CharsData = new List<CharData>();
     

#if UNITY_EDITOR
    [Button(icon: SdfIconType.HexagonHalf), PropertyOrder( -101)] void DuplicateMirror() {
        var mirror = Instantiate(this);
        mirror.name += "_mirror";
        mirror.OriginPoint.x = -mirror.OriginPoint.x;
        mirror.OriginYRotation = -mirror.OriginYRotation;
        foreach(var c in mirror.CharsData) {
            c.yRotation = -c.yRotation;
            c.FieldStandardPosition.x = -c.FieldStandardPosition.x;
        }

        var path = AssetDatabase.GetAssetPath(this);
        var dir = Path.GetDirectoryName(path);
        var extention = Path.GetExtension(path);  
        var newPath = Path.Combine(dir, mirror.name + extention);
        AssetDatabase.CreateAsset(mirror, newPath); 
    }
#endif

    void OnTriggerBeginGUI(int idx) {
#if UNITY_EDITOR
        EditorGUILayout.BeginHorizontal();
        var rect = EditorGUILayout.GetControlRect(GUILayout.MaxWidth(20));
        SdfIcons.DrawIcon(rect,SdfIconType.Compass);
        EditorGUILayout.LabelField("[" + idx.ToString() + "]", GUILayout.MaxWidth(20));
#endif
    }
    void OnTriggerEndGUI(int idx) {
#if UNITY_EDITOR 
        EditorGUILayout.EndHorizontal();
#endif
    }

    void OnCharDataGUI(int idx){
#if UNITY_EDITOR
        SirenixEditorGUI.Title("Player: " + idx.ToString(), "",TextAlignment.Left, true, true);
        EditorGUILayout.Space();
#endif
    }

}