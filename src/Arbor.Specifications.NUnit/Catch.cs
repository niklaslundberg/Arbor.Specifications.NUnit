using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Arbor.Specifications.NUnit
{
    [DebuggerStepThrough]
    public static class Catch
    {
        public static async Task<Exception> ExceptionAsync([NotNull] Func<Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Exception exception = null;

            try
            {
                await action();
            }
            catch (Exception e)
            {
                exception = e;
            }

            return exception;
        }
    }
}
