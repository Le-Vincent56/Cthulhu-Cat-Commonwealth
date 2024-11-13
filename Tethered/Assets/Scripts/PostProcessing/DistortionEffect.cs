using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Custom/Distortion", typeof(UniversalRenderPipeline))]
    public class DistortionEffect : VolumeComponent, IPostProcessComponent
    {
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
