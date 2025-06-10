using Events;
using Events.EventTypes;
using UnityEngine;

namespace UI.ViewModels
{
    public class DeterminedNumberSelectorViewModel
    {
        public static BindableProperty<int> SelectedNumber = new();
        public readonly int ViewNumber;

        public DeterminedNumberSelectorViewModel(int viewNumber)
        {
            ViewNumber = viewNumber;
            RegisterUpdates();
        }
        
        public void SelectDeterminedNumber(int selectedNumber)
        {
            EventBus<DeterminedNumberSelectedEvent>.Raise(
                new DeterminedNumberSelectedEvent
                {
                    SelectedNumber = selectedNumber
                });
        }

        private void RegisterUpdates()
        {
            EventBus<DeterminedNumberSelectedEvent>.Subscribe(DeterminedChanged,true);
        }

        private void DeterminedChanged(DeterminedNumberSelectedEvent arg)
        {
            SelectedNumber.Value = arg.SelectedNumber;
        }

        public void Unregister() => EventBus<DeterminedNumberSelectedEvent>.Unsubscribe(DeterminedChanged);
    }
}
