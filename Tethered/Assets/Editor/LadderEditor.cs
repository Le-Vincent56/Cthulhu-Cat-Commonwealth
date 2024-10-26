using System.Collections;
using System.Collections.Generic;
using Tethered.Utilities.Hash;
using UnityEditor;
using UnityEngine;

namespace Tethered.Interactables.Editors
{
    [CustomEditor(typeof(Ladder))]
    public class LadderEditor : Editor
    {
        private Ladder ladder;

        private void OnEnable()
        {
            // Cast the target
            ladder = (Ladder)target;
        }

        public override void OnInspectorGUI()
        {
            // Call the default inspector
            base.OnInspectorGUI();

            // Create a button to generate a random hash
            if (GUILayout.Button("Generate Key Hash"))
                ladder.SetHash(HashUtils.GenerateHash());

        }
    }
}