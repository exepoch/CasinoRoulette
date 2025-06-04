using Events;
using Gameplay.Betting.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Betting
{
    /// <summary>
    /// Represents a betting anchor on the roulette table.
    /// Handles mouse interaction, glow effect, and manages chips placed on this anchor.
    /// </summary>
    public class BetAnchor : MonoBehaviour
    {
        [SerializeField] private GameObject glowObject; // Visual indicator for hover effect
        [SerializeField] public int[] numbers;          // Numbers this anchor covers (e.g., 1-18)
        /// <summary>
        /// Public getter for the anchor's unique identifier.
        /// </summary>
        public int AnchorID => anchorID;
        
        private ChipStack stack;  // Stack of chips placed on this anchor (TODO)
        private BetType betType;  // Type of bet this anchor represents
        [SerializeField] private int anchorID; // Unique ID assigned at Editor

        void Start()
        {
            // Disable glow effect initially
            if (glowObject)
                glowObject.SetActive(false);
        }

        private void OnMouseEnter()
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;
            SetGlow(true);

            // Broadcast highlight event to show glow/highlight on related numbers
            EventBus<HighlightEvent>.Raise(new HighlightEvent
            {
                Type = HighlightEvent.HighlightType.Show,
                NumberIds = numbers
            });
        }

        void OnMouseExit()
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;
            SetGlow(false);

            // Broadcast highlight event to hide glow/highlight on related numbers
            EventBus<HighlightEvent>.Raise(new HighlightEvent
            {
                Type = HighlightEvent.HighlightType.Hide,
                NumberIds = numbers
            });
        }

        /// <summary>
        /// Adds chip value to the current chip stack on this anchor.
        /// </summary>
        /// <param name="value">Chip value to add.</param>
        public void AddChips(int value)
        {
            // TODO: Add chips to stack
            Debug.LogWarning($"{value} Chips Added to stack:{anchorID}");
        }

        /// <summary>
        /// Clears all chips placed on this anchor.
        /// </summary>
        public void ClearBets()
        {
            // TODO: Clear chips at stack
        }

        /// <summary>
        /// Enables or disables the glow visual effect.
        /// </summary>
        /// <param name="set">True to enable glow, false to disable.</param>
        public void SetGlow(bool set)
        {
            if (glowObject)
                glowObject.SetActive(set);
        }
    }
}
