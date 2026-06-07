using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InputGUI : MonoBehaviour {
    [SerializeField] CharComponent _charPrefab;
    [SerializeField] CharSelection _selection;
    [SerializeField] FreeCamera _freeCam;
    [SerializeField] AnimationClip[] _allClips;
    bool _isDropdownOpen;

    private void OnValidate() {
#if UNITY_EDITOR
        var guids = AssetDatabase.FindAssets($"t:{nameof(AnimationClip)}");
        if (_allClips.Length != guids.Length)
            _allClips = new AnimationClip[guids.Length];
        for (int i = 0; i < _allClips.Length; i++)
            if (GUID.TryParse(guids[i], out GUID guid))
            _allClips[i] = AssetDatabase.LoadAssetByGUID<AnimationClip>(guid);
#endif
    }

    private void Awake() {
        _selection.SelectedChar.Sub(OnSelectionChange);
    }

    void OnSelectionChange(CharComponent _) => _isDropdownOpen = false;

    void OnGUI() {
        var isRightClicking = Mouse.current.rightButton.isPressed;
        _freeCam.enabled = isRightClicking;
        if (isRightClicking)
            return;

        if (GUILayout.Button("+ Character")) {
            Instantiate(_charPrefab);
        }

        var selected = _selection.SelectedChar.Value;
        if (selected) {
            if (GUILayout.Button((_isDropdownOpen ? "△" : "▽") + "Animations"))
                _isDropdownOpen = !_isDropdownOpen;
            if (_isDropdownOpen) {
                //DrawDropdown();
            }
        } 
    }

    void DrawDropdown(CharComponent selected) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(84); // Offset to align with the button text

        GUILayout.BeginVertical("box", GUILayout.Width(120));
        var lbl = GUI.skin.label;
        var selectedStyle = new GUIStyle(lbl);
        selectedStyle.normal.textColor = Color.aliceBlue;
        for (int i = 0; i < _allClips.Length; i++) {
            if (GUILayout.Button(_allClips[i].name, lbl)) // Renders as clickable text list
            {
                //selected.SetAnim
                //isDropdownOpen = false; // Close layout after selection
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}