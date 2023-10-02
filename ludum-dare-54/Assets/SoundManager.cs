using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public List<AudioClip> sounds;

    private float lastTime = 0;

    void Awake() => Instance = this;

    public static void Play(string name) {
        var sound = Instance.sounds.Where(x => x.name.ToLower() == name.ToLower()).FirstOrDefault();
        if (sound == null || Time.unscaledTime - Instance.lastTime <= 0.05f) return;
        var ts = Time.timeScale;
        Time.timeScale = 1;
        AudioSource.PlayClipAtPoint(sound, PlayerController.Instance.transform.position);
        Time.timeScale = ts;
        Instance.lastTime = Time.unscaledTime;
    }

    public static void Play(params string[] sounds) {
        foreach (var sound in sounds) {
            Instance.lastTime = 0;
            Play(sound);
        }
    }

    public void PlaySound(string name) => Play(name);
}
