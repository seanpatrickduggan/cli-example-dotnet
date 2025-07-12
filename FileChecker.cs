// FileChecker.cs
using Spectre.Console;
using System;
using System.Threading;

public static class FileChecker
{
    /// <summary>
    /// Runs the live file scan simulation with ASCII art and a live updating table reflecting total, processed,
    /// and categorized counts of good, warning, and error files.
    /// Adds extra Spectre.Console widgets for visual flair without altering core logic.
    /// </summary>
    /// <param name="maxErrors">Maximum number of errors to simulate (will cap error files).</param>
    /// <param name="mode">Run mode: "fast" for quick updates, "normal" for slower pacing.</param>
    public static void Run(int maxErrors, string mode)
    {
        // Clear screen and print banner
        // AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("FileChecker").Color(Color.MediumPurple3));

        // 1) Define total files and initialize counters
        int totalFiles = mode == "fast" ? 1000 : 5000;
        int processed = 0;
        int goodCount = 0;
        int warnCount = 0;
        int errorCount = 0;

        // 2) Prepare table with rows for each metric
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Metric")
            .AddColumn("Value");
        table.AddRow("Total Files", totalFiles.ToString());
        table.AddRow("Processed", "0 / " + totalFiles);
        table.AddRow("Good Files", "0");
        table.AddRow("Warn Files", "0");
        table.AddRow("Error Files", "0");
        table.AddRow("% Complete", "0%");

        // 3) Live update loop
        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                for (int i = 1; i <= totalFiles; i++)
                {
                    // Thread.Sleep(mode == "fast" ? 1 : 2);
                    processed = i;
                    if (i % 11 == 0 && errorCount < maxErrors) errorCount++;
                    else if (i % 7 == 0) warnCount++;
                    else goodCount++;
                    int percent = (int)(processed / (double)totalFiles * 100);

                    table.UpdateCell(1, 1, $"{processed} / {totalFiles}");
                    table.UpdateCell(2, 1, $"[green]{goodCount}[/]");
                    table.UpdateCell(3, 1, $"[yellow]{warnCount}[/]");
                    table.UpdateCell(4, 1, $"[red]{errorCount}[/]");
                    table.UpdateCell(5, 1, $"[blue]{percent}%[/]");
                    ctx.Refresh();
                }
            });

        // 4) Final status spinner
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Finalizing...", _ => Thread.Sleep(500));

        // 5) Display a bar chart of results
        var chart = new BarChart()
            .Width(100)
            .Label("File Scan Results")
            .CenterLabel()
            .AddItem("Good", goodCount, Color.Green)
            .AddItem("Warn", warnCount, Color.Yellow)
            .AddItem("Error", errorCount, Color.Red);
        AnsiConsole.Write(chart);
        AnsiConsole.WriteLine('\n');
        // Display a breakdown chart
        var breakdownTitle = new Text("Breakdown Chart\n");
        AnsiConsole.Write(breakdownTitle);
        var breakdown = new BreakdownChart()
            .Width(100)
            .AddItem("Good", goodCount, Color.Green)
            .AddItem("Warn", warnCount, Color.Yellow)
            .AddItem("Error", errorCount, Color.Red);
        AnsiConsole.Write(breakdown);
        AnsiConsole.WriteLine('\n');
        // 6) Fancy summary panel
        // var summary = new Panel(
        //     new Text($"Processed {processed}/{totalFiles} files\nGood: {goodCount}, Warn: {warnCount}, Error: {errorCount}", new Style(Color.White))
        // )
        // .Header("Scan Summary", Justify.Center)
        // .Border(BoxBorder.Double)
        // .BorderStyle(Style.Parse("green"));
        // AnsiConsole.Write(summary);

        // 7) Completion message
        AnsiConsole.MarkupLine($"[bold green]Scan complete in [underline]{mode}[/] mode with max errors [red]{maxErrors}[/].[/]");
    }
}
