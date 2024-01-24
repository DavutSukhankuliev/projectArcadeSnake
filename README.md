# projectArcadeSnake

if Headset emulator doesn't work use this:

OVRManager.cs
Seems to be a not "!" messing , OVRPlugin.initialized is set to true inside InitOVRManager().
so the test should be is  OVRPlugin.initialized == false is so call InitOVRManager().
```c#
private void Awake()
{
	//If OVRPlugin is initialized on Awake(), or if the device is OpenVR, OVRManager should be initialized right away.
	if (!OVRPlugin.initialized || (Settings.enabled && Settings.loadedDeviceName == OPENVR_UNITY_NAME_STR))
	{
		InitOVRManager();
	}
}
```
