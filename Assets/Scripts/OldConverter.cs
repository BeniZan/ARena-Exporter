#if UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class OldConverter : MonoBehaviour
{
    [SerializeField] TextAsset _json;
     
    public class Data {
        public Vector3 position;
        public Quaternion rotation; 
        public struct InternalData {
            public string Animation;
        }
        public InternalData data;
    } 
    [Button]
    void TestConversion() { 
        JObject jObject = JObject.Parse(_json.text);
        var jPropAllMoves = jObject["data"];
        int i = 0;
        var path = "Assets/Data/Moves/Converted/";
        foreach (var moveProp in jPropAllMoves.Children()) {
            i++;
            var move = Convert(moveProp);
            var fullPath = path + "converted_move_" + i + ".asset";
            AssetDatabase.DeleteAsset(fullPath);
            AssetDatabase.CreateAsset(move, fullPath);
            Debug.Log("Created asset at " + fullPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    TeamManeuver Convert(JToken moveProp) {
        var move = ScriptableObject.CreateInstance<TeamManeuver>();
        foreach (var playerData in moveProp.Children().Children()) {
            var charData = Convert(playerData.ToObject<Data>());
            move.CharsData.Add(charData);
        }
        return move;
    }

    CharData Convert(Data oldData) {
        return new CharData() {
            FieldStandardPosition = oldData.position.XZ(),
            yRotation = oldData.rotation.eulerAngles.y,
            Animation = CharData.GetAnimation(oldData.data.Animation)
        };
    } 
}
#endif