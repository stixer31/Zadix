//----------------------------------------------
// ServerOverseer
// Copyright � 2016 OuijaPaw Games LLC
//----------------------------------------------

Version 1.0.0
Initial Release

Version 1.1.0
- Multiple connections from same client no longer allowed
- Implemented automatic retry for failed connections
- Updated UI ServerClient example to reflect connected status
- Fixed NetworkConnection tracker to use unique values
- Removed use of delegate responses for Register/Unregister Host
- Added ability to use EventHandlers for responses for most/all methods in Client
- Created method for registering default type handler after all handlers are removed for type
- Updated ClientHostExample for new updates
- Implemented password protection ability for add/remove host
- Added IsPlayerConnected method for Client to send msg to Server to check if specific Player can be found
- Added PlayerNames to add/remove/find/checkHost response messages so you know who it was actually for
- Added Email property to Player class
- Added ability to lookup players by Email property
- Changed PlayerName and PlayerEmail to PlayerKey and PlayerValue respectively
	- Allows less brain-bending when you want to use something other than a name for the players key (email, accound number, etc.)
- Added UpdatePlayer and UpdatePlayerResponse for updating player information
- Changed PlayerHostChange to UpdatePlayerHost and all associated references / handlers / return handlers / definitions / etc.
- Added option to turn off debug messages

Version 1.1.1
- Fixed issue when server not available, after 16 retries there are no more hostIDs available
  http://forum.unity3d.com/threads/maximum-hosts-cannot-exceed-16.359579/
- Fixed bug in one example

Version 1.2
- Changed player/host/client custom enumerations into single enum - CustomEventType
- Added ListHosts and ListHostsResponse methods/messages
- Added ListHostsExample.cs for how to set up / use new ListHosts functionality
- Fixed a few inaccurate comments

Version 1.2.1
- compiled in Unity 2017.2.0f3 without issue
- made it free, enjoy

ServerOverseer is a MasterServer of sorts built upon UNet HLAPI
It was not intended for mobile, though it most certainly can be used as such

The purpose is for Host-to-Host communication in an authoritative manner.  Each host must
register with the MasterServer, and once registered, can track their own players in each host,
and even send messages back and forth between the players/hosts.  There are password settings
to keep Host-to-MasterServer registration secure, in addition to Host passwords to prevent
Players from being added indiscriminantly.

Each host must connect to the MasterServer, as shown below, in a sort of 'wheel and spoke' pattern.

A     B     C
 \    |    /
  \   |   /
   \  |  /
  MasterServer
   /  |  \
  /   |   \
 /    |    \
D     E     F

The MasterServer can reside in the same Unity instance as a ClientHost.  It's up to you.

Lastly, this can be used as a learning tool for HLAPI, as it has a good deal of comments.

Setup:
- Master Server
1. Create empty GameObject in scene
2. Add MasterServer script to Object
3. Set relevant values
	a. Setting a password here will require every host to have the same password set before allowing them to fully connect
	b. MaxPlayerLimit at the MasterServer level will limit ALL players added for ALL HostInstances

- ClientHost
1. Create empty GameObject in scene
2. Add MasterServer script to Object
3. Set relevant values
	- Same IP/Port/Password as MasterServer

To register a host / add a player, see associated scripts / examples.
The UI methods are not necessary.  Just don't use them once you understand how it works better.  They are there
mostly for visual feedback.
