using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(Hitbox))]
public class HitboxEditor : Editor
{
    private SerializedProperty hitboxesProp;
    private SerializedProperty layermaskProp;
    private SerializedProperty inactiveColorProp;
    private SerializedProperty collisionOpenColorProp;
    private SerializedProperty collidingColorProp;
    private SerializedProperty defaultOpenProp;
    private SerializedProperty isComboHitboxProp;
    private SerializedProperty knockBackProp;

    private BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle();

    private void OnEnable()
    {
        hitboxesProp = serializedObject.FindProperty("hitboxes");
        layermaskProp = serializedObject.FindProperty("layermask");
        inactiveColorProp = serializedObject.FindProperty("inactiveColor");
        collisionOpenColorProp = serializedObject.FindProperty("collisionOpenColor");
        collidingColorProp = serializedObject.FindProperty("collidingColor");
        defaultOpenProp = serializedObject.FindProperty("defaultOpen");
        isComboHitboxProp = serializedObject.FindProperty("isComboHitbox");
        knockBackProp = serializedObject.FindProperty("knockBack");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(layermaskProp);
        EditorGUILayout.PropertyField(inactiveColorProp);
        EditorGUILayout.PropertyField(collisionOpenColorProp);
        EditorGUILayout.PropertyField(collidingColorProp);
        EditorGUILayout.PropertyField(defaultOpenProp);
        EditorGUILayout.PropertyField(isComboHitboxProp);
        EditorGUILayout.PropertyField(knockBackProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hitboxes");

        EditorGUI.indentLevel++;
        int hitboxCount = hitboxesProp.arraySize;
        for (int i = 0; i < hitboxCount; i++)
        {
            SerializedProperty hitbox = hitboxesProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(hitbox);
            if (GUILayout.Button("Remove Hitbox"))
            {
                hitboxesProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add Hitbox"))
        {
            hitboxesProp.InsertArrayElementAtIndex(hitboxCount);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        Handles.color = Color.red;
        var hitbox = (Hitbox)target;
        var direction = Mathf.Clamp((int)hitbox.transform.root.localScale.x, -1, 1);

        for (int i = 0; i < hitbox.hitboxes.Count; i++)
        {
            var box = hitbox.hitboxes[i];
            var position = new Vector2(hitbox.transform.position.x + (box.center.x * direction), hitbox.transform.position.y + box.center.y);
            boxBoundsHandle.center = position;
            boxBoundsHandle.size = box.size;

            EditorGUI.BeginChangeCheck();
            boxBoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(hitbox, "Change Hitbox Bounds");
                var newCenter = new Vector3((boxBoundsHandle.center.x - hitbox.transform.position.x) / direction, boxBoundsHandle.center.y - hitbox.transform.position.y, 0);
                var newSize = boxBoundsHandle.size;
                hitbox.hitboxes[i] = new Bounds { center = newCenter, size = newSize };
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


}