//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ServerOverseer
{
	/// <summary>
	/// PlayerStub is just an abbreviated bit of data from the actual player
	/// Used when requesting a list of players from the MasterServer, so much
	/// less data is sent as the other information is not really needed
	/// </summary>
	public struct PlayerStub
	{
		public string playerKey;
		public string hostName;

		public PlayerStub(string playerKey, string hostName)
		{
			this.playerKey = playerKey;
			this.hostName = hostName;
		}
	}

	/// <summary>
	/// Instance of a Connected Player to a HostInstance
	/// There -should- be only a single Player per HostInstance
	/// If you want to have players in multiple HostInstances, you'll need to rework some methods
	/// </summary>
	public class PlayerInstance
	{
		public string PlayerKey { get; set; }
		public string PlayerValue { get; set; }
		public int HostFlags { get; set; }
		public DateTime ClientLoginTime { get; set; }
		public DateTime HostLoginTime { get; set; }
		public HostInstance HostLocation { get; set; }

		public bool IsPFileHandover { get; set; }
		public int SceneID { get; set; }
		public string Email { get; set; }
		public string Type { get; set; }
		public Guid GUID { get; set; }
		public string Name { get; set; }
		public string PassHash { get; set; }
		public string Salt { get; set; }
		public int Flags { get; set; }
		public int UnlockedClasses { get; set; }
		public int Wealth { get; set; }
		public int Fame { get; set; }
		public int Shards { get; set; }
		public short CharIDCounter { get; set; }
		public List<List<int>> CharSlots { get; set; }
		public string VerifyString { get; set; }
		public DateTime VerifyRequestTime { get; set; }
		public DateTime PasswordRequestTime { get; set; }
		public DateTime LastOnline { get; set; }
		public DateTime LockAccountTime { get; set; }
		public int SceneTarget { get; set; }
		public List<List<List<int>>> Vaults { get; set; }
		public List<List<int>> DeathRecords { get; set; }

		public bool IsLoaded { get; set; }
		public bool IsNewCharacter { get; set; }
		public int UpdateFlags { get; set; }
	

		/// <summary>
		/// Default constructor empty / unused
		/// </summary>
		private PlayerInstance()
		{
			Debug.LogError("<PlayerInstance> Incorrect constructor used");
		}

		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <param name="clientLoginTime"></param>
		/// <param name="location"></param>
		public PlayerInstance(string playerKey, string playerValue, int flags, DateTime clientLoginTime, HostInstance location)
		{
			PlayerKey = playerKey;
			PlayerValue = playerValue;
			Flags = flags;
			ClientLoginTime = clientLoginTime;
			HostLoginTime = DateTime.UtcNow;
			HostLocation = location;
			//Debug.Log("<PlayerInstance> " + playerName + " Created");
		}

		/// <summary>
		/// Destructor to free references
		/// </summary>
		~PlayerInstance()
		{
			HostLocation = null;
		}
	}

	/// <summary>
	/// This holds ALL off the instances of the other servers and manages adding/removing them
	/// along with any service instance specific methods
	/// There is a single instance of this only
	/// </summary>
	public class Players
	{
		// Track the PlayerCount via static value
		public static int CurrentPlayerCount = 0;

		/// <summary>
		/// Check whether the player count is valid or not
		/// true = under threshold, false = over threshold
		/// </summary>
		/// <returns></returns>
		public static bool IsPlayerCountValid(int maxPlayerCount)
		{
			bool result = true;
			if (maxPlayerCount > 0 && CurrentPlayerCount + 1 > maxPlayerCount)
				result = false;
			return result;
		}

		/// <summary>
		/// Collection of PlayerInstances for each individual HostInstance
		/// </summary>
		public Dictionary<string, PlayerInstance> Instances = new Dictionary<string, PlayerInstance>();
		
		/// <summary>
		/// Add a player to the Instances collection
		/// </summary>
		/// <param name="playerKey"></param>
		/// <param name="playerValue"></param>
		/// <param name="flags"></param>
		/// <param name="clientLoginTime"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public bool AddPlayer(string playerKey, string playerValue, int flags, DateTime clientLoginTime, HostInstance location)
		{
			bool result = false;
			if (!Instances.ContainsKey(playerKey))
			{
				Instances.Add(playerKey, new PlayerInstance(playerKey, playerValue, flags, clientLoginTime, location));
				Players.CurrentPlayerCount++;
				result = true;
				if (MasterServer.ViewDebugMessages)
					Debug.Log("<Players> AddPlayer [" + playerKey + "] success");
			}
			else
			{
				Debug.LogError("<Players> AddPlayer [" + playerKey + "] duplicate detected. Failure.");
			}
			return result;
		}

		/// <summary>
		/// Add predefined Player instance to the Instances collection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool AddPlayer(PlayerInstance p)
		{
			bool result = false;
			if (!Instances.ContainsKey(p.PlayerKey))
			{
				Instances.Add(p.PlayerKey, p);
				Players.CurrentPlayerCount++;
				result = true;
				if (MasterServer.ViewDebugMessages)
					Debug.Log("<Players> AddPlayer [" + p.PlayerKey + "] success");
			}
			else
			{
				Debug.Log("<Players> AddPlayer [" + p.PlayerKey + "] duplicate detected. Failure.");
			}
			return result;
		}

		/// <summary>
		/// Remove player from Instances collection
		/// </summary>
		/// <param name="playerKey"></param>
		/// <returns></returns>
		public PlayerInstance RemovePlayer(string playerKey)
		{
			PlayerInstance result = null;
			if (Instances.TryGetValue(playerKey, out result))
			{
				Instances.Remove(playerKey);
				Players.CurrentPlayerCount--;
			}
			if (MasterServer.ViewDebugMessages)
				Debug.Log("<Players> RemovePlayer [" + playerKey + "] " + (result != null? "succes" : "failure"));
			return result;
		}

		/// <summary>
		/// Clear all players and update the count
		/// </summary>
		public void ClearPlayers()
		{
			CurrentPlayerCount -= Instances.Count;
			Instances.Clear();
		}

		/// <summary>
		/// Return Array copy of Player Instances
		/// </summary>
		/// <returns></returns>
		public PlayerInstance[] GetInstances()
		{
			return Instances.Values.ToArray();
		}

		/// <summary>
		/// Attempts to find whether a Player exists within this specific Collection
		/// </summary>
		/// <param name="playerKey"></param>
		/// <returns></returns>
		public PlayerInstance FindPlayer(string playerKey)
		{
			PlayerInstance result = null;
			Instances.TryGetValue(playerKey, out result);
			return result;
		}

		/// <summary>
		/// Attempts to find whether a Player exists within this specific Collection
		/// Uses email for lookup.  Not efficient but works.
		/// </summary>
		/// <param name="playerValue"></param>
		/// <returns></returns>
		public PlayerInstance FindPlayerByEmail(string playerValue)
		{
			PlayerInstance result = null;
			foreach (var pl in Instances.Values)
			{
				if (pl.PlayerValue.Equals(playerValue))
				{
					result = pl;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a List of PlayerStubs, which are smaller data structures to send across the network
		/// when a list of all players is requested
		/// </summary>
		/// <returns></returns>
		public List<PlayerStub> GetPlayerStubs()
		{
			List<PlayerStub> stubs = new List<PlayerStub>();
			foreach (var player in Instances)
				stubs.Add(new PlayerStub(player.Value.PlayerKey, player.Value.HostLocation.HostName));
			return stubs;
		}
	}
}
