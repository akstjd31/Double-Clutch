using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    private const int NAME_MAX = 13;

    [SerializeField] private BannedWordDataReader _reader;
    [SerializeField] private TMP_InputField _schoolNameField;
    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private Button _schoolSelectButton;
    [SerializeField] private Button _playerSelectButton;
    [SerializeField] private GameObject _warningTextObj;
    [SerializeField] private TextMeshProUGUI _warningText;
    [SerializeField] private Button _confirmButton;
    private Coroutine _warningCoroutine;

    [Header("학교 수정 팝업")]
    [SerializeField] private GameObject _schoolModifyPanel;
    [SerializeField] private TMP_InputField _schoolPopupInputField;
    [SerializeField] private Button _confirmSchoolButton;
    [SerializeField] private Button _cancelSchoolButton;
    [SerializeField] private TextMeshProUGUI _schoolWarningText;

    [Header("감독 수정 팝업")]
    [SerializeField] private GameObject _coachModifyPanel;
    [SerializeField] private TMP_InputField _coachPopupInputField;
    [SerializeField] private Button _confirmCoachButton;
    [SerializeField] private Button _cancelCoachButton;
    [SerializeField] private TextMeshProUGUI _coachWarningText;

    [Header("아이콘 설정 및 해금")]
    [SerializeField] private LobbyProfileIcon _LobbyProfileIcon;
    [SerializeField] private Image _currentIcon;
    [SerializeField] private ProfileIcon _profilePrefab;
    [SerializeField] private GameObject _pagePanelPrefab;
    [SerializeField] private Transform _pageWindow;
    [SerializeField] private ProfileDataReader _profileDataReader;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _profileBoxImage;

    private GenericObjectPool<ProfileIcon> _iconPool;
    private GenericObjectPool<GameObject> _pagePool;
    private List<ProfileIcon> _activeIcons = new List<ProfileIcon>();
    private List<GameObject> _activePages = new List<GameObject>();
    private int _currentPageIndex = 0;

    [Header("Selected Info")]
    private ProfileIcon _selectedIcon;
    private ProfileData? _selectedData;

    private void Awake()
    {
        _iconPool = new GenericObjectPool<ProfileIcon>(_profilePrefab, this.transform);
        _pagePool = new GenericObjectPool<GameObject>(_pagePanelPrefab, this._pageWindow);
    }

    private void Start()
    {
        RefreshProfileList();
        var saveData = GameManager.Instance.SaveData;

        // 초기 이미지 설정
        string imgKey = (saveData == null || string.IsNullOrEmpty(saveData.currentProfileImage))
            ? _profileDataReader.DataList[0].playerImage
            : saveData.currentProfileImage;

        if (_LobbyProfileIcon != null && saveData != null)
            _LobbyProfileIcon.SetImage(SpriteManager.Instance.GetSprite(imgKey));

        if (_profileBoxImage != null)
            _profileBoxImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage);
    }

    private void OnEnable()
    {
        if (_confirmButton != null) _confirmButton.onClick.AddListener(OnClickConfirmButton);
        if (_prevButton != null) _prevButton.onClick.AddListener(() => ChangePage(-1));
        if (_nextButton != null) _nextButton.onClick.AddListener(() => ChangePage(1));

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        if (gameManager.SaveData != null)
        {
            SpriteManager spriteManager = SpriteManager.Instance;

            string currentImg = gameManager.SaveData?.currentProfileImage;
            int index = _profileDataReader.DataList.FindIndex(x => x.playerImage == currentImg);

            if (index != -1)
            {
                _selectedData = _profileDataReader.DataList[index];
            }
            else
            {
                _selectedData = _profileDataReader.DataList[0];
            }

            _currentIcon.sprite = spriteManager.GetSprite(_selectedData.Value.playerImage);

            if (_warningText != null) _warningText.text = "";

            _schoolNameField.text = gameManager.SaveData.schoolName;
            _playerNameField.text = gameManager.SaveData.coachName;
            _schoolNameField.interactable = false;
            _playerNameField.interactable = false;

            _schoolSelectButton?.onClick.AddListener(() => OpenPopup(_schoolModifyPanel, _schoolPopupInputField));
            _playerSelectButton?.onClick.AddListener(() => OpenPopup(_coachModifyPanel, _coachPopupInputField));
            _confirmSchoolButton?.onClick.AddListener(OnClickConfirmSchool);
            _confirmCoachButton?.onClick.AddListener(OnClickConfirmCoach);
            _cancelSchoolButton?.onClick.AddListener(() => _schoolModifyPanel.SetActive(false));
            _cancelCoachButton?.onClick.AddListener(() => _coachModifyPanel.SetActive(false));

            RefreshProfileList();
        }
    }

    private void OnDisable()
    {
        _confirmButton?.onClick.RemoveAllListeners();
        _prevButton?.onClick.RemoveAllListeners();
        _nextButton?.onClick.RemoveAllListeners();

        if (GameManager.Instance.SaveData != null)
        {
            _schoolSelectButton?.onClick.RemoveAllListeners();
            _playerSelectButton?.onClick.RemoveAllListeners();
            _confirmSchoolButton?.onClick.RemoveAllListeners();
            _confirmCoachButton?.onClick.RemoveAllListeners();
            _cancelSchoolButton?.onClick.RemoveAllListeners();
            _cancelCoachButton?.onClick.RemoveAllListeners();
        }
    }

    private void ChangePage(int direction)
    {
        int nextIndex = _currentPageIndex + direction;
        if (nextIndex < 0 || nextIndex >= _activePages.Count) return;

        _activePages[_currentPageIndex].SetActive(false);
        _currentPageIndex = nextIndex;
        _activePages[_currentPageIndex].SetActive(true);

        UpdatePageButtons();
    }

    private void UpdatePageButtons()
    {
        if (_prevButton != null) _prevButton.interactable = (_currentPageIndex > 0);
        if (_nextButton != null) _nextButton.interactable = (_currentPageIndex < _activePages.Count - 1);
    }

    private void OpenPopup(GameObject popupPanel, TMP_InputField popupInputField)
    {
        popupPanel.SetActive(true);
        popupInputField.text = "";
        popupInputField.Select();
    }

    public void OnClickConfirmSchool()
    {
        if (!IsValidInput(_schoolPopupInputField.text, _schoolWarningText)) return;
        GameManager.Instance.SetSchoolName(_schoolPopupInputField.text);
        _schoolNameField.text = _schoolPopupInputField.text;
        _schoolModifyPanel.SetActive(false);
    }

    public void OnClickConfirmCoach()
    {
        if (!IsValidInput(_coachPopupInputField.text, _coachWarningText)) return;
        GameManager.Instance.SetCoachName(_coachPopupInputField.text);
        _playerNameField.text = _coachPopupInputField.text;
        _coachModifyPanel.SetActive(false);
    }

    public void OnClickConfirmButton()
    {
        // if (string.IsNullOrWhiteSpace(_schoolNameField.text) || string.IsNullOrWhiteSpace(_playerNameField.text))
        // {
        //     _warningTextObj.SetActive(true);
        //     _warningText.text = "공백인 필드가 존재합니다!";
        //     return;
        // }
        var gm = GameManager.Instance;

        if (!IsValidInput(_schoolNameField.text, _warningText) || !IsValidInput(_playerNameField.text, _warningText)) return;
        if (!gm.HasData())
        {
            var data = new PlayerSaveData
            {
                schoolName = _schoolNameField.text,
                coachName = _playerNameField.text,
                weekId = 8,
                year = 0,
                isTutorialCompleted = false
            };

            gm.InitData(data);
            CalendarManager.Instance.CalcWeek(data.weekId, gm);
            gm.Dispatch(UIAction.Tutorial);
        }

        if (_selectedData.HasValue && _selectedData.Value.playerImage != null)
        {
            gm.SetCurrentProfileIcon(_selectedData.Value.playerImage);
            _LobbyProfileIcon?.Refresh();
        }

        this.gameObject.SetActive(false);
    }

    private bool IsValidNameLength(string text)
    {
        string normalized = NormalizeText(text);
        if (string.IsNullOrEmpty(normalized)) return false;
        bool hasKorean = Regex.IsMatch(normalized, @"[가-힣]");
        bool hasEnglish = Regex.IsMatch(normalized, @"[a-zA-Z]");
        if (hasEnglish) return normalized.Length >= 2 && normalized.Length <= NAME_MAX;
        if (hasKorean) return normalized.Length >= 1 && normalized.Length <= NAME_MAX;
        return false;
    }

    private bool CheckBadWord(string text)
    {
        string normalizedInput = NormalizeText(text);
        if (string.IsNullOrEmpty(normalizedInput)) return false;
        foreach (string word in _reader.WordData)
        {
            if (string.IsNullOrWhiteSpace(word)) continue;
            string normalizedWord = NormalizeText(word);
            if (string.IsNullOrEmpty(normalizedWord) || normalizedWord.Length < 2) continue;
            if (normalizedInput.Contains(normalizedWord)) return true;
        }
        return false;
    }

    private string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return Regex.Replace(text, @"[^a-zA-Z가-힣]", "").ToLower();
    }

    // private IEnumerator PrintWarningText(string prompt)
    // {
    //     if (_warningText == null) yield break;
    //     _warningText.text = prompt;
    //     yield return new WaitForSeconds(2.0f);
    //     _warningText.text = "";
    //     _warningCoroutine = null;
    // }

    private IEnumerator PrintWarningTextPopup(TextMeshProUGUI targetText, string prompt)
    {
        if (targetText == null) yield break;
        targetText.text = prompt;
        targetText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        targetText.text = "";
        _warningCoroutine = null;
    }

    private bool IsValidInput(string inputText, TextMeshProUGUI targetWarningText)
    {
        string t = null;
        if (string.IsNullOrWhiteSpace(inputText))
            t = "공백인 필드가 존재합니다!";

        else if (!IsValidNameLength(inputText))
            t = "한글 1자 이상, 또는 영어 2자 이상으로 구성해주세요!";

        else if (CheckBadWord(inputText))
            t = "비속어가 포함되어 있습니다!";
        
        if (t != null)
        {
            if (GameManager.Instance.SaveData == null)
            {
                _warningTextObj.SetActive(true);
                targetWarningText.text = t;
            }
            else
            {
                if (_warningCoroutine == null)
                    _warningCoroutine = StartCoroutine(PrintWarningTextPopup(targetWarningText, t));
            }
            
            return false;
        }

        return true;
    }

    public void RefreshProfileList()
    {
        // 1. 기존 아이콘 풀 회수 (Destroy 대신 사용)
        foreach (var icon in _activeIcons) _iconPool.Release(icon);
        _activeIcons.Clear();

        foreach (var page in _activePages) _pagePool.Release(page);
        _activePages.Clear();

        if (_pageWindow == null) return;        

        int iconsPerPage = 15;
        int totalPage = (_profileDataReader.DataList.Count + iconsPerPage - 1) / iconsPerPage;

        //필요한 페이지 일괄 생성
        for (int p = 0; p < totalPage; p++)
        {
            GameObject newPage = _pagePool.Get();
            newPage.name = $"Page_{p + 1}";
                      
            newPage.SetActive(p == _currentPageIndex);
            _activePages.Add(newPage);
        }

        for (int i = 0; i < _profileDataReader.DataList.Count; i++)
        {
            int pageIndex = i / iconsPerPage;

            GameObject currentPage = _activePages[pageIndex];
            if (pageIndex == _currentPageIndex) currentPage.SetActive(true);

            ProfileIcon icon = _iconPool.Get();
            if (icon.transform.parent != currentPage.transform)
            {
                icon.transform.SetParent(currentPage.transform, false);
            }
            

            ProfileData data = _profileDataReader.DataList[i];
            icon.Init(data);

            int gradCount = GameManager.Instance.GetGraduationCount(data.visualId);
            bool isUnlocked = data.isDefault || gradCount >= data.unlockCount;

            if (isUnlocked)
            {
                icon.Unlock();
                bool isSelected = _selectedData.HasValue && data.playerImage == _selectedData.Value.playerImage;
                icon.OnOffOutLine(isSelected);
                if (isSelected) _selectedIcon = icon;

                icon.GetButton().onClick.RemoveAllListeners();
                icon.GetButton().onClick.AddListener(() => OnSelectProfile(icon));
            }
            else
            {
                icon.OnOffOutLine(false);
                icon.GetButton().interactable = false;
            }

            _activeIcons.Add(icon);
        }

        UpdatePageButtons();
    }

    private void OnSelectProfile(ProfileIcon icon)
    {
        _selectedIcon?.OnOffOutLine(false);
        _selectedIcon = icon;
        _selectedData = icon.Data;
        _selectedIcon.OnOffOutLine(true);
    }
}