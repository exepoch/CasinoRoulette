using TMPro;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    // Connects the ViewModel to the UI and updates UI elements based on game state
    public class RouletteView : MonoBehaviour
    {
        [SerializeField] private Button undoButton;
        [SerializeField] private Button clearBetsButton;
        [SerializeField] private Button spinButton;

        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI totalBetAmountText;
        [SerializeField] private TextMeshProUGUI winningAmountText;
        [SerializeField] private TextMeshProUGUI lastWinnerNumberText;
        [SerializeField] private GameObject winPanel;

        private RouletteViewModel _viewModel;

        private void Awake()
        {
            // Create and initialize the ViewModel
            _viewModel = new RouletteViewModel();
            _viewModel.Enable(); // Subscribe to events
            winPanel.SetActive(false); // Hide win panel initially

            BindUI(); // Bind ViewModel data to UI
        }

        private void OnDestroy() =>
            _viewModel.Disable(); // Unsubscribe from events

        private void BindUI()
        {
            // Update balance display when value changes
            _viewModel.Balance.OnValueChanged += value =>
                balanceText.text = $"Balance: ${value:N0}";

            // Update total bet display
            _viewModel.TotalBet.OnValueChanged += value =>
                totalBetAmountText.text = $"Total Bet: ${value:N0}";

            // Enable/disable undo button
            _viewModel.CanUndo.OnValueChanged += value =>
                undoButton.interactable = value;

            // Enable/disable all action buttons
            _viewModel.ButtonsEnabled.OnValueChanged += value =>
            {
                undoButton.interactable = value;
                clearBetsButton.interactable = value;
                spinButton.interactable = value;
            };

            // Update win panel and winner number color
            _viewModel.LastWinnings.OnValueChanged += winning =>
            {
                var isWin = winning.Item1 > 0;
                if (isWin)
                {
                    winningAmountText.text = $"+ ${winning.Item1:N0}";
                    winPanel.SetActive(true);
                }

                lastWinnerNumberText.color = isWin ? Color.green : Color.red;
                lastWinnerNumberText.text = $"{winning.Item2}";
            };

            // Hide win panel when game state changes to betting
            _viewModel.StateChanged.OnValueChanged += state =>
            {
                if (state == GameState.Betting)
                    winPanel.SetActive(false);
            };

            // Bind UI buttons to ViewModel commands
            undoButton.onClick.AddListener(_viewModel.OnUndoClicked);
            clearBetsButton.onClick.AddListener(_viewModel.OnClearClicked);
            spinButton.onClick.AddListener(_viewModel.OnSpinClicked);
        }
    }
}
