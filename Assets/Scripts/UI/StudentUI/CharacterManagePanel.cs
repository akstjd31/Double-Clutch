using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManagePanel : MonoBehaviour
{
    [SerializeField] GameObject _characterBoxPrefab;
    [SerializeField] Transform _characterBoxParent;
    [SerializeField] GameObject _backButtonObj;

    List<GameObject> boxList = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < StudentManager.Instance.MyStudents.Count; i++)
        {
            GameObject newBox = Instantiate(_characterBoxPrefab, _characterBoxParent);
            var cBox = newBox.GetComponent<CharacterBox>();

            if (cBox != null)
            {
                cBox.Init(StudentManager.Instance.MyStudents[i]);
                var btn = cBox.GetSelectButton();

                btn.onClick.AddListener(delegate { _backButtonObj.SetActive(false); });
            }
        
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
