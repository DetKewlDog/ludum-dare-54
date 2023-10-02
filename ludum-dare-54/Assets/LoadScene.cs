using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string sceneName;
    public void Trigger() => SceneManager.LoadScene(sceneName);
}
