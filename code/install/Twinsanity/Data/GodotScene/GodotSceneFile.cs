using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RehabSetup
{
    public class GodotSceneFile : GodotFileBase
    {
        public List<Node> Nodes = new List<Node>();

        public List<string> Editables = new List<string>();

        public override void WriteExtra(List<string> FileLines)
        {
            foreach (Node node in Nodes)
            {
                FileLines.Add(node.ToString());
                foreach (string line in node.Lines)
                {
                    FileLines.Add(line);
                }
            }
            if (Editables.Count != 0)
            {
                foreach (var edit in Editables)
                {
                    FileLines.Add($"[editable path=\"{edit}\"]");
                }
            }
        }

        public override string GetFileDescriptor()
        {
            int ResourceCount = InternalResourceList.Count + ExternalResourceList.Count;
            if (ResourceCount > 0)
            {
                ResourceCount++;
                return $"[gd_scene load_steps={ResourceCount} format=2]";
            }
            return $"[gd_scene format={ExportGodot.Format}]";
        }

        public class Node
        {
            public string Name = string.Empty;
            public string Type = string.Empty;
            public int InstanceID = -1;
            public Dictionary<string, string> KeyValues = new Dictionary<string, string>();
            public List<string> Lines = new List<string>();
            public List<string> Groups = new List<string>();

            public Node(string name)
            {
                Name = name;
            }
            public Node(string name, string type)
            {
                Name = name;
                Type = type;
            }
            public Node(string name, Dictionary<string, string> keys)
            {
                Name = name;
                KeyValues = keys;
            }

            public override string ToString()
            {
                string nodeText = $"[node ";
                if (!string.IsNullOrEmpty(Name))
                {
                    nodeText += $"name=\"{Name}\"";
                }
                if (!string.IsNullOrEmpty(Type))
                {
                    nodeText += $" type=\"{Type}\"";
                }
                foreach (KeyValuePair<string, string> pair in KeyValues)
                {
                    nodeText += $" {pair.Key}=\"{pair.Value}\"";
                }
                if (Groups.Count != 0)
                {
                    nodeText += $" groups=[";
                    for (int i = 0; i < Groups.Count - 1; i++)
                    {
                        nodeText += $"\"{Groups[i]}\", ";
                    }
                    nodeText += $"\"{Groups.Last()}\"]";
                }
                if (InstanceID != -1)
                {
                    nodeText += $" instance=ExtResource({InstanceID})";
                }
                nodeText += "]";
                return nodeText;
            }

            public void SetAsNode3D()
            {
                Type = ExportGodot.Node3D;
            }
        }
    }
}
