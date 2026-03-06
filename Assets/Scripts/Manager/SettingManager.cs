using UnityEngine;

public class SettingManager : Singleton<SettingManager>
{
    [SerializeField] SettingSaveData _settingSaveData;
    const string SAVE_FILE = "SettingSave.json";

    private void Start()
    {
        LoadSetting();
        ApplyFPS();        
    }

    public void SetFPS(int fps)
    {        
        _settingSaveData.fps = fps;
    }

    private void ApplyFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _settingSaveData.fps;
    }

    public void ToggleVibration(bool isOn)
    {
        _settingSaveData.isVibOn = isOn;
    }

    public void PlayVibration()
    {
        if (_settingSaveData.isVibOn)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }

    public void SaveSetting()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.Save(SAVE_FILE, _settingSaveData);
            Debug.Log("설정 데이터 저장 완료");
        }
    }

    public void LoadSetting()
    {
        if (SaveLoadManager.Instance.TryLoad(SAVE_FILE, out SettingSaveData data))
        {
            _settingSaveData = data;
            Debug.Log("설정 데이터 로드 완료");
        }
    }
   
}
