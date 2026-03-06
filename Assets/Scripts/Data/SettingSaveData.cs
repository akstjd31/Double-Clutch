using System;
using UnityEngine;

[Serializable]
public class SettingSaveData : SaveBase
{
    public int fps;
    public bool isVibOn;
    public float masterVol;
    public bool isMasterVolOn;
    public float bgmVol;
    public bool isBGMVolOn;
    public float sfxVol;
    public bool isSFXVolOn;
}
