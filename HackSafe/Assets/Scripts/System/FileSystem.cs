using System;
using System.Linq;
using System.Collections.Generic;

public class FileSystem
{
    public TreeNode Root { get; private set; }
    public TreeNode CurrentDirectory { get; private set; }

    public FileSystem ()
    {
        Root = new TreeNode ("/", true);
        CurrentDirectory = Root;
    }

    public void LoadData (GameData data, bool playerData)
    {
        Root = new TreeNode ("/", true);
        CurrentDirectory = Root;

        if (playerData)
        {
            foreach (SerializedNode serializedNode in data.PlayerNodes)
            {
                TreeNode parentNode = FindNode (serializedNode.ParentPath);
                if (parentNode != null)
                {
                    var newNode = new TreeNode (serializedNode.Name, serializedNode.IsDirectory, parentNode);
                    parentNode.AddChild (newNode);
                }
            }
        }
    }

    public void SaveData (ref GameData data, bool playerData)
    {
        if (playerData)
        {
            data.PlayerNodes.Clear ();
            SavePlayerNode (Root, data, "/");
        }
    }

    private void SavePlayerNode (TreeNode node, GameData data, string parentPath)
    {
        if (node != Root)
        {
            var serializedNode = new SerializedNode
            {
                Name = node.Name,
                IsDirectory = node.IsDirectory,
                ParentPath = parentPath
            };
            data.PlayerNodes.Add (serializedNode);
        }

        foreach (var child in node.Children)
        {
            SavePlayerNode (child, data, GetPath (node));
        }
    }

    public TreeNode CreateNode (string path, bool isDirectory, bool fromCurrentDirectory = false)
    {
        var parts = path.Split (new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        var current = Root;
        if (fromCurrentDirectory)
            current = CurrentDirectory;

        foreach (var part in parts)
        {
            var next = current.FindChild (part);
            if (next == null)
            {
                next = new TreeNode (part, isDirectory, current);
                current.AddChild (next);
            }
            current = next;
        }

        return current;
    }

    public TreeNode FindNode (string path)
    {
        var parts = path.Split (new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var current = Root;

        foreach (var part in parts)
        {
            current = current.FindChild (part);
            if (current == null)
            {
                return null;
            }
        }

        return current;
    }

    public bool ChangeDirectory (string path)
    {
        if (path == "/")
        {
            CurrentDirectory = Root;
            return true;
        }

        var parts = path.Split (new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var current = CurrentDirectory;

        foreach (var part in parts)
        {
            if (part == "..")
            {
                if (current.Parent == null)
                    return false; 

                current = current.Parent;
            }
            else
            {
                var next = current.FindChild (part);

                if (next == null || !next.IsDirectory)
                    return false; 

                current = next;
            }
        }

        CurrentDirectory = current;
        return true;
    }

    public string GetPath (TreeNode node)
    {
        if (node == Root)
            return "/";

        var path = new Stack<string> ();
        while (node != null && node != Root)
        {
            path.Push (node.Name);
            node = node.Parent;
        }

        return "/" + string.Join ("/", path);
    }

    public string GetPathOfCurrentDirectory ()
    {
        if (CurrentDirectory != Root)
            return GetPath (CurrentDirectory);
        else
            return "/";
    }

    public List<string> ListChildOfCurrentDirectory ()
    {
        return CurrentDirectory.Children.Select (child => child.Name).ToList ();
    }

    internal List<string> ListChildOfGivenPath (string path)
    {
        return FindNode(path).Children.Select (child => child.Name).ToList ();
    }
}

