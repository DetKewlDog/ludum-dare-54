using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    [Header("Money")]
    public Text moneyText;

    [Header("Clock")]
    public GameObject clockContainer;
    public UIClock clock;
    public Text clockText;

    [Header("Wave End Screen")]
    public WaveEndScreen waveEndScreen;

    void Awake() => Instance = this;

    public void ToggleClock(bool toggle) => clockContainer.SetActive(toggle);

    public void SetClock(float value, float maxValue) {
        clockText.text = System.TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
        clock.SetPercentage(value / maxValue);
    }

    public void ToggleWaveEndScreen(bool toggle, System.Action callback) => waveEndScreen.ToggleHider(toggle, callback);

    internal void SetMoney(int value) => moneyText.text = value.ToString();
}
