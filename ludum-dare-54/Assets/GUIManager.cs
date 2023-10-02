using System;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    [Header("HUD")]
    public GameObject hud;
    public Text moneyText;
    public Text cropText;
    public Text landText;

    [Header("Clock")]
    public UIClock clock;
    public Text clockText;

    [Header("Round End Screen")]
    public RoundEndScreen roundEndScreen;

    void Awake() => Instance = this;

    public void ToggleAlarmClock(bool value) => clock.ToggleAlarmClock(value);
    public void ToggleClock(bool value) => clock.gameObject.SetActive(value);

    public void SetClock(float value, float maxValue) {
        clockText.text = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
        clock.SetPercentage(value / maxValue);
    }

    public void ToggleRoundEndScreen(bool toggle, Action callback, bool gameOver = false) {
        roundEndScreen.ToggleHider(toggle, callback, gameOver);
        if (gameOver) hud.SetActive(false);
    }

    internal void SetMoney(int value) => moneyText.text = value.ToString();
    internal void SetCrop(int value) => cropText.text = value.ToString();
    internal void SetLand(Vector2Int size) => landText.text = $"{Mathf.Max(size.x, 0)}x{Mathf.Max(size.y, 0)}";
}
