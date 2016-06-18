using UnityEngine;
using UnityEditor;
using Valve.VR;

[CustomEditor(typeof(HOTK_Overlay))]
// ReSharper disable once CheckNamespace
public class HOTK_OverlayInspector : Editor
{
    private static GUIStyle _boldFoldoutStyle;

    public override void OnInspectorGUI()
    {
        if (_boldFoldoutStyle == null)
        {
            _boldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
        }

        var overlay = (HOTK_Overlay)target;

        overlay.ShowSettingsAppearance = EditorGUILayout.Foldout(overlay.ShowSettingsAppearance, "Appearance Settings", _boldFoldoutStyle);
        if (overlay.ShowSettingsAppearance)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OverlayTexture"), new GUIContent() {text = "Texture"});

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Alpha"));
            var hqProperty = serializedObject.FindProperty("Highquality");
            EditorGUILayout.PropertyField(hqProperty, new GUIContent() { text = "High Quality" });
            if (hqProperty.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Antialias"), new GUIContent() { text = "Anti Alias" });
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Curved"), new GUIContent() { text = "Curved Display" });
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UvOffset"), true);
        }

        overlay.ShowSettingsInput = EditorGUILayout.Foldout(overlay.ShowSettingsInput, "Input Settings", _boldFoldoutStyle);
        if (overlay.ShowSettingsInput)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MouseScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CurvedRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InputMethod"));
        }
        
        overlay.ShowSettingsAttachment = EditorGUILayout.Foldout(overlay.ShowSettingsAttachment, "Attachment Settings", _boldFoldoutStyle);
        if (overlay.ShowSettingsAttachment)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnchorDevice"), new GUIContent() { text = "Anchor Point" });
            if (overlay.AnchorDevice != HOTK_Overlay.AttachmentDevice.Screen &&
                overlay.AnchorDevice != HOTK_Overlay.AttachmentDevice.World)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AnchorPoint"), new GUIContent() { text = "Base Position" });
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnchorOffset"), new GUIContent() {text = "Offset"});
        }
        serializedObject.ApplyModifiedProperties();
    }
}