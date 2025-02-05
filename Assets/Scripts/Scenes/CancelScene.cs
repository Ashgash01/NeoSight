using UnityEngine;
using UnityEngine.SceneManagement;

public class CancelScene : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
