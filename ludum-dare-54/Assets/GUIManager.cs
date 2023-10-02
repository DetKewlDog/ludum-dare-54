using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    [Header("HUD")]
    public Text moneyText;
    public Text cropText;
    public Text landText;

    [Header("Clock")]
    public GameObject clockContainer;
    public UIClock clock;
    public Text clockText;

    [Header("Wave End Screen")]
    public WaveEndScreen waveEndScreen;

    void Awake() => Instance = this;

    public void ToggleAlarmClock(bool value) => clock.ToggleAlarmClock(value);

    public void SetClock(float value, float maxValue) {
        clockText.text = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
        clock.SetPercentage(value / maxValue);
    }

    public void ToggleWaveEndScreen(bool toggle, System.Action callback) => waveEndScreen.ToggleHider(toggle, callback);

    internal void SetMoney(int value) => moneyText.text = value.ToString();
    internal void SetCrop(int value) => cropText.text = value.ToString();
    internal void SetLand(Vector2Int size) => landText.text = $"{size.x}x{size.y}";
}
