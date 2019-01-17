using UnityEngine;
using Valve.VR;

using System.Collections;
public class ControllerManager : MonoBehaviour {

	public SteamVR_TrackedObject mTrackeObject = null;
	public SteamVR_Controller.Device mDevice;

	void Awake()
	{
		mTrackeObject = GetComponent<SteamVR_TrackedObject> ();
	}

	void Update()
	{
		mDevice = SteamVR_Controller.Input((int)mTrackeObject.index);
		//Trigger Down
		if (mDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			print ("Trigger down");
		}
		//Trigger Up
		if (mDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			print ("Trigger Up");
		}
		//value
		//Vector2 triggerValue = mDeviceGetAxis(EVRButtonId.K_EButton_SteamVR_Trigger);

	}



}