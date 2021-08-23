using Thoughts.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thoughts
{
    /// <summary>
    /// Object that must be kept always alive while running the app. It initializes the game process and keeps track of the current GameManager.
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        /// <summary>
        /// The singleton reference to this class
        /// </summary>
        public static AppManager instance { get; private set; }
        
        /// <summary>
        /// Reference to the scene containing the basic elements of the game
        /// </summary>
        [SerializeField] private SceneReference gameScene;
        
        /// <summary>
        /// Tries to set up this AppManager instance as singleton instance. If possible, marks the GameObject containing this MonoBehaviour as DontDestroyOnLoad.
        /// </summary>
        private void Awake()
        {
            if (InitializeSingleton())
                DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Triggers the start of a new game.
        /// </summary>
        void Start()
        {
            StartNewGame();
        }
        
        /// <summary>
        /// If there is no other instance set as singleton of an AppManager, this is set as it. Being the only one following the Singleton pattern.
        /// </summary>
        /// <returns>True if it became the singleton instance, false if another AppManager set up as the singleton instance already exists.</returns>
        private bool InitializeSingleton()
        {

            if (instance == null)
            {
                instance = this;
                return true;
            }
            
            Debug.LogError($"More than one {nameof(AppManager)} exist. The gameObject '{instance.gameObject.name}' was assigned before '{this.gameObject.name}'");
            return false;
        }
        
        /// <summary>
        /// Starts a new game by loading the Game scene.
        /// </summary>
        private void StartNewGame()
        {
            SceneManager.LoadScene(gameScene, LoadSceneMode.Additive);
        }

    }
}
