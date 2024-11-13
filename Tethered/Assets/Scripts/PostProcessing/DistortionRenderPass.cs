using UnityEngine;

namespace PostProcessing
{
    class DistortionRenderPass : PostProcessingRenderPass<DistortionEffect>
    {
        readonly int _distanceID = Shader.PropertyToID("_Distance");
        readonly int _angleID = Shader.PropertyToID("_Angle");
        readonly int _intensityID = Shader.PropertyToID("_Intensity");
    
        public DistortionRenderPass(Material material) : base(material)
        {
        }
    
        protected override void ApplyEffect(DistortionEffect customEffect)
        {
            material.SetFloat(_intensityID, customEffect.intensity.value);
        }
    }
}
