using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MercenaryMaker : MonoBehaviour
{
    [SerializeField] Mercenary_DataReader _mercenaryDB;

    public Student MakeMercenary(Position position)
    {
        Mercenary_Data data = FindData(position);
        Student robot = new Student();
        robot.SetName(StringManager.Instance.GetString(data.mercName));
        robot.SetPosition(position);
        robot.SetStat(MakeMercenaryStats(data));
        robot.ChangeCondition(70);

        return robot;
    }

    private Mercenary_Data FindData(Position position)
    {
        return _mercenaryDB.DataList.FirstOrDefault(d => d.positionType == position);
    }

    public List<Stat> MakeMercenaryStats(Mercenary_Data data)
    {
        List<Stat> newStat = new List<Stat>();

        newStat.Add(new Stat(potential.Stat2pt, data.stat2ptValue, data.stat2ptValue, 1));
        newStat.Add(new Stat(potential.Stat3pt, data.stat3ptValue, data.stat3ptValue, 1));
        newStat.Add(new Stat(potential.StatPass, data.statAssistValue, data.statAssistValue, 1));
        newStat.Add(new Stat(potential.StatBlock, data.statBlockValue, data.statBlockValue, 1));
        newStat.Add(new Stat(potential.StatSteal, data.statStealValue, data.statStealValue, 1));
        newStat.Add(new Stat(potential.StatRebound, data.statReboundValue, data.statReboundValue, 1));

        return newStat;
    }

}
