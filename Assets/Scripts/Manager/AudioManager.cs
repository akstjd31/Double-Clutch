using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioMixer _mixer;

    public void UpdateVolume()
    {
        var data = SettingManager.Instance.SettingData;
        
        ApplyVolume("masterVol", data.isMasterVolOn? data.masterVol : 0);
        ApplyVolume("bgmVol", data.isBGMVolOn? data.bgmVol : 0);
        ApplyVolume("sfxVol", data.isSFXVolOn? data.sfxVol : 0);
    }


    private void ApplyVolume(string name, float sliderValue)
    {
        float db;
        
        if (sliderValue <= 0.0001f)// 슬라이더가 0이거나 매우 낮으면 아예 최저치(-80)로 고정
        {
            db = -80f;
        }
        else
        {            
            db = Mathf.Log10(sliderValue) * 20;// 0.0001보다 클 때만 로그 계산
        }
        _mixer.SetFloat(name, db);
    }
}
