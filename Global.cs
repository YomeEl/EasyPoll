using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPoll
{
    public static class Global
    {
        public static Poll ActivePoll { get; set; }

        private static (string username, string password) superuser;
        private static string superuserToken;

        private static Task timeTrackingTask;

        public static void Initialize()
        {
            UpdateActivePoll();
            StartTimeTrackingTask();
            LoadSUData();
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

        private static int DetermineActivePollId()
        {
            var dbcontext = Data.ServiceDBContext.GetDBContext();
            int id = (from poll in dbcontext.Polls
                      orderby poll.CreatedAt
                      where poll.FinishAt > DateTime.Now
                      select poll.Id).LastOrDefault();
            return id;
        }

        private static void UpdateActivePoll()
        {
            int activePollId = DetermineActivePollId();
            if (activePollId > 0)
            {
                ActivePoll = new Poll(activePollId);
            }
            else
            {
                ActivePoll = null;
            }
        }

        private static void StartTimeTrackingTask()
        {
            timeTrackingTask = new Task(() =>
            {
                while (true)
                {
                    UpdateActivePoll();
                    Task.Delay(TimeSpan.FromSeconds(30)).Wait();
                }
            });

            timeTrackingTask.Start();
        }
    }
}
