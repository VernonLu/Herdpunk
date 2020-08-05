using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour {

    public AudioMixer audioMixer;

    public float bgmVolume;
    public float sfxVolume;

    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start() {
        audioMixer.GetFloat("bgmVolume", out bgmVolume);
        audioMixer.GetFloat("sfxVolume", out sfxVolume);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
    }

    // Update is called once per frame
    void Update() {

    }

    public void ChangeBgmVolume(float value) {
        bgmVolume = value;
        audioMixer.SetFloat("bgmVolume", bgmVolume);
    }

    public void ChangeSfxVolume(float value) {
        sfxVolume = value;
        audioMixer.SetFloat("sfxVolume", sfxVolume);
    }

}
