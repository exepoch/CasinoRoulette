using Utils;

namespace Gameplay.Wheel
{
    public class AmericanWheel : Wheel
    {
        protected override void Awake()
        {
            SlotOrder = RouletteType.RouletTypeToNumbers();
            base.Awake();
        }

        public override RouletteType RouletteType => RouletteType.American;
    }
}