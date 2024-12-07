using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Tethered.Menus
{
    public class CreditsMenu : MonoBehaviour
    {
        [SerializeField] private Button backButton;


        // Start is called before the first frame update
        void Start()
        {
            // Initialize the buttons in the menu
            Button backBtn = backButton.GetComponent<Button>();

            // Add EventListeners to the buttons for when they're clicked
            backBtn.onClick.AddListener(GoToMain);

        }

        // Opens the game scene
        void GoToMain()
        {
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
    }
}

