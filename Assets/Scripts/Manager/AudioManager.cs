using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] ResourceDataReader _reader;
    [SerializeField] AudioMixer _mixer;
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    private Dictionary<string, string> _pathIndex = new Dictionary<string, string>();
    private Dictionary<string, AudioClip> _audioCache = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();
        InitPathTable();
    }

    public void InitPathTable()
    {
        if (_reader == null) return;

        _pathIndex.Clear();
        foreach (var data in _reader.DataList)
        {
            if (string.IsNullOrEmpty(data.resourceId)) continue;

            string fullPath = $"{data.resourcePath}/{data.resourceId}";

            if (!_pathIndex.ContainsKey(data.resourceId))
            {
                _pathIndex.Add(data.resourceId, fullPath);
            }
        }

        Debug.Log($"AudioManager {_pathIndex.Count}개의 리소스 경로 Init 완료.");
    }

    public AudioClip GetAudioClip(string resourceId)
    {
        if (_audioCache.TryGetValue(resourceId, out AudioClip cachedAudioClip))
            return cachedAudioClip;

        if (_pathIndex.TryGetValue(resourceId, out string fullPath))
        {
            AudioClip loadedAudioClip = Resources.Load<AudioClip>(fullPath);

            if (loadedAudioClip != null)
            {
                _audioCache.Add(resourceId, loadedAudioClip);
                return loadedAudioClip;
            }
        }

        Debug.LogWarning($"[AudioManager] 리소스를 로드할 수 없습니다. ID: {resourceId}");
        return null;
    }

    public void PlaySound(AudioClip clip)
    {
        if (_bgmAudioSource == null) return;
        _bgmAudioSource.clip = clip;
        _bgmAudioSource.Play();
    }

    public void PlaySoundOneShot(AudioClip clip)
    {
        if (_sfxAudioSource == null) return;
        _sfxAudioSource.PlayOneShot(clip);
    }

    public void StopSound()
    {
        if (_bgmAudioSource == null) return;

        _bgmAudioSource.Stop();
    }

    // 메모리 최적화를 위한 캐시 클리어.    
    public void ClearCache()
    {
        _audioCache.Clear();
        Resources.UnloadUnusedAssets();
    }

    public void UpdateVolume()
    {
        var data = SettingManager.Instance.SettingData;
        
        ApplyVolume("masterVol", data.isMasterVolOn? data.masterVol : 0);
        ApplyVolume("bgmVol", data.isBGMVolOn? data.bgmVol : 0);
        ApplyVolume("sfxVol", data.isSFXVolOn? data.sfxVol : 0);
    }


    // 볼륨 조절
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
