using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPoll
{
    public static class Global
    {
        public static Poll ActivePoll { get; set; }

        public static void Initialize()
        {
            int activePollId = 1;

            ActivePoll = new Poll(activePollId);
        }
    }
}
