using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProfileUI : MonoBehaviour
{
    private const int NAME_MAX = 13;

    [SerializeField] private BannedWordDataReader _reader;
    [SerializeField] private TMP_InputField _schoolNameField;
    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private Button _schoolSelectButton;
    [SerializeField] private Button _playerSelectButton;
    [SerializeField] private GameObject _warningTextObj;
    private TextMeshProUGUI _warningText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private bool _isFirstTime;
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
    [SerializeField] private Image _currentLobbyProfileImage;
    [SerializeField] private Image _currentIcon;
    [SerializeField] private ProfileIcon _profilePrefab;
    [SerializeField] private GameObject _pagePanelPrefab;
    [SerializeField] private Transform _pageWindow;
    [SerializeField] private ProfileDataReader _profileDataReader;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _profileBoxImage;

    private GenericObjectPool<ProfileIcon> _pool;
    private List<ProfileIcon> _activeIcons = new List<ProfileIcon>();
    private List<GameObject> _pages = new List<GameObject>(); // 페이지 추적용 리스트
    private int _currentPageIndex = 0; // 페이지 상태 관리를 위한 변수 추가

    [Header("Selected Info")]
    private ProfileIcon _selectedIcon;
    private ProfileData? _selectedData;

    private void Awake()
    {
        _pool = new GenericObjectPool<ProfileIcon>(_profilePrefab, this.transform);
    }

    private void Start()
    {        
        RefreshProfileList();
        if (GameManager.Instance.SaveData == null || string.IsNullOrEmpty(GameManager.Instance.SaveData.currentProfileImage))
        {
            if (_currentLobbyProfileImage != null)
            _currentLobbyProfileImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage);
            if (_profileBoxImage != null)
                _profileBoxImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage); 
        }
        else
        {
            _currentLobbyProfileImage.sprite = SpriteManager.Instance.GetSprite(GameManager.Instance.SaveData.currentProfileImage);
            _profileBoxImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage);
        }
            
    }

    private void OnEnable()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnClickConfirmButton);

        // 페이지 버튼 이벤트 연결 추가
        if (_prevButton != null) _prevButton.onClick.AddListener(() => ChangePage(-1));
        if (_nextButton != null) _nextButton.onClick.AddListener(() => ChangePage(1));

        if (!_isFirstTime)
        {
            GameManager gameManager = GameManager.Instance;
            SpriteManager spriteManager = SpriteManager.Instance;

            
            string currentImg = gameManager.SaveData?.currentProfileImage;

            if (_selectedData == null && _profileDataReader.DataList.Count > 0)
                _selectedData = _profileDataReader.DataList[0];

            _selectedData = _profileDataReader.DataList.Find(x => x.playerImage == currentImg);
            if (_selectedData == null) _selectedData = _profileDataReader.DataList[0];

            _currentIcon.sprite = string.IsNullOrEmpty(gameManager.SaveData.currentProfileImage) ?
                spriteManager.GetSprite(_profileDataReader.DataList[0].playerImage) : spriteManager.GetSprite(gameManager.SaveData.currentProfileImage);

            if (_warningText == null)
                _warningText = _warningTextObj.transform.GetComponentInChildren<TextMeshProUGUI>();
            _warningText.text = "";

            _schoolNameField.text = GameManager.Instance.SaveData.schoolName;
            _playerNameField.text = GameManager.Instance.SaveData.coachName;
            _schoolNameField.interactable = false;
            _playerNameField.interactable = false;

            if (_schoolSelectButton != null && _playerSelectButton != null)
            {
                _schoolSelectButton.onClick.AddListener(() => OpenPopup(_schoolModifyPanel, _schoolPopupInputField));
                _playerSelectButton.onClick.AddListener(() => OpenPopup(_coachModifyPanel, _coachPopupInputField));
            }

            if (_confirmSchoolButton != null) _confirmSchoolButton.onClick.AddListener(OnClickConfirmSchool);
            if (_confirmCoachButton != null) _confirmCoachButton.onClick.AddListener(OnClickConfirmCoach);
            if (_cancelSchoolButton != null) _cancelSchoolButton.onClick.AddListener(() => _schoolModifyPanel.SetActive(false));
            if (_cancelCoachButton != null) _cancelCoachButton.onClick.AddListener(() => _coachModifyPanel.SetActive(false));

            RefreshProfileList();            
        }        
    }

    private void OnDisable()
    {
        if (_confirmButton != null) _confirmButton.onClick.RemoveAllListeners();

        // 페이지 버튼 리스너 해제 추가
        if (_prevButton != null) _prevButton.onClick.RemoveAllListeners();
        if (_nextButton != null) _nextButton.onClick.RemoveAllListeners();

        if (!_isFirstTime)
        {
            if (_schoolSelectButton != null) _schoolSelectButton.onClick.RemoveAllListeners();
            if (_playerSelectButton != null) _playerSelectButton.onClick.RemoveAllListeners();
            if (_confirmSchoolButton != null) _confirmSchoolButton.onClick.RemoveAllListeners();
            if (_confirmCoachButton != null) _confirmCoachButton.onClick.RemoveAllListeners();
            if (_cancelSchoolButton != null) _cancelSchoolButton.onClick.RemoveAllListeners();
            if (_cancelCoachButton != null) _cancelCoachButton.onClick.RemoveAllListeners();
        }
    }

    // 페이지 전환 함수 추가
    private void ChangePage(int direction)
    {
        int nextIndex = _currentPageIndex + direction;
        if (nextIndex < 0 || nextIndex >= _pages.Count) return;

        _pages[_currentPageIndex].SetActive(false);
        _currentPageIndex = nextIndex;
        _pages[_currentPageIndex].SetActive(true);

        UpdatePageButtons();
    }

    // 버튼 활성화/비활성화 함수 추가
    private void UpdatePageButtons()
    {
        if (_prevButton != null) _prevButton.interactable = (_currentPageIndex > 0);
        if (_nextButton != null) _nextButton.interactable = (_currentPageIndex < _pages.Count - 1);
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
        if (_schoolNameField.text == "" || _playerNameField.text == "")
        {
            if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningText("공백의 이름이 존재합니다!"));
            return;
        }

        if (GameManager.Instance == null || _reader == null) return;

        if (!IsValidNameLength(_schoolNameField.text) || !IsValidNameLength(_playerNameField.text))
        {
            if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningText("한글 1자 이상, 영어 2자 이상으로 구성되게 작성해주세요!"));
            return;
        }

        if (CheckBadWord(_schoolNameField.text) || CheckBadWord(_playerNameField.text))
        {
            if (_isFirstTime) { if (_warningTextObj != null) _warningTextObj.SetActive(true); }
            else { if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningText("비속어가 포함되어 있습니다!")); }
            return;
        }

        var gm = GameManager.Instance;
        if (_isFirstTime)
        {
            var data = new PlayerSaveData { schoolName = _schoolNameField.text, coachName = _playerNameField.text, weekId = 9, year = 0 };
            gm.InitData(data);
            CalendarManager.Instance.CalcWeek(data.weekId, gm);
            gm.Dispatch(UIAction.Main_Start);
        }

        if (_selectedData.HasValue && _selectedData.Value.playerImage != null)
        {
            gm.SetCurrentProfileIcon(_selectedData.Value.playerImage);
            _currentLobbyProfileImage.sprite = _currentIcon.sprite;
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

    private IEnumerator PrintWarningText(string prompt)
    {
        if (_warningText == null) yield break;
        _warningText.text = prompt;
        yield return new WaitForSeconds(2.0f);
        _warningText.text = "";
        _warningCoroutine = null;
    }

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
        if (string.IsNullOrEmpty(inputText))
        {
            if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningTextPopup(targetWarningText, "공백인 이름이 존재합니다."));
            return false;
        }
        if (!IsValidNameLength(inputText))
        {
            if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningTextPopup(targetWarningText, "한글 1자 이상, 영어 2자 이상으로 구성해주세요!"));
            return false;
        }
        if (CheckBadWord(inputText))
        {
            if (_isFirstTime && _warningTextObj != null) _warningTextObj.SetActive(true);
            else if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningTextPopup(targetWarningText, "비속어가 포함되어 있습니다!"));
            return false;
        }
        return true;
    }

    public void RefreshProfileList()
    {        
        foreach (var icon in _activeIcons) _pool.Release(icon);
        _activeIcons.Clear();

        if (_pageWindow == null)
        {
            return;
        }
        if (GameManager.Instance.SaveData == null || string.IsNullOrEmpty(GameManager.Instance.SaveData.currentProfileImage))
        {
            _profileBoxImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage);
        }
        else
        {
            _profileBoxImage.sprite = SpriteManager.Instance.GetSprite(_profileDataReader.DataList[0].playerImage);
        }

            foreach (Transform child in _pageWindow) Destroy(child.gameObject);
        _pages.Clear(); // 페이지 리스트 초기화
        _currentPageIndex = 0; // 초기화 시 인덱스 리셋 추가

        GameObject currentPage = null;

        for (int i = 0; i < _profileDataReader.DataList.Count; i++)
        {
            if (i % 15 == 0)
            {
                currentPage = Instantiate(_pagePanelPrefab, _pageWindow);
                currentPage.name = $"Page_{_pages.Count + 1}";

                // 첫 번째 페이지만 활성화, 나머지는 비활성화
                if (_pages.Count > 0) currentPage.SetActive(false);

                _pages.Add(currentPage);
            }

            ProfileIcon icon = _pool.Get();
            icon.transform.SetParent(currentPage.transform, false);

            ProfileData data = _profileDataReader.DataList[i];
            icon.Init(data);

            int gradCount = GameManager.Instance.GetGraduationCount(data.visualId);
            bool isUnlocked = data.isDefault || gradCount >= data.unlockCount;

            if (isUnlocked)
            {
                icon.Unlock();
                if (_selectedData.HasValue && data.playerImage == _selectedData.Value.playerImage)
                {
                    icon.OnOffOutLine(true);
                    _selectedIcon = icon;
                }
                else icon.OnOffOutLine(false);

                icon.GetButton().onClick.RemoveAllListeners();
                icon.GetButton().onClick.AddListener(() => OnSelectProfile(icon));
            }
            else
            {
                icon.GetButton().interactable = false;
                // 필요 시 아이콘 실루엣/잠금 처리 추가 가능
            }

            _activeIcons.Add(icon);
        }

        UpdatePageButtons(); // 리스트 생성 후 버튼 상태 업데이트 추가
    }

    private void OnSelectProfile(ProfileIcon icon)
    {
        _selectedIcon?.OnOffOutLine(false);
        _selectedIcon = icon;
        _selectedData = icon.Data;        
        _selectedIcon.OnOffOutLine(true);
    }
}