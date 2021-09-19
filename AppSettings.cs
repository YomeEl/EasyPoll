using Microsoft.Extensions.Configuration;

namespace LightPoll
{
    public static class AppSettings
    {
        public static IConfiguration Configuration { get; set; }
    }
}