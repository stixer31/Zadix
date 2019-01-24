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
	/// Example of how a client would request a list of hosts from the master server
	/// If master server has a password, it has to be sent via the request
	/// 1. Client sends request to master server
	/// 2. Master server gets request and validates it
	/// 3. Master server builds host list as an array of HostStub structs and sends it to client
	/// 4. Client's delegate method recieves info, parses it
	/// 
	/// If you want to change what information is returned via this request
	/// - modify the HostStub struct for what information you want
	/// - modify the OnListHosts() method in MasterServer.cs when building the array
	/// </summary>
	public class ListHostsExample : MonoBehaviour
	{
		// if the master server has a password, this has to match.  if no password, leave this blank
		[SerializeField]
		public string serverPassword;

		// this test field is for displaying the host information
		[SerializeField]
		Text responseText;

		/// <summary>
		/// Invoke this method via Button event, or via code, to send request to master server
		/// </summary>
		public void ListHosts()
		{
			ClientHost.Instance.ListHosts(serverPassword, ListHostsResponse);
		}

		/// <summary>
		/// Delegate response that will be used when the MasterServer sends back a response with the hosts
		/// </summary>
		/// <param name="netMsg"></param>
		public void ListHostsResponse(NetworkMessage netMsg)
		{
			var msg = netMsg.ReadMessage<MessageTypes.ListHostsResponseMessage>();
			Debug.Log("<ListHostsExample> ListHosts result: " + (MessageTypes.CustomEventType)msg.resultCode);

			// we just parse through it all and pull out what we want
			// there is more that is currently being sent -- players for each host, max players, and a comment
			string txt = "ListHosts result: " + (MessageTypes.CustomEventType)msg.resultCode;
			txt += "\n   count: " + msg.hosts.Length;

			for (int i = 0; i < msg.hosts.Length; i++)
			{
				txt += "\n" + msg.hosts[i].hostName;
			}

			responseText.text = txt;
		}
	}
}
