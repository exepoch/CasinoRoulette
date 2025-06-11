using UnityEngine;
using Utils;

namespace SubSystems
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private SceneHideCG cg;
        private void Awake()
        {
            cg.FadeOut(1);
        }
    }
}
