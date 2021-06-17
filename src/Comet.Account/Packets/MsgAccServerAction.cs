using Comet.Account.States;
using Comet.Network.Packets.Internal;
using Comet.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comet.Account.Packets
{
    public sealed class MsgAccServerAction : MsgAccServerAction<GameServer>
    {
        public override async Task ProcessAsync(GameServer client)
        {
            switch (Action)
            {
                
            }
        }
    }
}
