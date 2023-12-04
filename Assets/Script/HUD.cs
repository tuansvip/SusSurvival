using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health, HighestKill, State, HighestState }
    public InfoType type;

    Text myText;
    Slider mySlider;
    private void Awake()
    {
        myText= GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float currExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = currExp / maxExp;
                break;
            case InfoType.Level:
                myText.text =string.Format("Lv.{0:F0}",GameManager.instance.level);

                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.HighestKill:
                myText.text = string.Format("Best: {0:F0}", GameManager.instance.highestScore);
                break;
            case InfoType.State:
                myText.text = string.Format("State: {0:F0}", GameManager.instance.gameLevel);
                break;
            case InfoType.HighestState:
                myText.text = string.Format("Best: {0:F0}", GameManager.instance.highestLevel);
                break;

            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:F0}:{1:D2}", min, sec);

                break;
            case InfoType.Health:
                float currHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = currHealth / maxHealth;
                break;
        }
    }
}
