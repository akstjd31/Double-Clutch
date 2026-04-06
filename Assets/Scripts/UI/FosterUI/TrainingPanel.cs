using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Constants;

/// <summary>
/// ���� ��ư�� ������ ������ TrainingPanel�� ����.
/// ���� ����� ������ ���� ����ŭ ��ư �����ϴ� ����
/// </summary>

public class TrainingPanel : MonoBehaviour
{
    [SerializeField] private GameObject _backButton;
    [SerializeField] private GameObject _homeButton;
    [SerializeField] TrainingCharacterBox _trainingCharacterBoxPrefab;
    [SerializeField] Transform _trainingCharacterBoxParent;

    [Header("패널이 켜지면 비활성화해야 하는 버튼 목록")]
    [SerializeField] Button[] _unavailableButtons;

    GenericObjectPool<TrainingCharacterBox> _trainingBoxPool; //Ʈ���̴� �ڽ� ������Ʈ Ǯ

    List<TrainingCharacterBox> _boxList = new List<TrainingCharacterBox>(); //������Ʈ �ݳ��� ���� ����Ʈ

    private void Awake()
    {
        _trainingBoxPool = new GenericObjectPool<TrainingCharacterBox>(_trainingCharacterBoxPrefab, _trainingCharacterBoxParent, 8, 20);
    }

    private void OnEnable()
    {
        foreach(var button in _unavailableButtons)
        {
            button.interactable = false;
        }
        
        RefreshPlayerList();
        PlaySound(SoundName.BGM_DEVELOP_01);
    }

    private void PlaySound(string id)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(id);
    }

    private void OnDisable()
    {
        foreach (var button in _unavailableButtons)
        {
            button.interactable = true;
        }
        if (_boxList == null) return;
        PlaySound(SoundName.BGM_LOBBY_01);
    }

    public void RefreshPlayerList()
    {        
        for (int i = 0; i < _boxList.Count; i++)
        {
            _trainingBoxPool.Release(_boxList[i]);
        }

        _boxList.Clear();
        
        var students = StudentManager.Instance.MyStudents;// ������ ���� ����ŭ Ǯ���� �����ͼ� ���� (Ǯ�� ������ ���� ������Ʈ Ǯ�� �ڵ� ����)
        for (int i = 0; i < students.Count; i++)
        {            
            TrainingCharacterBox newBox = _trainingBoxPool.Get(); //�ڽ� ä���
            newBox.transform.SetAsLastSibling();
            newBox.Init(students[i]); //�ڽ��� ���� ���� ����   

            var btn = newBox.GetComponent<Button>();
            btn.onClick.AddListener(delegate
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);

                OnClickBackAndHomeButtonSetActive(false);
            });
            
            _boxList.Add(newBox);
        }
    }

    public void RefreshPositionMark()
    {
        foreach(var box in _boxList)
        {
            box.RefreshPositionMark();
        }
    }

    public void RefreshAllBoxesState()
    {
        for (int i = 0; i < _boxList.Count; i++)
        {
            if (_boxList[i] != null)
            {
                _boxList[i].SetStudentState();
            }
        }
    }
    
    public void OnClickBackAndHomeButtonSetActive(bool active)
    {
        _backButton.SetActive(active);
        _homeButton.SetActive(active);
    }
}
