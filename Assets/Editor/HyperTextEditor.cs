using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HyperLinkText))]
public class HyperTextEditor : Editor
{
    SerializedProperty m_HyoerColorProperty;

    private void OnEnable()
    {
        m_HyoerColorProperty = serializedObject.FindProperty("hyperLinkColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        //EditorGUILayout.PropertyField(m_HyoerColorProperty, true);

        serializedObject.ApplyModifiedProperties();
    }


}
