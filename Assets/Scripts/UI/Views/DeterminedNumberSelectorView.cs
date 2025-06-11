using System;
using TMPro;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Views
{
    public class DeterminedNumberSelectorView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private Button button;
        private DeterminedNumberSelectorViewModel viewModel;

        public void InitModel(int determinedNumber)
        {
            viewModel = new DeterminedNumberSelectorViewModel(determinedNumber);
            SetBackgorundColorOfSelected(DeterminedNumberSelectorViewModel.SelectedNumber.Value); //Set to initial value
            DeterminedNumberSelectorViewModel.SelectedNumber.OnValueChanged += SetBackgorundColorOfSelected;
            numberText.text = viewModel.ViewNumber.NumberToName();
            button.onClick.AddListener(()=> viewModel.SelectDeterminedNumber(determinedNumber));
        }

        private void OnDestroy()
        {
            DeterminedNumberSelectorViewModel.SelectedNumber.OnValueChanged -= SetBackgorundColorOfSelected;
        }

        private void SetBackgorundColorOfSelected(int selected)
        {
            button.image.color = selected == viewModel.ViewNumber ? Color.magenta : Color.white;
        }

        private void OnDisable() => viewModel.Unregister();
    }
}
