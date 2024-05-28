using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Common
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FunctionRunner
    {
        /// <summary>
        /// RunFuncUntilSuccess provides a way to repeatedly run an action until it runs without exception for the specified duration
        /// useful for wrapping around an assert when waiting for a screen/list to refresh
        /// after the duration a timeout exception is thrown
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeToWait"></param>
        /// <param name="exceptionAction"></param>
        /// <param name="pollInterval"></param>
        /// <param name="callerName"></param>
        /// <param name="callerLine"></param>
        public static void RunFuncUntilSuccess(Action action, int? timeToWait = null, Action exceptionAction = null, int pollInterval = 1000, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLine = 0)
        {
            if (timeToWait == null)
                timeToWait = 5000;

            string lastException = string.Empty;

            RunFuncUntilSuccess(() =>
            {
                try
                {
                    Console.WriteLine($"Polling...{callerName} line {callerLine} for success");

                    action();

                    Console.WriteLine("Succeeded!");

                    return true;
                }
                catch (Exception e)
                {
                    lastException = e.ToStringDemystified();

                    exceptionAction?.Invoke();

                    return false;
                }
            }, () => "Calling method " + callerName + " threw an exception and failed to re-run without exception within {0}", (int)timeToWait, true, pollInterval, () => $" - Exception: {lastException}");
        }

        /// <summary>
        /// RunFuncUntilSuccess provides a way to repeatedly run a function for the specified duration, after which a timeout exception is thrown
        /// if true is not passed into the function by the caller
        /// Note: the exception will fail the current test
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="timeToWait">time to wait - default 5000ms</param>
        /// <param name="pollInterval">poll interval in ms</param>
        public static void RunFuncUntilSuccess(Func<bool> condition, Func<string> message, int? timeToWait = null, int pollInterval = 1000)
        {
            if (timeToWait == null)
                timeToWait = 5000;

            RunFuncUntilSuccess(condition, message, (int)timeToWait, true, pollInterval);
        }

        public static void RunFuncUntilSuccess(Func<bool> condition, Func<string> message, int timeToWait, bool pollEverySecond, int pollInterval = 1000, Func<string> additionalMessage = null)
        {
            bool result;

            var endTime = DateTime.Now.AddMilliseconds(timeToWait).Ticks;

            do
            {
                result = condition();

                if (result == false && pollEverySecond) Thread.Sleep(pollInterval);

                if (result == false && DateTime.Now.Ticks >= endTime)
                {
                    var addMessage = additionalMessage != null ? additionalMessage() : string.Empty;

                    throw new TimeoutException(string.Format(message(), timeToWait.ToString()) + addMessage);
                }

            } while (result == false);
        }

        /// <summary>
        /// RunTaskUntilTimeout provides a way to repeatedly run a task for up to 120 seconds, after which a timeout exception is thrown
        /// if true is not passed into the function by the caller
        /// </summary>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <param name="waitInterval"></param>
        /// <returns>the time taken</returns>
        public static async Task<int> RunTaskUntilTimeout(Func<Task<bool>> func, string errorMessage = "Task Timeout Exception", int waitInterval = 500)
        {
            var runtime = 0;
            var errorText = string.Empty;

            while (true)
            {
                var res = false;

                try
                {
                    Console.WriteLine($"Task runner executing {runtime} ms");
                    res = await func();
                }
                catch (Exception e)
                {
                    errorText = $"{errorMessage} Stack trace: {e.StackTrace}";
                    Console.WriteLine(errorText);
                }

                if (res)
                {
                    return runtime;
                }

                await Task.Delay(waitInterval);

                runtime += waitInterval;

                if (runtime >= 120000)
                    throw new TimeoutException(string.IsNullOrEmpty(errorText) ? errorMessage : errorText);
            }
        }

        /// <summary>
        /// RunTaskUntilTimeout provides a way to repeatedly run a task for up to 120 seconds, after which a timeout exception is thrown
        /// if true is not passed into the function by the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <param name="waitInterval"></param>
        /// <returns>the result of the task</returns>
        public static async Task<T> RunTaskUntilTimeout<T>(Func<Task<T>> func, string errorMessage = "Task Timeout Exception", int waitInterval = 500)
        {
            var runtime = 0;

            while (true)
            {
                string errorText;
                try
                {
                    Console.WriteLine($"Task runner executing {runtime} ms");
                    return await func();
                }
                catch (Exception e)
                {
                    errorText = $"{errorMessage} Stack trace: {e.StackTrace}";
                    Console.WriteLine(errorText);
                }

                await Task.Delay(waitInterval);

                runtime += waitInterval;

                if (runtime >= 120000)
                    throw new TimeoutException(string.IsNullOrEmpty(errorText) ? errorMessage : errorText);
            }
        }

    }
}
