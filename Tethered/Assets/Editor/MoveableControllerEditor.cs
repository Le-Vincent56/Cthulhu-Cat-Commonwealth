using UnityEngine;
using UnityEditor;

namespace Tethered.Player.Editors
{
    [CustomEditor(typeof(MoveableController))]
    public class MoveableControllerEditor : Editor
    {
        private SerializedProperty playerOneInputReaderProp;
        private SerializedProperty playerTwoInputReaderProp;
        private SerializedProperty playerTypeProp;
        private SerializedProperty spriteTransformProp;

        private SerializedProperty movingObjectProp;
        private SerializedProperty objectToMoveProp;
        private SerializedProperty moveSpeedProp;

        private SerializedProperty positionToMoveableDurationProp;

        private MoveableController moveableController;

        private void OnEnable()
        {
            // Link the Serialized Properties to their corresponding fields
            playerOneInputReaderProp = serializedObject.FindProperty("playerOneInputReader");
            playerTwoInputReaderProp = serializedObject.FindProperty("playerTwoInputReader");
            playerTypeProp = serializedObject.FindProperty("playerType");
            spriteTransformProp = serializedObject.FindProperty("spriteTransform");

            movingObjectProp = serializedObject.FindProperty("movingObject");
            objectToMoveProp = serializedObject.FindProperty("objectToMove");
            moveSpeedProp = serializedObject.FindProperty("moveSpeed");

            positionToMoveableDurationProp = serializedObject.FindProperty("positionToMoveableDuration");

            // Cast the target
            moveableController = (MoveableController)target;
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

            // Get the Player's Sprite Transform
            EditorGUILayout.PropertyField(spriteTransformProp, new GUIContent("Sprite Transform"));

            // Create space
            EditorGUILayout.Space();

            // Show the current Interactable field
            EditorGUILayout.LabelField("Moveables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(movingObjectProp, new GUIContent("Is Moving Object?"));
            EditorGUILayout.PropertyField(moveSpeedProp, new GUIContent("Moving Speed"));
            EditorGUILayout.PropertyField(objectToMoveProp, new GUIContent("Current Interactable"));

            // Create space
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tweening Variables", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(positionToMoveableDurationProp, new GUIContent("Duration for Positioning"));

            // Apply any modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}