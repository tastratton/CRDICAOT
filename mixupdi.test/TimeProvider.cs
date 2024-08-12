using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using mixupdi.Time;
using Moq;
using System.CommandLine;
using System.CommandLine.Parsing;
using Xunit.Abstractions;


namespace mixupdi.test;

public class TimeProviderFixture : IDisposable
{
    public FakeTimeProvider FakeTimeProvider { get; }
    public Mock<ILogger<TimeCommandHandler>> MockLogger { get; }
    public ILogger<TimeCommandHandler> Logger { get; }
    public TimeCommandHandler TimeCommandHandler { get; }

      public TimeProviderFixture()
    {
        FakeTimeProvider = new FakeTimeProvider();
        FakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time"));
        FakeTimeProvider.SetUtcNow(new DateTime(2023, 12, 5, 1,2,3,DateTimeKind.Utc));
        MockLogger = new Mock<ILogger<TimeCommandHandler>>();
        Logger = MockLogger.Object;
        TimeCommandHandler = new TimeCommandHandler(FakeTimeProvider, Logger);
    }

    public void Dispose()
{

}
}
public class TimeProvider : IClassFixture<TimeProviderFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    TimeProviderFixture fixture;
    public TimeProvider(TimeProviderFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        this.fixture = fixture;
    }

    [Fact]
    public void ExitCode()
    {
        var command = new TimeCommand();
        var config = new System.CommandLine.CommandLineConfiguration(command);
        command.Handler = new TimeCommandHandler(fixture.FakeTimeProvider);
        ParseResult parseResult = command.Parse("time");
        var invoker = new System.CommandLine.Invocation.InvocationContext(parseResult);
        var result = fixture.TimeCommandHandler.Invoke(invoker);

        //Assert
        Assert.NotNull(invoker);
        Assert.True(result == 0, "Exit Code is 0");
        _testOutputHelper.WriteLine("Assert Exit Code is 0");
    }
    
    [Fact]
    public void ConsoleOutputTimeZoneIsMinus4()
    {
        StringWriter testconsole = new StringWriter();
        Console.SetOut(testconsole);
        Console.SetError(testconsole);

        var command = new TimeCommand();
        var config = new System.CommandLine.CommandLineConfiguration(command);
        command.Handler = new TimeCommandHandler(fixture.FakeTimeProvider);
        ParseResult parseResult = command.Parse("time -z -4");
        var invoker = new System.CommandLine.Invocation.InvocationContext(parseResult);
        fixture.TimeCommandHandler.Invoke(invoker);
        Assert.NotNull(parseResult);
        Assert.Equal("21:02:03\r\n", testconsole.ToString());

    }
    
    [Fact]
    public void ConsoleOutputTime()
    {
        StringWriter testconsole = new StringWriter();
        Console.SetOut(testconsole);
        Console.SetError(testconsole);

        var command = new TimeCommand();
        var config = new System.CommandLine.CommandLineConfiguration(command);
        command.Handler = new TimeCommandHandler(fixture.FakeTimeProvider);
        ParseResult parseResult = command.Parse("time");
        var invoker = new System.CommandLine.Invocation.InvocationContext(parseResult);
        fixture.TimeCommandHandler.Invoke(invoker);
        Assert.NotNull(parseResult);
        //now using invariant culture
        Assert.Equal("01:02:03\r\n", testconsole.ToString());


    }

}
