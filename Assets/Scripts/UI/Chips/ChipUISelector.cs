using Data;
using Events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chips
{
    public class ChipUISelector : MonoBehaviour
    {
        // The chip value this selector represents
        public ChipType chipType;
        // Visual ring to indicate selection
        public GameObject ring;

        // Flag indicating whether the player has enough balance to bet this chip
        private bool _bettable = true;
        private Button _button;

        private void Awake()
        {
            // Register click listener on the Button component
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(OnClick);
            SetSelected(false);
        }

        private void OnEnable()
        {
            // Subscribe to balance change events to update bettable state
            EventBus<BalanceChangedEvent>.Subscribe(OnBalanceUpdate);
        }

        private void OnDisable()
        {
            // Unsubscribe when disabled to avoid memory leaks
            EventBus<BalanceChangedEvent>.Unsubscribe(OnBalanceUpdate);
        }

        // Called when player's balance changes, updates whether this chip can be bet
        private void OnBalanceUpdate(BalanceChangedEvent eventArgs)
        {
            // Player can bet if balance is greater or equal chip value
            _bettable = eventArgs.UpdatedBalance >= (int)chipType;
            if(!_bettable) ring.SetActive(false);
            _button.interactable = _bettable;
        }

        // Called when user clicks the chip button
        private void OnClick()
        {
            // Only proceed if chip is bettable
            if (!_bettable) return;

            // Inform UI manager about the new selection
            ChipUIManager.Instance.SetNewSelector(this);
        }

        // Enables or disables the ring visual to show selection state
        public void SetSelected(bool set)
        {
            ring.SetActive(set);
        }

        // Pointer enter event (for hover), can add animation later
        public void OnPointEnter()
        {
            if (!_bettable) return;
            // TODO: Add scale animation or visual effect here
        }

        // Pointer exit event (hover out), can add animation later
        public void OnPointExit()
        {
            if (!_bettable) return;
            // TODO: Add scale animation or visual effect here
        }
    }
}
