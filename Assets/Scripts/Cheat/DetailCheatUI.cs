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
    [SerializeField] private GameObject[] _detailsPanelObjs;

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
        for (int i = 0; i < _detailsPanelObjs.Length; i++)
            _detailsPanelObjs[i].SetActive((int)detail == i);

        switch (detail)
        {
            case CheatDetails.Stat:
                if (_detailsPanelObjs[(int)detail].TryGetComponent<StatCheatUI>(out var statCheat))
                    statCheat.SetStudent(_student);

                break;
            case CheatDetails.Passive:
                if (_detailsPanelObjs[(int)detail].TryGetComponent<PassiveCheatUI>(out var passiveCheat))
                    passiveCheat.SetStudent(_student);
                
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
