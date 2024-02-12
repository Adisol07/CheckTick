using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace CheckTick;

internal class Program
{
    public static Settings Settings { get; set; } = null;
    public static List<Group> Groups { get; set; } = null;
    public static Group CurrentGroup { get; set; } = null;
    public static int CurrentGroupIndex { get; set; } = 0;
    public static Item CurrentItem { get; set; } = null;
    public static int CurrentItemIndex { get; set; } = 0;

    public static void Main(string[] args)
    {
        Console.Title = "Check Tick : Loading..";
        if (File.Exists("./data.json") == false)
            FillWithDefault();
        if (File.Exists("./settings.json") == false)
            File.WriteAllText("./settings.json", JsonConvert.SerializeObject(new Settings(), Formatting.Indented));

        Load();

        while (true)
        {
            Render();
            var key = Console.ReadKey(true);
            if (Settings.KeyBindings["Save"].Contains(key.Key.ToString()))
                Save();
            if (Settings.KeyBindings["Exit"].Contains(key.Key.ToString()))
                return;
            if (Settings.KeyBindings["Create tick"].Contains(key.Key.ToString()))
            {
                Item item = new Item(Prompt(), false, false);
                Groups[CurrentGroupIndex].Items.Add(item);
                if (Settings.AutoSave)
                    Save();
            }
            if (Settings.KeyBindings["Create group"].Contains(key.Key.ToString()))
            {
                Group group = new Group(Prompt());
                Item nothingMessage = new Item("Empty group", false, true);
                group.Items.Add(nothingMessage);
                Groups.Add(group);
                if (Settings.AutoSave)
                    Save();
            }
            if (Groups.Count != 0)
                ObjectManipulation(key.Key);
        }
    }

    public static void ObjectManipulation(ConsoleKey key)
    {
        if (Settings.KeyBindings["Up"].Contains(key.ToString()))
        {
            if (CurrentItemIndex <= Groups[CurrentGroupIndex].Items.Count - 1 && CurrentItemIndex > 0)
            {
                CurrentItemIndex--;
                CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
            }
            else if (CurrentItemIndex > Groups[CurrentGroupIndex].Items.Count - 1)
            {
                CurrentGroupIndex++;
                CurrentItemIndex = 0;
                CurrentGroup = Groups[CurrentGroupIndex];
            }
            else
            {
                if (CurrentGroupIndex > 0)
                {
                    CurrentGroupIndex--;
                    CurrentItemIndex = Groups[CurrentGroupIndex].Items.Count - 1;
                    CurrentGroup = Groups[CurrentGroupIndex];
                    CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
                }
            }
        }
        else if (Settings.KeyBindings["Down"].Contains(key.ToString()))
        {
            if (CurrentItemIndex < Groups[CurrentGroupIndex].Items.Count - 1)
            {
                CurrentItemIndex++;
                CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
            }
            else
            {
                if (CurrentGroupIndex < Groups.Count - 1)
                {
                    CurrentGroupIndex++;
                    CurrentGroup = Groups[CurrentGroupIndex];
                    CurrentItemIndex = 0;
                    CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
                }
            }
        }
        else if (Settings.KeyBindings["Tick"].Contains(key.ToString()))
        {
            Groups[CurrentGroupIndex].Items[CurrentItemIndex].IsChecked = !Groups[CurrentGroupIndex].Items[CurrentItemIndex].IsChecked;
            if (Settings.AutoSave)
                Save();
        }
        else if (Settings.KeyBindings["Delete tick"].Contains(key.ToString()))
        {
            Groups[CurrentGroupIndex].Items.Remove(CurrentItem);

            CurrentItemIndex = CurrentItemIndex == 0 ? 0 : CurrentItemIndex - 1;

            if (Groups[CurrentGroupIndex].Items.Count != 0)
            {
                CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
            }
            else
            {
                ObjectManipulation(ConsoleKey.Delete);
            }

            if (Settings.AutoSave)
                Save();
        }
        else if (Settings.KeyBindings["Delete group"].Contains(key.ToString()))
        {
            Groups.Remove(CurrentGroup);

            CurrentGroupIndex = CurrentGroupIndex == 0 ? 0 : CurrentGroupIndex - 1;

            if (Groups.Count != 0)
            {
                CurrentItemIndex = Groups[CurrentGroupIndex].Items.Count - 1;
                CurrentGroup = Groups[CurrentGroupIndex];
                CurrentItem = Groups[CurrentGroupIndex].Items[CurrentItemIndex];
            }
            else
            {
                CurrentItemIndex = 0;
                CurrentGroup = null;
                CurrentItem = null;
            }

            if (Settings.AutoSave)
                Save();
        }
    }

    public static void Render()
    {
        Console.Clear();
        Console.Title = "Check Tick";
        foreach (Group group in Groups)
        {
            Console.WriteLine(group.Name);
            RenderHorizontalLine();
            foreach (Item item in group.Items)
            {
                Console.ResetColor();

                if (item.IsMessage == false)
                {
                    if (item == CurrentItem && group == CurrentGroup)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    if (item.IsChecked)
                        Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write('[');
                    if (item.IsChecked)
                    {
                        Console.Write("✔️");
                    }
                    Console.Write(']');
                    Console.WriteLine(" " + item.Value);
                }
                else
                {
                    if (item == CurrentItem && group == CurrentGroup)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(item.Value);
                }
            }
            RenderHorizontalLine();
        }
    }
    public static string Prompt()
    {
        Console.SetCursorPosition(0, Console.WindowHeight);
        for (int x = 0; x < Console.WindowWidth; x++)
            Console.Write(' ');
        Console.Write("> ");
        string result = Console.ReadLine().Trim();

        return result;
    }
    public static void Save()
    {
        string json = JsonConvert.SerializeObject(Groups);
        File.WriteAllText("./data.json", json);
    }
    public static void Load()
    {
        string configJson = File.ReadAllText("./settings.json");
        Settings = JsonConvert.DeserializeObject<Settings>(configJson);

        string json = File.ReadAllText("./data.json");
        Groups = JsonConvert.DeserializeObject<List<Group>>(json);

        if (Groups.Count != 0)
        {
            CurrentItem = Groups[0].Items[0];
            CurrentItemIndex = 0;

            CurrentGroup = Groups[0];
            CurrentGroupIndex = 0;
        }
        else
        {
            FillWithDefault();
        }
    }
    public static void FillWithDefault()
    {
        Groups = new List<Group>();
        List<Item> todayItems = new List<Item>();
        List<Item> myItems = new List<Item>();

        todayItems.Add(new Item("Go outside", false, false));
        todayItems.Add(new Item("Learn new thing", false, false));
        todayItems.Add(new Item("Go to sleep early", false, false));

        myItems.Add(new Item("Go to work", false, false));

        Group today = new Group("Today", todayItems);
        Group my = new Group(Environment.UserName, myItems);
        Groups.Add(today);
        Groups.Add(my);
        Save();
    }
    public static void RenderHorizontalLine()
    {
        Console.ResetColor();
        for (int x = 0; x < Console.WindowWidth; x++)
        {
            Console.Write('-');
        }
    }
}