using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TeamManeuverPlacer : SingletonBehaviors.SingletonMono<TeamManeuverPlacer> {
    public TeamManeuver CurrentActive { get; private set; }
    [SerializeField] CharComponent _template;
    [SerializeField] Transform _courtTf;
    [SerializeField] List<CharComponent> _placedChars = new List<CharComponent>();
#if UNITY_EDITOR
    [field: SerializeField, Get] public EditorTeamAnimator EditorAnimator { get; private set; }
#endif
    public IReadOnlyList<CharComponent> PlacedChars => _placedChars;
     

    public void Activate(TeamManeuver move) {
        if (CurrentActive)
            Deactivate();
        if (move) 
            CurrentActive = move;
        UpdateChars();
    }

    public void UpdateChars() {
        if (!CurrentActive)
            return;
        int i = 0;
        for (; i < CurrentActive.CharsData.Count; i++) {
            if (_placedChars.Count <= i) {
                var spawned = Instantiate(_template, transform);
                spawned.gameObject.SetActive(true);
                _placedChars.Add(spawned);
            }
            _placedChars[i].SetData(CurrentActive.CharsData[i]);
        }
        while(i < _placedChars.Count) {
            if (_placedChars[i])
                _placedChars[i].gameObject.SafeDestroy();
            _placedChars.RemoveAt(i);
        }
    }

    private void Update() {
        UpdateChars();
        _courtTf.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Deactivate() {
        foreach (var placedChar in _placedChars)
            if(placedChar)
                placedChar.gameObject.SafeDestroy();
        _placedChars.Clear();
        CurrentActive = null;
    }

    private void OnEnable() {
        Activate(CurrentActive);
    }

#if UNITY_EDITOR 
    private void OnGUI() {
        for (int i=0; i < _placedChars.Count; i++) {
            var head = _placedChars[i].Head;
            Handles.Label(head.position + new Vector3(0, 0.2f, 0), i.ToString());
        }
    }
#endif
}
