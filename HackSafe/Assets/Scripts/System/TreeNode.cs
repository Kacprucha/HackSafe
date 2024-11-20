using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class TreeNode
{
    public string Name;
    public string Content;
    public bool IsDirectory { get; private set; }
    public bool IsKeyFile { get; private set; }
    public bool WasFileSigned { get; set; }
    public List<TreeNode> Children { get; private set; }
    public TreeNode Parent { get; set; }

    public TreeNode (string name, bool isDirectory, string content = null, TreeNode parent = null, bool isKey = false, bool wasSigned = false)
    {
        Name = name;
        IsDirectory = isDirectory;
        IsKeyFile = isKey;
        WasFileSigned = wasSigned;
        Children = new List<TreeNode> ();
        Parent = parent;

        if (IsKeyFile)
        {
            using (var rsa = RSA.Create (2048)) // Generate a 2048-bit RSA key
            {
                RSAParameters publicKeyParams = rsa.ExportParameters (false);
                string publicKeyPem = buildPublicKeyPem (publicKeyParams);
                Content = publicKeyPem;
            }
        }
        else
        {
            Content = content;
        }
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

    private string buildPublicKeyPem (RSAParameters publicKeyParams)
    {
        // Combine the modulus and exponent into a single byte array
        var keyData = new byte[publicKeyParams.Modulus.Length + publicKeyParams.Exponent.Length + 2];
        keyData[0] = (byte)publicKeyParams.Exponent.Length; // Exponent length prefix
        Buffer.BlockCopy (publicKeyParams.Exponent, 0, keyData, 1, publicKeyParams.Exponent.Length);
        Buffer.BlockCopy (publicKeyParams.Modulus, 0, keyData, publicKeyParams.Exponent.Length + 1, publicKeyParams.Modulus.Length);

        // Convert to base64 and wrap in PEM format
        string base64Key = Convert.ToBase64String (keyData, Base64FormattingOptions.InsertLineBreaks);
        return $"-----BEGIN PUBLIC KEY-----\n{base64Key}\n-----END PUBLIC KEY-----";
    }
}

