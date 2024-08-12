using System.CommandLine;

namespace mixupdi.Time;
public class TimeCommand : Command
{

    public Option<double> TimezoneOption = new Option<double>(
        aliases: new[] { "--timezone", "-z" },
        description: "The positive or negative GMT offset");
    public TimeCommand()
        : base("time", "Outputs the current time")
    {
        this.AddOption(TimezoneOption);
    }

}
