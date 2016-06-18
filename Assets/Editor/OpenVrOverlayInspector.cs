using UnityEngine;
using UnityEditor;
using Valve.VR;

[CustomEditor(typeof(OpenVrOverlay))]
// ReSharper disable once CheckNamespace
public class OpenVrOverlayInspector : Editor
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

        var overlay = (OpenVrOverlay)target;

        overlay.ShowSettingsAppearance = EditorGUILayout.Foldout(overlay.ShowSettingsAppearance, "Appearance Settings", _boldFoldoutStyle);
        if (overlay.ShowSettingsAppearance)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OverlayTexture"), new GUIContent() {text = "Texture"});

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Alpha"));
            var hqProperty = serializedObject.FindProperty("Highquality");
            EditorGUILayout.PropertyField(hqProperty);
            if (hqProperty.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Antialias"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Curved"));
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
            if (overlay.AnchorDevice != OpenVrOverlay.AttachmentDevice.Screen &&
                overlay.AnchorDevice != OpenVrOverlay.AttachmentDevice.World)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AnchorPoint"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Scale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AnchorOffset"), new GUIContent() {text = "Offset"});
        }
        serializedObject.ApplyModifiedProperties();
    }
}