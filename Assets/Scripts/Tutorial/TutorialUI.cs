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
    private int _reward;
    [SerializeField] private List<TutorialData> _currentTutorialData;

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

        _reward = InfraManager.Instance.GetCostListByEffectType(infraEffectType.TrainingBonus)[1];

        if (CalendarManager.Instance == null) return;

        var gm = GameManager.Instance;
        if (gm == null) return;

        var data = gm.SaveData;
        var tId = CalendarManager.Instance.GetTutorialId(data.weekId - 1);
        if (tId == null) return;

        int idx = int.Parse(tId[tId.Length - 1].ToString()) - 1;
        if (!data.tutorialCompleted[idx])
        {
            TutorialManager.Instance.StartTutorial(tId);
        }
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

        _skipPanelObj.SetActive(false);
        _endPanelObj.SetActive(false);

        _currentTutorialData = dataList;

        _index = 0;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_currentTutorialData == null) return;
        if (_currentTutorialData.Count == 0) return;

        var data = _currentTutorialData[_index];

        var sMgr = StringManager.Instance;
        var spriteMgr = SpriteManager.Instance;
        var gm = GameManager.Instance;

        if (sMgr == null || spriteMgr == null || gm == null) return;

        if (_backgroundImage != null)
            _backgroundImage.sprite = spriteMgr.GetSprite(data.tutorialImageId);

        if (_narraitionText != null)
            _narraitionText.text = sMgr.GetString(data.narrationKey);

        if (_nameText != null)
            _nameText.text = sMgr.GetString(data.speakerKey);

        if (_dialogueText != null)
        {
            string dialogue = sMgr.GetString(data.dialogueKey);

            // 만약 중괄호가 있을 시 포맷팅
            var keys = TextParser.GetKeys(dialogue);

            string result = dialogue;

            foreach (var key in keys)
            {
                string value = key switch
                {
                    "coachName" => gm.SaveData.coachName,
                    "schoolName" => gm.SaveData.schoolName,
                    _ => ""
                };

                result = result.Replace("{" + key + "}", value);
            }

            _dialogueText.text = result;
        }

        if (_pageText != null)
            _pageText.text = $"{_index + 1}/{_currentTutorialData.Count}";
    }

    private void OnClickNextButton()
    {
        if (_currentTutorialData == null) return;
        if (_currentTutorialData.Count == 0) return;

        if (_index >= _currentTutorialData.Count - 1)
        {
            TutorialEnd();
            return;
        }

        _index++;
        RefreshUI();

    }

    private void TutorialEnd()
    {
        string tId = _currentTutorialData[_currentTutorialData.Count - 1].tutorialId;
        int idx = int.Parse(tId.Substring(tId.Length - 2)) - 1;

        var gm = GameManager.Instance;
        if (gm == null) return;
        
        gm.SetTutorialCompleted(idx, true);
        if (idx >= gm.SaveData.tutorialCompleted.Length - 1)
            gm.SetMoney(gm.SaveData.money + _reward);

        _child.SetActive(false);
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

        TutorialEnd();
    }
}