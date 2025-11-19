
namespace UAct
{
    public interface ICommand
    {
        public void Execute(ICommandContext context);

    }

    public interface ICommandContext
    {
        T GetData<T>();
        BaseCommandContext SetData<T>(T data);
        void ShowAll();

    }

}