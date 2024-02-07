using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RehabSetup
{
    public class GodotResourceFile : GodotFileBase
    {
        public InternalResource Resource = new InternalResource();

        public override void WriteExtra(List<string> FileLines)
        {
            FileLines.Add($"[resource]");
            foreach (string Line in Resource.Lines)
            {
                FileLines.Add(Line);
            }
        }

        public override string GetFileDescriptor()
        {
            int ResourceCount = InternalResourceList.Count + ExternalResourceList.Count;
            if (ResourceCount > 0)
            {
                ResourceCount++;
                return $"[gd_resource type=\"{Resource.Type}\" load_steps={ResourceCount} format={ExportGodot.Format}]";
            }
            return $"[gd_resource type=\"{Resource.Type}\" format={ExportGodot.Format}]";
        }

    }
}
