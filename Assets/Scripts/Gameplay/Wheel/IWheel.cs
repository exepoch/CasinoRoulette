
namespace Core.Interfaces
{
    public interface IWheel
    {
        public RouletteType RouletteType { get; }
        public void Spin();
    }
}
