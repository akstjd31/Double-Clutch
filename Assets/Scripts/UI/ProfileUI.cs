using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// 1 ~ 13

public class ProfileUI : MonoBehaviour
{
    // 문자의 최대 길이
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
    [SerializeField] private GameObject _schoolModifyPanel;       // 팝업 패널
    [SerializeField] private TMP_InputField _schoolPopupInputField; // 팝업 안의 입력 칸
    [SerializeField] private Button _confirmSchoolButton;         // 확인 버튼
    [SerializeField] private Button _cancelSchoolButton;          // 취소 버튼
    [SerializeField] private TextMeshProUGUI _schoolWarningText;  // 팝업 안의 경고 텍스트

    [Header("감독 수정 팝업")]
    [SerializeField] private GameObject _coachModifyPanel;        // 팝업 패널
    [SerializeField] private TMP_InputField _coachPopupInputField;  // 팝업 안의 입력 칸
    [SerializeField] private Button _confirmCoachButton;          // 확인 버튼
    [SerializeField] private Button _cancelCoachButton;           // 취소 버튼
    [SerializeField] private TextMeshProUGUI _coachWarningText;   // 팝업 안의 경고 텍스트

    private void OnEnable()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnClickConfirmButton);

        if (!_isFirstTime)
        {

            if (_warningText == null)
                _warningText = _warningTextObj.transform.GetComponentInChildren<TextMeshProUGUI>();
            _warningText.text = "";

            // 패널이 열릴 때 GameManager에서 현재 이름을 불러와 메인 화면에 띄워줍니다.
            _schoolNameField.text = GameManager.Instance.SaveData.schoolName;
            _playerNameField.text = GameManager.Instance.SaveData.coachName;

            _schoolNameField.interactable = false;
            _playerNameField.interactable = false;

            if (_schoolSelectButton == null) return;
            if (_playerSelectButton == null) return;

            // 팝업을 열고, 팝업의 인력칸을 세팅해줍니다.
            _schoolSelectButton.onClick.AddListener(() => OpenPopup(_schoolModifyPanel, _schoolPopupInputField));
            _playerSelectButton.onClick.AddListener(() => OpenPopup(_coachModifyPanel, _coachPopupInputField));

            // 팝업창 버튼들 이벤트 연결
            if (_confirmSchoolButton != null) _confirmSchoolButton.onClick.AddListener(OnClickConfirmSchool);
            if (_confirmCoachButton != null) _confirmCoachButton.onClick.AddListener(OnClickConfirmCoach);

            if (_cancelSchoolButton != null) _cancelSchoolButton.onClick.AddListener(() => _schoolModifyPanel.SetActive(false));
            if (_cancelCoachButton != null) _cancelCoachButton.onClick.AddListener(() => _coachModifyPanel.SetActive(false));

            //_schoolSelectButton.onClick.AddListener(delegate { OnClickFocusInput(_schoolNameField); });
            //_playerSelectButton.onClick.AddListener(delegate { OnClickFocusInput(_playerNameField); });
        }
    }

    private void OnDisable()
    {
        //_confirmButton.onClick.RemoveAllListeners();
        //
        //if (!_isFirstTime)
        //{
        //    _schoolSelectButton.onClick.RemoveAllListeners();
        //    _playerSelectButton.onClick.RemoveAllListeners();
        //}
        if (_confirmButton != null) _confirmButton.onClick.RemoveAllListeners();

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

    // 팝업 열기 헬퍼 함수
    private void OpenPopup(GameObject popupPanel, TMP_InputField popupInputField)
    {
        popupPanel.SetActive(true);
        popupInputField.text = ""; // 팝업이 열릴 때 빈칸
        popupInputField.Select(); // 열리자마자 바로 키보드 타이핑 가능하게
    }

    // 학교 이름 팝업 확인 버튼 눌렀을 때
    public void OnClickConfirmSchool()
    {
        // 팝업창의 텍스트를 검사
        if (!IsValidInput(_schoolPopupInputField.text, _schoolWarningText)) return;

        // GameManager에 새 이름 저장
        GameManager.Instance.SetSchoolName(_schoolPopupInputField.text);

        // 메인 화면 텍스트 즉시 갱신
        _schoolNameField.text = _schoolPopupInputField.text;

        _schoolModifyPanel.SetActive(false);
    }

    // 감독 이름 팝업 확인 버튼 눌렀을 때
    public void OnClickConfirmCoach()
    {
        // 팝업창의 텍스트를 검사
        if (!IsValidInput(_coachPopupInputField.text, _coachWarningText)) return;

        // GameManager에 새 이름 저장
        GameManager.Instance.SetCoachName(_coachPopupInputField.text);

        // 메인 화면 텍스트 즉시 갱신
        _playerNameField.text = _coachPopupInputField.text;

        _coachModifyPanel.SetActive(false);
    }

    public void OnClickConfirmButton()
    {
        if (_schoolNameField.text == "" || _playerNameField.text == "")
        {
            if (_warningCoroutine != null) return;
            _warningCoroutine = StartCoroutine(PrintWarningText("공백의 이름이 존재합니다!"));
            return;
        }

        if (GameManager.Instance == null) return;
        if (_reader == null) return;

        if (!IsValidNameLength(_schoolNameField.text) || !IsValidNameLength(_playerNameField.text))
        {
            if (_warningCoroutine != null) return;
            _warningCoroutine = StartCoroutine(PrintWarningText("한글 1자 이상, 영어 2자 이상으로 구성되게 작성해주세요!"));
            return;
        }

        // 비속어가 포함될 경우 팝업 띄우면서 다시 요청
        if (CheckBadWord(_schoolNameField.text) || CheckBadWord(_playerNameField.text))
        {
            if (_isFirstTime)
            {
                if (_warningTextObj == null) return;
                _warningTextObj.SetActive(true);
            }
            else
            {
                if (_warningCoroutine != null) return;
                _warningCoroutine = StartCoroutine(PrintWarningText("비속어가 포함되어 있습니다!"));
            }

            return;
        }

        var gm = GameManager.Instance;

        // 튜토리얼일때만 데이터 초기값 주기
        if (_isFirstTime)
        {
            var data = new PlayerSaveData();

            data.schoolName = _schoolNameField.text;
            data.coachName = _playerNameField.text;

            data.weekId = 9;
            data.year = 0;

            gm.InitData(data);

            CalendarManager.Instance.CalcWeek(data.weekId, gm);

            GameManager.Instance.Dispatch(UIAction.Main_Start);
        }

        // 로비 씬에서 프로필 눌렀을 때
        else
        {
            //gm.SetSchoolName(_schoolNameField.text);
            //gm.SetCoachName(_playerNameField.text);
        }

        this.gameObject.SetActive(false);
    }


    // 한글, 영문 최소 글자 수에 적합하는지?
    private bool IsValidNameLength(string text)
    {
        string normalized = NormalizeText(text);

        if (string.IsNullOrEmpty(normalized))
            return false;

        bool hasKorean = Regex.IsMatch(normalized, @"[가-힣]");
        bool hasEnglish = Regex.IsMatch(normalized, @"[a-zA-Z]");

        // 영문이 하나라도 들어가면 최소 2글자
        if (hasEnglish)
            return normalized.Length >= 2 && normalized.Length <= NAME_MAX;

        // 한글만 있으면 최소 1글자
        if (hasKorean)
            return normalized.Length >= 1 && normalized.Length <= NAME_MAX;

        return false;
    }

    // 정규화 후 금칙어 테이블에 속하는지 확인
    private bool CheckBadWord(string text)
    {
        string normalizedInput = NormalizeText(text);

        if (string.IsNullOrEmpty(normalizedInput))
            return false;

        foreach (string word in _reader.WordData)
        {
            if (string.IsNullOrWhiteSpace(word))
                continue;

            string normalizedWord = NormalizeText(word);

            if (string.IsNullOrEmpty(normalizedWord))
                continue;

            if (normalizedWord.Length < 2)
                continue;

            if (normalizedInput.Contains(normalizedWord))
                return true;
        }

        return false;
    }

    // 공백, 특수문자, 숫자 제거 후 반환
    private string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        string normalized = Regex.Replace(text, @"[^a-zA-Z가-힣]", "");

        return normalized.ToLower();
    }

    // 버튼누르면 인풋 필드 포커싱
    /*
    public void OnClickFocusInput(TMP_InputField inputField)
    {
        inputField.interactable = true;

        inputField.Select();
        inputField.ActivateInputField();

        if (inputField.Equals(_schoolNameField))
            _playerNameField.interactable = false;
        else
            _schoolNameField.interactable = false;
    }
    */
    // 위험 문구 출력
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
            if (_isFirstTime && _warningTextObj != null)
            {
                _warningTextObj.SetActive(true);
            }
            else
            {
                if (_warningCoroutine == null) _warningCoroutine = StartCoroutine(PrintWarningTextPopup(targetWarningText, "비속어가 포함되어 있습니다!"));
            }
            return false;
        }

        return true;
    }
}
