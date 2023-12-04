using UnityEngine;
using System.IO;
using System;


public class PlayerData
{
    public int gameLevel;
    public int kill;
    public int highestKill;
    public int highestLevel;

    public PlayerData(int gameLevel, int highestLevel, int kill, int highestKill)
    {
        this.gameLevel = gameLevel;
        this.kill = kill;
        this.highestKill = highestKill;
        this.highestLevel = highestLevel;
    }
}
