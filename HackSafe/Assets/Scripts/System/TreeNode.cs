using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class TreeNode
{
    public string Name;
    public bool IsDirectory { get; private set; }
    public List<TreeNode> Children { get; private set; }
    public TreeNode Parent { get; set; }

    public TreeNode (string name, bool isDirectory, TreeNode parent = null)
    {
        Name = name;
        IsDirectory = isDirectory;
        Children = new List<TreeNode> ();
        Parent = parent;
    }

    public void AddChild (TreeNode child)
    {
        child.Parent = this;
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

