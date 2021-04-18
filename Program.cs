using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CinemaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
       
            CreateHostBuilder(args).Build().Run();
        }

        public bool IsPalendrome(string text)
        {
            return IsPalendrome(0, text.Length - 1, text);
        }

        public bool IsPalendrome(int startIndex,int lastIndex,string text)
        {
            if (startIndex >= lastIndex)
            {
                return true;
            }
            if (text[startIndex] != text[lastIndex]) return false;

            return IsPalendrome(startIndex + 1, lastIndex - 1, text);
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

       
    }
}
