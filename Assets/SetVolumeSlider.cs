using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolumeSlider : MonoBehaviour
{

    public AudioMixer mixer;
    public string floatName = "Master";
    public Slider slider;



    public void Start()
    {
        float initialVolume = 10;
        mixer.GetFloat(floatName, out initialVolume);
        
        Debug.Log(initialVolume);
        slider.SetValueWithoutNotify(Mathf.Pow(10, initialVolume / 20));
    }
    public void SetLevel(float slider_value)
    {
        mixer.SetFloat(floatName, Mathf.Log10(slider_value) * 20);
    }
}
