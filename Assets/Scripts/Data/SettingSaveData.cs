using System;
using UnityEngine;

[Serializable]
public class SettingSaveData : SaveBase
{
    public int fps = 60;
    public bool isVibOn = true;
    public float masterVol = 0.8f;
    public bool isMasterVolOn = true;
    public float bgmVol = 0.8f;
    public bool isBGMVolOn = true;
    public float sfxVol = 0.8f;
    public bool isSFXVolOn = true;
}
