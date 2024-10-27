using Tethered.Utilities.Hash;
using UnityEditor;
using UnityEngine;

namespace Tethered.Interactables.Editors
{
    [CustomEditor(typeof(Stairway))]
    public class StairwayEditor : Editor
    {
        private Stairway stairway;

        private void OnEnable()
        {
            // Cast the target
            stairway = (Stairway)target;
        }

        public override void OnInspectorGUI()
        {
            // Call the default inspector
            base.OnInspectorGUI();

            // Create a button to generate a random hash
            if (GUILayout.Button("Generate Key Hash"))
                stairway.SetHash(HashUtils.GenerateHash());

        }
    }
}