
// FileChecker.cs
using Spectre.Console;
using System.Threading;

public static class FileChecker
{
    /// <summary>
    /// Runs the live stats display, simulating file discovery, warnings, and errors.
    /// </summary>
    /// <param name="maxErrors">Maximum errors to simulate before stopping incrementing.</param>
    /// <param name="mode">Run mode: "fast" for quick updates, "normal" for slower pacing.</param>
    public static void Run(int maxErrors, string mode)
    {
        // Clear the console screen
        AnsiConsole.Clear();

        // Prepare the table with rounded borders :contentReference[oaicite:0]{index=0}
        var table = new Table()
            .AddColumn("Metric")
            .AddColumn("Count")
            .Border(TableBorder.Rounded);

        // Seed initial counts
        table.AddRow("Found Files", "0");
        table.AddRow("Warnings", "0");
        table.AddRow("Errors", "0");

        // Live‑update the table in place :contentReference[oaicite:1]{index=1}
        AnsiConsole.Live(table)
            .Start(ctx =>
            {
                var iterations = mode == "fast" ? 50 : 20;
                var files = 0;
                var warns = 0;
                var errs = 0;
                for (var i = 0; i < iterations; i++)
                {
                    // Pause between updates based on mode
                    Thread.Sleep(mode == "fast" ? 50 : 150);
                    files++;
                    if (i % 7 == 0) warns++;
                    if (i % 11 == 0 && errs < maxErrors) errs++;

                    // Refresh each cell with new values
                    table.UpdateCell(0, 1, files.ToString());
                    table.UpdateCell(1, 1, warns.ToString());
                    table.UpdateCell(2, 1, errs.ToString());
                    ctx.Refresh();
                }
            });

        // Display a final markup line when complete :contentReference[oaicite:2]{index=2}
        AnsiConsole.MarkupLine(
            $"[green]Scan complete in [bold]{mode}[/][/] mode " +
            $"with a max error threshold of [bold]{maxErrors}[/].");
    }
}
