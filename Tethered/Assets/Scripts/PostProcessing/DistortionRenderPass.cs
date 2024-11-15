using UnityEngine;

namespace PostProcessing
{
    class DistortionRenderPass : PostProcessingRenderPass<DistortionEffect>
    {
        readonly int _intensityID = Shader.PropertyToID("_Intensity");
        private readonly int _glowID = Shader.PropertyToID("_Glow");
    
        public DistortionRenderPass(Material material) : base(material)
        {
        }
    
        protected override void ApplyEffect(DistortionEffect customEffect)
        {
            material.SetFloat(_intensityID, customEffect.intensity.value);
            material.SetFloat(_glowID, customEffect.glow.value);
        }
    }
}
