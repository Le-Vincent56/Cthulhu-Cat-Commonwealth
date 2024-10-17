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

        private InteractController interactController;

        private void OnEnable()
        {
            // Link the Serialized Properties to their corresponding fields
            playerOneInputReaderProp = serializedObject.FindProperty("playerOneInputReader");
            playerTwoInputReaderProp = serializedObject.FindProperty("playerTwoInputReader");
            playerTypeProp = serializedObject.FindProperty("playerType");
            currentInteractableProp = serializedObject.FindProperty("currentInteractable");

            // Cast the target
            interactController = (InteractController)target;
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

            // Create space
            EditorGUILayout.Space();

            // Show the Keys HashSet
            EditorGUILayout.LabelField("Keys", EditorStyles.boldLabel);

            // Check if the Keys HashSet exists
            if(interactController.Keys != null)
            {
                // If so, cast into a List
                List<int> keysList = new List<int>(interactController.Keys);

                // Iterate through the List
                for(int i = 0; i < keysList.Count; i++)
                {
                    // Display each Key
                    EditorGUILayout.LabelField($"Key {i + 1}: {keysList[i]}");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No keys", MessageType.Info);
            }

            // Apply any modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}