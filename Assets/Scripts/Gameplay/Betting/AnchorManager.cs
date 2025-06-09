using System.Collections.Generic;
using System.Linq;
using Events;
using Events.EventTypes;
using Gameplay.Betting.Interfaces;
using UnityEngine;

namespace Gameplay.Betting
{
    /// <summary>
    /// Singleton managing all BetAnchors, handles highlight events and anchor retrieval.
    /// </summary>
    public class AnchorManager : MonoBehaviour, IAnchorService
    {
        private static IAnchorService instance;

        public static IAnchorService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<AnchorManager>();

                    if (instance != null) return instance;
                    var go = new GameObject("AnchorManager");
                    instance = go.AddComponent<AnchorManager>();
                }
                return instance;
            }
        }

        [SerializeField] private List<BetAnchor> allAnchors;

        private void Awake()
        {
            if (instance != null && (AnchorManager)instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (allAnchors == null || allAnchors.Count == 0)
                Debug.LogWarning("Anchors not assigned!");

            EventBus<HighlightEvent>.Subscribe(OnHighlightEvent);
        }

        private void OnDisable()
        {
            EventBus<HighlightEvent>.Unsubscribe(OnHighlightEvent);
        }
        
        /// <summary>
        /// Highlights anchors based on event data.
        /// </summary>
        private void OnHighlightEvent(HighlightEvent evt)
        {
            foreach (var anchor in allAnchors)
                anchor.SetGlow(evt.NumberIds.Contains(anchor.AnchorID) && evt.Type == HighlightEvent.HighlightType.Show);
        }

        /// <summary>
        /// Get anchor by ID.
        /// </summary>
        public BetAnchor GetAnchorById(int id) => allAnchors.FirstOrDefault(x => x.AnchorID == id);

        /// <summary>
        /// Get all anchors.
        /// </summary>
        public List<BetAnchor> GetAll() => allAnchors;
    }
}