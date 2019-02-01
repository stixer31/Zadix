//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ServerOverseer
{
	/// <summary>
	/// The master server itself.  There is only one instance of this ever.
	/// 
	/// Master Server holds multiple references to other Hosts
	/// 
	/// You can protect who can register with the master server with a password; however, ideally you wany to prevent it with a firewall/etc.
	/// </summary>
	public class MasterServer : MonoBehaviour
	{
		// singleton
		public static MasterServer Instance = null;

		// This is set during awake
		public static bool ViewDebugMessages = true;

		// The master server starts on localhost IP address, so if you're sure what the IP address
		// for the Hosts to connect, you'll have to figure it out
		public int MasterServerPort;

		// Leave password empty if you are not password protecting the master server connection
		// Ideally you use a firewall to restrict who can connect / attempt to connect
		public string MasterServerPassword;

		// Maximum amount of players (not hosts) allowed from any/all hosts
		// leaving this at zero or negative means there is no limit
		public int MaxPlayerLimitForAllHosts;

		// Whether you want to UI enabled, must have a MasterServerUI behavior set somewhere to actually use this
		public bool EnableUI;

		// turn off if you don't want a bunch of debug messages - note, changing at runtime does nothing.  Change ViewDebugMessages instead
		public bool IsDebug = true;

		// create list of servers we can reference
		public Hosts ConnectedHosts { get; private set; }

		// delegate method for updating the UI when something changes
		public UpdateUIDelegate UpdateUIMethod { get; set; }

		// keep track of connections since we have to validate them ONLY if we're using a password to protect the server
		public Dictionary<int, bool> HostConnections { get; private set; }

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
				ViewDebugMessages = IsDebug;
				ConnectedHosts = new Hosts();
				HostConnections = new Dictionary<int, bool>();
				DontDestroyOnLoad(gameObject);
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
			StartServer();
		}

		/// <summary>
		/// Start the MasterServer if applicable
		/// If you are using UNet in other players, you may want to start this directly from there
		/// </summary>
		public void StartServer()
		{
			Players.CurrentPlayerCount = 0;

			if (!NetworkServer.active)
			{
				Debug.Log("<MasterServer> Starting on Port " + MasterServerPort);
				NetworkServer.Listen(MasterServerPort);

				// system msgs
				RegisterDefaultHandler(MsgType.Connect);
				RegisterDefaultHandler(MsgType.Disconnect);
				RegisterDefaultHandler(MsgType.Error);
				RegisterDefaultHandler(MsgType.AddPlayer);
				RegisterDefaultHandler(MsgType.RemovePlayer);

				// application msgs
				RegisterDefaultHandler(MessageTypes.RegisterHost);
				RegisterDefaultHandler(MessageTypes.UnregisterHost);
				RegisterDefaultHandler(MessageTypes.IsPlayerConnected);
				RegisterDefaultHandler(MessageTypes.ListAllPlayers);
				RegisterDefaultHandler(MessageTypes.ValidatePassword);
				RegisterDefaultHandler(MessageTypes.UpdatePlayer);
				RegisterDefaultHandler(MessageTypes.UpdatePlayerHost);
				RegisterDefaultHandler(MessageTypes.ListHosts);
			}
			else
			{
				Debug.LogError("<MasterServer> Already Initialized");
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
				case (int)MsgType.Connect: handler = OnHostConnect; break;
				case (int)MsgType.Disconnect: handler = OnHostDisconnect; break;
				case (int)MsgType.Error: handler = OnHostError; break;
				case (int)MsgType.AddPlayer: handler = OnAddPlayer; break;
				case (int)MsgType.RemovePlayer: handler = OnRemovePlayer; break;
				case (int)MessageTypes.RegisterHost: handler = OnRegisterHost; break;
				case (int)MessageTypes.UnregisterHost: handler = OnUnregisterHost; break;
				case (int)MessageTypes.IsPlayerConnected: handler = OnIsPlayerConnected; break;
				case (int)MessageTypes.ListAllPlayers: handler = OnListAllPlayers; break;
				case (int)MessageTypes.ValidatePassword: handler = OnValidatePassword; break;
				case (int)MessageTypes.UpdatePlayer: handler = OnUpdatePlayer; break;
				case (int)MessageTypes.UpdatePlayerHost: handler = OnUpdatePlayerHost; break;
				case (int)MessageTypes.ListHosts: handler = OnListHosts; break;
				default:
					break;
			}

			if (NetworkServer.active)
				NetworkServer.RegisterHandler(messageType, handler);
			else
				Debug.LogError("<MasterServer> RegisterDefaultHandler could not find default handler");
		}

		/// <summary>
		/// Register an Event Handler for a specific event response
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="handler"></param>
		public void RegisterHandler(short messageType, NetworkMessageDelegate handler)
		{
			if (!NetworkServer.active)
				Debug.LogError("<MasterServer> RegisterHandler attempted inactive NetworkServer");
			else
				NetworkServer.RegisterHandler(messageType, handler);
		}

		/// <summary>
		/// Unregister an Event Handler for a specific event response
		/// It will remove all handlers, then add the default handler used by the Client instance
		/// </summary>
		/// <param name="messageType"></param>
		public void UnregisterHandler(short messageType)
		{
			if (NetworkServer.active)
			{
				NetworkServer.UnregisterHandler(messageType);
				RegisterDefaultHandler(messageType);
			}
		}

		/// <summary>
		/// Shutdown the Master Server
		/// </summary>
		public void ShutdownServer()
		{
			Debug.Log("<MasterServer> Shutdown");
			NetworkServer.Shutdown();
		}

		/// <summary>
		/// If the UI is going to be updated, call this method which will check whether the 
		/// delegate method was set or not.  if it's not set, do nothing.  if it's set, call it.
		/// </summary>
		private void UpdateUI()
		{
			if (UpdateUIMethod != null)
				UpdateUIMethod();
		}

		#region --------------- System Handlers -----------------
		/// <summary>
		/// Event Handler for System Event Connect
		/// </summary>
		/// <param name="netMsg"></param>
		void OnHostConnect(NetworkMessage netMsg)
		{
			// if there is already a connection established, don't let them connect twice
			if (HostConnections.ContainsKey(netMsg.conn.connectionId))
			{
				Debug.Log("<MasterServer> Host Already Connected, New Connection Refused: " + netMsg.conn.connectionId);
			}
			else
			{
				Debug.Log("<MasterServer> Host Connected: " + netMsg.conn.connectionId);
				AddConnection(netMsg.conn);
			}
			UpdateUI();
		}

		/// <summary>
		/// Event Handler for System Event Disconnect
		/// </summary>
		/// <param name="netMsg"></param>
		void OnHostDisconnect(NetworkMessage netMsg)
		{
			Debug.Log("<MasterServer> Host Disconnected: " + netMsg.conn.connectionId);
			RemoveConnection(netMsg.conn);

			// remove the associated host
			foreach (var host in ConnectedHosts.Connected.Values)
			{
				// a network message does not have a custom identifier, so we have to check for matching connectionID
				if (host.ConnectionId == netMsg.conn.connectionId)
				{
					// tell other players?

					// remove room
					ConnectedHosts.RemoveHost(host.HostName, "", true);

					Debug.Log("<MasterServer> Host Instance [" + host.HostName + "] disconnected");
					break;
				}
			}
			UpdateUI();
		}

		/// <summary>
		/// Event Handler for System Event HostError
		/// </summary>
		/// <param name="netMsg"></param>
		void OnHostError(NetworkMessage netMsg)
		{
			Debug.Log("<MasterServer> OnHostError from Host: " + netMsg.conn.connectionId);
			UpdateUI();
		}
		#endregion


		#region --------------- Application Handlers -----------------

		/// <summary>
		/// Event Handler for Application Event RegisterHost
		/// </summary>
		/// <param name="netMsg"></param>
		void OnRegisterHost(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				MessageTypes.CustomEventType result;
				var msg = netMsg.ReadMessage<MessageTypes.RegisterHostMessage>();

				if (string.IsNullOrEmpty(msg.hostName))
					result = MessageTypes.CustomEventType.KeyEmpty;
				else if (ConnectedHosts.AddHost(msg.hostName, msg.comment, msg.hostPassword, netMsg.conn.address, msg.playerLimit, netMsg.conn.connectionId))
					result = MessageTypes.CustomEventType.NoError;
				else
					result = MessageTypes.CustomEventType.DuplicateDetected;

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnHostRegister result: " + result);

				var response = new MessageTypes.RegisterHostResponseMessage();
				response.resultCode = (int)result;
				netMsg.conn.Send(MessageTypes.RegisterHostResponse, response);

				UpdateUI();
			}
		}

		/// <summary>
		/// Event Handler for Application Event UnregisterHost
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUnregisterHost(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				MessageTypes.CustomEventType result = MessageTypes.CustomEventType.UnknownError;
				var msg = netMsg.ReadMessage<MessageTypes.UnregisterHostMessage>();

				// find the instance
				if (ConnectedHosts.RemoveHost(msg.hostName, msg.hostPassword))
					result = MessageTypes.CustomEventType.NoError;
				else
					Debug.LogError("<MasterServer> OnHostUnregister Host not found: " + msg.hostName);

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnHostUnregister result: " + result);

				// tell other hosts?

				var response = new MessageTypes.UnregisterHostResponseMessage();
				response.resultCode = (int)result;
				netMsg.conn.Send(MessageTypes.UnregisterHostResponse, response);

				UpdateUI();
			}
		}

		/// <summary>
		/// Event Handler for Application Event AddPlayer
		/// If a HostPassword is set on the HostInstance, it is required to add a new player to that host
		/// </summary>
		/// <param name="netMsg"></param>
		void OnAddPlayer(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				MessageTypes.CustomEventType result = MessageTypes.CustomEventType.UnknownError;
				var msg = netMsg.ReadMessage<MessageTypes.AddPlayerMessage>();

				if (string.IsNullOrEmpty(msg.playerKey))
				{
					result = MessageTypes.CustomEventType.KeyEmpty;
				}
				else if (!Players.IsPlayerCountValid(MaxPlayerLimitForAllHosts))
				{
					result = MessageTypes.CustomEventType.MaxPlayersExceeded;
				}
				else if (!string.IsNullOrEmpty(msg.hostName) && ConnectedHosts.Connected.ContainsKey(msg.hostName))
				{
					// check given host first for quick check, then check all hosts
					if (ConnectedHosts.FindPlayer(msg.hostName, msg.playerKey) != null)
					{
						result = MessageTypes.CustomEventType.DuplicateDetected;
					}
					else if (ConnectedHosts.FindPlayer(msg.playerKey) != null)
					{
						result = MessageTypes.CustomEventType.DuplicateDetected;
					}
					else if (!ConnectedHosts.Connected[msg.hostName].VerifyPassword(msg.hostPassword))
					{
						result = MessageTypes.CustomEventType.HostPasswordFail;
					}
					else
					{
						result = MessageTypes.CustomEventType.NoError;
						ConnectedHosts.AddPlayer(msg.hostName, msg.playerKey, msg.playerValue, msg.flags);
					}
				}
				else
				{
					result = MessageTypes.CustomEventType.HostUnknown;
				}

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnAddPlayer result: " + result);

				var response = new MessageTypes.AddPlayerResponseMessage();
				response.resultCode = (int)result;
				response.playerKey = msg.playerKey;
				netMsg.conn.Send(MessageTypes.AddPlayerResponse, response);

				UpdateUI();
			}
		}

		/// <summary>
		/// Event Handler for Application Event RemovePlayer
		/// Unlike AddPlayer, RemovePlayer does not require a HostPassword
		/// It is assumed that by now you're trusted and you can remove the player
		/// Also, this allows us to CYA by removing a player from ALL hosts if necessary
		/// </summary>
		/// <param name="netMsg"></param>
		void OnRemovePlayer(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				var msg = netMsg.ReadMessage<MessageTypes.RemovePlayerMessage>();
				bool result = false;

				if (!string.IsNullOrEmpty(msg.playerKey))
					result = (ConnectedHosts.RemovePlayer(msg.playerKey) != null);

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnRemovePlayer result: " + result);

				var response = new MessageTypes.RemovePlayerResponseMessage();
				response.result = result;
				response.playerKey = msg.playerKey;
				netMsg.conn.Send(MessageTypes.RemovePlayerResponse, response);

				UpdateUI();
			}
		}

		/// <summary>
		/// Event Handler for Application Event IsPlayerConnected
		/// </summary>
		/// <param name="netMsg"></param>
		void OnIsPlayerConnected(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				var msg = netMsg.ReadMessage<MessageTypes.IsPlayerConnectedMessage>();
				bool result = (ConnectedHosts.FindPlayer(msg.playerKey) != null);

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnIsPlayerConnected result: " + result);
				var response = new MessageTypes.IsPlayerConnectedResponseMessage();
				response.result = result;
				response.playerKey = msg.playerKey;
				netMsg.conn.Send((short)MessageTypes.IsPlayerConnectedResponse, response);
			}
		}

		/// <summary>
		/// Event Handler for Application Event ListAllPlayers
		/// </summary>
		/// <param name="netMsg"></param>
		void OnListAllPlayers(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				var msg = netMsg.ReadMessage<MessageTypes.ListAllPlayersMessage>();

				// no hostname provided, get all players from all hosts
				if (string.IsNullOrEmpty(msg.hostName))
				{
					if (MasterServer.ViewDebugMessages)
						Debug.Log("<MasterServer> OnListAllPlayers empty hostName, getting all players from all hosts");
					var response = new MessageTypes.ListAllPlayersResponseMessage();
					response.result = true;
					response.players = ConnectedHosts.GetPlayerStubs().ToArray();
					netMsg.conn.Send(MessageTypes.ListAllPlayersResponse, response);
				}
				// specifc hostname check
				else if (ConnectedHosts.Connected.ContainsKey(msg.hostName))
				{
					if (MasterServer.ViewDebugMessages)
						Debug.Log("<MasterServer> OnListAllPlayers getting all players from specific host: " + msg.hostName);
					var response = new MessageTypes.ListAllPlayersResponseMessage();
					response.result = true;
					response.players = ConnectedHosts.GetPlayerStubs(msg.hostName).ToArray();
					netMsg.conn.Send(MessageTypes.ListAllPlayersResponse, response);
				}
				// error
				else
				{
					if (MasterServer.ViewDebugMessages)
						Debug.Log("<MasterServer> OnListAllPlayers invalid hostName");
					var response = new MessageTypes.ListAllPlayersResponseMessage();
					response.result = false;
					netMsg.conn.Send(MessageTypes.ListAllPlayersResponse, response);
				}
			}
		}

		/// <summary>
		/// Event Handler for Application Event ValidatePassword
		/// </summary>
		/// <param name="netMsg"></param>
		void OnValidatePassword(NetworkMessage netMsg)
		{
			// make sure they've actually connected correctly first
			if (HostConnections.ContainsKey(netMsg.conn.connectionId))
			{
				bool result = false;
				var msg = netMsg.ReadMessage<MessageTypes.ValidatePasswordMessage>();

				if (!string.IsNullOrEmpty(msg.password) && msg.password.Equals(MasterServerPassword))
				{
					VerifyConnection(netMsg.conn, true);
					result = true;
				}

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnValidatePassword result: " + result);
				var response = new MessageTypes.ValidatePasswordResponseMessage();
				response.result = result;
				netMsg.conn.Send(MessageTypes.ValidatePasswordResponse, response);

				if (!result)
					DisconnectClient(netMsg.conn);

				UpdateUI();
			}
			else
			{
				Debug.LogError("<MasterServer> ConnectionID:" + netMsg.conn.connectionId + " attempting to ValidatePassword with MasterServer without Connecting");
			}
		}

		/// <summary>
		/// Event Handler for Application Event UpdatePlayer
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUpdatePlayer(NetworkMessage netMsg)
		{
			// make sure they've actually connected correctly first
			if (HostConnections.ContainsKey(netMsg.conn.connectionId))
			{
				bool result = false;
				var msg = netMsg.ReadMessage<MessageTypes.UpdatePlayerMessage>();

				result = ConnectedHosts.UpdatePlayer(msg.playerKey, msg.playerValue, msg.flags);
				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnUpdatePlayer result: " + result);

				var response = new MessageTypes.UpdatePlayerResponseMessage();
				response.playerKey = msg.playerKey;
				response.result = result;
				netMsg.conn.Send(MessageTypes.UpdatePlayerResponse, response);

				UpdateUI();
			}
			else
			{
				Debug.LogError("<MasterServer> ConnectionID:" + netMsg.conn.connectionId + " attempting to UpdatePlayer with MasterServer without Connecting");
			}
		}

		/// <summary>
		/// Event Handler for Application Event PlayerChangeHost
		/// </summary>
		/// <param name="netMsg"></param>
		void OnUpdatePlayerHost(NetworkMessage netMsg)
		{
			if (IsConnectionVerified(netMsg.conn))
			{
				var msg = netMsg.ReadMessage<MessageTypes.UpdatePlayerHostMessage>();
				MessageTypes.CustomEventType result = MessageTypes.CustomEventType.UnknownError;

				if (string.IsNullOrEmpty(msg.playerKey))
				{
					result = MessageTypes.CustomEventType.KeyEmpty;
				}
				else if (!string.IsNullOrEmpty(msg.hostName) && ConnectedHosts.Connected.ContainsKey(msg.hostName))
				{
					PlayerInstance player = null;
					// check given host first for quick check, then check all hosts
					if (!ConnectedHosts.Connected[msg.hostName].VerifyPassword(msg.hostPassword))
					{
						result = MessageTypes.CustomEventType.HostPasswordFail;
					}
					else if ((player = ConnectedHosts.RemovePlayer(msg.playerKey)) != null)
					{
						// successful removal, now add to new Host
						player.HostLocation = ConnectedHosts.Connected[msg.hostName];
						player.HostLoginTime = System.DateTime.UtcNow;
						ConnectedHosts.AddPlayer(msg.hostName, player);
						result = MessageTypes.CustomEventType.NoError;
					}
					else
					{
						result = MessageTypes.CustomEventType.DuplicateDetected;
					}
				}
				else
				{
					result = MessageTypes.CustomEventType.HostUnknown;
				}

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnUpdatePlayerHost result: " + result);
				var response = new MessageTypes.UpdatePlayerHostResponseMessage();
				response.resultCode = (int)result;
				response.playerKey = msg.playerKey;
				netMsg.conn.Send((short)MessageTypes.UpdatePlayerHostResponse, response);

				UpdateUI();
			}
		}

		/// <summary>
		/// Event Handler for Application Event ListHosts
		/// </summary>
		/// <param name="netMsg"></param>
		void OnListHosts(NetworkMessage netMsg)
		{
			Debug.Log("<MasterServer> OnListHosts invoked");
			if (IsConnectionVerified(netMsg.conn))
			{
				var msg = netMsg.ReadMessage<MessageTypes.ListHostsMessage>();
				MessageTypes.CustomEventType result = MessageTypes.CustomEventType.UnknownError;

				// only make this check is the master server has a password!
				if (string.IsNullOrEmpty(MasterServerPassword) || (!string.IsNullOrEmpty(msg.serverPassword) && MasterServerPassword.Equals(msg.serverPassword)))
				{
					// successful
					result = MessageTypes.CustomEventType.NoError;
				}
				else
				{
					result = MessageTypes.CustomEventType.ServerPasswordFail;
				}

				if (MasterServer.ViewDebugMessages)
					Debug.Log("<MasterServer> OnListHosts result: " + result);

				var response = new MessageTypes.ListHostsResponseMessage();

				// if there are hosts connected, build the array of HostStubs to send to the client
				if (ConnectedHosts.Connected.Count > 0)
				{
					response.resultCode = (int)MessageTypes.CustomEventType.NoError;
					var stubs = new List<HostStub>();

					// loop through the connected hosts, get info for each host, get the players for each host, and build stubs to send back
					foreach (var host in ConnectedHosts.Connected)
					{
						if (MasterServer.ViewDebugMessages)
							Debug.Log("<MasterServer> OnListHosts Adding Host: " + host.Value.HostName);
						stubs.Add(new HostStub(host.Value.ConnectedPlayers.GetPlayerStubs().ToArray(), host.Value.HostName, host.Value.Comment, host.Value.PlayerLimit));
					}
					response.hosts = stubs.ToArray();
				}
				else
				{
					response.resultCode = (int)MessageTypes.CustomEventType.NoHostsRegistered;
				}

				netMsg.conn.Send((short)MessageTypes.ListHostsResponse, response);
				UpdateUI();
			}
		}
		#endregion


		#region --------------- Connection Tracking / Verification -----------------
		/// <summary>
		/// Add a connection to the collection
		/// </summary>
		/// <param name="conn"></param>
		public void AddConnection(NetworkConnection conn)
		{
			// empty MasterServer password means true - enabled by default
			HostConnections.Add(conn.connectionId, string.IsNullOrEmpty(MasterServerPassword));
		}

		/// <summary>
		/// Verify a connection.  No MasterServerPassword set means everyone is verified by default.
		/// </summary>
		/// <param name="conn"></param>
		/// <param name="isVerified"></param>
		public void VerifyConnection(NetworkConnection conn, bool isVerified)
		{
			// empty MasterServer password means it will always be valid
			if (string.IsNullOrEmpty(MasterServerPassword))
				isVerified = true;

			if (HostConnections.ContainsKey(conn.connectionId))
				HostConnections[conn.connectionId] = isVerified;
		}

		/// <summary>
		/// Remove connection from the collection
		/// </summary>
		/// <param name="conn"></param>
		public void RemoveConnection(NetworkConnection conn)
		{
			HostConnections.Remove(conn.connectionId);
		}

		/// <summary>
		/// Check whether connection has been verified.  No MasterServerPassword set means everyone is verified by default.
		/// </summary>
		/// <param name="conn"></param>
		/// <returns></returns>
		public bool IsConnectionVerified(NetworkConnection conn)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(MasterServerPassword))
				HostConnections.TryGetValue(conn.connectionId, out result);
			else
				result = true;

			if (!result)
				Debug.LogError("<MasterServer> ConnectionID:" + conn.connectionId + " attempting to use MasterServer without validating password");

			return result;
		}

		/// <summary>
		/// Force a disconnect, flush the channels first so they get a reply if anything is there
		/// </summary>
		/// <param name="conn"></param>
		public void DisconnectClient(NetworkConnection conn)
		{
			if (MasterServer.ViewDebugMessages)
				Debug.Log("<MasterServer> Disconnecting ConnectionID:" + conn.connectionId);
			conn.FlushChannels();
			conn.Dispose();
			conn.Disconnect();
		}
		#endregion

	}
}
