using System;
using System.Collections;
using System.Linq;

namespace Common.Utilities
{
    public static class Assert
    {
        public static void NotNull<T>(T obj, string name, string message = null)
            where T : class
        {
            if (obj is null)
                throw new ArgumentNullException($"{name} : {typeof(T)}", message);
        }

        public static void NotNull<T>(T? obj, string name, string message = null)
            where T : struct
        {
            if (!obj.HasValue)
                throw new ArgumentNullException($"{name} : {typeof(T)}", message);

        }

        public static void NotNullArgument<T>(T input, string message = null)
        {
            if (input is null)
                throw new ArgumentNullException($"{message}");
        }

        public static void NotNullArgument(string input, string message = null)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException($"{message}");
        }

        public static void NotNull(int input, string message = null)
        {
            if (input == 0)
                throw new ArgumentNullException($"{message}");
        }

        public static void NotEmpty<T>(T obj, string name, string message = null, T defaultValue = null)
            where T : class
        {
            if (obj == defaultValue
                || (obj is string str && string.IsNullOrWhiteSpace(str))
                || (obj is IEnumerable list && !list.Cast<object>().Any()))
            {
                throw new ArgumentException("Argument is empty : " + message, $"{name} : {typeof(T)}");
            }
        }
    }
}
