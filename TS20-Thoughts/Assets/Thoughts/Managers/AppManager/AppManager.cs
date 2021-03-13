using Essentials.Scripts.Extensions;
using UnityEngine;

namespace Thoughts
{
    public class AppManager : MonoBehaviour
    {

        [SerializeField] private GameObject gameManagerPrefab;
        public static GameManager gameManager { get; private set; }

        void Start()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponentRequired<GameManager>();
            gameManager.Initialize();
        }

    }
}
