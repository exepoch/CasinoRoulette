using System;
using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chips
{
    public class ChipUISelectorView : MonoBehaviour
    {
        [SerializeField] private ChipType chipType;
        [SerializeField] private GameObject ring;
        private Button _selectButton;
        private ChipSelectorViewModel _viewModel;

        private void Awake()
        {
            _viewModel = new ChipSelectorViewModel(chipType);
            ChipSelectorViewModel.SelectedChipType.OnValueChanged += type => ring.SetActive(type == chipType);
            _viewModel.Enable(); // Subscribes to EventBus
            _selectButton = GetComponentInChildren<Button>();
            if(_selectButton == null)
                Debug.LogWarning("Button component is missing at chip selection button!");
            
            BindUI();
        }

        private void OnDestroy() => _viewModel.Disable(); // Unsubscribes from EventBus

        private void BindUI()
        {
            if(_selectButton == null) return;
            _viewModel.Bettable.OnValueChanged += b =>
            {
                if (!b) ring.SetActive(false);
                _selectButton.interactable = b;
            };
            _viewModel.StateInteractable.OnValueChanged += can => _selectButton.interactable = can;
            _selectButton.onClick.AddListener(() =>
            {
                _viewModel.OnSelectChipType(chipType);
            });
            
            ring.SetActive(ChipSelectorViewModel.SelectedChipType.Value == chipType);
        }
    }
}
