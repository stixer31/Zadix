//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using UnityEngine.Networking;

namespace ServerOverseer
{
	/// <summary>
	/// Keep all the different MessageTypes, Enums, Definitions, etc. here in one spot
	/// </summary>
	public class MessageTypes
	{
		#region -------------- Event Enumerations --------------
		public enum CustomEventType : int
		{
			UnknownError = 0,
			NoError = 1,
			DuplicateDetected = 2,
			HostUnknown = 3,
			PlayerLocked = 4,
			PlayerBanned = 5,
			Wizlock = 6,
			NewLock = 7,
			KeyEmpty = 8,
			HostPasswordFail = 9,
			MaxPlayersExceeded = 10,
			InvalidKey = 11,
			InvalidHost = 12,
			UnknownPlayer = 13,
			ServerPasswordFail = 14,
			NoHostsRegistered = 15
		}

		#endregion


		#region -------------- UNet Built-in Message Type IDs (For Reference) --------------
		//AddPlayer = 37;
		//Animation = 40;
		//AnimationParameters = 41;
		//AnimationTrigger = 42;
		//Command = 5;
		//Connect = 32;
		//CRC = 14;
		//Disconnect = 33;
		//Error = 34;
		//Highest = 46;
		//InternalHighest = 31;
		//LobbyAddPlayerFailed = 45;
		//LobbyReadyToBegin = 43;
		//LobbyReturnToLobby = 46;
		//LobbySceneLoaded = 44;
		//LocalChildTransform = 16;
		//LocalClientAuthority = 15;
		//LocalPlayerTransform = 6;
		//NetworkInfo = 11;
		//NotReady = 36;
		//ObjectDestroy = 1;
		//ObjectHide = 13;
		//ObjectSpawn = 3;
		//ObjectSpawnScene = 10;
		//Owner = 4;
		//Ready = 35;
		//RemovePlayer = 38;
		//Rpc = 2;
		//Scene = 39;
		//SpawnFinished = 12;
		//SyncEvent = 7;
		//SyncList = 9;
		//UpdateVars = 8;
		#endregion


		#region -------------- Host to MasterServer Message IDs --------------

		public const short RegisterHost = 100;
		public const short UnregisterHost = 101;
		public const short IsPlayerConnected = 102;
		public const short ListAllPlayers = 103;
		public const short PlayerIM = 104;
		public const short ValidatePassword = 105;
		public const short UpdatePlayer = 106;
		public const short UpdatePlayerHost = 107;
		public const short ListHosts = 108;

		public const short P2PCheckHandover = 120;
		public const short P2PHandover = 121;

		#endregion


		#region -------------- MasterServer to Host Message IDs --------------

		public const short RegisterHostResponse = 150;
		public const short UnregisterHostResponse = 151;
		public const short IsPlayerConnectedResponse = 152;
		public const short ListAllPlayersResponse = 153;
		public const short PlayerIMResponse = 154;
		public const short ValidatePasswordResponse = 155;
		public const short UpdatePlayerResponse = 156;
		public const short UpdatePlayerHostResponse = 157;
		public const short ListHostsResponse = 158;

		public const short P2PCheckHandoverResponse = 170;
		public const short P2PHandoverResponse = 171;


		// these are responses to built-in Message Types AddPlayer and RemovePlayer
		public const short AddPlayerResponse = 200;
		public const short RemovePlayerResponse = 201;
		#endregion


		#region -------------- Host to MasterServer Messages --------------

		public class RegisterHostMessage : MessageBase
		{
			public string hostName;
			public string comment;
			public string hostPassword;
			public int playerLimit;
		}

		public class UnregisterHostMessage : MessageBase
		{
			public string hostName;
			public string hostPassword;
		}

		public class IsPlayerConnectedMessage : MessageBase
		{
			public string playerKey;
		}

		public class ListAllPlayersMessage : MessageBase
		{
			public string hostName;
		}

		public class AddPlayerMessage : MessageBase
		{
			public string playerKey;
			public string playerValue;
			public int flags;
			public string hostName;
			public string hostPassword;
		}

		public class RemovePlayerMessage : MessageBase
		{
			public string playerKey;
			public string hostName;
		}

		public class PlayerIMMessage : MessageBase
		{
			public string senderKey;
			public string targetKey;
			public string message;
		}

		public class ValidatePasswordMessage : MessageBase
		{
			public string password;
		}

		public class UpdatePlayerMessage : MessageBase
		{
			public string playerKey;
			public string playerValue;
			public int flags;
		}

		public class UpdatePlayerHostMessage : MessageBase
		{
			public string playerKey;
			public string hostName;
			public string hostPassword;
		}

		public class ListHostsMessage : MessageBase
		{
			public string serverPassword;
		}
		#endregion


		#region -------------- MasterServer to Host Messages --------------


		public class RegisterHostResponseMessage : MessageBase
		{
			public int resultCode;
		}

		public class UnregisterHostResponseMessage : MessageBase
		{
			public int resultCode;
		}

		public class IsPlayerConnectedResponseMessage : MessageBase
		{
			public string playerKey;
			public bool result;
		}

		public class ListAllPlayersResponseMessage : MessageBase
		{
			public bool result;
			public PlayerStub[] players;
		}

		public class AddPlayerResponseMessage : MessageBase
		{
			public string playerKey;
			public int resultCode;
		}

		public class RemovePlayerResponseMessage : MessageBase
		{
			public string playerKey;
			public bool result;
		}

		public class PlayerIMMessageResponse : MessageBase
		{
			public bool result;
			public string senderKey;
			public string targetKey;
			public string message;
		}

		public class ValidatePasswordResponseMessage : MessageBase
		{
			public bool result;
		}

		public class UpdatePlayerResponseMessage : MessageBase
		{
			public string playerKey;
			public bool result;
		}

		public class UpdatePlayerHostResponseMessage : MessageBase
		{
			public string playerKey;
			public int resultCode;
		}

		public class ListHostsResponseMessage : MessageBase
		{
			public int resultCode;
			public HostStub[] hosts;
		}

		#endregion
	}
}


