using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*
public class GroupedAnimation : ScriptableObject {
    public string GroupedAnimationName;
    public CharData[] Chars;
}

public class CharSelection : MonoBehaviour{ 
    public Notifier<CharComponent> SelectedChar = new Notifier<CharComponent>();
    
    public void SetSelectChar(CharComponent c) {
        //if (c == SelectedChar.Value)
       //     return;
       // if(SelectedChar.Value)
       //     SelectedChar.Value.OnIsSelected(false);
       // SelectedChar.Value = c;
       // if (SelectedChar.Value)
      //      SelectedChar.Value.OnIsSelected(true);
    }

    RealTimer _pressTime;

    void Update() {
        var pressed = Mouse.current.leftButton.wasPressedThisFrame;
        if (pressed)
            _pressTime.Restart();

        var released = Mouse.current.leftButton.wasReleasedThisFrame;
        if (released && _pressTime.TimeRunning < 0.3f)
            SelectRay();
    }

    void SelectRay() {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Layers.CharMask)) {
            Debug.Log("Selection hit " + hit.collider.gameObject.name, hit.collider.gameObject);
            SetSelectChar(hit.collider.GetComponent<CharComponent>());
        }
    }

}
*/
