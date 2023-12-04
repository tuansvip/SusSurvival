using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    public RectTransform rect1;
    Item[] items;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        rect1.localScale = Vector3.zero;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);

    }
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        rect1.localScale = Vector3.one;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }
    public void Select(int index)
    {
        items[index].OnClick();
    }
    void Next()
    {
        foreach(Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length - 1);
            ran[2] = Random.Range(0, items.Length - 1);
            ran[1] = Random.Range(0, items.Length - 1);
            if (ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2]) break;
        }
        for (int i = 0; i < ran.Length; i++)
        {
            Item ranItem = items[ran[i]];
            if(ranItem.level == ranItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);

            }
        }
    }
}
