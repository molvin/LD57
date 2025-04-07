using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI startGameText;
    public TextMeshProUGUI exitGameText;

    public Animator animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        animator.SetTrigger("start_game");
    }

    public void ActuallyStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        animator.SetTrigger("exit_game");
    }

    public void ActuallyExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetStartGameButtonText(string text)
    {
        startGameText.text = text;
    }


    public void SetExitGameButtonText(string text)
    {
        exitGameText.text = text;
    }
}
