//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------


using UnityEngine;
using UnityEngine.UI;

namespace ServerOverseer
{
	/// <summary>
	/// Example using the UI to add/remove a player
	/// Ideally you'd do solely through code as a player has logged in to a host
	/// </summary>
	public class AddRemovePlayerExample : MonoBehaviour
	{
		[SerializeField]
		InputField PlayerKeyInput;
		[SerializeField]
		InputField PlayerHostTargetInput;

		/// <summary>
		/// Simple add player
		/// This assumes there is no password set for the HostInstance
		/// The random number could be used for player info, such as 1 = Admin, 2 = Invisible, 4 = Silenced, etc.
		/// Number can be used however you like.
		/// </summary>
		public void AddPlayer()
		{
			// no client-side error checking, this is just an example
			ClientHost.Instance.AddPlayer(PlayerKeyInput.text, "", Random.Range(0, 32), PlayerHostTargetInput.text);
		}

		/// <summary>
		/// Simple remove player
		/// </summary>
		public void RemovePlayer()
		{
			// no client-side error checking, this is just an example
			ClientHost.Instance.RemovePlayer(PlayerKeyInput.text);
		}

		/// <summary>
		/// Change a host reference for a player
		/// </summary>
		public void UpdatePlayerHost()
		{
			// no client-side error checking, this is just an example
			ClientHost.Instance.UpdatePlayerHost(PlayerKeyInput.text, PlayerHostTargetInput.text);
		}
	}
}
