using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public string LevelID;

    public void StartLevel()
    {
        SceneManager.LoadScene(LevelID);
    }
}
