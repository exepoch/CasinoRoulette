using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Data;
using UnityEngine;

namespace Gameplay.Betting.Chips
{
    /// <summary>
    /// Manages chip GameObjects stack, creation, pooling and value representation.
    /// Supports adding/removing chips and calculating wins with multiplier.
    /// </summary>
    public class ChipStack : MonoBehaviour
    {
        private readonly Dictionary<ChipType, List<GameObject>> _chips = new();
        private IChipFactory _chipFactory;
        private long _value = 0;

        public void Initialize(IChipFactory factory) => _chipFactory = factory;

        public void SetInitialPosition(Vector3 pos) => transform.position = pos;

        public void Add(long value) => SetValue(_value + value);

        public void Remove(long value) => SetValue(_value - value);

        public long GetValue() => _value;

        /// <summary>
        /// Clears all chips and returns them to the pool.
        /// </summary>
        public void Clear()
        {
            if (_value == 0) return;
            _value = 0;

            foreach (var pair in _chips)
                foreach (var chip in pair.Value)
                    _chipFactory.ReturnChip(pair.Key, chip);

            _chips.Clear();
        }

        private void SetValue(long newValue)
        {
            Clear();
            if (newValue <= 0) return;

            _value = newValue;
            SpawnChipsForValue(newValue);
        }

        /// <summary>
        /// Spawns chips based on value using highest to lowest chip denominations.
        /// Chips stacked vertically.
        /// </summary>
        private void SpawnChipsForValue(float value)
        {
            var chipConfigs = _chipFactory.GetChipConfigs();
            chipConfigs.Sort((a, b) => b.value.CompareTo(a.value));

            foreach (var config in chipConfigs)
            {
                int count = (int)(value / config.value);
                if (count <= 0) continue;

                value -= count * config.value;
                if (!_chips.ContainsKey(config.chipType))
                    _chips[config.chipType] = new List<GameObject>();

                for (int i = 0; i < count; i++)
                {
                    float yOffset = _chips.Values.Sum(l => l.Count) * 0.06f;
                    var chip = _chipFactory.CreateChip(config.chipType, transform, new Vector3(0, yOffset, 0));
                    _chips[config.chipType].Add(chip);
                }

                if (value <= 0) break;
            }
        }

        /// <summary>
        /// Applies win multiplier, updates stack, and triggers chip collection.
        /// </summary>
        public long WinAmount(int multiplier)
        {
            var winAmount = _value * multiplier;
            if (winAmount > 0) CollectChips();
            return winAmount;
        }

        public void CollectChips()
        {
            // TODO: Add chip collection animation
            Clear();
        }
    }
}
