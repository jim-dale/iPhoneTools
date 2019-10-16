namespace iPhoneTools
{
    public interface ICommand<T>
    {
        int Run(T opts);
    }
}
