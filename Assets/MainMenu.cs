using TMPro;
using UnityEngine;

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
        //TODO PÄÄÄÄR
    }

    public void ExitGame()
    {
        animator.SetTrigger("exit_game");
    }

    public void ActuallyExitGame()
    {
        Application.Quit();
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
