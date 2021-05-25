using Thoughts.Game;
using UnityEngine;

namespace Thoughts
{
    /// <summary>
    /// Object that must be kept always alive while running the app. It initializes the game process and keeps track of the current GameManager.
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance of this class (AppManager)
        /// </summary>
        public static AppManager instance { get; private set; }
        
        /// <summary>
        /// The GameObject prefab that is going to be spawned to initialize a new game process.
        /// </summary>
        [SerializeField] private GameObject gameManagerPrefab;
        
        /// <summary>
        /// The current GameManager of the game/app
        /// </summary>
        public static GameManager gameManager { get; private set; }

        /// <summary>
        /// Tries to set up this AppManager as singleton instance. If possible, marks the GameObject containing this MonoBehaviour as DontDestroyOnLoad.
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
        /// If there is no other instance of an AppManager, this is set as it. Being the only one following the Singleton pattern.
        /// </summary>
        /// <returns>True if it became the singleton instance, false if another AppManager set up as the singleton instance already exists.</returns>
        private bool InitializeSingleton()
        {

            if (instance == null)
            {
                instance = this;
                return true;
            }
            
            Debug.LogError($"More than one AppManager exist. The gameObject '{instance.gameObject.name}' was assigned before '{this.gameObject.name}'");
            return false;
        }
        
        /// <summary>
        /// Starts a new game by instantiating the GameManager prefab and triggering its method StartNewGame.
        /// </summary>
        private void StartNewGame()
        {
            //To-Do: Destroy the previous GameManager (that contains all the data/info/state of the previous game) 
            gameManager = Instantiate(gameManagerPrefab).GetComponentRequired<GameManager>();
            gameManager.StartNewGame();
        }

    }
}
