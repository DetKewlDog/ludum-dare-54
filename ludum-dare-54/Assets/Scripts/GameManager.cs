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
    CameraController cameraController;
    PlayerController player;

    void Awake() => Instance = this;

    void Start() {
        Time.timeScale = 0;
        levelManager = LevelManager.Instance;
        guiManager = GUIManager.Instance;
        cameraController = CameraController.Instance;
        player = PlayerController.Instance;
        levelManager.GenerateLevel();
        guiManager.ToggleWaveEndScreen(false, () => {
            Time.timeScale = 1;
            cameraController.target = player.transform;
        });
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        float timeRemaining = timePerWave - Time.time % timePerWave;
        guiManager.SetClock(timeRemaining, timePerWave);
        if ((int)Time.time / timePerWave <= wavesCompleted) return;
        StartCoroutine(EndWaveCo());
    }

    IEnumerator EndWaveCo() {
        Time.timeScale = 0;
        wavesCompleted++;
        cameraController.target = transform;
        guiManager.SetClock(0, timePerWave);
        guiManager.ToggleAlarmClock(true);
        levelManager.SellAllCrops();
        yield return new WaitForSecondsRealtime(levelManager.sellDuration * levelManager.farmSize.magnitude);
        levelManager.ShrinkFarm();
        yield return new WaitForSecondsRealtime(1.5f);
        guiManager.ToggleWaveEndScreen(true, () => { });
    }

    public void StartNewWave() {
        guiManager.ToggleAlarmClock(false);
        guiManager.SetClock(timePerWave, timePerWave);
        guiManager.ToggleWaveEndScreen(false, () => {
            Time.timeScale = 1;
            cameraController.target = player.transform;
        });
    }

    public void BuyLand() {
        if (MoneyAmount < 200) return;
        MoneyAmount -= 200;
        levelManager.EnlargeFarm();
    }
}