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

    [Header("기타 버튼")]
    [SerializeField] Button _userPolicyButton;
    [SerializeField] Button _privacyPolicyButton;

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

        _userPolicyButton.onClick.RemoveAllListeners();
        _privacyPolicyButton.onClick.RemoveAllListeners();

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

        _userPolicyButton.onClick.AddListener(OpenUserPolicy);
        _privacyPolicyButton.onClick.AddListener(OpenPrivacyPolicy);
    }

    private void OnEnable()
    {
        if (SettingManager.Instance.SettingData == null)
        {
            _fps60.SetIsOnWithoutNotify(true);
            _masterVolSlider.SetValueWithoutNotify(0.8f);
            _bgmVolSlider.SetValueWithoutNotify(0.8f);
            _sfxVolSlider.SetValueWithoutNotify(0.8f);

            _korean.SetIsOnWithoutNotify(true);

            _masterMuteToggle.SetIsOnWithoutNotify(false);
            _bgmMuteToggle.SetIsOnWithoutNotify(false);
            _sfxMuteToggle.SetIsOnWithoutNotify(false);
            _viberationToggle.SetIsOnWithoutNotify(true);
            return;
        }
        else
        {
            RefreshUI();
        }            
    }

    public void RefreshUI()
    {
        SettingManager manager = SettingManager.Instance;
        if (manager == null || manager.SettingData == null) return;

        var data = manager.SettingData;
        

        // 볼륨 슬라이더 동기화
        _masterVolSlider.SetValueWithoutNotify(data.masterVol);
        _bgmVolSlider.SetValueWithoutNotify(data.bgmVol);
        _sfxVolSlider.SetValueWithoutNotify(data.sfxVol);

        // 음소거 토글 동기화 (Mute 체크박스라면 데이터의 반대값을 넣어야 함)
        _masterMuteToggle.SetIsOnWithoutNotify(!data.isMasterVolOn);
        _bgmMuteToggle.SetIsOnWithoutNotify(!data.isBGMVolOn);
        _sfxMuteToggle.SetIsOnWithoutNotify(!data.isSFXVolOn);

        // FPS 및 진동 동기화
        _fps60.SetIsOnWithoutNotify(data.fps == 60);
        _fps30.SetIsOnWithoutNotify(data.fps == 30);
        _viberationToggle.SetIsOnWithoutNotify(data.isVibOn);

        // 언어 토글 동기화
        _korean.SetIsOnWithoutNotify(data.language == Language.Ko);
        _english.SetIsOnWithoutNotify(data.language == Language.En);
        _japanese.SetIsOnWithoutNotify(data.language == Language.Ja);
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

    private void OpenPrivacyPolicy()
    {
        SettingManager.Instance.OpenPrivacyPolicy();
    }

    private void OpenUserPolicy()
    {
        SettingManager.Instance.OpenUserPolicy();
    }



    public void OnQuitSetting()
    {
        SettingManager.Instance.OnQuitSetting();
    }

}
