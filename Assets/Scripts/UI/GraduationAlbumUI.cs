using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Game.Constants;

/// <summary>
/// 졸업 앨범 UI
/// </summary>
public class GraduationAlbumUI : MonoBehaviour
{
    [SerializeField] private Transform _classParent;        // 기수 버튼 부모 (Content)
    [SerializeField] private Transform _classButtonPrefab;  // 기수 버튼
    [SerializeField] private GameObject _albumPanelObj;
    [SerializeField] private GameObject _charProfilePopupObj;
    [SerializeField] private AlbumStudentProfileUI[] _albumStudentProfiles;
    private void OnEnable()
    {
        if (_albumPanelObj == null) return;
        _albumStudentProfiles = _albumPanelObj.GetComponentsInChildren<AlbumStudentProfileUI>(true);

        if (_classParent == null) return;
        if (_classButtonPrefab == null) return;

        var gaMgr = GraduationAlbumManager.Instance;
        if (gaMgr == null) return;

        var data = gaMgr.SaveData;
        if (data == null) return;

        var gList = data.graduationStudentList;
        if (gList == null) return;

        for (int i = 0; i < gList.Count; i++)
        {
            // 이미 생성된 데이터 간주
            if (i < _classParent.childCount) continue;

            var newObj = Instantiate(_classButtonPrefab, _classParent);
            newObj.GetComponentInChildren<TextMeshProUGUI>().text = StringManager.Instance.GetFormattedString("Str_졸업앨범_기", i + 1);
            StringManager.Instance.ApplyFont(newObj.GetComponentInChildren<TextMeshProUGUI>());

            int index = i;
            var btn = newObj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickClassButton(index + 1));
        }
        StringManager.OnLanguageChanged += UpdateLanguageTexts;

        UpdateLanguageTexts();
    }

    private void OnDisable()
    {
        _albumPanelObj.SetActive(false);

        // ⭐ 언어 변경 이벤트 구독 해제 (메모리 누수 방지)
        StringManager.OnLanguageChanged -= UpdateLanguageTexts;
    }

    private void UpdateLanguageTexts()
    {
        if (_classParent == null) return;

        // _classParent 아래에 생성된 모든 기수 버튼을 순회합니다.
        for (int i = 0; i < _classParent.childCount; i++)
        {
            var textComp = _classParent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                // 인덱스는 0부터 시작하므로 i + 1이 기수가 됩니다.
                textComp.text = StringManager.Instance.GetFormattedString("Str_졸업앨범_기", i + 1);
                StringManager.Instance.ApplyFont(textComp);
            }
        }

        // (선택 사항) 만약 앨범 우측 패널(_albumPanelObj)이 켜져 있는 상태에서 언어가 바뀔 수 있다면,
        // 현재 열려있는 학생들의 이름도 여기서 다시 갱신해주는 로직을 추가하면 좋습니다.
    }
    private void OnDestroy()
    {
        // 기수 버튼 리스너 제거
        if (_classParent.childCount < 1) return;

        for (int i = 0; i < _classParent.childCount; i++)
        {
            var btn = _classParent.GetChild(i).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
        }

        // 앨범 프로필 버튼 리스너 제거
        if (_albumStudentProfiles == null) return;
        for (int i = 0; i < _albumStudentProfiles.Length; i++)
        {
            _albumStudentProfiles[i].GetButton().onClick.RemoveAllListeners();
        }
    }

    public void OnClickClassButton(int idx)
    {
        if (_albumPanelObj == null) return;
        if (_albumStudentProfiles == null) return;
        if (GraduationAlbumManager.Instance == null) return;

        _albumPanelObj.SetActive(true);

        var gList = GraduationAlbumManager.Instance.GetGraduationStudentListByIndex(idx);
        if (gList == null) return;

        if (SpriteManager.Instance == null) return;
        if (StringManager.Instance == null) return;

        int i = 0;
        for (; i < gList.studentList.Count; i++)
        {
            var sprite = SpriteManager.Instance.GetSprite(gList.studentList[i].VisualData.portraitResource);

            string name = StringManager.Instance.GetString(gList.studentList[i].Name[0]) +
                            StringManager.Instance.GetString(gList.studentList[i].Name[1]) +
                            StringManager.Instance.GetString(gList.studentList[i].Name[2]);

            _albumStudentProfiles[i].SetSprite(sprite);
            _albumStudentProfiles[i].SetName(name);

            if (_charProfilePopupObj != null)
            {
                int index = i;
                _albumStudentProfiles[index].GetButton().onClick.RemoveAllListeners();
                _albumStudentProfiles[index].GetButton().onClick.AddListener(() => OnClickProfileBox(gList.studentList[index]));
            }
        }

        // 남은 빈 프로필 처리
        for (; i < _albumStudentProfiles.Length; i++)
        {
            _albumStudentProfiles[i].SetName("");
        }
    }

    public void OnClickProfileBox(Student std)
    {
        _charProfilePopupObj.SetActive(true);
        var profile = _charProfilePopupObj.GetComponent<CharacterProfilePopUp>();

        profile.Init(std);
    }
}
