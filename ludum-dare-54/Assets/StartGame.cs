using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void Trigger() => GUIManager.Instance.ToggleRoundEndScreen(true, () => SceneManager.LoadScene("Game"));
}
