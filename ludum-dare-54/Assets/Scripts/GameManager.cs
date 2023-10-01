using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    LevelManager levelManager;
    GUIManager guiManager;
    int resizeCount = 1;

    void Awake() => Instance = this;

    void Start() {
        levelManager = LevelManager.Instance;
        guiManager = GUIManager.Instance;
        levelManager.GenerateLevel();
    }

    void Update()
    {
        guiManager.SetClock(20 - Time.time % 20, 20);
        if (Time.time / 20 < resizeCount) return;
        levelManager.ShrinkFarm();
        resizeCount++;
    }
}