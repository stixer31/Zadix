//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace ServerOverseer
{
	/// <summary>
	/// Delegate for MasterServer.cs to use when updating the UI
	/// </summary>
	public delegate void UpdateUIDelegate();

	/// <summary>
	/// This class is completely optional.  It is used mainly for testing if you want to see
	/// in the editor/player when/where things are conncted.
	/// 
	/// 1.  Add MasterServerUI behavior to same (or different even) object as MasterServer.cs
	/// 2.  Create Unity UI Text elements for HostHeader, Hosts, and Players
	/// 3.  Make sure these Texts have enough space to accomodate the info, set their Vertical Overlow setting to 'Overflow'
	/// 4.  In the inspector, set which text field goes to which reference for this class
	/// 5.  That's it.
	/// </summary>
	public class MasterServerUI : MonoBehaviour
	{
		[SerializeField]
		string ObjectName = "MasterServer";

		[SerializeField]
		Text UIHostHeaderText;
		[SerializeField]
		Text UIHostText;
		[SerializeField]
		Text UIPlayerText;

		private StringBuilder _sb = new StringBuilder();
		private GameObject _uiObject = null;
		private bool _uiEnabled;
		private PlayerInstance[] _players;

		/// <summary>
		/// Unity Start - called after Awake so MasterServer can 'set up' without having to mess with script order execution
		/// the sets the MasterServer UpdateUIDelegate method to use one found here
		/// </summary>
		void Start()
		{
			if (MasterServer.Instance && MasterServer.Instance.EnableUI)
			{
				_uiObject = GameObject.Find(ObjectName);
				_uiEnabled = (_uiObject != null);
				
				if (_uiEnabled)
				{
					Debug.Log("Found MasteServer Object for UI Information");
					_uiObject.GetComponent<MasterServer>().UpdateUIMethod = UpdateUI;
				}
				else
				{
					Debug.LogError("Could not find MasterServer GameObject for UI Information");
					_uiObject.GetComponent<MasterServer>().UpdateUIMethod = null;
				}
			}
		}

		#region --------------- UI Management -----------------

		/// <summary>
		/// Public method used as the delegate target when Updating the UI from the MasterServer.cs behavior
		/// </summary>
		public void UpdateUI()
		{
			UIUpdateHostHeader();
			UIUpdateHosts();
			UIUpdatePlayers();
		}

		/// <summary>
		/// Update the host header, which will show whether the MasterServer is online or not
		/// </summary>
		public void UIUpdateHostHeader()
		{
			if (_uiEnabled)
				UIHostHeaderText.text = "Server: " + (NetworkServer.active ? "Online @" + MasterServer.Instance.MasterServerPort : "Offline");
		}

		/// <summary>
		/// Updates all Connected Hosts and Lists them
		/// </summary>
		public void UIUpdateHosts()
		{
			if (_uiEnabled)
			{
				bool found = false;
				_sb.Length = 0;
				foreach (var host in MasterServer.Instance.ConnectedHosts.Connected.Values)
				{
					_sb.Append(host.HostName).Append(" hostID:").Append(host.HostID).Append(" connID:").Append(host.ConnectionId).AppendLine();
					found = true;
				}
				UIHostText.text = found ? _sb.ToString() : "-------";
			}
		}

		/// <summary>
		/// Updates all Connected Players, which HostInstance they belong to, and lists them
		/// </summary>
		public void UIUpdatePlayers()
		{
			if (_uiEnabled)
			{
				bool found = false;
				_sb.Length = 0;
				foreach (var host in MasterServer.Instance.ConnectedHosts.Connected.Values)
				{
					_players = host.GetPlayerInstances();
					for (int i = 0; i < _players.Length; i++)
					{
						_sb.Append(_players[i].PlayerKey).Append(" : ").Append(_players[i].Flags).Append(" @ ").Append(_players[i].HostLocation.HostName).AppendLine();
						found = true;
					}
				}
				UIPlayerText.text = found ? _sb.ToString() : "-------";
			}
		}

		#endregion

	}
}
