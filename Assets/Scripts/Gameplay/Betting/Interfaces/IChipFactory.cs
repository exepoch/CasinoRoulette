using System.Collections.Generic;
using Data;
using Gameplay.Betting.Data;
using UnityEngine;

namespace Gameplay.Betting.Interfaces
{
    public interface IChipFactory
    {
        GameObject CreateChip(ChipType type, Transform parent, Vector3 localPosition);
        void ReturnChip(ChipType type,GameObject chip);
        public List<ChipData> GetChipConfigs();
    }
}