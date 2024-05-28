using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace TestApp.ViewModels
{
    public static class IEnumerableExtensions
    {
        public static void RunCommandForAll<T>(this IEnumerable<T> items, Func<T, ICommand> command)
        {
            foreach (var item in items)
            {
                if (command(item).CanExecute(null))
                {
                    command(item).Execute(null);
                }
            }
        }
    }
}
