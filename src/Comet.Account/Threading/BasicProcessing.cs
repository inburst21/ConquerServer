using Comet.Core;
using Comet.Shared.Comet.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Account.Threading
{
    public sealed class BasicProcessing : TimerBase
    {
        private const string TITLE_S = "Conquer Online Account Server - Servers[{0}], Players[{1}] - {2}";

        public BasicProcessing()
            : base(1000, "System Thread")
        {
        }

        protected override Task OnStartAsync()
        {
            return base.OnStartAsync();
        }

        protected override Task<bool> OnElapseAsync()
        {
            Console.Title = string.Format(TITLE_S, Kernel.Realms.Values.Count(x => x.Server != null), Kernel.Players.Count, DateTime.Now.ToString("G"));
            return base.OnElapseAsync();
        }
    }
}
