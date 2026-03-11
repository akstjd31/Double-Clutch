using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// 1 ~ 13

public class ProfileUI : MonoBehaviour
{
    // 문자의 최소, 최대 길이
    private const int NAME_MIN = 1;
    private const int NAME_MAX = 13;

    [SerializeField] private BannedWordDataReader _reader;
    [SerializeField] private TMP_InputField _schoolNameField;
    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private GameObject _warningTextObj;
    private TextMeshProUGUI _warningText;
    [SerializeField] private Button _schoolSelectButton;
    [SerializeField] private Button _playerSelectButton;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private bool _isFirstTime;
    private Coroutine _warningCoroutine;

    private void OnEnable()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnClickConfirmButton);

        if (!_isFirstTime)
        {
            _schoolNameField.text = "";
            _playerNameField.text = "";

            if (_warningText == null)
                _warningText = _warningTextObj.transform.GetComponentInChildren<TextMeshProUGUI>();
            _warningText.text = "";

            _schoolNameField.interactable = false;
            _playerNameField.interactable = false;

            if (_schoolSelectButton == null) return;
            if (_playerSelectButton == null) return;

            _schoolSelectButton.onClick.AddListener(delegate { OnClickFocusInput(_schoolNameField); });
            _playerSelectButton.onClick.AddListener(delegate { OnClickFocusInput(_playerNameField); });
        }
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveAllListeners();

        if (!_isFirstTime)
        {
            _schoolSelectButton.onClick.RemoveAllListeners();
            _playerSelectButton.onClick.RemoveAllListeners();
        }
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

        if (!CheckLength(_schoolNameField.text) || !CheckLength(_playerNameField.text))
        {
            if (_warningCoroutine != null) return;
            _warningCoroutine = StartCoroutine(PrintWarningText("문자의 최소/최대 길이는 1/13입니다!"));
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
            gm.SetSchoolName(_schoolNameField.text);
            gm.SetCoachName(_playerNameField.text);
        }

        this.gameObject.SetActive(false);
    }

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

    private string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        // 공백, 특수문자, 숫자 제거
        string normalized = Regex.Replace(text, @"[^a-zA-Z가-힣]", "");

        return normalized.ToLower();
    }

    // 버튼누르면 인풋 필드 포커싱
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

    private IEnumerator PrintWarningText(string prompt)
    {
        if (_warningText == null) yield break;
        _warningText.text = prompt;

        yield return new WaitForSeconds(2.0f);
        _warningText.text = "";
        _warningCoroutine = null;
    }

    private bool CheckLength(string text)
    {
        int len = text.Trim().Length;
        return len >= NAME_MIN && len <= NAME_MAX;
    }
}
