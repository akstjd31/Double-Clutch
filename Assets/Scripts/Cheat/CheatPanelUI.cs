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
    [SerializeField] private GameObject _tabAreaObj;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _confirmButton;

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
    }

    public void OnClickButton(CheatButton cheatBtn)
    {
        _currentCheatButton = cheatBtn;

        string title = cheatBtn switch
        {
            CheatButton.Money => "보유 지원금 설정",
            CheatButton.Fame => "보유 명성 설정",
            CheatButton.Student => "보유 선수 설정",
            _ => ""
        };

        _titleText.text = title;

        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(() => OnClickConfirmButton(_currentCheatButton));
    }

    public void OnClickConfirmButton(CheatButton cheatBtn)
    {
        switch (cheatBtn)
        {
            case CheatButton.Money:
                Debug.Log("지원금 설정");
                break;
            case CheatButton.Fame:
                Debug.Log("명성 설정");
                break;
            case CheatButton.Student:
                Debug.Log("선수 설정");
                break;
        }
    }
}