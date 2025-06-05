using Events;
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

        private ChipStack stack;
        private BetType betType;
        [SerializeField] private int anchorID;

        void Start()
        {
            glowObject?.SetActive(false);

            // Initialize chip stack under this anchor
            stack = new GameObject("Stack").AddComponent<ChipStack>();
            stack.Initialize(ChipFactory.Instance);
            stack.SetInitialPosition(transform.position);
            stack.transform.SetParent(transform);
            stack.transform.localPosition = Vector3.zero;
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
            if (EventSystem.current.IsPointerOverGameObject()) return;
            SetGlow(false);

            // Notify to remove highlight
            EventBus<HighlightEvent>.Raise(new HighlightEvent
            {
                Type = HighlightEvent.HighlightType.Hide,
                NumberIds = numbers
            });
        }

        public void AddChips(int value) => stack.Add(value);
        public void RemoveChips(int value) => stack.Remove(value);
        public void ClearBets() => stack.Clear();

        public void SetGlow(bool set)
        {
            if (glowObject) glowObject.SetActive(set);
        }
    }
}
