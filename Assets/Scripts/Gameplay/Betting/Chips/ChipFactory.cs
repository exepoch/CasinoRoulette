using System.Collections.Generic;
using Core.Interfaces;
using Data;
using Gameplay.Betting.Data;
using UnityEngine;

namespace Gameplay.Betting
{
    /// <summary>
    /// Singleton factory for creating and recycling chip GameObjects via ChipPool.
    /// </summary>
    public class ChipFactory : MonoBehaviour, IChipFactory
    {
        public static ChipFactory Instance { get; private set; }

        [SerializeField] private ChipPool chipPool;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Gets a chip from the pool, sets its parent and position, activates it.
        /// </summary>
        public GameObject CreateChip(ChipType type, Transform parent, Vector3 localPosition)
        {
            var chip = chipPool.Get(type);
            chip.transform.SetParent(parent);
            chip.transform.localPosition = localPosition;
            chip.SetActive(true);
            return chip;
        }

        /// <summary>
        /// Returns a chip to the pool for reuse.
        /// </summary>
        public void ReturnChip(ChipType type, GameObject chip)
        {
            chipPool.Return(type, chip);
        }

        /// <summary>
        /// Retrieves chip configuration data.
        /// </summary>
        public List<ChipDataSO> GetChipConfigs() => chipPool.GetConfigs();
    }
}