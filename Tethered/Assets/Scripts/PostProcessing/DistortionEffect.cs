using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Custom/Distortion", typeof(UniversalRenderPipeline))]
    public class DistortionEffect : VolumeComponent, IPostProcessComponent
    {
        [Range(0, 2)] public FloatParameter intensity = new ClampedFloatParameter(0f, 0f, 2f);
        [Range(0, 1)] public FloatParameter glow = new ClampedFloatParameter(0f, 0f, 1f);

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
