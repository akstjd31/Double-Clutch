using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 졸업 앨범 UI
/// </summary>
public class GraduationAlbumUI : MonoBehaviour
{
    [SerializeField] private Transform _classParent;        // 기수 버튼 부모 (Content)
    [SerializeField] private Transform _classButtonPrefab;  // 기수 버튼
    [SerializeField] private GameObject _albumPanelObj;
    private AlbumStudentProfileUI[] _albumStudentProfiles;
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
            var newObj = Instantiate(_classButtonPrefab, _classParent);
            newObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{i+1}기";

            int index = i;
            newObj.GetComponent<Button>().onClick.AddListener(() => OnClickClassButton(index));
        }
    }

    public void OnClickClassButton(int idx)
    {
        if (_albumStudentProfiles[idx] == null) return;
        if (GraduationAlbumManager.Instance == null) return;

        var gList = GraduationAlbumManager.Instance.GetGraduationStudentListByIndex(idx);

        for (int i = 0; i < _albumStudentProfiles.Length; i++)
        {
            bool hasData = gList.studentList != null;
            _albumStudentProfiles[i].gameObject.SetActive(hasData);
            if (hasData)
            {
                if (SpriteManager.Instance == null) return;
                var sprite = SpriteManager.Instance.GetSprite(gList.studentList[i].VisualData.portraitResource);
                _albumStudentProfiles[i].GetComponent<Image>().sprite = sprite;

                string name = gList.studentList[i].Name[0] + gList.studentList[i].Name[1] + gList.studentList[i].Name[2];
                _albumStudentProfiles[i].GetComponentInChildren<TextMeshProUGUI>().text = name;
            }
        }
    }
}
