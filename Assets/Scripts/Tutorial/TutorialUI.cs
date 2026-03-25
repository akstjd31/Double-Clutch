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
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _skipPanelObj;
    [SerializeField] private GameObject _endPanelObj;
    private int index;
    private int reward;

    private void Awake()
    {
        index = 0;
    }

    private void Start()
    {
        if (_tutorialMgr == null) return;
        OnClickNextButton();

        if (_nextButton == null) return;
        _nextButton.onClick.AddListener(OnClickNextButton);

        if (_skipButton == null) return;
        _skipButton.onClick.AddListener(OnClickSkipButton);

        // 보상 저장
        reward = InfraManager.Instance.GetCostListByEffectType(infraEffectType.TrainingBonus)[0];
    }

    private void OnDestroy()
    {
        if (_nextButton == null) return;
        _nextButton.onClick.RemoveAllListeners();

        if (_skipButton == null) return;
        _skipButton.onClick.RemoveAllListeners();

        if (_startButton == null) return;
        _startButton.onClick.RemoveAllListeners();
    }

    private void OnClickNextButton()
    {
        var data = _tutorialMgr.GetData(index);
        if (data == null) return;

        // 임시 (나중에 해당 리소스, 스트링 테이블 채워지면 그떄 반영할 예정)
        _narraitionText.text = data.Value.narrationKey;
        _nameText.text = data.Value.speakerKey;
        _dialogueText.text = data.Value.dialogueKey;

        _pageText.text = $"{index + 1}/{_tutorialMgr.GetDataListLength()}";
        index++;

        // 마지막 슬라이드라면
        if (_tutorialMgr.GetDataListLength() <= index)
        {
            _skipButton.gameObject.SetActive(false);

            if (_startButton == null) return;
            _startButton.gameObject.SetActive(true);
            _startButton.onClick.AddListener(OnClickStartButton);
        }
    }

    private void OnClickSkipButton()
    {
        _skipPanelObj.SetActive(true);

        var tmp = _skipPanelObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        tmp.text = $"튜토리얼을 스킵 하시겠습니까?\n튜토리얼 지원금 : {reward} G";
    }

    private void OnClickStartButton()
    {
        _endPanelObj.SetActive(true);
    }

    public void OnClickConfirmButton()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // 지원금 지급 후 로비 이동
        gm.SetMoney(reward);
        gm.SetTutorialCompleted(true);
        gm.GoToLobby();
    }
}
