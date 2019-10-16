using CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace iPhoneTools
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = 0;
            using (var host = AppHost.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ICommand<DecryptOptions>, DecryptCommand>();
                services.AddSingleton<ICommand<MessageOptions>, MessageCommand>();
                services.AddTransient<AppContext>();
            }).Build())
            {
                result = Parser.Default.ParseArguments<DecryptOptions, MessageOptions>(args)
                    .MapResult(
                        (DecryptOptions opts) => AppHost.RunCommand(host.Services, opts),
                        (MessageOptions opts) => AppHost.RunCommand(host.Services, opts),
                        errs => 1);
            }

            return result;
        }
    }
}
