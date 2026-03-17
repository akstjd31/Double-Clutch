using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("FPS 버튼")]
    [SerializeField] Toggle _fps30;
    [SerializeField] Toggle _fps60;

    [Header("언어 토글")]
    [SerializeField] Toggle _korean;
    [SerializeField] Toggle _english;
    [SerializeField] Toggle _japanese;

    [Header("볼륨 조절 슬라이더")]
    [SerializeField] Slider _masterVolSlider;
    [SerializeField] Slider _bgmVolSlider;
    [SerializeField] Slider _sfxVolSlider;

    [Header("음소거 토글")]
    [SerializeField] Toggle _masterMuteToggle;
    [SerializeField] Toggle _bgmMuteToggle;
    [SerializeField] Toggle _sfxMuteToggle;

    [Header("진동 토글")]
    [SerializeField] Toggle _viberationToggle;

    private void Start()
    {
        //------------------------------------------------------------
        // 구독 해제
        //------------------------------------------------------------
        _fps30.onValueChanged.RemoveAllListeners();
        _fps60.onValueChanged.RemoveAllListeners();

        _korean.onValueChanged.RemoveAllListeners();
        _english.onValueChanged.RemoveAllListeners();
        _japanese.onValueChanged.RemoveAllListeners();

        _masterVolSlider.onValueChanged.RemoveAllListeners();
        _bgmVolSlider.onValueChanged.RemoveAllListeners();
        _sfxVolSlider.onValueChanged.RemoveAllListeners();

        _masterMuteToggle.onValueChanged.RemoveAllListeners();
        _bgmMuteToggle.onValueChanged.RemoveAllListeners();
        _sfxMuteToggle.onValueChanged.RemoveAllListeners();

        _viberationToggle.onValueChanged.RemoveAllListeners();

        //------------------------------------------------------------
        // 구독 연결
        //------------------------------------------------------------        
        SettingManager manager =  SettingManager.Instance;
        StringManager stringManager = StringManager.Instance;

        _fps30.onValueChanged.AddListener(OnFPS30Changed);
        _fps60.onValueChanged.AddListener(OnFPS60Changed);

        _korean.onValueChanged.AddListener(OnKoreanToggleChanged);
        _english.onValueChanged.AddListener(OnEnglishToggleChanged);
        _japanese.onValueChanged.AddListener(OnJapanToggleChanged);

        _masterVolSlider.onValueChanged.AddListener(manager.SetMasterVolume);
        _bgmVolSlider.onValueChanged.AddListener(manager.SetBGMVolume);
        _sfxVolSlider.onValueChanged.AddListener(manager.SetSFXVolume);

        _masterMuteToggle.onValueChanged.AddListener(manager.ToggleMasterVolum);
        _bgmMuteToggle.onValueChanged.AddListener(manager.ToggleBGMVolum);
        _sfxMuteToggle.onValueChanged.AddListener(manager.ToggleSFXVolum);

        _viberationToggle.onValueChanged.AddListener(manager.ToggleVibration);


        //------------------------------------------------------------
        // 초기값 세팅
        //------------------------------------------------------------        
        if (manager.SettingData.fps == 30) _fps30.isOn = true;

        else _fps60.isOn = true;


        _masterVolSlider.value = manager.SettingData.masterVol;
        _bgmVolSlider.value = manager.SettingData.bgmVol;
        _sfxVolSlider.value = manager.SettingData.sfxVol;

        
        _masterMuteToggle.isOn = !manager.SettingData.isMasterVolOn;
        _bgmMuteToggle.isOn = !manager.SettingData.isBGMVolOn;
        _sfxMuteToggle.isOn = !manager.SettingData.isSFXVolOn;
        _viberationToggle.isOn = manager.SettingData.isVibOn;
    }
    private void OnFPS30Changed(bool isOn)
    {
        if (isOn) SettingManager.Instance.SetFPS(30);
    }
    private void OnFPS60Changed(bool isOn)
    {
        if (isOn) SettingManager.Instance.SetFPS(60);
    }

    private void OnKoreanToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.Ko);
    }

    private void OnEnglishToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.En);
    }

    private void OnJapanToggleChanged(bool isOn)
    {
        if (isOn) StringManager.Instance.SetLanguage(Language.Ja);
    }

    public void OnQuitSetting()
    {
        SettingManager.Instance.OnQuitSetting();
    }
}
