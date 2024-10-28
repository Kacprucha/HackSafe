using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class TreeNode
{
    public string Name;
    public string Content;
    public bool IsDirectory { get; private set; }
    public List<TreeNode> Children { get; private set; }
    public TreeNode Parent { get; set; }

    public TreeNode (string name, bool isDirectory, string content = null, TreeNode parent = null)
    {
        Name = name;
        IsDirectory = isDirectory;
        Children = new List<TreeNode> ();
        Parent = parent;
        Content = content;
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

    public bool FindElementInContent (string element)
    {
        if (Content.NullIfEmpty () == null)
        {
            return false;
        }
        else
        {
            return Content.Contains (element);
        }
    }
}

