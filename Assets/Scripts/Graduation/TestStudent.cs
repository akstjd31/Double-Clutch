using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestStudent
{
    [SerializeField] public int ID;
    [SerializeField] public string Name;
    [SerializeField] public int Grade;
    [SerializeField] public int Honor;

    [SerializeField] public List<string> PassiveId = new List<string>();

    public TestStudent(int id, string name, int grade, int honer)
    {
        ID = id;
        Name = name;
        Grade = grade; 
        Honor = honer;
        PassiveId.Add("스킬1");
    }
}
