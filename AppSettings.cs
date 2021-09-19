using Microsoft.Extensions.Configuration;

namespace EasyPoll
{
    public static class AppSettings
    {
        public static IConfiguration Configuration { get; set; }
    }
}