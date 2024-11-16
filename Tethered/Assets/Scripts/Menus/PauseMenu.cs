using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tethered.Input;

namespace Tethered.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private PlayerOneInputReader playerOneInputReader;

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private CanvasGroup canvasGroup;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize the buttons in the menu
            Button resumetBtn = resumeButton.GetComponent<Button>();
            Button exitBtn = exitButton.GetComponent<Button>();

            // Initialize the Canvas Group attached to the menu and make it invisible and not interactable
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;

            // Add EventListeners to the buttons for when they're clicked
            //resumeBtn.onClick.AddListener(OpenGame);
            //exitBtn.onClick.AddListener(QuitGame);

        }

        // Update is called once per frame
        void Update()
        {
            //if(Input.GetKeyDown(KeyCode.Escape))
            //{

            //}
        }

        /// <summary>
        /// Interact with the current interactable
        /// </summary>
        //protected void Pause() => Time.timeScale = 0f;
    }
}

