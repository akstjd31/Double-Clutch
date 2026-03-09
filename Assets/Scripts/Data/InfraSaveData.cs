using System;
using System.Collections.Generic;

[Serializable]
public class InfraSaveData : SaveBase
{
    public List<Infra> infraList = new List<Infra>();
}