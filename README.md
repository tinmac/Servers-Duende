# Projects
This solution has three main projects, A fourth simple Api was project added to show that the [Authorise] attribute works for HTTP Api.  

#### IdSvr = Identityserver4.

#### WpfClient Authenticates via IdSvr 'Code' flow & receives Token, then calls SyncHub with said Token.

#### SyncHub SignalR project that wont [Authorize] - error 'Failed to invoke 'ContactHub' because user is unauthorized'.

#### Api = fourth simple Api project added to show successful [Authorise] via IS4.


## To run 

I personally set all projects run as multiple projects, with Wpf & Api last.

Wpf client has three buttons:

(1) Login 

(2) Call Hub - calls the SignalR sighub 'ContactHub' method wich never Authorises. 

(3) Call Api - calls the Api/Identity endpoint which works ok using the same token. 


## The error

The error in question is in SyncHub project:

Failed to invoke 'ContactHub' because user is unauthorized.

The SignalR 'ContactHub' method is in SyncHub --> SignalrHub.cs 
