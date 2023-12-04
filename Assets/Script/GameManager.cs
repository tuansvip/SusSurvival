using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 5 * 10f;
    public bool isLive;
    public int gameLevel;
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = {3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    public int highestLevel;
    public int highestScore;
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;
    public Transform uiJoy;

    public string savePath;
    private void Awake()
    {
        instance = this;
        savePath = Application.persistentDataPath + "/playerData.json";
        Application.targetFrameRate = 144;
    }
    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);

        //load data
        
        PlayerData playerData = LoadPlayerData();
        gameLevel = playerData.gameLevel;
        highestLevel = playerData.highestLevel;
        kill = playerData.kill;
        highestScore = playerData.highestKill;
        //

        uiLevelUp.Select(playerId % 2);
        Resume();
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

    }
    //save and load data
    public PlayerData LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found, creating a new one!");
            string json = JsonUtility.ToJson(new PlayerData(1, 0, 0, 0));
            File.WriteAllText(savePath, json);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
    public void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }
    //


    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive = false;
        if (highestLevel < gameLevel) highestLevel = gameLevel;
        if (highestScore < kill) highestScore = kill;
        gameLevel = 1;
        kill = 0;

        //savedata
        PlayerData playerData = new PlayerData(gameLevel, highestLevel, kill, highestScore);
        SavePlayerData(playerData);
        //

        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();

        Stop();

        AudioManager.instance.PlayBgm(false);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);

    }
    public void GameWin()
    {
        StartCoroutine(GameWinRoutine());
    }
    IEnumerator GameWinRoutine()
    {
        isLive = false;
        gameLevel++;
        if (highestLevel < gameLevel) highestLevel = gameLevel;
        if (highestScore < kill) highestScore = kill;

        //savedata
        PlayerData playerData = new PlayerData(gameLevel, highestLevel, kill, highestScore);
        SavePlayerData(playerData);
        //

        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        Stop();
        AudioManager.instance.PlayBgm(false);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);

    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
    private void Update()
    {
        if (!isLive)
        {
            return;
        }
        gameTime += Time.deltaTime;

        //update gametime
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameWin();
        }

    }

    public void GetExp()
    {
        if (!isLive) return;
        exp++;
        if (exp == nextExp[Mathf.Min(level,nextExp.Length - 1) ])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;

    }

}
