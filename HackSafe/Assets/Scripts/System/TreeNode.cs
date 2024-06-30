using System.Collections;
using System.Collections.Generic;

public class TreeNode
{
    public string Name;
    public bool IsDirectory { get; private set; }
    public List<TreeNode> Children { get; private set; }

    public TreeNode (string name, bool isDirectory)
    {
        Name = name;
        IsDirectory = isDirectory;
        Children = new List<TreeNode> ();
    }

    public void AddChild (TreeNode child)
    {
        Children.Add (child);
    }

    public void RemoveChild (TreeNode child)
    {
        Children.Remove (child);
    }

    public TreeNode FindChild (string name)
    {
        return Children.Find (child => child.Name == name);
    }
}

