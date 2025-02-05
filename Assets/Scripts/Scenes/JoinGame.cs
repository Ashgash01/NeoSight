using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGame : MonoBehaviour
{
    public void JoinGameScene()
    {
        SceneManager.LoadScene("MainGame");
    }
}
