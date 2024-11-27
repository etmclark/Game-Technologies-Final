using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPEffect : MonoBehaviour
{
    public Slider Slider1;
    public Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    void Start()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.intensity.overrideState = true;
        }
        if (volume.profile.TryGet(out chromaticAberration))
        {
            chromaticAberration.intensity.overrideState = true;
        }
        Slider1.onValueChanged.AddListener(UpdatePostProcessingEffects);
    }

    void UpdatePostProcessingEffects(float value)
    {
        float healthPercentage = value / Slider1.maxValue;

        if (value > Slider1.maxValue * 0.75f)
        {
            if (vignette != null)
            {
                vignette.intensity.value = 0.0f;
            }
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = 0.0f;
            }
        }
        else if (value > Slider1.maxValue * 0.25f)
        {
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0.4f, 0.2f, (value - Slider1.maxValue * 0.25f) / (Slider1.maxValue * 0.5f));
            }
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(0.6f, 0.2f, (value - Slider1.maxValue * 0.25f) / (Slider1.maxValue * 0.5f));
            }
        }
        else
        {
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0.6f, 0.4f, value / (Slider1.maxValue * 0.25f));
            }
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(1.0f, 0.6f, value / (Slider1.maxValue * 0.25f));
            }
        }
    }
}
