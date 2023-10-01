using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public int timePerWave = 30;

    [Header("Info")]
    public int wavesCompleted = 0;

    LevelManager levelManager;
    GUIManager guiManager;

    void Awake() => Instance = this;

    void Start() {
        levelManager = LevelManager.Instance;
        guiManager = GUIManager.Instance;
        levelManager.GenerateLevel();
    }

    void Update()
    {
        float timeRemaining = timePerWave - Time.time % timePerWave;
        guiManager.SetClock(timeRemaining, timePerWave);
        if ((int)Time.time / timePerWave <= wavesCompleted) return;
        levelManager.ShrinkFarm();
        wavesCompleted++;
    }
}