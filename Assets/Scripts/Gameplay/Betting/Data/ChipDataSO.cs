using Data;
using UnityEngine;

namespace Gameplay.Betting.Data
{
    [CreateAssetMenu(menuName = "Scriptable/Chips/Chip Data")]
    public class ChipDataSO : ScriptableObject
    {
        public ChipType chipType;
        public GameObject prefab;
        public int value;
    }

}