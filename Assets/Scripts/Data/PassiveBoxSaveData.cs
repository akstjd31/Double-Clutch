using UnityEngine;
using System.Collections.Generic;
using System;

public class PassiveBoxSaveData : SaveBase
{
    public List<PassiveBoxSkillEntry> entries = new List<PassiveBoxSkillEntry>();
}

[Serializable]
public class PassiveBoxSkillEntry
{
    public int studentKey;                                  // 해당 학생
    public List<string> passiveIds = new List<string>();    // 선택된 3개의 패시브 ID 리스트
    public string selectedPassiveId;                        // 선택된 패시브 ID
}
