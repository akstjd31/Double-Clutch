using UnityEngine;
using UnityEngine.UI;

public enum CheatDetails
{
    Stat = 0, Passive, Trait
}

public class DetailCheatUI : MonoBehaviour
{
    [SerializeField] private Student _student;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private GameObject[] _statDetailsPanelObjs;

    private void OnEnable()
    {
        if (_buttons == null) return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            _buttons[index].onClick.RemoveAllListeners();
            _buttons[index].onClick.AddListener(() => OnClickDetailButton((CheatDetails)index));
        }
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
    }

    public void OnClickDetailButton(CheatDetails detail)
    {
        switch (detail)
        {
            case CheatDetails.Stat:
                _statDetailsPanelObjs[(int)detail].SetActive(true);
                
                if (_statDetailsPanelObjs[(int)detail].TryGetComponent<StatCheatUI>(out var statCheat))
                {
                    statCheat.SetStudent(_student);
                }
                break;
            case CheatDetails.Passive:
                break;
            case CheatDetails.Trait:
                break;
        }

        this.gameObject.SetActive(false);
    }

    public void SetStudent(Student std)
    {
        _student = std;
    }
}
