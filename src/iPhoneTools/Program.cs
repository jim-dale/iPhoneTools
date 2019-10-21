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
                services.AddTransient<ICommand<DecryptOptions>, DecryptCommand>();
                services.AddTransient<ICommand<MessageOptions>, MessageCommand>();
                services.AddTransient<ICommand<ContactOptions>, ContactCommand>();
                services.AddTransient<KeyStore>();
                services.AddTransient<BackupFileProvider>();
                services.AddTransient<AppContext>();
            }).Build())
            {
                result = Parser.Default.ParseArguments<DecryptOptions, MessageOptions, ContactOptions>(args)
                    .MapResult(
                        (DecryptOptions opts) => AppHost.RunCommand(host.Services, opts),
                        (MessageOptions opts) => AppHost.RunCommand(host.Services, opts),
                        (ContactOptions opts) => AppHost.RunCommand(host.Services, opts),
                        errs => 1);
            }

            return result;
        }
    }
}
