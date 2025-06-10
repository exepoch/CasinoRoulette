using Core;
using Gameplay.Wheel;
using UnityEngine;
using Utils;

namespace UI.Views
{
    public class DeterminationSelectControllerView : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private DeterminedNumberSelectorView viewPref;

        private void Start()
        {
            var rouletType = GenericInstanceProvider<Wheel>.Get().RouletteType;
            for (int i = 0; i < rouletType.RouletTypeToNumbers().Length; i++)
            {
                Instantiate(viewPref,content).InitModel(i);
            }
        }
    }
}
