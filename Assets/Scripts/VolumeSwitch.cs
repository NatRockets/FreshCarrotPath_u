using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSwitch : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup mixerMasterGroup;
    
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    
    private Image toggleImage;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("GVolume"))
        {
            PlayerPrefs.SetInt("GVolume", 1);
        }
        
        toggleImage = audioToggle.GetComponent<Image>();
        
        audioToggle.onValueChanged.AddListener(SwitchAudio);
        audioToggle.isOn = (PlayerPrefs.GetInt("GVolume") == 1);
        
        toggleImage.sprite = audioToggle.isOn ? onSprite : offSprite;
    }

    private void OnDisable()
    {
        audioToggle.onValueChanged.RemoveListener(SwitchAudio);
        PlayerPrefs.SetInt("GVolume", audioToggle.isOn ? 1 : 0);
    }
    
    private void SwitchAudio(bool isOn)
    {
        mixerMasterGroup.audioMixer.SetFloat("Param1", isOn ? 0 : -80);
        toggleImage.sprite = audioToggle.isOn ? onSprite : offSprite;
    }
}
