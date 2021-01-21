using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncHub.Hubs
{
    public interface ISignalrHub
    {
        Task CallAll(TestDto Payload);
    }
}
