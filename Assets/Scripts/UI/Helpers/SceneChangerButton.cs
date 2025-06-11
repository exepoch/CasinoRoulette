using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.Helpers
{
    public class SceneChangerButton : MonoBehaviour
    {
        [SerializeField] private int sceneNumber;
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                SceneHideCG.Instance.FadeIn(1, () =>
                {
                    SceneManager.LoadSceneAsync(sceneNumber);
                });
            });
        }
    }
}
