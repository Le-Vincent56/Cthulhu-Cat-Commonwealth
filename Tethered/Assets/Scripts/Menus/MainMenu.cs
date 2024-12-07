using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Tethered.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button creditsButton;


        // Start is called before the first frame update
        void Start()
        {
            // Initialize the buttons in the menu
            Button startBtn = startButton.GetComponent<Button>();
            Button exitBtn = exitButton.GetComponent<Button>();
            Button creditsBtn = creditsButton.GetComponent<Button>();

            // Add EventListeners to the buttons for when they're clicked
            startBtn.onClick.AddListener(OpenGame);
            exitBtn.onClick.AddListener(QuitGame);
            creditsBtn.onClick.AddListener(OpenCredits);
        }

        // Opens the game scene
       void OpenGame()
        {
            SceneManager.LoadScene(sceneName:"IntroCutscene");
        }

        // Closes the game
        void QuitGame()
        {
            Application.Quit();
        }
        
        // Opens the credits menu
        void OpenCredits()
        {

        }
    }
}

