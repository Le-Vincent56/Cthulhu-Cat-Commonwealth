using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Custom/ChromaticAberration", typeof(UniversalRenderPipeline))]
    public class ChromaticAberrationEffect : VolumeComponent, IPostProcessComponent
    {
        [Range(0, 1)] public FloatParameter distance = new FloatParameter(1f);
        [Range (0, 2*Mathf.PI)] public FloatParameter angle = new FloatParameter(0f);
        [Range(0, 1)] public FloatParameter intensity = new FloatParameter(0f);

        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return true;
        }
    }
}
