using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider UiVolumeSlider;
    [SerializeField]
    private Slider SfxVolumeSlider;
    [SerializeField]
    private Slider MusicVolumeSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    private void OnEnable()
    {
        UiVolumeSlider.onValueChanged.AddListener(SetUiVolume);
        SfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        MusicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void OnDisable()
    {
        UiVolumeSlider.onValueChanged.RemoveListener(SetUiVolume);
        SfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
        MusicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUiVolume(float value)
    {
        audioMixer.SetFloat("UiVolume",Mathf.Log10((float)value) * 20);
    }

    public void SetSfxVolume(float value)
    {
        audioMixer.SetFloat("SfxVolume", Mathf.Log10((float)value) * 20);
    }

    public void SetMusicVolume(float value) 
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10((float)value) * 20);
    }
}
