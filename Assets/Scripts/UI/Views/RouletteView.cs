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
        [SerializeField] private TextMeshProUGUI winningHeaderLabel;
        [SerializeField] private TextMeshProUGUI winningAmountText;
        [SerializeField] private TextMeshProUGUI lastWinnerNumberText;
        [SerializeField] private TextMeshProUGUI profitText;
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
            
            // Update profit display when value changes
            _viewModel.Profit.OnValueChanged += value =>
            {
                profitText.color = value > 0 ? Color.green : Color.red;
                profitText.text = $"Overall Profit: ${value:N0}";
            };

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
                var isWin = winning.WinningAmount > 0;
                var isLose = winning.LoseAmount > 0;
                if(!isWin && !isLose) return;
                winningAmountText.text = isWin ?  $"+ ${winning.WinningAmount:N0}" : $"+ $0";
                winningAmountText.color = isWin ? Color.green : Color.red;
                winningHeaderLabel.color = isWin ? Color.green : Color.red;
                winningHeaderLabel.text = isWin ? "Win" : "Lose";
                lastWinnerNumberText.color = isWin ? Color.green : Color.red;
                lastWinnerNumberText.text = $"{winning.WinnerNumber}";
                profitText.color = _viewModel.Profit.Value > 0 ? Color.green : Color.red;
                profitText.text = $"Overall Profit: ${_viewModel.Profit.Value:N0}";
                winPanel.SetActive(true);
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
