using System;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public Text totalPlayTimeText;
    public Text roundsCompletedText;
    public Text totalEarningsText;
    public Text cropsPlantedText;

    protected float totalPlayTime = 0;
    protected int roundsCompleted = 0;
    protected int totalEarnings = 0;
    protected int cropsPlanted = 0;

    public float TotalPlayTime {
        get => totalPlayTime;
        set {
            totalPlayTime = value;
            var pb = PlayerPrefs.GetFloat("total_playtime", 0);
            totalPlayTimeText.text = $"{TimeSpan.FromSeconds(totalPlayTime):mm\\mss\\s} ({(totalPlayTime > pb ? "New PB!" : $"PB: {TimeSpan.FromSeconds(pb):mm\\mss\\s}")})";
            if (pb >= totalPlayTime) return;
            PlayerPrefs.SetFloat("total_playtime", totalPlayTime);
            PlayerPrefs.Save();
        }
    }
    public int RoundsCompleted {
        get => roundsCompleted;
        set {
            roundsCompleted = value;
            var pb = PlayerPrefs.GetInt("rounds_completed", 0);
            roundsCompletedText.text = $"{roundsCompleted} ({(roundsCompleted > pb ? "New PB!" : $"PB: {pb}")})";
            if (pb >= roundsCompleted) return;
            PlayerPrefs.SetInt("rounds_completed", roundsCompleted);
            PlayerPrefs.Save();
        }
    }
    public int TotalEarnings {
        get => totalEarnings;
        set {
            totalEarnings = value;
            var pb = PlayerPrefs.GetInt("total_earnings", 0);
            totalEarningsText.text = $"${totalEarnings} ({(totalEarnings > pb ? "New PB!" : $"PB: ${pb}")})";
            if (pb >= totalEarnings) return;
            PlayerPrefs.SetInt("total_earnings", totalEarnings);
            PlayerPrefs.Save();
        }
    }
    public int CropsPlanted {
        get => cropsPlanted;
        set {
            cropsPlanted = value;
            var pb = PlayerPrefs.GetInt("crops_planted", 0);
            cropsPlantedText.text = $"{cropsPlanted} ({(cropsPlanted > pb ? "New PB!" : $"PB: {pb}")})";
            if (pb >= cropsPlanted) return;
            PlayerPrefs.SetInt("crops_planted", cropsPlanted);
            PlayerPrefs.Save();
        }
    }

    void Awake() => Instance = this;
}
