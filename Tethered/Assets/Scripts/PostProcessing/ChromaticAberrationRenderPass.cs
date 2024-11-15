using UnityEngine;

namespace PostProcessing
{
    class ChromaticAberrationRenderPass : PostProcessingRenderPass<ChromaticAberrationEffect>
    {
        readonly int _distanceID = Shader.PropertyToID("_Distance");
        readonly int _angleID = Shader.PropertyToID("_Angle");
        readonly int _intensityID = Shader.PropertyToID("_Intensity");
    
        public ChromaticAberrationRenderPass(Material material) : base(material)
        {
        }
    
        protected override void ApplyEffect(ChromaticAberrationEffect customEffect)
        {
            material.SetFloat(_distanceID, customEffect.distance.value);
            material.SetFloat(_angleID, customEffect.angle.value);
            material.SetFloat(_intensityID, customEffect.intensity.value);
        }
    }
}
