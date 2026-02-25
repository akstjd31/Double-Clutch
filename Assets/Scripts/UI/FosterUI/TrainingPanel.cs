using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 육성 버튼을 누르면 나오는 TrainingPanel에 부착.
/// 선수 목록을 가져와 선수 수만큼 버튼 생성하는 역할
/// </summary>

public class TrainingPanel : MonoBehaviour
{
    [SerializeField] GameObject _playerBoxPrefab;
    [SerializeField] Transform _playerBoxParent;
    [SerializeField] GameObject _backButtonObj;

    List<GameObject> boxList = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            GameObject newBox = Instantiate(_playerBoxPrefab, _playerBoxParent);
            var tBox = newBox.GetComponent<TrainingCharacterBox>();

            if (tBox != null)
            {
                tBox.Init(StudentManager.Instance.MyStudents[i]);
                var btn = tBox.GetSelectButton();

                btn.onClick.AddListener(delegate { _backButtonObj.SetActive(false); }); //누르면 뒤로가기 비활성화 처리
            }

            boxList.Add(newBox);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < boxList.Count; i++)
        {
            Destroy(boxList[i]);
        }
    }
}
