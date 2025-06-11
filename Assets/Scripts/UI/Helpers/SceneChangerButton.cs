using SubSystems.SaveSystem;
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
                if(SaveManager.Instance != null)
                    SaveManager.Instance.SaveAll();
                SceneHideCG.Instance.FadeIn(1, () =>
                {
                    SceneManager.LoadSceneAsync(sceneNumber);
                });
            });
        }
    }
}
