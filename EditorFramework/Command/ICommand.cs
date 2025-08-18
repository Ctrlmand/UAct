using System;
using System.Collections.Generic;

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
    
	public class BaseCommandContext : ICommandContext
    {
        private Dictionary<Type, object> _data = new();
        
        public BaseCommandContext() { }
        public BaseCommandContext(object data) => SetData(data);

        public T GetData<T>()
        {
            return _data.TryGetValue(typeof(T), out object value) ? (T)value : default;
        }

        public bool HasData<T>()
        {
            return _data.ContainsKey(typeof(T));
        }

        public void SetData<T>(T data)
        {
            _data[typeof(T)] = data;
        }
    }

}