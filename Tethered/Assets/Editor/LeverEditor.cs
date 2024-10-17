using Tethered.Utilities.Hash;
using UnityEditor;
using UnityEngine;

namespace Tethered.Interactables.Editors
{
    [CustomEditor(typeof(Lever))]
    public class LeverEditor : Editor
    {
        private Lever lever;

        private void OnEnable()
        {
            // Cast the target
            lever = (Lever)target;
        }

        public override void OnInspectorGUI()
        {
            // Call the default inspector
            base.OnInspectorGUI();

            // Create a button to generate a random hash
            if (GUILayout.Button("Generate Key Hash"))
                lever.SetHash(HashUtils.GenerateHash());

        }
    }
}