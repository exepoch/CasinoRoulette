using Events;
using Events.EventTypes;
using Gameplay.Betting.Chips;
using Gameplay.Betting.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Betting
{
    /// <summary>
    /// Represents a betting anchor on the table.
    /// Manages mouse hover glow, broadcasts highlight events, and handles chip stacking.
    /// </summary>
    public class BetAnchor : MonoBehaviour
    {
        [SerializeField] private GameObject glowObject; // Glow effect object
        [SerializeField] public int[] numbers;          // Covered numbers
        public int AnchorID => anchorID;

        private ChipStack _stack;
        private BetType _betType;
        [SerializeField] private int anchorID;

        void Start()
        {
            if (glowObject != null) glowObject.SetActive(false);

            // Initialize chip stack under this anchor
            _stack = new GameObject("Stack").AddComponent<ChipStack>();
            _stack.Initialize(ChipFactory.Instance);
            _stack.SetInitialPosition(transform.position);
            _stack.transform.SetParent(transform);
            _stack.transform.localPosition = Vector3.zero;
        }

        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            SetGlow(true);

            // Notify to highlight covered numbers
            EventBus<HighlightEvent>.Raise(new HighlightEvent
            {
                Type = HighlightEvent.HighlightType.Show,
                NumberIds = numbers
            });
        }

        private void OnMouseExit()
        {
            SetGlow(false);

            // Notify to remove highlight
            EventBus<HighlightEvent>.Raise(new HighlightEvent
            {
                Type = HighlightEvent.HighlightType.Hide,
                NumberIds = numbers
            });
        }

        public void AddChips(long value) => _stack.Add(value);
        public void RemoveChips(long value) => _stack.Remove(value);
        public void ClearBets() => _stack.Clear();

        public long Winnings(int numsCount, int resultNumber)
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                var numb = numbers[i];
                if (numb != resultNumber) continue;
                
                return _stack.WinAmount(numsCount / numbers.Length);
            }
            
            _stack.Clear();
            return 0;
        }

        public void SetGlow(bool set)
        {
            if (glowObject) glowObject.SetActive(set);
        }
    }
}
