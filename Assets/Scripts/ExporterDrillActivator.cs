#if DRILL_EXPORT_EDITOR
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static CharData;
[ExecuteInEditMode] 
public class ExporterDrillActivator : SingletonBehaviors.SingletonMono<ExporterDrillActivator> {
    public DrillData CurrentActive { get; private set; }
    [SerializeField] CharComponent _template;
    [SerializeField] Transform _courtTf;
    [field: SerializeField, Get] public DrillAnimator Animator { get; private set; } 
    [SerializeField] List<CharComponent> _placedChars = new List<CharComponent>(); 
    public IReadOnlyList<CharComponent> PlacedChars => _placedChars;
    private void OnEnable() {
        Activate(CurrentActive);
    }
    public void Activate(DrillData move) {
        if (CurrentActive)
            Deactivate();
        if (move) 
            CurrentActive = move;
        UpdateChars();
        RandomizeAllSkins();
#if UNITY_EDITOR
        SceneView.duringSceneGui -= SceneView_duringSceneGui;
        SceneView.duringSceneGui += SceneView_duringSceneGui;
#endif
    }

    public void UpdateChars() {
        if (!CurrentActive)
            return;
        CharAnimationTrigger.MAX_TRIGGER_IDX = Mathf.Max(0, CurrentActive.Triggers.Count - 1);
        var originPos = CurrentActive.OriginPoint;
        var originRot = Quaternion.Euler(0f, CurrentActive.OriginYRotation, 0f);
        if (CurrentActive.MirrorLeftRight)
            originRot *= Quaternion.Euler(0f, 180f, 0f);
        _courtTf.SetLocalPositionAndRotation(originPos, originRot);

        int i = 0;
        for (; i < CurrentActive.CharsData.Count; i++) {
            if (_placedChars.Count <= i) {
                var spawned = Instantiate(_template, transform);
                spawned.gameObject.SetActive(true);
                _placedChars.Add(spawned);
            }
            _placedChars[i].SetData(CurrentActive.CharsData[i], CurrentActive.MirrorLeftRight);
        }
        while(i < _placedChars.Count) {
            if (_placedChars[i])
                _placedChars[i].gameObject.SafeDestroy();
            _placedChars.RemoveAt(i);
        }
    }

    void RandomizeAllSkins() {
        foreach(var charComp in _placedChars) {
            charComp.RandomizeSkin();
        }
    }

#if UNITY_EDITOR 
    private void Update() {
        UpdateChars(); 
    }
#endif
    public void Deactivate() {
        foreach (var placedChar in _placedChars)
            if(placedChar)
                placedChar.gameObject.SafeDestroy();
        _placedChars.Clear();
        CurrentActive = null;
#if UNITY_EDITOR 
        SceneView.duringSceneGui -= SceneView_duringSceneGui;
#endif
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
        GizmosU.GizmosArrow(transform.position, transform.rotation.EulerSeperateY() * Vector3.forward);
    } 
     

#if UNITY_EDITOR  
    protected override void OnDestroy() {
        base.OnDestroy();
        SceneView.duringSceneGui -= SceneView_duringSceneGui;
    }
    GUIStyle _style;
    private void SceneView_duringSceneGui(SceneView obj) {
        if (!CurrentActive)
            return;

        _style ??= new GUIStyle(SirenixGUIStyles.WhiteLabel) {
                richText = true,
                fontSize = 35,
                alignment = TextAnchor.MiddleCenter
            };
        var color = Color.softGreen;
        DrawTrigger(CurrentActive.LocalPlayerStartPosition, color, "Start Position");
        if (TryDragToField(CurrentActive.LocalPlayerStartPosition, out var startField)) {
            Undo.RecordObject(CurrentActive, "Move Player Start");
            CurrentActive._localPlayerStartPos = startField;
            EditorUtility.SetDirty(CurrentActive);
        }

        var triggers = CurrentActive.Triggers;
        for(int i=0; i < triggers.Count; i++) { 
            Color.RGBToHSV(color, out var h, out var s, out var v);
            h += (i+1) * 0.1f;
            h = Mathf.Repeat(h, 1f);
            color = Color.HSVToRGB(h, s, v);
            var localPos = triggers[i].LocalPosition;
            DrawTrigger(localPos, color, $"<u>{i}</u>");
            if (TryDragToField(localPos, out var triggerField)) {
                Undo.RecordObject(CurrentActive, "Move Trigger");
                CurrentActive.SetTriggerPosition(i, triggerField);
                EditorUtility.SetDirty(CurrentActive);
            }
        } 

        Handles.BeginGUI();  
        for (int i = 0; i < _placedChars.Count; i++) {
            var head = _placedChars[i].ActiveSkin.HeadTf;
            var pos = head.position + new Vector3(0, 0.1f, 0);
            var lbl = $"<u>{i}</u>";
            Handles.Label(pos, lbl, _style);
        }
        Handles.EndGUI();
    } 

    void DrawTrigger(Vector3 localPosition, Color color, string label) {
        var trigPos = _courtTf.TransformPoint(localPosition);
        Handles.color = color;
        Handles.DrawWireDisc(trigPos, Vector3.up, 0.6f);
        Handles.DrawSolidDisc(trigPos, Vector3.up, 0.45f);
        Handles.Label(trigPos, label, _style);
    }

    /// <summary>
    /// Draws a move handle at a court-local point and reports the dragged result back
    /// in field-standard coordinates. Height is dropped, so the handle stays on the floor.
    /// </summary>
    bool TryDragToField(Vector3 localPosition, out Vector2 fieldStandard) {
        fieldStandard = default;
        var worldPos = _courtTf.TransformPoint(localPosition);
        EditorGUI.BeginChangeCheck();
        var moved = Handles.PositionHandle(worldPos, _courtTf.rotation);
        if (!EditorGUI.EndChangeCheck())
            return false;

        fieldStandard = CourtSpace.ToField(_courtTf.InverseTransformPoint(moved));
        return true;
    }

#endif
}
#endif