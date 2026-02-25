using System.Collections.Generic;
using UnityEngine;

public class WeeklyTrainingReportPopUp : MonoBehaviour
{
    [SerializeField] GameObject _characterRowPrefab;
    public void Init(List<Student> students)
    {
        for (int i = 0; i < students.Count; i++)
        {
            GameObject row = Instantiate(_characterRowPrefab);
            row.GetComponent<CharacterRow>().Init(students[i]);
        }
    }
}
