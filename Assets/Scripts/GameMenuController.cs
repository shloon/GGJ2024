using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void RetryGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
