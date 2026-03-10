using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private BannedWordDataReader _reader;
    [SerializeField] private TMP_InputField _schoolNameField;
    [SerializeField] private TMP_InputField _playerNameField;
    [SerializeField] private GameObject _wrongTextPanelObj;
    [SerializeField] private Button _confirmButton;

    private void Start()
    {
        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnClickConfirmButton);
    }
    public void OnClickConfirmButton()
    {
        if (_schoolNameField.text == "" || _playerNameField.text == "") return;
        if (GameManager.Instance == null) return;
        if (_reader == null) return;

        // 비속어가 포함될 경우 팝업 띄우면서 다시 요청
        if (CheckBadWord(_schoolNameField.text) || CheckBadWord(_playerNameField.text))
        {
            if (_wrongTextPanelObj == null) return;

            _wrongTextPanelObj.SetActive(true);
            return;
        } 

        var gm = GameManager.Instance;
        var data = new PlayerSaveData();

        data.schoolName = _schoolNameField.text;
        data.coachName = _playerNameField.text;
        data.weekId = 9;
        data.year = 0;

        gm.InitData(data);
    
        CalendarManager.Instance.CalcWeek(data.weekId, gm);

        GameManager.Instance.Dispatch(UIAction.Main_Start); 
        this.gameObject.SetActive(false);
    }

    private bool CheckBadWord(string text)
    {
        foreach (string word in _reader.WordData)
        {
            if (text.Contains(word))
                return true;
        }

        return false;
    }
}
