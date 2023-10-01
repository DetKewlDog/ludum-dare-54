using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    [Header("Clock")]
    public UIClock clock;
    public Text clockText;

    void Awake() => Instance = this;

    public void SetClock(float value, float maxValue) {
        clockText.text = System.TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
        clock.SetPercentage(value / maxValue);
    }
}
