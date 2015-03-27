namespace Busybody
{
    public interface IHandle<T> where T : BusybodyEvent
    {
        void Handle(T t);
    }
}