using UnityEngine;

namespace PostProcessing
{
    class HeartbeatRenderPass : PostProcessingRenderPass<HeartbeatEffect>
    {
        readonly int _intensityID = Shader.PropertyToID("_Intensity");
        private readonly int _flowID = Shader.PropertyToID("_Flow");
    
        public HeartbeatRenderPass(Material material) : base(material)
        {
        }
    
        protected override void ApplyEffect(HeartbeatEffect customEffect)
        {
            material.SetFloat(_intensityID, customEffect.intensity.value);
            material.SetFloat(_flowID, customEffect.flow.value);
        }
    }
}