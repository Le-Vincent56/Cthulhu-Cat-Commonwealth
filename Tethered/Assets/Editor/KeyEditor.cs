using Tethered.Utilities.Hash;
using UnityEditor;
using UnityEngine;

namespace Tethered.Interactables.Editors
{
    [CustomEditor(typeof(Key))]
    public class KeyEditor : Editor
    {
        private Key key;

        private void OnEnable()
        {
            // Cast the target
            key = (Key)target;
        }

        public override void OnInspectorGUI()
        {
            // Call the default inspector
            base.OnInspectorGUI();

            // Create a button to generate a random hash
            if (GUILayout.Button("Generate Key Hash"))
                key.SetHash(HashUtils.GenerateHash());

        }
    }
}