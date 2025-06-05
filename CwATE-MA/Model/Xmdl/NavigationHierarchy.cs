namespace CzSoft.CwateMa.Model.Xmdl;
using System;
using System.Collections.Generic;
using System.Linq;

public class NavigationItem
{
    public string Title { get; set; }
    public string Id { get; set; }
    public List<NavigationItem> Children { get; } = new();

    internal NavigationItem(string title, string link)
    {
        Title = title;
        Id = link;
    }
}

public enum NavigationItemType
{
    Item,
    Dropdown,
}

public class NavigationHierarchy
{
    private readonly List<NavigationItem> _items = new();
    public IReadOnlyList<NavigationItem> Items { get; private set; }

    public void BuildHierarchy(string path, string link)
    {
        var parts = path.Split("->", StringSplitOptions.RemoveEmptyEntries);
        var currentLevel = _items;

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            var title = part.Trim('[', ']');
            var item = FindOrCreateItem(currentLevel, title, (i + 1) == parts.Length ? link : null);
            currentLevel = item.Children;
        }
        Items = _items;
    }

    private NavigationItem FindOrCreateItem(List<NavigationItem> items, string title, string link)
    {
        var existingItem = items.FirstOrDefault(item => item.Title == title);
        if (existingItem == null)
        {
            existingItem = new NavigationItem(title, link);
            items.Add(existingItem);
        }
        return existingItem;
    }

}
