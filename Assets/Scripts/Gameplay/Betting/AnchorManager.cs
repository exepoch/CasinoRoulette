using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.EventTypes;
using Gameplay.Betting.Interfaces;
using SubSystems.SaveSystem;
using UnityEngine;

namespace Gameplay.Betting
{
    /// <summary>
    /// Singleton managing all BetAnchors, handles highlight events and anchor retrieval.
    /// </summary>
    public class AnchorManager : MonoBehaviour, IAnchorService,ISaveable<BetAnchorDataSave>
    {
        private static IAnchorService instance;

        [SerializeField] private List<BetAnchor> allAnchors;

        private void Awake()
        {
            instance = this;
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

        public string SaveKey => "BetAnchorDataSave";
        public BetAnchorDataSave CaptureState()
        {
            var save = new BetAnchorDataSave();
            save.entry = new List<BetAnchorDataSaveEntry>();
            for (var i = 0; i < allAnchors.Count; i++)
            {
                var betAnchor = allAnchors[i];
                save.entry.Add(new BetAnchorDataSaveEntry
                {
                    anchorId = betAnchor.AnchorID,
                    lastStackValue = betAnchor.Stack.GetValue()
                });
            }

            return save;
        }

        public void RestoreState(BetAnchorDataSave state)
        {
            for (int i = 0; i < allAnchors.Count; i++)
            {
                allAnchors[i].Stack.Add(state.entry[i].lastStackValue);
            }
        }
    }
    [Serializable]
    public class BetAnchorDataSave
    {
        public List<BetAnchorDataSaveEntry> entry;
    }
    [Serializable]
    public class BetAnchorDataSaveEntry
    {
        public int anchorId;
        public long lastStackValue;
    }
}