# Demo showing Identity Server working with a WPF client which calls a SignalR hub and an Api.

## Projects
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


## The problems encountered along the way.

```Exception Message: Response status code does not indicate success: 401 (Unauthorized).```
This was fixed in Bearer event onMessageReceived by removing the code which plucked the token out of the Authorization Header & letting the Middleware do its thing.
Also the Audience had either be assigned or disabled, it was disabled.

Stack Trace:

 at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnection.<NegotiateAsync>d__45.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnection.<GetNegotiationResponseAsync>d__52.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnection.<SelectAndStartTransport>d__44.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnection.<StartAsyncCore>d__41.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at System.Threading.Tasks.ForceAsyncAwaiter.GetResult()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnection.<StartAsync>d__40.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnectionFactory.<ConnectAsync>d__3.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at Microsoft.AspNetCore.Http.Connections.Client.HttpConnectionFactory.<ConnectAsync>d__3.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Threading.Tasks.ValueTask`1.get_Result()
   at System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
   at Microsoft.AspNetCore.SignalR.Client.HubConnection.<StartAsyncCore>d__58.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Microsoft.AspNetCore.SignalR.Client.HubConnection.<StartAsyncInner>d__50.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at System.Threading.Tasks.ForceAsyncAwaiter.GetResult()
   at Microsoft.AspNetCore.SignalR.Client.HubConnection.<StartAsync>d__49.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at WpfClient.MainWindow.<ConnectToHub>d__7.MoveNext() in C:\Users\mmcca\source\repos\Servers - Duende\WpfClient\MainWindow.xaml.cs:line 171
