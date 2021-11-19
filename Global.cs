using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPoll
{
    public static class Global
    {
        public static Poll ActivePoll { get; set; }

        private static int activePollId = 1;

        public static void Initialize()
        {
            UpdateActivePoll();
        }

        public static void UpdateActivePoll()
        {
            ActivePoll = new Poll(activePollId);
        }
    }
}
