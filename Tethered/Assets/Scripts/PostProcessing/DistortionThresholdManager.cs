using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PostProcessing;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DistortionThresholdManager : MonoBehaviour
{
    private EventBinding<AttractionChanged> onAttractionChanged;
    private DistortionEffect distortion;

    private void Start()
    {
        onAttractionChanged = new EventBinding<AttractionChanged>(OnAttractionChanged);
        EventBus<AttractionChanged>.Register(onAttractionChanged);
        
        Volume volume = gameObject.GetComponent<Volume>();
        volume.profile.TryGet<DistortionEffect>(out distortion);
    }
    
    private void OnAttractionChanged(AttractionChanged eventData)
    {
        var ratio = RescaleByTier(eventData);
        
        // change tween to tweening a separate variable, for better consolidation, fewer Tweens running.
        DOTween.To(() => distortion.intensity.value, x => distortion.intensity.value = x, ratio, 1);
    }

    private float RescaleByTier(AttractionChanged eventData)
    {
        // 0   - 0.0
        // 25  - 0.2
        // 50  - 0.3
        // 75  - 0.5
        // 99  - 0.55
        // 100 - 2
        
        var ratio = eventData.AttractionLevelTotal / eventData.AttractionLevelMax;

        if (ratio < 0.25f) // 0 - 25
        {
            ratio = RemapRange(ratio, 0, 0.25f, 0, 0.2f);
        }
        else if (ratio < 0.50f) // 25 - 50
        {
            ratio = RemapRange(ratio, 0.25f, 0.5f, 0.2f, 0.3f);
        }
        else if (ratio < 0.75f) // 50 - 75
        {
            ratio = RemapRange(ratio, 0.5f, 0.75f, 0.3f, 0.5f);
        }
        else if (ratio < 1.0f) // 75 - 100
        {
            ratio = RemapRange(ratio, 0.75f, 1, 0.5f, 0.55f);
        }
        else // 100
        {
            ratio = 2;
        }

        return ratio;
    }
    
    /// <summary>
    /// From this thread: https://discussions.unity.com/t/re-map-a-number-from-one-range-to-another/465623/5
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float RemapRange (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
