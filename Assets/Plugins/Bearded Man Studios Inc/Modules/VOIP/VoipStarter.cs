#if !UNITY_WEBGL && !WINDOWS_UWP
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Unity.Modules;
#endif

using UnityEngine;

public class VoipStarter : MonoBehaviour
{
#if !UNITY_WEBGL && !WINDOWS_UWP
	private VOIP voip;
	public string hostAddress;
	public ushort hostPort;
	public bool isLocalTesting;
	public VOIP.Quality MicQuality = VOIP.Quality.High;

	public void Start()
	{
		voip = new VOIP(0.0f, MicQuality);
		voip.IsLocalTesting = isLocalTesting;

		//if (NetworkManager.Instance == null) return;

		//var client = new UDPServer(2);
		//client.Connect("127.0.0.1", 12345);
		//NetworkManager.Instance.Initialize(client);

		//Debug.Log(NetworkManager.Instance.Networker == null);

		//if (NetworkManager.Instance.IsServer)
			voip.StartServer(32, hostAddress, hostPort);
		//else
		//	voip.StartClient(hostAddress, hostPort);
	}

	public void Stop()
	{
		voip.StopSending();
	}

	private void Update()
	{
		voip.Update();
	}

	private void OnApplicationQuit()
	{
		Stop();
	}
#endif
}