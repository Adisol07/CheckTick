using System;

namespace CheckTick;

public class Group
{
    public List<Item> Items { get; set; } = new List<Item>();
    public string Name { get; set; } = "";

    public Group()
    { }
    public Group(string name)
    {
        Name = name;
    }
    public Group(string name, List<Item> items)
    {
        Name = name;
        Items = items;
    }
}