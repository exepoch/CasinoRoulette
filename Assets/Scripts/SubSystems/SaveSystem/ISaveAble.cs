namespace SubSystems.SaveSystem
{
    public interface ISaveable<T>
    {
        string SaveKey { get; }
        T CaptureState();
        void RestoreState(T state);
    }
}