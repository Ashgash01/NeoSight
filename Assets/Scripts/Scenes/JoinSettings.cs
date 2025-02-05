using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSettings : MonoBehaviour
{
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }
}
