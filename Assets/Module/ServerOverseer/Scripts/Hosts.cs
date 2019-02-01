//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ServerOverseer
{

	public struct HostStub
	{
		public PlayerStub[] connectedPlayers;
		public string hostName;
		public string comment;
		public int playerLimit;

		public HostStub(PlayerStub[] connectedPlayers, string hostName, string comment, int playerLimit)
		{
			this.connectedPlayers = connectedPlayers;
			this.hostName = hostName;
			this.comment = comment;
			this.playerLimit = playerLimit;
		}
	}

	/// <summary>
	/// This is an instance of another host (aka client), not the MasterServer itself
	/// The MasterServer is its own class
	/// It will hold various peer information
	/// </summary>
	public class HostInstance
	{
		// fill out whatever type of player Host/Client information you want to have here, and update constructor
		public Players ConnectedPlayers { get; private set; }
		public string HostName { get; private set; }
		public string Comment { get; private set; }
		public string Password { get; private set; }
		public string IpAddress { get; private set; }
		public int HostID { get; private set; }
		public int PlayerLimit { get; private set; }
		public int ConnectionId { get; private set; }

		/// <summary>
		/// Default constructor private on purpose, always use public constructor
		/// </summary>
		private HostInstance()
		{
			ConnectedPlayers = new Players();
		}

		/// <summary>
		/// Public constructor for a new Host
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="comment"></param>
		/// <param name="password"></param>
		/// <param name="ipAddress"></param>
		/// <param name="playerLimit"></param>
		/// <param name="connectionId"></param>
		public HostInstance(string hostName, string comment, string password, string ipAddress, int playerLimit, int connectionId) : this()
		{
			HostName = hostName;
			Comment = comment;
			Password = password;
			IpAddress = ipAddress;
			PlayerLimit = playerLimit;
			ConnectionId = connectionId;
			//Debug.Log("<Hosts> HostInstance [" + hostName + "] Created");
		}

		/// <summary>
		/// Destructor - free references
		/// </summary>
		~HostInstance()
		{
			ConnectedPlayers.ClearPlayers();
			ConnectedPlayers.Instances.Clear();
			ConnectedPlayers.Instances = null;
			ConnectedPlayers = null;
		}

		/// <summary>
		/// Add an already created player to a host.  A player can only exist in one host at a time,
		/// unless you modify some logic to work around it
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool AddPlayer(PlayerInstance p)
		{
			return ConnectedPlayers.AddPlayer(p);
		}

		/// <summary>
		/// Add a new player to a host.  A player can only exist in one host at a time,
		/// unless you modify some logic to work around it
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool AddPlayer(string playerKey, string playerValue, int flags)
		{
			return ConnectedPlayers.AddPlayer(playerKey, playerValue, flags, DateTime.UtcNow, this);
		}

		/// <summary>
		/// Remove player from a host.  It is only checking this HostInstance's Players collection for removal.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public PlayerInstance RemovePlayer(PlayerInstance p)
		{
			return ConnectedPlayers.RemovePlayer(p.PlayerKey);
		}

		/// <summary>
		/// Remove player from a host.  It is only checking this HostInstance's Players collection for removal.
		/// </summary>
		/// <param name="playerKey"></param>
		/// <returns></returns>
		public PlayerInstance RemovePlayer(string playerKey)
		{
			return ConnectedPlayers.RemovePlayer(playerKey);
		}

		/// <summary>
		/// Clear all players from collection
		/// </summary>
		public void ClearPlayers()
		{
			ConnectedPlayers.ClearPlayers();
		}

		/// <summary>
		/// Returns an array of all the player instances for this particular HostInstance
		/// </summary>
		/// <returns></returns>
		public PlayerInstance[] GetPlayerInstances()
		{
			return ConnectedPlayers.GetInstances();
		}

		/// <summary>
		/// Password match.  If no password was set for HostInstance, then it will always succeed.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool VerifyPassword(string password)
		{
			bool result = false;
			if (string.IsNullOrEmpty(Password) || Password.Equals(password))
				result = true;
			return result;
		}
	}

	/// <summary>
	/// This holds ALL off the instances of the other servers and manages adding/removing them
	/// along with any service instance specific methods
	/// There is a single instance of this only
	/// </summary>
	public class Hosts
	{
		/// <summary>
		/// Collection of all connected HostInstances
		/// </summary>
		public Dictionary<string, HostInstance> Connected = new Dictionary<string, HostInstance>();

		private HostInstance _temp = null;

		#region --------------- Host Management -----------------

		/// <summary>
		/// Add a new Host to the collection of Connected HostInstances
		/// Does not check if HostID is unique - Name must be unique
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="comment"></param>
		/// <param name="password"></param>
		/// <param name="ipAddress"></param>
		/// <param name="playerLimit"></param>
		/// <param name="connectionId"></param>
		/// <returns></returns>
		public bool AddHost(string hostName, string comment, string password, string ipAddress, int playerLimit, int connectionId)
		{
			bool result = false;
			if (!Connected.ContainsKey(hostName))
			{
				Connected.Add(hostName, new HostInstance(hostName, comment, password, ipAddress, playerLimit, connectionId));
				result = true;
				if (MasterServer.ViewDebugMessages)
					Debug.Log("<Hosts> Host Instance [" + hostName + "] added successfully.");
			}
			else
			{
				Debug.LogError("<Hosts> Host Instance [" + hostName + "] duplicate detected.  Add Failure.");
			}
			return result;
		}

		/// <summary>
		/// Remove Host from Collection of Connected HostInstances
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="hostPassword">Optional</param>
		/// <param name="forceRemoval">Overrides password check</param>
		/// <returns></returns>
		public bool RemoveHost(string hostName, string hostPassword, bool forceRemoval = false)
		{
			bool result = false;
			if (Connected.ContainsKey(hostName))
			{
				_temp = Connected[hostName];

				// only get removed if allowed
				if (forceRemoval || _temp.VerifyPassword(hostPassword))
					result = Connected.Remove(hostName);
				if (MasterServer.ViewDebugMessages)
					Debug.Log("<Hosts> Host Instance [" + hostName + "] removed successfully.");
			}
			return result;
		}

		/// <summary>
		/// Gets an array of Connected HostInstances
		/// </summary>
		/// <returns></returns>
		public HostInstance[] GetInstances()
		{
			return Connected.Values.ToArray();
		}
		#endregion


		#region --------------- Players Management -----------------

		/// <summary>
		/// Add Playter to specifc HostInstance
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool AddPlayer(string hostName, string playerKey, string playerValue, int flags)
		{
			return Connected[hostName].AddPlayer(playerKey, playerValue, flags);
		}

		/// <summary>
		/// Add Playter to specifc HostInstance
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool AddPlayer(string hostName, PlayerInstance player)
		{
			return Connected[hostName].AddPlayer(player);
		}
		
		/// <summary>
		/// Remove Player from any/all Host instances
		/// It will stop checking on first match, so if somehow have duplicates, which you shouldn't, you could
		/// make this not stop when it finds a match or put in an optional check for all HostInstances
		/// </summary>
		/// <param name="playerKey"></param>
		/// <returns></returns>
		public PlayerInstance RemovePlayer(string playerKey)
		{
			PlayerInstance result = null;
			foreach (var host in Connected)
			{
				if ((result = host.Value.ConnectedPlayers.RemovePlayer(playerKey)) != null)
					break;
			}
			return result;
		}

		/// <summary>
		/// Find Player in any/all HostInstances
		/// It will stop checking on first match, so if somehow have duplicates, which you shouldn't, you could
		/// make this not stop when it finds a match or put in an optional check for all HostInstances
		/// </summary>
		/// <param name="playerKey"></param>
		/// <returns></returns>
		public PlayerInstance FindPlayer(string playerKey)
		{
			PlayerInstance result = null;
			foreach (var host in Connected)
			{
				if ((result = host.Value.ConnectedPlayers.FindPlayer(playerKey)) != null)
					break;
			}
			return result;
		}

		/// <summary>
		/// Find Player in SPECIFIC HostInstance
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="playerValue"></param>
		/// <returns></returns>
		public PlayerInstance FindPlayer(string hostName, string playerKey)
		{
			PlayerInstance result = null;
			if (Connected.ContainsKey(hostName))
				result = Connected[hostName].ConnectedPlayers.FindPlayer(playerKey);
			return result;
		}

		/// <summary>
		/// Update a player's values
		/// CANNOT change the key.  Remove/add new player if this is what you want.
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool UpdatePlayer(string playerKey, string playerValue, int flags)
		{
			bool result = false;
			PlayerInstance pl = null;
			foreach (var host in Connected)
			{
				if ((pl = host.Value.ConnectedPlayers.FindPlayer(playerKey)) != null)
				{
					pl.PlayerValue = playerValue;
					pl.Flags = flags;
					result = true;
				}
			}
			return result;
		}

		/// <summary>
		/// Gets a List of 'PlayerStubs' to send across the network
		/// Finds them in ALL host instances
		/// </summary>
		/// <returns></returns>
		public List<PlayerStub> GetPlayerStubs()
		{
			List<PlayerStub> stubs = new List<PlayerStub>();
			foreach (var host in Connected)
				stubs.AddRange(host.Value.ConnectedPlayers.GetPlayerStubs());
			return stubs;
		}

		/// <summary>
		/// Gets a List of 'PlayerStubs' to send across the network for specific HostInstance
		/// </summary>
		/// <returns></returns>
		public List<PlayerStub> GetPlayerStubs(string hostName)
		{
			List<PlayerStub> stubs = new List<PlayerStub>();
			if (Connected.ContainsKey(hostName))
				stubs.AddRange(Connected[hostName].ConnectedPlayers.GetPlayerStubs());

			return stubs;
		}

		#endregion
	}
}

