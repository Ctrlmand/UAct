using System;
using System.Collections.Generic;
using UnityEngine;
namespace UAct
{

	public class BaseCommandContext : ICommandContext
	{
		private Dictionary<Type, object> _data = new Dictionary<Type, object>();

		public BaseCommandContext() { }
		public BaseCommandContext(object data) => SetData(data);

		public T GetData<T>()
		{
			return _data.TryGetValue(typeof(T), out object value) ? (T)value : default;
		}

		public BaseCommandContext SetData<T>(T data)
		{
			_data[typeof(T)] = data;
			return this;
		}
		
		public void ShowAll()
		{
			foreach (var item in _data)
			{
				Debug.Log($"{item.Key} => {item.Value}");
			}
		}
	}
}
