using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads index 1 which corresponds directly to your GameplayScene layout
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Custom method to handle closing the application process cleanly.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Application Management: Closing space simulator protocol.");

        // This closes the built standalone executable (.exe) app
        Application.Quit();
    }
}
