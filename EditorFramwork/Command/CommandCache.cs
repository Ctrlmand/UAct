using System;
using System.Collections.Generic;

namespace UAct
{
    public static class CommandCache
    {
        private static Dictionary<Type, ICommand> m_Commands = new();

        public static T GetCommand<T>() where T : ICommand, new()
        {
            if (!m_Commands.TryGetValue(typeof(T), out var command))
            {
                command = new T();
                m_Commands[typeof(T)] = command;
            }
            return (T)command;
        }
        public static void Clear()
        {
            m_Commands.Clear();
        }

    }
}