using System;

namespace CheckTick;

public class Item
{
    public string Value { get; set; } = "";
    public bool IsChecked { get; set; } = false;
    public bool IsMessage { get; set; } = false;

    public Item()
    { }
    public Item(string value, bool isChecked, bool isMessage)
    {
        Value = value;
        IsChecked = isChecked;
        IsMessage = isMessage;
    }
}