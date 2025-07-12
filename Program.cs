// Program.cs
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Parsing;

class Program
{
    static int Main(string[] args)
    {
        // Define options
        var maxErrors = new Option<int>("--max-errors")
        {
            Description = "Max number of errors to simulate",
        };

        var modeOption = new Option<string>("--mode")
        {
            Description = "Run mode: normal or fast",
            Required = true,
            Arity = ArgumentArity.ExactlyOne
        };
        // Custom validator
        modeOption.Validators.Add(result =>
        {
            var val = result.GetValue(modeOption);
            if (val is not ("normal" or "fast"))
                result.AddError("Must be 'fast' or 'normal'");
        });

        // Build root command
        var rootCommand = new RootCommand("Live Spectre Stats CLI");
        rootCommand.Options.Add(maxErrors);
        rootCommand.Options.Add(modeOption);

        // Bind single action delegate
        rootCommand.SetAction(parseResult =>
        {
            // Retrieve parsed values
            var max = parseResult.GetValue<int>(maxErrors);
            // Default to fast if modeOption is null, but this should never happen
            var mode = parseResult.GetValue<string>(modeOption) ?? "fast";

            FileChecker.Run(max, mode);
            return 0;
        });

        // Parse + invoke handles help/errors automatically
        var result = rootCommand.Parse(args);
        return result.Invoke();
    }
}
