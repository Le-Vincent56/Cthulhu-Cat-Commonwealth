using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tethered.Player.Editors
{
    [CustomEditor(typeof(InteractController))]
    public class InteractControllerEditor : Editor
    {
        private SerializedProperty playerOneInputReaderProp;
        private SerializedProperty playerTwoInputReaderProp;
        private SerializedProperty playerTypeProp;
        private SerializedProperty currentInteractableProp;
        private SerializedProperty soundDataProp;

        private void OnEnable()
        {
            // Link the Serialized Properties to their corresponding fields
            playerOneInputReaderProp = serializedObject.FindProperty("playerOneInputReader");
            playerTwoInputReaderProp = serializedObject.FindProperty("playerTwoInputReader");
            playerTypeProp = serializedObject.FindProperty("playerType");
            currentInteractableProp = serializedObject.FindProperty("currentInteractable");
            soundDataProp = serializedObject.FindProperty("sfx");
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object so that it's up-to-date with the values
            // in the target object
            serializedObject.Update();

            // Display the PlayerType enum
            EditorGUILayout.LabelField("Player Type", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playerTypeProp);

            // Cast the playerType
            PlayerType playerType = (PlayerType)playerTypeProp.enumValueIndex;

            // Check if the PlayerType is older
            if(playerType == PlayerType.Older)
            {
                // If so, show the PlayerOneInputReader field
                EditorGUILayout.PropertyField(playerOneInputReaderProp, new GUIContent("Input Reader"));
            }
            else if (playerType == PlayerType.Younger)
            {
                // Otherwise, show the PlayerTwoInputReader field
                EditorGUILayout.PropertyField(playerTwoInputReaderProp, new GUIContent("Input Reader"));
            }

            // Create space
            EditorGUILayout.Space();

            // Show the current Interactable field
            EditorGUILayout.LabelField("Interactables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(currentInteractableProp, new GUIContent("Current Interactable"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SFX", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(soundDataProp);

            // Apply any modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}