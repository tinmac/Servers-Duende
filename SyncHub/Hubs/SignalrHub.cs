using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using SyncHub;
using SyncHub.Hubs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Agy.Server.Hubs
{
    // [Authorize]      
    // [Authorize("Bearer")] // exception policyn 'Bearer' not found
    // [Authorize(AuthenticationSchemes = "Bearer")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // 401
    [Authorize("SigScope")]
    public class SignalrHub : Hub<ISignalrHub>
    {
        public SignalrHub()
        {
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [Authorize(AuthenticationSchemes = "Bearer")]
        // [Authorize]        
        // [Authorize("SigScope")]
        // [Authorize("Bearer")]
        public async Task ContactHub(TestDto payload)
        {
            var sigBack = new TestDto();
            sigBack.Message = $"Hub says...... Hi {payload.Message}";

            await Clients.All.CallAll(sigBack);
        }


        public async void CallAll(TestDto payload)
        {
            var msg = payload.Message;
            Debug.WriteLine($"Sync Hub.......");
            Debug.WriteLine($"Messgae: {msg}");
            Debug.WriteLine("");

            payload.Message = "Sync Hub Server says Hi  >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> :-)";
            await Clients.All.CallAll(payload);
        }


        
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");

            // JWT token in header?
            string JWT = Context.GetHttpContext().Request.Headers["Authorization"];
            Debug.WriteLine($"Sync Hub.......");
            Debug.WriteLine($"On Connected Async: {ReadableJWT(JWT)}");
            Debug.WriteLine("");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // If exception null client disconnected intentionally with Connection.StopAsync
            // If not intentional exception will contain a description of the error.
            Debug.WriteLine($"Sync Hub.......");
            Debug.WriteLine($"On Disconnected Async");
            Debug.WriteLine("");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }


        // helpers
        private string ReadableJWT(string token)
        {
            // Pass back the first 10 & last digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 10)}...{token.Substring(token.Length - 10, 10)}";

            return "no token";
        }

    }
}
