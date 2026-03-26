using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject _child;
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

    private int _index;
    [SerializeField] private List<TutorialData> _currentTutorialData;

    // private int reward;

    private void Awake()
    {
        _index = 0;
        _currentTutorialData = null;
    }

    private void Start()
    {
        if (_nextButton != null)
            _nextButton.onClick.AddListener(OnClickNextButton);

        if (_skipButton != null)
            _skipButton.onClick.AddListener(OnClickSkipButton);

        if (_startButton != null)
            _startButton.onClick.AddListener(OnClickStartButton);

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnStartTutorial += StartTutorialUI;

        // reward = InfraManager.Instance.GetCostListByEffectType(infraEffectType.TrainingBonus)[1];
    }

    private void OnDestroy()
    {
        if (_nextButton != null)
            _nextButton.onClick.RemoveListener(OnClickNextButton);

        if (_skipButton != null)
            _skipButton.onClick.RemoveListener(OnClickSkipButton);

        if (_startButton != null)
            _startButton.onClick.RemoveListener(OnClickStartButton);

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnStartTutorial -= StartTutorialUI;
    }

    public void StartTutorialUI(string id)
    {
        if (_child == null) return;

        var tutorialMgr = TutorialManager.Instance;
        if (tutorialMgr == null) return;

        var dataList = tutorialMgr.GetData(id);
        if (dataList == null || dataList.Count == 0) return;

        _child.SetActive(true);

        _currentTutorialData = dataList;
        _index = 0;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_currentTutorialData == null) return;
        if (_currentTutorialData.Count == 0) return;
        if (_index < 0 || _index >= _currentTutorialData.Count) return;

        var data = _currentTutorialData[_index];

        var sMgr = StringManager.Instance;
        var spriteMgr = SpriteManager.Instance;

        if (sMgr == null || spriteMgr == null) return;

        if (_backgroundImage != null)
            _backgroundImage.sprite = spriteMgr.GetSprite(data.tutorialImageId);

        if (_narraitionText != null)
            _narraitionText.text = sMgr.GetString(data.narrationKey);

        if (_nameText != null)
            _nameText.text = sMgr.GetString(data.speakerKey);

        if (_dialogueText != null)
            _dialogueText.text = sMgr.GetString(data.dialogueKey);

        if (_pageText != null)
            _pageText.text = $"{_index + 1}/{_currentTutorialData.Count}";

        bool isLastPage = _index >= _currentTutorialData.Count - 1;

        if (isLastPage)
        {
            int idx = int.Parse(data.tutorialId.Substring(data.tutorialId.Length - 2)) - 1;
            Debug.Log(idx);
            GameManager.Instance.SetTutorialCompleted(idx, true);
            _child.SetActive(false);
        }
    }

    private void OnClickNextButton()
    {
        if (_currentTutorialData == null) return;
        if (_currentTutorialData.Count == 0) return;

        if (_index < _currentTutorialData.Count - 1)
        {
            _index++;
            RefreshUI();
        }
    }

    private void OnClickSkipButton()
    {
        if (_skipPanelObj == null) return;

        _skipPanelObj.SetActive(true);

        var tmp = _skipPanelObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = "튜토리얼을 스킵 하시겠습니까?";
    }

    private void OnClickStartButton()
    {
        if (_endPanelObj == null) return;

        _endPanelObj.SetActive(true);

        var tmp = _endPanelObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = "튜토리얼 종료";
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