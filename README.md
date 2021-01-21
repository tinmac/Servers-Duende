# Projects
This solution has three main projects, A fourth simple Api was project added to show that the [Authorise] attribute works for HTTP Api.  

1) IdSvr = Duende Identityserver.

2) WpfClient Authenticates via IdSvr 'Code' flow & receives Token, then calls SyncHub with said Token.

3) SyncHub SignalR project that wont [Authorize] - error '401 unauthorized'.

4) Api = fourth simple Api project added to show successful [Authorise] via Identity Server.


## To run 

Set the projects to run as Multiple Startup Projects, in this order:

First) IdSvr

Next) Sync Hub

Next) WpfClient

Last) Api


Wpf client has three buttons:

(1) Login 

(2) Call Hub - calls the SignalR sighub 'ContactHub' method wich never Authorises. 

(3) Call Api - calls the Api/Identity endpoint which works ok using the same token. 


## The error

The error in question is in SyncHub project:

Failed to invoke 'ContactHub' because user is unauthorized 401.

The SignalR 'ContactHub' method is in SyncHub --> SignalrHub.cs 
