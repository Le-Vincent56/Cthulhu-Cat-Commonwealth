using Tethered.Monster.Triggers;
using Tethered.Utilities.Hash;
using UnityEditor;
using UnityEngine;

namespace Tethered.Monster.Editors
{
    [CustomEditor(typeof(AttractionTrigger), true)]
    public class AttractionTriggerEditor : Editor
    {
        private AttractionTrigger trigger;

        private void OnEnable()
        {
            // Cast the target
            trigger = (AttractionTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            // Call the default inspector
            base.OnInspectorGUI();

            // Create a button to generate a random hash
            if (GUILayout.Button("Generate Key Hash"))
                trigger.SetHash(HashUtils.GenerateHash());

        }
    }
}