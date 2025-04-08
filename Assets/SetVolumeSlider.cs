using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SetVolumeSlider : MonoBehaviour
{

    public AudioMixer mixer;
    public string floatName = "Master";
    private Slider slider;



    public void Start()
    {
        slider = GetComponent<Slider>();

        float initialVolume = 10;
        mixer.GetFloat(floatName, out initialVolume);
        
        slider.SetValueWithoutNotify(Mathf.Pow(10, initialVolume / 20));
        slider.onValueChanged.AddListener(SetLevel);
    }
    public void SetLevel(float slider_value)
    {
        if(Mathf.Approximately(slider_value, float.Epsilon))
        {
            mixer.SetFloat(floatName, -80);
        }
        else
        {
            mixer.SetFloat(floatName, Mathf.Log10(slider_value) * 20);
        }
    }
}
