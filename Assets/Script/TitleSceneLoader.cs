using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneLoader : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("MazeMakingScene");
    }
}