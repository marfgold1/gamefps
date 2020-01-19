using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(CustomButton), true)]
[CanEditMultipleObjects]
public class CustomButtonEditor : ButtonEditor{
    GUIStyle tittle;
    public void DefineGUIStyle()
    {
        tittle = new GUIStyle();
        tittle.alignment = TextAnchor.MiddleCenter;
        tittle.fontStyle = FontStyle.Bold;
    }
    
    SerializedProperty selectedColor;
    SerializedProperty unselectedColor;
    SerializedProperty _image;
    SerializedProperty _text;
    SerializedProperty hoverClip;
    protected override void OnEnable()
    {
        base.OnEnable();
        DefineGUIStyle();
        selectedColor = serializedObject.FindProperty("selectedColor");
        unselectedColor = serializedObject.FindProperty("unselectedColor");
        _image = serializedObject.FindProperty("_image");
        _text = serializedObject.FindProperty("_text");
        hoverClip = serializedObject.FindProperty("hoverClip");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("=== Button Inherited Property ===", tittle);
        GUILayout.BeginVertical("box");
        base.OnInspectorGUI();
        GUILayout.EndVertical();

        EditorGUILayout.LabelField("=== Custom Button Property ===", tittle);
        GUILayout.BeginVertical("box");
        serializedObject.Update();
        GUILayout.Label("Appearance", tittle);
        EditorGUILayout.PropertyField(selectedColor);
        EditorGUILayout.PropertyField(unselectedColor);
        EditorGUILayout.HelpBox("Make sure selected color are different than image color and otherwise", MessageType.Warning);
        EditorGUILayout.PropertyField(_image);
        EditorGUILayout.PropertyField(_text);
        GUILayout.Label("Audio Clip", tittle);
        EditorGUILayout.PropertyField(hoverClip);
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical();
    }
}
