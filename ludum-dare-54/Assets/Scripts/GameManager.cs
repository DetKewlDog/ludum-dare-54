using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isMainMenu = false;

    [Header("Settings")]
    public int timePerRound = 30;
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

    private int roundsCompleted = 0;

    LevelManager levelManager;
    GUIManager guiManager;
    StatsManager statsManager;
    CameraController cameraController;
    PlayerController player;

    void Awake() => Instance = this;

    void Start() {
        Time.timeScale = isMainMenu ? 1 : 0;
        levelManager = LevelManager.Instance;
        guiManager = GUIManager.Instance;
        statsManager = StatsManager.Instance;
        cameraController = CameraController.Instance;
        player = PlayerController.Instance;

        levelManager.GenerateLevel();

        if (!isMainMenu) {
            MoneyAmount = moneyAmount;
            CropAmount = cropAmount;
            statsManager.RoundsCompleted = statsManager.TotalEarnings = statsManager.CropsPlanted = 0;
            guiManager.SetLand(levelManager.farmSize);
        }

        guiManager.ToggleRoundEndScreen(false, () => {
            Time.timeScale = 1;
            if (isMainMenu) return;
            cameraController.target = player?.transform;
        });
    }

    void Update()
    {
        if (isMainMenu) return;
        if (Time.timeScale == 0) return;
        float timeRemaining = timePerRound - Time.time % timePerRound;
        guiManager.SetClock(timeRemaining, timePerRound);
        if ((int)Time.time / timePerRound <= roundsCompleted) return;
        StartCoroutine(EndRoundCo());
    }

    IEnumerator EndRoundCo() {
        Time.timeScale = 0;
        statsManager.TotalPlayTime = Time.unscaledTime;

        player.ResetAnimator();
        cameraController.target = transform;

        guiManager.SetClock(0, timePerRound);
        guiManager.ToggleAlarmClock(true);

        yield return new WaitForSecondsRealtime(1.5f);
        levelManager.SellAllCrops();

        yield return new WaitForSecondsRealtime(levelManager.sellDuration * levelManager.farmSize.magnitude);

        levelManager.ShrinkFarm();
        guiManager.SetLand(levelManager.farmSize);

        yield return new WaitForSecondsRealtime(1.5f);

        guiManager.ToggleClock(false);
        guiManager.ToggleRoundEndScreen(true, () => { }, levelManager.placedCrops.Count == 0);
    }

    public void StartNewRound() {
        roundsCompleted++;
        statsManager.RoundsCompleted++;

        player.transform.position = Vector2.zero;
        levelManager.UpdateFence();

        guiManager.ToggleAlarmClock(false);
        guiManager.SetClock(timePerRound, timePerRound);
        guiManager.ToggleClock(true);

        guiManager.ToggleRoundEndScreen(false, () => {
            Time.timeScale = 1;
            cameraController.target = player.transform;
        });
    }

    public void BuyLand() {
        if (MoneyAmount < landPrice) return;
        MoneyAmount -= landPrice;
        levelManager.EnlargeFarm();
        guiManager.SetLand(levelManager.farmSize);
        SoundManager.Play("buy");
    }

    public void BuyCrop() {
        if (MoneyAmount < cropPrice) return;
        MoneyAmount -= cropPrice;
        CropAmount += cropsSold;
        SoundManager.Play("buy");
    }
}