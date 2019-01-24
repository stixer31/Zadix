//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ServerOverseer
{
	/// <summary>
	/// This is an example class
	/// 1.  Create empty Gameobject in scene
	/// 2.  Attack ClientHostExample script to object
	/// 3.  Fill in relevant fields
	/// 4.  Hook UI button events (or whatever you want) to call relevant methods
	/// 5.  Link UI Text for visual response info (not necessary, just here as example) to responseText field
	/// </summary>
	public class ClientHostExample : MonoBehaviour
	{
		[SerializeField]
		string hostName;
		[SerializeField]
		string hostPassword;
		[SerializeField]
		string comment;
		[SerializeField]
		int maxPlayer;

		[SerializeField]
		Text responseText;
		[SerializeField]
		Text connectionStatusText;

		/// <summary>
		/// Use method to register host
		/// </summary>
		public void RegisterHost()
		{
			// no client-side error checking, this is just an example
			// note that the last parameter is an event handler delegate
			// you can add this directly / at any time via ClientHost.Instance.RegisterHandler(short msgType, NetworkMessageDelegate methodName)
			ClientHost.Instance.RegisterHost(hostName, comment, hostPassword, maxPlayer, RegisterResponse);
		}

		/// <summary>
		/// Response method sent as a delegate method for RegisterHost
		/// Not really needed, but it's here for informational purposes / example
		/// </summary>
		/// <param name="netMsg"></param>
		public void RegisterResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.RegisterHostResponseMessage>();
			if (responseText)
				responseText.text = "Register result: " + (MessageTypes.CustomEventType)msg.resultCode;
			Debug.Log("<ClientHostExample> Register result: " + (MessageTypes.CustomEventType)msg.resultCode);
		}

		/// <summary>
		/// Use method to unregister host
		/// </summary>
		public void UnregisterHost()
		{
			// no client-side error checking, this is just an example
			// note that the last parameter is an event handler delegate
			// you can add this directly / at any time via ClientHost.Instance.UnregisterHandler(short msgType, NetworkMessageDelegate methodName)
			ClientHost.Instance.UnregisterHost(hostName, hostPassword, UnregisterResponse);
		}

		/// <summary>
		/// Response method sent as a delegate method for RegisterHost
		/// Not really needed, but it's here for informational purposes / example
		/// </summary>
		/// <param name="netMsg"></param>
		public void UnregisterResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.UnregisterHostResponseMessage>();
			if (responseText)
				responseText.text = "Unregister result: " + (MessageTypes.CustomEventType)msg.resultCode;
			Debug.Log("<ClientHostExample> Unregister result: " + (MessageTypes.CustomEventType)msg.resultCode);
		}

		/// <summary>
		/// I was going to get all fancy, but then it's just making unnecessary UI stuff in the core code
		/// So instead, this will have to do.
		/// If you want some kind of UI stuff, you'll just have to plug it into the code, etc.
		/// </summary>
		void Update()
		{
			if (ClientHost.Instance && ClientHost.Instance.IsConnected)
				connectionStatusText.text = "Connected";
			else
				connectionStatusText.text = "Disconnected";
		}

		/// <summary>
		/// If you're going to have an object that will be destroyed at some point, best to clean
		/// up the event handlers for the Client.  Note that this is not necessary if you don't
		/// plan on destroying the object.
		/// </summary>
		void OnDestroy()
		{
			ClientHost.Instance.UnregisterHandler(MessageTypes.RegisterHostResponse);
			ClientHost.Instance.UnregisterHandler(MessageTypes.UnregisterHostResponse);
		}
	}
}

