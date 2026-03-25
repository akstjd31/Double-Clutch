using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialManager _tutorialMgr;
    [SerializeField] private TextMeshProUGUI _narraitionText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _pageText;
    private int index;

    private void Awake()
    {
        index = 0;
    }

    private void Start()
    {
        if (_tutorialMgr == null) return;

        var data = _tutorialMgr.GetData(index);
        if (data == null) return;

        Init(data.Value);
    }

    private void Init(TutorialData data)
    {
        // 임시 (나중에 해당 리소스, 스트링 테이블 채워지면 그떄 반영할 예정)
        _narraitionText.text = data.narrationKey;
        _nameText.text = data.speakerKey;
        _dialogueText.text = data.dialogueKey;

        _pageText.text = $"{index+1}/{_tutorialMgr.GetDataListLength()}";
    }
}
