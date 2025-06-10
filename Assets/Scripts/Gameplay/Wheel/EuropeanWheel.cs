using Utils;

namespace Gameplay.Wheel
{
    public class EuropeanWheel : Wheel
    {
        public override RouletteType RouletteType => RouletteType.European;
        protected override void Awake()
        {
            SlotOrder = RouletteType.RouletTypeToNumbers();
            
            base.Awake();
        }
    }
}

