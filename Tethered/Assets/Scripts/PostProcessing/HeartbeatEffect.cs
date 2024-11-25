using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Custom/Heartbeat", typeof(UniversalRenderPipeline))]
    public class HeartbeatEffect : VolumeComponent, IPostProcessComponent
    {
        [Range(0, 1)] public FloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        [Range(-1, 3)] public FloatParameter flow = new ClampedFloatParameter(0f, -1f, 3f);

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
