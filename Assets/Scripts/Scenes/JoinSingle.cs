using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSingle : MonoBehaviour
{
    public void JoinSingleScene()
    {
        SceneManager.LoadScene("SinglePlayer");
    }
}
