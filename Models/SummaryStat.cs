namespace HighFidelity.Ui.Models;

/// <summary>One row of the Last Month Summary popup.</summary>
public sealed record SummaryStat(string Label, string Value, bool Highlight = false);
