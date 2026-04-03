using UnityEngine;
using System.Collections.Generic;
using Game.Constants;

public class PassiveSkillSelectPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private PromotionPanel _promotionPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private GameObject _guideBox;
    [SerializeField] private GameObject _afterGuideBox;
    [SerializeField] private GameObject _warningBox;

    private PromotionProgressSaveData _progressSaveData;

    private void Start()
    {
        LoadPromotionProgress();
    }

    public void OnClickOKButton()
    {
        PlayConfirmSound();

        if (_graduationManager == null ||
            _graduationManager.PromotionStudentList == null ||
            _graduationManager.MyStudents == null)
            return;

        if (_graduationManager.Turn < 0 ||
            _graduationManager.Turn >= _graduationManager.PromotionStudentList.Count)
            return;

        if (_graduationManager.PromotionPanel.IsSkillChoise == false)
        {
            _warningBox.SetActive(true);
            return;
        }

        int currentStudentId = _graduationManager.PromotionStudentList[_graduationManager.Turn];
        Student student = FindStudentById(currentStudentId);

        if (student == null)
        {
            Debug.LogWarning($"Student not found. studentId = {currentStudentId}");
            return;
        }

        if (_passiveBox.SelectSkill == null)
        {
            Debug.LogWarning("선택된 스킬이 없습니다.");
            _warningBox.SetActive(true);
            return;
        }

        gameObject.SetActive(false);

        // 선택한 스킬 추가
        student.SetPassive(_passiveBox.SelectSkill.Value);

        // 현재 학생 프로필 반영
        _graduationManager.PromotionPanel.UpdateProfile();

        // 다음 학생으로 진행
        _graduationManager.Turn++;
        SavePromotionProgress();

        if (_graduationManager.Turn < _graduationManager.PromotionStudentList.Count)
        {
            int nextStudentId = _graduationManager.PromotionStudentList[_graduationManager.Turn];
            Student nextStudent = FindStudentById(nextStudentId);

            if (nextStudent != null)
            {
                Debug.Log($"다음 순서: {nextStudent.Name} 학생");
            }

            _afterGuideBox.SetActive(true);
        }
        else
        {
            // 진급 대상 전부 완료
            ClearPromotionProgress();
            _afterGuideBox.SetActive(true);
        }

        _graduationManager.PromotionPanel.IsSkillChoise = false;
    }

    private Student FindStudentById(int studentId)
    {
        for (int i = 0; i < _graduationManager.MyStudents.Count; i++)
        {
            if (_graduationManager.MyStudents[i].StudentId == studentId)
                return _graduationManager.MyStudents[i];
        }

        return null;
    }

    private void PlayConfirmSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
    }

    private void LoadPromotionProgress()
    {
        if (!SaveLoadManager.Instance.TryLoad(FilePath.PROMOTION_PROGRESS_SAVE_PATH, out _progressSaveData) || _progressSaveData == null)
        {
            _progressSaveData = new PromotionProgressSaveData();
        }
    }

    private void SavePromotionProgress()
    {
        if (_progressSaveData == null)
            _progressSaveData = new PromotionProgressSaveData();

        _progressSaveData.currentTurn = _graduationManager.Turn;
        _progressSaveData.isPromotionFinished =
            _graduationManager.PromotionStudentList != null &&
            _graduationManager.Turn >= _graduationManager.PromotionStudentList.Count;

        SaveLoadManager.Instance.Save(FilePath.PROMOTION_PROGRESS_SAVE_PATH, _progressSaveData);
    }

    private void ClearPromotionProgress()
    {
        _progressSaveData = new PromotionProgressSaveData();
        SaveLoadManager.Instance.Save(FilePath.PROMOTION_PROGRESS_SAVE_PATH, _progressSaveData);
    }
}