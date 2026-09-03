using Sirenix.OdinInspector; 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

[Serializable]
public struct DrillTrigger {
    public string Name;
    [LabelText("Position", SdfIconType.ArrowsMove)]
    public Vector2 FieldStandardPositionXZ;
    /// <summary>
    /// When the local player is expected to reach this trigger. Drives the editor
    /// scrubber, and gives the runtime a timeout so a drill cannot stall if the
    /// player never walks into the trigger.
    /// </summary>
    [Min(0f)] public float NominalTime;

    public Vector3 LocalPosition => CourtSpace.ToLocal(FieldStandardPositionXZ);
}

[System.Serializable]
public class DrillData : ScriptableObject {
    public static readonly Vector2 FieldStandardSize = new Vector2(28f, 15f);
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
    public Vector2 _localPlayerStartPos;
    public Vector3 LocalPlayerStartPosition => CourtSpace.ToLocal(_localPlayerStartPos);
    [ListDrawerSettings(OnBeginListElementGUI = nameof(OnTriggerBeginGUI), ShowFoldout = false)]
    [LabelText(SdfIconType.Compass, Text = "Triggers"), BoxGroup("Player Position"), SerializeField]
    List<DrillTrigger> _triggers = new();

    public IReadOnlyList<DrillTrigger> Triggers => _triggers;

#if UNITY_EDITOR
    public void SetTriggerPosition(int idx, Vector2 fieldStandard) {
        var trigger = _triggers[idx];
        trigger.FieldStandardPositionXZ = fieldStandard;
        _triggers[idx] = trigger;
    }
#endif


    [PropertyOrder(100), ListDrawerSettings(OnBeginListElementGUI = nameof(OnCharDataGUI), ShowFoldout = false)] 
    public List<CharData> CharsData = new List<CharData>();
     

#if UNITY_EDITOR
    [Button(icon: SdfIconType.HexagonHalf), PropertyOrder( -101)] void DuplicateMirror() {
        var mirror = Instantiate(this);
        mirror.name += "_mirror";
        mirror.OriginPoint.x = -mirror.OriginPoint.x;
        mirror.OriginYRotation = -mirror.OriginYRotation;
        // Left-to-right is world X, which is the width component of a field-standard
        // pair, so it is .y that flips here and not .x.
        foreach(var c in mirror.CharsData) {
            c.yRotation = -c.yRotation;
            c.FieldStandardPositionXZ.y = -c.FieldStandardPositionXZ.y;
        }
        mirror._localPlayerStartPos.y = -mirror._localPlayerStartPos.y;
        for (int i = 0; i < mirror._triggers.Count; i++) {
            var trigger = mirror._triggers[i];
            trigger.FieldStandardPositionXZ.y = -trigger.FieldStandardPositionXZ.y;
            mirror._triggers[i] = trigger;
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
        var color = GUI.color;
        GUI.color = Color.white;
        SirenixEditorGUI.Title(TriggerLabel(idx), "", TextAlignment.Left, true, true);
        GUI.color = color;
#endif
    }

    public string TriggerLabel(int idx) {
        if (idx < 0 || idx >= _triggers.Count)
            return "Trigger [" + idx + "]";
        var name = _triggers[idx].Name;
        return string.IsNullOrWhiteSpace(name)
            ? "Trigger [" + idx + "]"
            : "Trigger [" + idx + "] " + name;
    }

    void OnCharDataGUI(int idx){
#if UNITY_EDITOR
        SirenixEditorGUI.Title("Player: " + idx.ToString(), "",TextAlignment.Left, true, true);
        EditorGUILayout.Space();
#endif
    }

}