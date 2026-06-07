using Sirenix.OdinInspector; 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

[System.Serializable]
public class TeamManeuver : ScriptableObject {
    [ShowInInspector] public TeamManeuver Reference => this;
    [PropertyOrder(100), ListDrawerSettings(OnBeginListElementGUI = nameof(OnBeginItemGUI), ShowFoldout = false)] 
    public List<CharData> CharsData = new List<CharData>();


    void OnBeginItemGUI(int idx){
#if UNITY_EDITOR
        SirenixEditorGUI.Title("Player: " + idx.ToString(), "",TextAlignment.Left, true, true);
        EditorGUILayout.Space();
#endif
    }

}