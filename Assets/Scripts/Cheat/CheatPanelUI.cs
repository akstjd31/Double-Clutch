using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CheatButton
{
    Money = 0,
    Fame = 1,
    Student = 2
}

public class CheatPanelUI : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private GameObject _tabAreaObj;
    [SerializeField] private GameObject _inputAreaObj;
    [SerializeField] private GameObject _studentAreaObj;

    [SerializeField] private Button[] _buttons;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    private CheatButton _currentCheatButton;

    private void OnEnable()
    {
        if (_tabAreaObj == null) return;
        if (_confirmButton == null) return;

        _buttons = _tabAreaObj.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            _buttons[index].onClick.RemoveAllListeners();
            _buttons[index].onClick.AddListener(() => OnClickButton((CheatButton)index));
        }

        _backButton.onClick.AddListener(OnClickBackButton);
        _confirmButton.onClick.RemoveAllListeners();
    }

    private void OnDisable()
    {
        if (_buttons != null)
        {
            foreach (var btn in _buttons)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        if (_confirmButton != null)
            _confirmButton.onClick.RemoveAllListeners();

        if (_backButton != null)
            _backButton.onClick.RemoveAllListeners();
    }

    public void OnClickButton(CheatButton cheatBtn)
    {
        if (cheatBtn.Equals(CheatButton.Money) || cheatBtn.Equals(CheatButton.Fame))
        {
            _inputAreaObj.SetActive(true);
            _studentAreaObj.SetActive(false);
        }
        else
        {
            _studentAreaObj.SetActive(true);
            _inputAreaObj.SetActive(false);
        }

        _currentCheatButton = cheatBtn;

        string title = cheatBtn switch
        {
            CheatButton.Money => "보유 지원금 설정",
            CheatButton.Fame => "보유 명성 설정",
            CheatButton.Student => "보유 선수 설정",
            _ => ""
        };

        _titleText.text = title;
        _inputField.text = "";

        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() => OnClickConfirmButton(_currentCheatButton));
    }

    public void OnClickBackButton()
    {
        _inputField.text = "";
        _inputAreaObj.SetActive(false);

        this.gameObject.SetActive(false);
    }

    public void OnClickConfirmButton(CheatButton cheatBtn)
    {
        if (string.IsNullOrWhiteSpace(_inputField.text)) return;
        if (!int.TryParse(_inputField.text, out var num)) return;
        
        var gameMgr = GameManager.Instance;
        if (gameMgr == null) return;

        switch (cheatBtn)
        {
            case CheatButton.Money:
                gameMgr.SetMoney(num);
                Debug.Log($"지원금 설정 완료! 현재 보유 지원금: {gameMgr.SaveData.money}");
                break;
            case CheatButton.Fame:
                gameMgr.SetHonor(num);
                Debug.Log($"명성 설정 완료! 현재 보유 명성: {gameMgr.SaveData.honor}");
                break;
            case CheatButton.Student:
                Debug.Log("선수 설정");
                break;
        }
    }
}