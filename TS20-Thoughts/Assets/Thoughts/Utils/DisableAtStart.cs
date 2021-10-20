using UnityEngine;

namespace Thoughts.Utils
{
    public class DisableAtStart : MonoBehaviour
    {
        void Start()
        {
            this.gameObject.SetActive(false);
        }
    }
}
