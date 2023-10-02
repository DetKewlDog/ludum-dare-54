using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int timePerWave = 30;
    public int landPrice = 100;
    public int cropPrice = 50;
    public int cropsSold = 100;

    [Header("Start Values")]

    [SerializeField] private int moneyAmount = 0;
    public int MoneyAmount {
        get => moneyAmount;
        set {
            moneyAmount = value;
            guiManager?.SetMoney(value);
        }
    }
    [SerializeField] private int cropAmount = 100;
    public int CropAmount {
        get => cropAmount;
        set {
            cropAmount = value;
            guiManager?.SetCrop(value);
        }
    }

    private int wavesCompleted = 0;

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

        MoneyAmount = moneyAmount;
        CropAmount = cropAmount;

        levelManager.GenerateLevel();
        guiManager.SetLand(levelManager.farmSize);
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
        guiManager.SetLand(levelManager.farmSize);
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
        if (MoneyAmount < landPrice) return;
        MoneyAmount -= landPrice;
        levelManager.EnlargeFarm();
        guiManager.SetLand(levelManager.farmSize);
    }

    public void BuyCrop() {
        if (MoneyAmount < cropPrice) return;
        MoneyAmount -= cropPrice;
        CropAmount += cropsSold;
    }
}