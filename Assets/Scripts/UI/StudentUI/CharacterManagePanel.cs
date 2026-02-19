using System.Collections.Generic;
using UnityEngine;

public class CharacterManagePanel : MonoBehaviour
{
    [SerializeField] GameObject _characterBoxPrefab;
    [SerializeField] Transform _characterBoxParent;

    List<GameObject> boxList = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            GameObject newBox = Instantiate(_characterBoxPrefab, _characterBoxParent);
            newBox.GetComponent<CharacterBox>().Init(StudentManager.Instance.MyStudents[i]);
            boxList.Add(newBox);
        }
    }

    private void OnDestroy()
    {
        for(int i = 0;i < boxList.Count; i++)
        {
            Destroy(boxList[i]);
        }
    }
}
