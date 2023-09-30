using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    int resizeCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LevelManager.Instance.GenerateLevel();
    }

    void Update()
    {
        if (Time.time / 20 < resizeCount) return;
        LevelManager.Instance.ShrinkFarm();
        resizeCount++;
    }
}