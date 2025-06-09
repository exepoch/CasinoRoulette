using System.Collections.Generic;
using Data;
using Gameplay.Betting.Data;
using UnityEngine;

namespace Gameplay.Betting
{
    /// <summary>
    /// Object pool managing reusable chip GameObjects by ChipType.
    /// Instantiates new chips if pool is empty.
    /// </summary>
    public class ChipPool : MonoBehaviour
    {
        [SerializeField] private List<ChipDataSO> chipConfigs;

        private Dictionary<ChipType, Queue<GameObject>> pools = new();
        private Dictionary<ChipType, GameObject> prefabMap = new();

        private void Awake()
        {
            // Initialize pools and prefab mappings per chip type
            foreach (var config in chipConfigs)
            {
                pools[config.chipType] = new Queue<GameObject>();
                prefabMap[config.chipType] = config.prefab;
            }
        }

        /// <summary>
        /// Retrieves a chip GameObject from the pool or instantiates a new one if none available.
        /// </summary>
        public GameObject Get(ChipType type)
        {
            if (!pools.ContainsKey(type)) return null;

            if (pools[type].Count > 0)
                return pools[type].Dequeue();

            return Instantiate(prefabMap[type]);
        }

        /// <summary>
        /// Returns a chip GameObject back to the pool and disables it.
        /// </summary>
        public void Return(ChipType type, GameObject chip)
        {
            chip.SetActive(false);
            chip.transform.SetParent(transform);
            pools[type].Enqueue(chip);
        }

        public List<ChipDataSO> GetConfigs() => chipConfigs;
    }
}