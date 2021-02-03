using System;
using System.Reflection;
using System.Threading;

namespace Blobby
{
    public static class ThreadHelper
    {
        /// <summary>
        /// Call this to stop all running tasks
        /// </summary>
        public static void Stop()
        {
#if UNITY_EDITOR
            var constructor = SynchronizationContext.Current.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
            var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
#endif
        }
    }
}