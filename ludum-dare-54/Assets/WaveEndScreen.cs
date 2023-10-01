using System.Collections;
using UnityEngine;

public class WaveEndScreen : MonoBehaviour
{
    public RectTransform Hider;
    public float toggleDuration = 1;
    public GameObject waveEndButtons;
    public GameObject buyLandButton;

    float size;

    void Awake() {
        var currentRes = Screen.currentResolution;
        size = Mathf.Sqrt(currentRes.width * currentRes.width + currentRes.height * currentRes.height);
        (Hider.parent as RectTransform).sizeDelta = Hider.sizeDelta = Vector2.one * size;
    }

    public void ToggleHider(bool toggle, System.Action callback) => StartCoroutine(ToggleHiderCo(toggle, callback));

    IEnumerator ToggleHiderCo(bool toggle, System.Action callback) {
        if (!toggle) waveEndButtons.SetActive(false);
        float startY = toggle ? size : 0;
        float stopY = toggle ? 0 : -size;
        float startTime = Time.realtimeSinceStartup;
        float time = 0;
        Vector3 position = Hider.localPosition;
        position.y = startY;
        Hider.localPosition = position;
        while (time < toggleDuration) {
            position.y = Mathf.Lerp(startY, stopY, time / toggleDuration);
            Hider.localPosition = position;
            time = Time.realtimeSinceStartup - startTime;
            yield return null;
        }
        position.y = stopY;
        Hider.localPosition = position;
        if (toggle) {
            waveEndButtons.SetActive(true);
            buyLandButton.SetActive(GameManager.Instance.MoneyAmount >= 200);
        }
        callback();
    }
}
