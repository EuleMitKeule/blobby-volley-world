using BeardedManStudios.Forge.Networking.Unity;
using Blobby.Game;
using Blobby.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Blobby.UserInterface
{
    public static class PanelQueue
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (ServerHandler.IsServer) return;

            ClientConnection.QueueStarted += OnQueueStarted;
            ClientConnection.QueueTicked += OnQueueTicked;
            ClientConnection.QueueStopped += OnQueueStopped;
            LoginHelper.QueueInfoChanged += OnQueueInfoChanged;

            // Update queue panel when online status changes
            OnlineStatusHelper.StatusChanged += OnOnlineStatusChanged;
        }

        static void OnQueueStarted()
        {
            GameObject panelQueue = GameObject.Find("panel_queue");
            panelQueue.GetComponent<Animator>().SetBool("show", true);
        }

        static void OnQueueTicked(int time)
        {
            GameObject labelQueueTime = GameObject.Find("label_queue_time");
            labelQueueTime.GetComponent<TextMeshProUGUI>().text = (time / 60).ToString("00") + ":" + (time % 60).ToString("00");
        }

        static void OnQueueStopped()
        {
            GameObject panelQueue = GameObject.Find("panel_queue");
            panelQueue.GetComponent<Animator>().SetBool("show", false);
        }

        static void OnQueueInfoChanged(int online, int queued)
        {
            MainThreadManager.Run(() =>
            {
                GameObject labelQueueOnline = GameObject.Find("label_queue_online");
                GameObject labelQueueQueued = GameObject.Find("label_queue_queued");

                if (labelQueueOnline != null)
                    labelQueueOnline.GetComponent<TextMeshProUGUI>().text = "Online: " + online;
                if (labelQueueQueued != null)
                    labelQueueQueued.GetComponent<TextMeshProUGUI>().text = "Queued: " + queued;
            });
        }

        static void OnOnlineStatusChanged(OnlineStatusHelper.Status status)
        {
            MainThreadManager.Run(() =>
            {
                // Update queue info labels to show offline status
                if (status == OnlineStatusHelper.Status.Offline)
                {
                    var labelQueueOnline = GameObject.Find("label_queue_online");
                    var labelQueueQueued = GameObject.Find("label_queue_queued");

                    if (labelQueueOnline != null)
                        labelQueueOnline.GetComponent<TextMeshProUGUI>().text = "Online: --";
                    if (labelQueueQueued != null)
                        labelQueueQueued.GetComponent<TextMeshProUGUI>().text = "Queued: --";
                }
            });
        }
    }
}
