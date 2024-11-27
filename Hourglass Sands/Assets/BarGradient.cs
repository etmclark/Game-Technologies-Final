using UnityEngine;
using UnityEngine.UI;

public class BarGradient : MonoBehaviour
{
    public Slider Slider1; 
    public Image fillImage;     
    public Color safe = Color.green; 
    public Color meh = Color.yellow; 
    public Color danger = Color.gray;  
    void Start()
    {
        Slider1.onValueChanged.AddListener(UpdateFillColor);
    }
    void UpdateFillColor(float value)
    {
        if (fillImage != null)
        {
            if (value > 15)
            {
                fillImage.color = safe; 
            }
            else if (value > 5)
            {
                fillImage.color = Color.Lerp(meh, safe, (value - 5) / 10f);
            }
            else
            {
                fillImage.color = Color.Lerp(danger, meh, value / 5f);
            }
        }
    }
}
