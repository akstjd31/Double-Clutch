using TinyJSON;
using UnityEngine;

public class SettingManager : Singleton<SettingManager>
{
    [SerializeField] SettingSaveData _settingData;
    const string SAVE_FILE = "SettingSave.json";

    public SettingSaveData SettingData => _settingData;
    private void Start()
    {
        LoadSetting();
        ApplyFPS();        
    }

    // 진동 기능은 여기서 호출
    public void PlayVibration() 
    {
        if (_settingData.isVibOn)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }

    #region 세팅 패널 UI에서 호출

    public void SetMasterVolume(float value) //마스터 볼륨 슬라이더
    {
        _settingData.masterVol = value;
        ApplyVolum();
    }

    public void SetBGMVolume(float value) //BGM 볼륨 슬라이더
    {
        _settingData.bgmVol = value;
        ApplyVolum();
    }
    public void SetSFXVolum(float value) //효과음 볼륨 슬라이더
    {
        _settingData.sfxVol = value;
        ApplyVolum();
    }

    public void SetFPS(int fps) //FPS 설정 토글
    {        
        _settingData.fps = fps;
        ApplyFPS();
    }    

    public void ToggleMasterVolum(bool isOn) //전체 음소거 토글
    {
        _settingData.isMasterVolOn = isOn;
        ApplyVolum();
    }

    public void ToggleBGMVolum(bool isOn) //배경음악 음소거 토글
    {
        _settingData.isBGMVolOn = isOn;
        ApplyVolum();
    }

    public void ToggleSFXVolum(bool isOn) //효과음 음소거 토글
    {
        _settingData.isSFXVolOn = isOn;
        ApplyVolum();
    }

    public void ToggleVibration(bool isOn) //진동 기능 토글
    {
        _settingData.isVibOn = isOn;
    }

    public void OnQuitSetting() //설정 창 닫을 때 호출
    {
        SaveSetting();
    }

    #endregion


    #region 세팅 후 값을 실제 적용하기 위한 내부함수
    private void ApplyFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _settingData.fps;
    }

    private void ApplyVolum()
    {
        AudioManager.Instance.UpdateVolume();
    }

    #endregion


    #region 세이브 & 로드 기능
    public void SaveSetting()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.Save(SAVE_FILE, _settingData);
            Debug.Log("설정 데이터 저장 완료");
        }
    }

    public void LoadSetting()
    {
        if (SaveLoadManager.Instance.TryLoad(SAVE_FILE, out SettingSaveData data)) //저장된 데이터 로드
        {
            _settingData = data;
            Debug.Log("설정 데이터 로드 완료");
        }
        else //첫 실행 또는 데이터 손실
        {
            _settingData = new SettingSaveData();
            SetFPS(60);
            ToggleVibration(false);
        }
    }

    #endregion

}
