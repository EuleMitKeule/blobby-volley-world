using BeardedManStudios.Forge.Networking.Unity;
using System;

namespace Blobby.Networking
{
    /// <summary>
    /// Tracks the online/offline status of backend services and notifies subscribers.
    /// UI components subscribe to StatusChanged to show/hide offline indicators.
    /// All events are dispatched on the Unity main thread.
    /// </summary>
    public static class OnlineStatusHelper
    {
        public enum Status
        {
            /// <summary>All services are reachable.</summary>
            Online,
            /// <summary>One or more backend services are unreachable.</summary>
            Offline
        }

        /// <summary>Current overall online status.</summary>
        public static Status CurrentStatus { get; private set; } = Status.Online;

        /// <summary>Fired on the main thread when the online status changes.</summary>
        public static event Action<Status> StatusChanged;

        /// <summary>Whether the login/API server is reachable.</summary>
        public static bool IsLoginOnline { get; private set; } = true;

        /// <summary>Whether the master server (server browser) is reachable.</summary>
        public static bool IsMasterServerOnline { get; private set; } = true;

        /// <summary>
        /// Call this when the login/API server becomes unreachable.
        /// Safe to call from any thread.
        /// </summary>
        public static void SetLoginOffline()
        {
            if (!IsLoginOnline) return;
            IsLoginOnline = false;
            UpdateOverallStatus();
        }

        /// <summary>
        /// Call this when the login/API server is reachable again.
        /// Safe to call from any thread.
        /// </summary>
        public static void SetLoginOnline()
        {
            if (IsLoginOnline) return;
            IsLoginOnline = true;
            UpdateOverallStatus();
        }

        /// <summary>
        /// Call this when the master server becomes unreachable.
        /// Safe to call from any thread.
        /// </summary>
        public static void SetMasterServerOffline()
        {
            if (!IsMasterServerOnline) return;
            IsMasterServerOnline = false;
            UpdateOverallStatus();
        }

        /// <summary>
        /// Call this when the master server is reachable again.
        /// Safe to call from any thread.
        /// </summary>
        public static void SetMasterServerOnline()
        {
            if (IsMasterServerOnline) return;
            IsMasterServerOnline = true;
            UpdateOverallStatus();
        }

        static void UpdateOverallStatus()
        {
            var newStatus = (IsLoginOnline && IsMasterServerOnline) ? Status.Online : Status.Offline;

            if (newStatus != CurrentStatus)
            {
                CurrentStatus = newStatus;

                // Dispatch event on the main thread to ensure UI operations are safe
                MainThreadManager.Run(() => StatusChanged?.Invoke(CurrentStatus));
            }
        }
    }
}
