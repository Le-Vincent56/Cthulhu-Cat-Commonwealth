using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tethered.Inventory.Editors
{
    [CustomEditor(typeof(SharedInventory))]
    public class SharedInventoryEditor : Editor
    {
        private SerializedProperty curtainsProp;

        private SharedInventory inventory;

        private void OnEnable()
        {
            curtainsProp = serializedObject.FindProperty("curtains");

            // Cast the target
            inventory = (SharedInventory)target;
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object so that it's up-to-date with the values
            // in the target object
            serializedObject.Update();

            // Show the Keys HashSet
            EditorGUILayout.LabelField("Keys", EditorStyles.boldLabel);

            // Check if the Keys HashSet exists
            if (inventory.Keys == null)
            {
                EditorGUILayout.HelpBox("Keys not initialized", MessageType.Info);
            } else if(inventory.Keys.Count <= 0)
            {
                EditorGUILayout.HelpBox("No keys", MessageType.Info);
            }
            else
            {
                // If so, cast into a List
                List<int> keysList = new List<int>(inventory.Keys);

                // Iterate through the List
                for (int i = 0; i < keysList.Count; i++)
                {
                    // Display each Key
                    EditorGUILayout.LabelField($"Key {i + 1}: {keysList[i]}");
                }
            }

            // Create space
            EditorGUILayout.Space();

            // Show the current Interactable field
            EditorGUILayout.PropertyField(curtainsProp);

            // Apply any modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}