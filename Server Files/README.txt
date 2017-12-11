______________________________________________________

	Leap Of Faith Server: Setup Summary
______________________________________________________

1) Make sure you are connected to a network.

2) Run "Server Files\StartServer.bat".

3) Wait 5 seconds.

4) Copy the folder "ServerConfig" inside of "Server Files\_REDIST".

5) Paste the folder into every game client at "Leap_Of_Faith_Data\".
	Filelist:
	- Leap_Of_Faith.exe
	- Leap_Of_Faith_Data\
	- Leap_Of_Faith_Data\...
	- Leap_Of_Faith_Data\ServerConfig\
	- Leap_Of_Faith_Data\ServerConfig\ServerIP.txt

6) The players may run the game "Leap_Of_Faith.exe" and start playing!


______________________________________________________

	Leap Of Faith Server: Introduction
______________________________________________________

Leap Of Faith is a networked multiplayer game.

The player is first connected to a central matchmaking 
server automatically by the game, which the matchmaking
server will match connected players based on their
peer-to-peer ping. 

The server then sends the instruction to the two
matched players to disconnect from the Leap Of Faith Server,
and connect to each other directly.


The servers may be hosted on a public server with 
public-accessible IP address and firewall, which allows 
anyone on the Wide-Area Network (WAN) to connect.

If hosted on a server with no dedicated IP address,
only people on the same Local Area Network (LAN) may connect.


______________________________________________________

	Leap Of Faith Server: Components
______________________________________________________

The Leap Of Faith Server consists of 3 components.

Master Server, provided by Unity3D, allows players
to connect to the servers.

Proxy Server, provided by Unity3D, allows the server
to allow for proxy connections. It is only used as a
last resort when the players connected to matchmaking
server are unable to connect to each other for a long
time.

Matchmaking Server, created by us, pairs up connected
players based on the best peer-to-peer ping results
as the player matchmaking process.


______________________________________________________

	Leap Of Faith Server: System Requirements
______________________________________________________

Minimum Requirements:

Operating System of Windows XP or above
Any Graphics card (DirectX 7.0-capable)
512 MB of RAM
50 MB of hard disk space
Internet Connection (WAN/LAN)


Listen Ports used:

10746
23466
25002

______________________________________________________

	Leap Of Faith Server: Setup Guide
______________________________________________________

Make sure that you are connected to a network (WAN/LAN).

Run "Server Files\StartServer.bat"

3 applications should appear:
- MasterServer.exe 
- ProxyServer.exe 
- Leap_Of_Faith_MMServer.exe

They should be left running at all times,
and may be minimized or put into the background.


Leap_Of_Faith_MMServer.exe may show the Unity splash screen.
Wait until the Unity splash screen fades out. 
(Usually takes up to 5 seconds)
Once it does, the Matchmaking Server should be running.

Locate "Server Files\_REDIST".
Inside this redistributable folder should contain another folder.

	Filelist should be:
	- Server Files\
	- Server Files\_REDIST\ServerConfig\
	- Server Files\_REDIST\ServerConfig\ServerIP.txt


The folder "ServerConfig" should be redistributed and copied into 
every player's Leap Of Faith game client folder, 
in the path "Leap_Of_Faith_Data\"

	Filelist should be:
	- Leap_Of_Faith.exe
	- Leap_Of_Faith_Data\
	- Leap_Of_Faith_Data\...
	- Leap_Of_Faith_Data\ServerConfig\
	- Leap_Of_Faith_Data\ServerConfig\ServerIP.txt

The contents inside ServerIP.txt is intentionally human-readable
on most text editors, and contains the IP address of the servers, 
such as "127.0.0.1". Players may edit ServerIP.txt to connect 
to a Leap Of Faith Server if they know its IP address.


______________________________________________________

	Leap Of Faith Server: Troubleshooting
______________________________________________________

Q: 
Server Files\StartServer.bat does not work!

A: 
Your version of Operating System may not support
batch files (.bat) or the batch commands used (START, EXIT),
or the command interpreter may not be available.

Leap Of Faith Server may be started by 
running the following executables in order:

- Server Files\MasterServer\Release\MasterServer.exe
- Server Files\ProxyServer\Release\ProxyServer.exe
- Server Files\MMServer\Leap_Of_Faith_MMServer.exe

______________________________________________

Q:
"Server Files\_REDIST\..." does not exist!

A:
"Server Files\_REDIST" is only created when 
Leap_Of_Faith_MMServer.exe is executed and the
matchmaking server is initialized successfully,
along with the Master Server.

Please try to run the Leap Of Faith Server 
at least once, via StartServer.bat.

If the problem persists,
please check your network connection.

______________________________________________

Q:
I am experiencing problems with MasterServer.exe
and/or ProxyServer.exe!

A:
Unfortunately, the Master Server and Proxy Server
is provided by Unity3D. You may wish to contact them 
instead.

Do also check that your network connection is available
when running the Master Server and Proxy Server.

______________________________________________

Q:
How can players edit the IP address of the server
they want to connect to?

A:
They may use any text editor to edit the file located at 
"Leap_Of_Faith_Data\ServerConfig\ServerIP.txt", or create
the file and directory if it does not exist.

The content of the file should be the IP address of the
server, in IPv4 Dotted Decimal format. 
An example would be: "127.0.0.1"

______________________________________________

Q:
Can I not host the Proxy Server?

A:
Yes, you may wish to not host the Proxy Server, 
as it may consume a lot of bandwidth when 
being used by players.

Simply close ProxyServer.exe and the 
Leap Of Faith Server will still work as intended.

However, do note that if no player in the
matchmaking process is able to connect to each
other via public IP or NAT punchthrough, there
will be no proxy server to assist. 

This may cause some players to experience long 
matchmaking times if there is no connectable peer.

You may choose to remove the line:
"start ProxyServer/Release/ProxyServer.exe"
from the contents of StartServer.bat too.

______________________________________________

Q:
Can I host the Leap Of Faith Server on a machine 
with no graphics card? Most rented server machines 
do not have a graphics card.

A:
Unfortunately, no. The matchmaking server is built
without a Unity Pro License, which resulted in it 
to have no headless mode capability.

______________________________________________________

Should you require any further assistance 
on setting up the Leap Of Faith Server, 
feel free to contact me via email at:
	1000054E@student.tp.edu.sg

- Ong Heng Le
