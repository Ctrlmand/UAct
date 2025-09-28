
namespace UAct
{
    public interface ICommand
    {
        public void Execute(ICommandContext context);

    }

    public interface ICommandContext
    {
        T GetData<T>();
        void SetData<T>(T data);
        bool HasData<T>();
    }

}