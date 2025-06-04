using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;

namespace Gameplay.Betting
{
    public class AnchorManager : MonoBehaviour,IAnchorService
    {
        [Tooltip("All BetAnchors to control.")]
        [SerializeField] private List<BetAnchor> allAnchors;

        private void OnEnable()
        {
            if (allAnchors == null || allAnchors.Count == 0)
            {
                Debug.LogWarning("Anchor highlight error: anchors are not initialized in the editor!");
                return;
            }
            EventBus<HighlightEvent>.Subscribe(OnHighlightEvent);
        }

        private void OnDisable()
        {
            EventBus<HighlightEvent>.Unsubscribe(OnHighlightEvent);
        }
        
        /// <summary>
        /// Called when a HighlightEvent is raised.
        /// Updates glow state for each anchor based on event data.
        /// </summary>
        private void OnHighlightEvent(HighlightEvent evt)
        {
            foreach (var anchor in allAnchors)
            {
                // Check if anchor's ID is included in the event number IDs
                bool shouldGlow = evt.NumberIds.Contains(anchor.AnchorID);

                // Set glow only if event type is Show and anchor is in the list
                anchor.SetGlow(shouldGlow && evt.Type == HighlightEvent.HighlightType.Show);
            }
        }

        public BetAnchor GetAnchorById(int id)
        {
            return allAnchors.FirstOrDefault(x => x.AnchorID == id);
        }
    }
}