using Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Betting
{
    /// <summary>
    /// Handles player input to detect clicks on BetAnchors and raises corresponding events.
    /// Decouples input logic from betting logic by using an event bus.
    /// </summary>
    public class BetInputHandler : MonoBehaviour
    {
        [Tooltip("Main camera used for raycasting input.")]
        public Camera mainCam;

        [Tooltip("LayerMask for BetAnchor objects to detect raycast hits only on valid anchors.")]
        public LayerMask anchorLayer;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Cast a ray from the mouse position into the scene
                var ray = mainCam.ScreenPointToRay(Input.mousePosition);

                // Check if the ray hits any collider on the anchorLayer within 100 units
                if (!Physics.Raycast(ray, out RaycastHit hit, 100f, anchorLayer)) 
                    return;

                // Try to get the BetAnchor component from the hit collider
                if (hit.collider.TryGetComponent<BetAnchor>(out var anchor))
                {
                    // Raise event to notify that a BetAnchor has been clicked
                    EventBus<BetAnchorClickedEvent>.Raise(new BetAnchorClickedEvent
                    {
                        ClickedAnchorId = anchor.AnchorID
                    });
                }
            }
        }
    }
}