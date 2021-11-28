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
        private static (string username, string password) superuser;
        private static string superuserToken;

        public static void Initialize()
        {
            UpdateActivePoll();
            LoadSUData();
        }

        public static void UpdateActivePoll()
        {
            ActivePoll = new Poll(activePollId);
        }

        public static string CheckSUAndGenerateToken(string username, string password)
        {
            const int TOKEN_LEN = 40;
            if ((username, password) == superuser)
            {
                var token = "";
                var rng = new Random();
                for (int i = 0; i < TOKEN_LEN; i++)
                {
                    token += Convert.ToChar(rng.Next(26) + 65);
                }
                superuserToken = token;
                return token;
            }
            return "";
        }

        public static bool CheckSU(string cookie)
        {
            return cookie == superuserToken;
        }

        private static void LoadSUData()
        {
            var sr = new System.IO.StreamReader("superuser.txt");
            superuser = (sr.ReadLine(), sr.ReadLine());
        }
    }
}
