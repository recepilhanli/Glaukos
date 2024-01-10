using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyInspector))]
public class ReadOnlyInspectorDrawer : PropertyDrawer
{
   public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
   {
       GUI.enabled = false;
       EditorGUI.PropertyField(position, property, label);
       GUI.enabled = true;
   }
}

public class ReadOnlyInspector : PropertyAttribute
{
}

#else
// Dummy class for build without UnityEditor
internal class ReadOnlyInspectorAttribute : System.Attribute
{
}
#endif