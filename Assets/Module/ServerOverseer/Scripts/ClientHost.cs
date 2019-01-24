//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ServerOverseer
{
	/// <summary>
	/// This is could be considered a client connecting to the master server, or if it's
	/// not an actual client, it's just a host.  It could have a bunch of different weird names.
	/// Things of it like a wheel with spokes.
	/// MasterServer is at the center.
	/// Each spoke ends in a ClientHost.  It's authoritative.  There is no ClientHost <-> ClientHost relationships.
	/// Eveyrthing goes through the MasterServer
	/// </summary>
	public class ClientHost : MonoBehaviour
	{
		// singleton
		public static ClientHost Instance { get; private set; }

		// IP address  127.0.0.1 works for localhost
		public string MasterServerIpAddress;

		// port MasterServer is set to run on
		public int MasterServerPort;

		// only set the password if the MasterServer has a password
		public string MasterServerPassword;

		// During Unity's Start phase, do you want this to automatically connect?
		public bool ConnectOnStart = true;

		// turn off if you don't want a bunch of debug messages
		public bool IsDebug = true;

		// The amount of time the client will wait before attempting another connection in Seconds
		// Note, updating this after startup will do nothing.  Update the WaitForSeconds instance below instead.
		[Header("------  Values are in Seconds  ------")]
		public float RetryAttemptTime = 0.6f;

		// The actual UNet client connection
		public NetworkClient Client { get; private set; }

		// cache these!
		private WaitForSeconds _waitRetryTime = null;

		// quick/easy check
		public bool IsConnected
		{
			get
			{
				if (Client == null)
					return false;
				else
					return Client.isConnected;
			}
		}

		/// <summary>
		/// Unity Awake
		/// </summary>
		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				Application.runInBackground = true;
				Application.targetFrameRate = 60;
				Client = null;
				_waitRetryTime = new WaitForSeconds(RetryAttemptTime);
				DontDestroyOnLoad(gameObject);
				InitializeClient();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		/// <summary>
		/// Unity Start
		/// </summary>
		void Start()
		{
			if (ConnectOnStart)
				StartCoroutine(RetryConnection(false));
		}

		#region --------------- Client Management -----------------
		/// <summary>
		/// Initialize the NetworkClient if it does not exist
		/// This does not create a connection, only intitialize the event handlers / client
		/// </summary>
		void InitializeClient()
		{
			if (Client == null)
			{
				// create new client, but do not connect
				Client = new NetworkClient();

				// system msgs
				RegisterDefaultHandler(MsgType.Connect);
				RegisterDefaultHandler(MsgType.Disconnect);
				RegisterDefaultHandler(MsgType.Error);

				// application msgs
				RegisterDefaultHandler(MessageTypes.RegisterHostResponse);
				RegisterDefaultHandler(MessageTypes.UnregisterHostResponse);
				RegisterDefaultHandler(MessageTypes.IsPlayerConnectedResponse);
				RegisterDefaultHandler(MessageTypes.ListAllPlayersResponse);
				RegisterDefaultHandler(MessageTypes.AddPlayerResponse);
				RegisterDefaultHandler(MessageTypes.RemovePlayerResponse);
				RegisterDefaultHandler(MessageTypes.ValidatePasswordResponse);
				RegisterDefaultHandler(MessageTypes.UpdatePlayerResponse);
				RegisterDefaultHandler(MessageTypes.UpdatePlayerHostResponse);
				RegisterDefaultHandler(MessageTypes.ListHostsResponse);
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Debug.LogError("<ClientHost> Already connected to MasterServer");
			}
		}

		/// <summary>
		/// For some reasom, UNet's UnregisterHandler method unregisters ALL delegates from an event handler and does not allow specific removals
		/// As such, if any Unregisters are performed, the default handlers should still be used
		/// </summary>
		/// <param name="messageType"></param>
		void RegisterDefaultHandler(short messageType)
		{
			NetworkMessageDelegate handler = null;
			switch (messageType)
			{
				case (int)MsgType.Connect: handler = OnClientConnect; break;
				case (int)MsgType.Disconnect: handler = OnClientDisconnect; break;
				case (int)MsgType.Error: handler = OnClientError; break;
				case (int)MessageTypes.RegisterHostResponse: handler = OnRegisterHostResponse; break;
				case (int)MessageTypes.UnregisterHostResponse: handler = OnUnregisterHostResponse; break;
				case (int)MessageTypes.IsPlayerConnectedResponse: handler = OnIsPlayerConnectedResponse; break;
				case (int)MessageTypes.ListAllPlayersResponse: handler = OnListAllPlayersReponse; break;
				case (int)MessageTypes.AddPlayerResponse: handler = OnAddPlayerReponse; break;
				case (int)MessageTypes.RemovePlayerResponse: handler = OnRemovePlayerReponse; break;
				case (int)MessageTypes.ValidatePasswordResponse: handler = OnValidatePasswordResponse; break;
				case (int)MessageTypes.UpdatePlayerResponse: handler = OnUpdatePlayerResponse; break;
				case (int)MessageTypes.UpdatePlayerHostResponse: handler = OnUpdatePlayerHostResponse; break;
				case (int)MessageTypes.ListHostsResponse: handler = OnListHostsResponse; break;
				default:
					break;
			}

			if (handler != null)
				Client.RegisterHandler(messageType, handler);
			else
				Debug.LogError("<ClientHost> RegisterDefaultHandler could not find default handler");
		}

		/// <summary>
		/// Register an Event Handler for a specific event response
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="handler"></param>
		public void RegisterHandler(short messageType, NetworkMessageDelegate handler)
		{
			if (Client != null)
				Client.RegisterHandler(messageType, handler);
			else
				Debug.LogError("<ClientHost> RegisterHandler attempted for a nonexistent client instance");
		}

		/// <summary>
		/// Unregister an Event Handler for a specific event response
		/// It will remove all handlers, then add the default handler used by the Client instance
		/// </summary>
		/// <param name="messageType"></param>
		public void UnregisterHandler(short messageType)
		{
			if (Client != null)
			{
				Client.UnregisterHandler(messageType);
				RegisterDefaultHandler(messageType);
			}
		}

		/// <summary>
		/// Disconnects the Client object instance and nulls it
		/// </summary>
		public void ResetClient()
		{
			if (Client != null)
			{
				if (Client.isConnected)
					Client.Disconnect();
				
				// do not destroy the client, we are going to reuse it to reconnect				
				//Client = null;
			}
		}

		/// <summary>
		/// Will attempt to continuously reconnect until connected
		/// </summary>
		/// <param name="initialPause"></param>
		/// <returns></returns>
		IEnumerator RetryConnection(bool initialPause)
		{
			if (initialPause)
				yield return _waitRetryTime;

			while (!IsConnected)
			{
				Connect();
				yield return _waitRetryTime;
			}
			yield return null;
		}

		/// <summary>
		/// Attempts to connect to the master server
		/// </summary>
		public void Connect()
		{
			if (!IsConnected)
			{
				Debug.Log("<ClientHost> Client Attempting to Connect to Master @ " + MasterServerIpAddress + ":" + MasterServerPort);

				// fix for retrying connection limit of 16 - http://forum.unity3d.com/threads/maximum-hosts-cannot-exceed-16.359579/
				if (Client.connection != null)
					NetworkTransport.RemoveHost(Client.connection.hostId);

				Client.Connect(MasterServerIpAddress, MasterServerPort);
			}
			else
			{
				Debug.LogError("<ClientHost> Client Conneciton failed, already connected to MasterServer");
			}
		}

		/// <summary>
		/// Client-initiated disconnect from the master server
		/// Does not attempt any sort of reconnection
		/// </summary>
		public void Disconnect()
		{
			Debug.Log("<ClientHost> Client Initiated Disconnect from Master");
			ResetClient();

			// do not destroy the NetworkManager objects, we are going to reuse it to reconnect
			//if (NetworkManager.singleton != null)
			//{
			//	NetworkManager.singleton.StopServer();
			//	NetworkManager.singleton.StopClient();
			//}

			if (RetryAttemptTime > 0)
				StartCoroutine(RetryConnection(true));
		}
		#endregion


		#region --------------- System Handlers -----------------
		/// <summary>
		/// Event Handler for System Event Connect
		/// </summary>
		/// <param name="netMsg"></param>
		void OnClientConnect(NetworkMessage netMsg)
		{
			Debug.Log("<ClientHost> Client Connected to MasterServer @ " + MasterServerIpAddress + ":" + MasterServerPort);

			if (!string.IsNullOrEmpty(MasterServerPassword))
			{
				if (IsConnected)
				{
					Debug.Log("<ClientHost> Attempting ValidatePassword with MasterServer");
					var msg = new MessageTypes.ValidatePasswordMessage();
					msg.password = MasterServerPassword;
					Client.Send(MessageTypes.ValidatePassword, msg);
				}
				else
				{
					Debug.LogError("<ClientHost> ValidatePassword failed, host not connected to MasterServer");
				}
			}
			else
			{
				// if not using a password, here is where you would set it to automatically register host if desired
				// otherwise, you would put this in the ValidatePasswordResponse event handler
				// These are values I use, you'd use whatever you like for identifiers / values
				//RegisterHost(ServerHandler.Instance.SceneID.ToString(), "", "", (int)ServerHandler.Instance.SceneID, 256);
			}
		}

		/// <summary>
		/// Event Handler for System Event Disconnect
		/// </summary>
		/// <param name="netMsg"></param>
		void OnClientDisconnect(NetworkMessage netMsg)
		{
			Debug.LogWarning("<ClientHost> Disconnected from MasterServer");
			ResetClient();
		}

		/// <summary>
		/// Event Handler for System Event Error
		/// </summary>
		/// <param name="netMsg"></param>
		void OnClientError(NetworkMessage netMsg)
		{
			Debug.LogError("<ClientHost> Client Error from MasterServer");
		}
		#endregion


		#region --------------- Application Handlers -----------------
		/// <summary>
		/// Event Handler for Application Event RegisterHostResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnRegisterHostResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.RegisterHostResponseMessage>();
			switch ((MessageTypes.CustomEventType)msg.resultCode)
			{
				default:
					Debug.LogError("<ClientHost> Host Registration Failed, Reason: " + (MessageTypes.CustomEventType)msg.resultCode);
					break;
				case MessageTypes.CustomEventType.NoError:
					Debug.Log("<ClientHost> Host Registration Successful");
					break;
			}
		}

		/// <summary>
		/// Event Handler for Application Event UnregisterHostResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUnregisterHostResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.UnregisterHostResponseMessage>();
			switch ((MessageTypes.CustomEventType)msg.resultCode)
			{
				default:
					Debug.LogError("<ClientHost> Host Unregistration Failed, Reason: " + (MessageTypes.CustomEventType)msg.resultCode);
					break;
				case MessageTypes.CustomEventType.NoError:
					Debug.Log("<ClientHost> Host Unregistration Successful");
					break;
			}
		}

		/// <summary>
		/// Event Handler for Application Event IsPlayerConnectedResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnIsPlayerConnectedResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.IsPlayerConnectedResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnIsPlayerConnectedResponse result: " + msg.result);
		}

		/// <summary>
		/// Event Handler for Application Event AddPlayerResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnAddPlayerReponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.AddPlayerResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnAddPlayerReponse: " + (MessageTypes.CustomEventType)msg.resultCode);
		}

		/// <summary>
		/// Event Handler for Application Event RemovePlayerResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnRemovePlayerReponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.RemovePlayerResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnRemovePlayerReponse: " + msg.result);
		}

		/// <summary>
		/// Event Handler for Application Event ListAllPlayersResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnListAllPlayersReponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.ListAllPlayersResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnListAllPlayersReponse: " + msg.result);

			if (!msg.result)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Error finding players");
			}
			else if (msg.players.Length > 0)
			{
				// ----> Here is where you would build / send the string of all players to whatever you want
				//for (int i = 0; i < msg.players.Length; i++)
				//	Debug.Log("Player - " + msg.players[i].playerKey + ":" + msg.players[i].hostName);
			}
			else
			{
				if (IsDebug)
					Debug.Log("<ClientHost> No Players Found");
			}
		}

		/// <summary>
		/// Event Handler for Application Event ValidatePasswordResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnValidatePasswordResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.ValidatePasswordResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnValidatePasswordResponse: " + msg.result);

			// if not using a password, here is where you would set it to automatically register host if desired
			// otherwise, you would put this in the ValidatePasswordResponse event handler
			// These are values I use, you'd use whatever you like for identifiers / values
			//if (msg.result)
				//RegisterHost(ServerHandler.Instance.SceneID.ToString(), "", "", (int)ServerHandler.Instance.SceneID, 256);
		}

		/// <summary>
		/// Event Handler for Application Event UpdatePlayerResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUpdatePlayerResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.UpdatePlayerResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnUpdatePlayerResponse: " + msg.result);
		}

		/// <summary>
		/// Event Handler for Application Event UpdatePlayerHostHostResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUpdatePlayerHostResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.UpdatePlayerHostResponseMessage>();
			if (IsDebug)
				Debug.Log("<ClientHost> OnUpdatePlayerHostResponse result: " + (MessageTypes.CustomEventType)msg.resultCode);
		}

		/// <summary>
		/// Event Handler for Application Event ListHostsResponse
		/// </summary>
		/// <param name="netMsg"></param>
		void OnListHostsResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.ListHostsResponseMessage>();
			if (IsDebug)
				Debug.Log("---------------<ClientHost> OnListHostsResponse result: " + (MessageTypes.CustomEventType)msg.resultCode);
		}

		#endregion


		#region --------------- Host Management -----------------
		/// <summary>
		/// Register a host with the MasterServer
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="comment"></param>
		/// <param name="hostPassword"></param>
		/// <param name="hostID"></param>
		/// <param name="playerLimit"></param>
		/// <param name="response"></param>
		public void RegisterHost(string hostName, string comment, string hostPassword, int playerLimit, NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Attempting to register host with MasterServer");
				var msg = new MessageTypes.RegisterHostMessage();
				msg.hostName = hostName;
				msg.hostPassword = hostPassword;
				msg.comment = comment;
				msg.playerLimit = playerLimit;
				Client.Send(MessageTypes.RegisterHost, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.RegisterHostResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> RegisterHost failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Unregister (remove) a host from the MasterServer
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="response"></param>
		public void UnregisterHost(string hostName, string hostPassword = "", NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Attempting to UN-register host with MasterServer");
				var msg = new MessageTypes.UnregisterHostMessage();
				msg.hostName = hostName;
				msg.hostPassword = hostPassword;
				Client.Send(MessageTypes.UnregisterHost, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.UnregisterHostResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> UnregisterHost failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Send request to MasterServer for a list of all connected hosts
		/// </summary>
		/// <param name="hostPassword"></param>
		/// <param name="response"></param>
		public void ListHosts(string hostPassword = "", NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> ListHosts attempting");
				var msg = new MessageTypes.ListHostsMessage();
				msg.serverPassword = hostPassword;
				Client.Send(MessageTypes.ListHosts, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.ListHostsResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> ListHosts failed, host not connected to MasterServer");
			}

		}
		#endregion


		#region --------------- Players Management -----------------

		/// <summary>
		/// Add a player with a Host reference/location with the MasterServer
		/// Valid Hostname required
		/// Flags can be used for view options (invisible, admin group, etc.)
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <param name="hostName"></param>
		/// <param name="hostPassword"></param>
		/// <param name="response"></param>
		public void AddPlayer(string playerKey, string playerValue, int flags, string hostName, string hostPassword = "", NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Attempting to add player to host");
				var msg = new MessageTypes.AddPlayerMessage();
				msg.playerKey = playerKey;
				msg.playerValue = playerValue;
				msg.flags = flags;
				msg.hostName = hostName;
				msg.hostPassword = hostPassword;
				Client.Send(MsgType.AddPlayer, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.AddPlayerResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> AddPlayer failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Remove player from MasterServer host
		/// If no HostName is provided, or player cannot be found on HostName specified,
		/// MasterServer will check all of its host instances to remove player
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="hostName"></param>
		/// <param name="response"></param>
		public void RemovePlayer(string playerKey, string hostName = "", NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Attempting to remove player from host");
				var msg = new MessageTypes.RemovePlayerMessage();
				msg.playerKey = playerKey;
				msg.hostName = hostName;
				Client.Send(MsgType.RemovePlayer, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.RemovePlayerResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> RemovePlayer failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Get list of players from specific host on MasterServer
		/// If no host is specified, will return all players from all hosts
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="response"></param>
		public void ListPlayers(string hostName = "", NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Attempting to get list of players from host: " + (string.IsNullOrEmpty(hostName) ? "All Hosts" : hostName));
				var msg = new MessageTypes.ListAllPlayersMessage();
				msg.hostName = hostName;
				Client.Send(MessageTypes.ListAllPlayers, msg);
	
				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.ListAllPlayersResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> ListPlayers failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Send message to server to check all hosts whether a player is found there or not
		/// Upon finding a match (or not) will respond to default event handler, and custom handler if designated
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="response"></param>
		public void IsPlayerConnected(string playerKey, NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Checking if Player [" + playerKey + "] is connected to a Host");
				var msg = new MessageTypes.IsPlayerConnectedMessage();
				msg.playerKey = playerKey;
				Client.Send(MessageTypes.IsPlayerConnected, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.IsPlayerConnectedResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> IsPlayerConnected failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Send message to server to update a player
		/// Cannot change the actual playerKey.  if that is desired, remove player / add new player
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <param name="response"></param>
		public void UpdatePlayer(string playerKey, string playerValue, int flags, NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Updating Player [" + playerKey + "]");
				var msg = new MessageTypes.UpdatePlayerMessage();
				msg.playerKey = playerKey;
				msg.playerValue = playerValue;
				msg.flags = flags;
				Client.Send(MessageTypes.UpdatePlayer, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.UpdatePlayerResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> UpdatePlayer failed, host not connected to MasterServer");
			}
		}

		/// <summary>
		/// Update which host the player is associated with
		/// Needs the key for the player and a valid key for the host
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="hostKey"></param>
		/// <param name="response"></param>
		public void UpdatePlayerHost(string playerKey, string hostKey, NetworkMessageDelegate response = null)
		{
			if (IsConnected)
			{
				if (IsDebug)
					Debug.Log("<ClientHost> Player [" + playerKey + "] updating host to [" + hostKey + "]");
				var msg = new MessageTypes.UpdatePlayerHostMessage();
				msg.playerKey = playerKey;
				msg.hostName = hostKey;
				Client.Send(MessageTypes.UpdatePlayerHost, msg);

				// use response type for handler!
				if (response != null)
					RegisterHandler(MessageTypes.UpdatePlayerHostResponse, response);
			}
			else
			{
				Debug.LogError("<ClientHost> UpdatePlayerHost failed, host not connected to MasterServer");
			}
		}
		#endregion
	}
}

