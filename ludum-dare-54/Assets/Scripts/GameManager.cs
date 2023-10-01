using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int timePerWave = 30;

    [Header("Info")]
    public int wavesCompleted = 0;

    [SerializeField] private int moneyAmount = 0;
    public int MoneyAmount {
        get => moneyAmount;
        set {
            moneyAmount = value;
            guiManager?.SetMoney(value);
        }
    }


    LevelManager levelManager;
    GUIManager guiManager;

    void Awake() => Instance = this;

    void Start() {
        Time.timeScale = 0;
        levelManager = LevelManager.Instance;
        guiManager = GUIManager.Instance;
        levelManager.GenerateLevel();
        guiManager.ToggleWaveEndScreen(false, () => Time.timeScale = 1);
    }

    void Update()
    {
        float timeRemaining = timePerWave - Time.time % timePerWave;
        guiManager.SetClock(timeRemaining, timePerWave);
        if ((int)Time.time / timePerWave <= wavesCompleted) return;
        StartCoroutine(EndWaveCo());
    }

    IEnumerator EndWaveCo() {
        Time.timeScale = 0;
        wavesCompleted++;
        guiManager.SetClock(timePerWave, timePerWave);
        guiManager.ToggleClock(false);
        levelManager.SellAllCrops();
        yield return new WaitForSecondsRealtime(levelManager.sellDuration * levelManager.farmSize.magnitude);
        levelManager.ShrinkFarm();
        yield return new WaitForSecondsRealtime(1.5f);
        guiManager.ToggleWaveEndScreen(true, () => StartCoroutine(WaveEndCo()));
    }

    IEnumerator WaveEndCo() {
        guiManager.ToggleClock(true);
        yield return new WaitForSecondsRealtime(5);
        guiManager.ToggleWaveEndScreen(false, () => Time.timeScale = 1);
    }
}