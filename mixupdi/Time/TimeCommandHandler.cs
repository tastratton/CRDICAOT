using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;


namespace mixupdi.Time;
internal class TimeCommandHandler : ICommandHandler
{
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<TimeCommandHandler> _logger;

    public TimeCommandHandler(TimeProvider timeProvider, ILogger<TimeCommandHandler> logger)
    {
        _timeProvider = timeProvider;
        _logger = logger;
    }
    public TimeCommandHandler(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
        _logger = loggerFactory.CreateLogger<TimeCommandHandler>();
    }

    public int Invoke(InvocationContext context)
    {
        TimeCommand tc = (TimeCommand)context.ParseResult.CommandResult.Command;
        double tzoffset = 0; //default
        if (context.ParseResult.HasOption(tc.TimezoneOption)) {
            tzoffset = context.ParseResult.GetValueForOption<double>(tc.TimezoneOption);
        }
    
        DateTimeOffset currentGmt = _timeProvider.GetUtcNow();
        _logger.LogInformation("Called at {Time}", currentGmt);
        Console.WriteLine($"{currentGmt.ToOffset(TimeSpan.FromHours(tzoffset)):T}");
        return 0;
    }
    
    public Task<int> InvokeAsync(InvocationContext context)
    {
        return Task.FromResult(Invoke(context));
    }
    public static Action<InvocationContext> SetHandlerAction(InvocationContext invCtx, IHost host)
    {
        Action<InvocationContext> timeCommandHandlerTarget;

        timeCommandHandlerTarget = delegate (InvocationContext invCtx)
        {
            TimeCommandHandler? timeCommandHandlerService;
            timeCommandHandlerService = host.Services.GetService<TimeCommandHandler>();
            if (timeCommandHandlerService != null)
            {
                
                timeCommandHandlerService.InvokeAsync(invCtx);
            }
            else
            {
                throw new Exception("Could not locate TimeCommandHandler service");
            }

        };
        return timeCommandHandlerTarget;
    }
}