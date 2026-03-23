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
    [SerializeField] private GameObject[] _childAlbumPanelObjs = new GameObject[3];
    private void OnEnable()
    {
        if (_albumPanelObj == null) return;
        for (int i = 0; i < _albumPanelObj.transform.childCount; i++)
        {
            _childAlbumPanelObjs[i] = _albumPanelObj.transform.GetChild(i).gameObject;
        }

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
        
    }
}
