using Data;
using UnityEngine;

namespace Gameplay.Betting.Data
{
    [CreateAssetMenu(menuName = "Chips/Chip Data")]
    public class ChipData : ScriptableObject
    {
        public ChipType chipType;
        public GameObject prefab;
        public int value;
    }

}