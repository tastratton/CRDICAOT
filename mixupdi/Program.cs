using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using mixupdi.Time;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace mixupdi;

internal class Program
{
    private static IHost InitHost()
    {
        var progHost = Host.CreateDefaultBuilder()
             .ConfigureServices(ConfigureServices)
             .Build();
        return progHost;
    }

    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        services.TryAddSingleton<TimeCommandHandler>();
    }

    static async Task<int> Main(string[] args)
    {
        using (IHost progHost = InitHost())
        {

            var rootCommand = new RootCommand("Sample app for System.CommandLine");

            // register time subcommand
            var timeCommand = new TimeCommand();
            var timeCommandHandler = new TimeCommandHandler(progHost.Services.GetRequiredService<TimeProvider>());
            rootCommand.Add(timeCommand);

            var cmdlineBuilder = new CommandLineBuilder(rootCommand);
            var parser = cmdlineBuilder.UseDefaults().Build();
            var parseResult = parser.Parse(args);
            var invocationContext = new InvocationContext(parseResult);

            timeCommand.SetHandler(TimeCommandHandler.SetHandlerAction(invocationContext, progHost));

            return await parser.InvokeAsync(args);
        }
    }

}