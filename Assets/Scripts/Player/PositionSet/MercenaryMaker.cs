using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MercenaryMaker : MonoBehaviour
{
    [SerializeField] Mercenary_DataReader _mercenaryDB;
    [SerializeField] Player_VisualDataReader _visualDB;

    public Student MakeMercenary(Position position)
    {
        Mercenary_Data data = FindData(position);
        Student robot = new Student();
        robot.SetName(StringManager.Instance.GetString(data.mercName), "", "");
        robot.SetPosition(position);
        robot.SetMatchPosition(position);
        robot.SetStat(MakeMercenaryStats(data));
        robot.ChangeCondition(70);        
        robot.SetVisual(GetMercenaryVisual(data.playerImageResource));

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
        newStat.Add(new Stat(potential.StatPass, data.statPassValue, data.statPassValue, 1));
        newStat.Add(new Stat(potential.StatBlock, data.statBlockValue, data.statBlockValue, 1));
        newStat.Add(new Stat(potential.StatSteal, data.statStealValue, data.statStealValue, 1));
        newStat.Add(new Stat(potential.StatRebound, data.statReboundValue, data.statReboundValue, 1));

        return newStat;
    }

    private Player_VisualData GetMercenaryVisual(string visualId)
    {
        Player_VisualData data =  _visualDB.DataList.Find(v => v.visualId == visualId);

        return data;
    }
}
