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

        Debug.Log($"AudioManager {_pathIndex.Count}���� ���ҽ� ��� Init �Ϸ�.");
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

        Debug.LogWarning($"[AudioManager] ���ҽ��� �ε��� �� �����ϴ�. ID: {resourceId}");
        return null;
    }

    public void PlaySound(string resourceId)
    {
        if (_bgmAudioSource == null) return;
        if (string.IsNullOrWhiteSpace(resourceId)) return;

        var clip = GetAudioClip(resourceId);

        if (_bgmAudioSource.isPlaying)
            _bgmAudioSource.Stop();

        _bgmAudioSource.clip = clip;
        _bgmAudioSource.Play();
    }

    // 현재 재생중인 클립과 동일한지?
    public bool IsSameClip(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId)) return false;

        var clip = GetAudioClip(resourceId);
        return _bgmAudioSource.clip.Equals(clip);
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

    // �޸� ����ȭ�� ���� ĳ�� Ŭ����.    
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


    // ���� ����
    private void ApplyVolume(string name, float sliderValue)
    {
        float db;
        
        if (sliderValue <= 0.0001f)// �����̴��� 0�̰ų� �ſ� ������ �ƿ� ����ġ(-80)�� ����
        {
            db = -80f;
        }
        else
        {            
            db = Mathf.Log10(sliderValue) * 20;// 0.0001���� Ŭ ���� �α� ���
        }
        _mixer.SetFloat(name, db);
    }
}
