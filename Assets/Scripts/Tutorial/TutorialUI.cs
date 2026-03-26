using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
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
        OnClickNextButton();

        if (_nextButton == null) return;
        _nextButton.onClick.AddListener(OnClickNextButton);

        if (_skipButton == null) return;
        _skipButton.onClick.AddListener(OnClickSkipButton);

        // 보상 저장
        reward = InfraManager.Instance.GetCostListByEffectType(infraEffectType.TrainingBonus)[1];
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
        var tutorialMgr = TutorialManager.Instance;
        if (tutorialMgr == null) return;

        var data = tutorialMgr.GetData(index);
        if (data == null) return;

        var sMgr = StringManager.Instance;
        if (sMgr == null) return;

        _backgroundImage.sprite = SpriteManager.Instance.GetSprite(data.Value.tutorialImageId);

        _narraitionText.text = sMgr.GetString(data.Value.narrationKey);
        _nameText.text = sMgr.GetString(data.Value.speakerKey);
        _dialogueText.text = sMgr.GetString(data.Value.dialogueKey);

        _pageText.text = $"{index + 1}/{tutorialMgr.GetDataListLength()}";
        index++;

        // 마지막 슬라이드라면
        if (tutorialMgr.GetDataListLength() <= index)
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
        tmp.text = $"튜토리얼을 스킵 하시겠습니까?"; //\n튜토리얼 지원금 : {reward} G";
    }

    private void OnClickStartButton()
    {
        _endPanelObj.SetActive(true);

        var tmp = _endPanelObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tmp.text = $"튜토리얼 종료"; //\n튜토리얼 지원금 : {reward} G";
    }

    public void OnClickConfirmButton()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // 지원금 지급 후 로비 이동
        // gm.SetMoney(reward);
        // gm.SetTutorialCompleted(true);
        // gm.GoToLobby();
    }
}
