using System;

public class FileSystem
{
    public TreeNode Root { get; private set; }

    public FileSystem ()
    {
        Root = new TreeNode ("/", true);
    }

    public TreeNode CreateNode (string path, bool isDirectory)
    {
        var parts = path.Split (new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var current = Root;

        foreach (var part in parts)
        {
            var next = current.FindChild (part);
            if (next == null)
            {
                next = new TreeNode (part, isDirectory);
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
}

