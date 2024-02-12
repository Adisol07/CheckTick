using System;

namespace CheckTick;

public class Settings
{
    public bool AutoSave { get; set; } = true;
    public Dictionary<string, List<string>> KeyBindings { get; set; } = new Dictionary<string, List<string>>() {
        { "Save", new List<string>() { "S" } },
        { "Exit", new List<string>() { "Q", "Escape" } },
        { "Create tick", new List<string>() { "N" } },
        { "Create group", new List<string>() { "G" } },
        { "Up", new List<string>() { "UpArrow" } },
        { "Down", new List<string>() { "DownArrow" } },
        { "Tick", new List<string>() { "Spacebar", "Enter" } },
        { "Delete tick", new List<string>() { "Backspace" } },
        { "Delete group", new List<string>() { "Delete" } },
    };
}