using Thoughts.Game;
using UnityEngine;

namespace Thoughts
{
    public class AppManager : MonoBehaviour
    {
        public static AppManager instance { get; private set; }
        [SerializeField] private GameObject gameManagerPrefab;
        public static GameManager gameManager { get; private set; }

        private void Awake()
        {
            if (InitializeSingleton())
                DontDestroyOnLoad(gameObject);
        }
        
        void Start()
        {
            StartNewGame();
        }
        
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
        
        private void StartNewGame()
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponentRequired<GameManager>();
            gameManager.StartNewGame();
        }

    }
}
