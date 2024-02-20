using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RehabSetup
{
    public class GodotFileBase
    {
        public List<ExternalResource> ExternalResourceList = new List<ExternalResource>();
        public List<InternalResource> InternalResourceList = new List<InternalResource>();
        public List<string> FileLines = new List<string>();

        public void WriteToFile(string path)
        {
            Serialize();
            if (!AssetExporter.Check(path))
            {
                using (MemoryStream mStream = new())
                {
                    using (StreamWriter writer = new StreamWriter(mStream, null, -1, true))
                    {
                        foreach (var line in FileLines)
                        {
                            writer.WriteLine(line);
                        }
                    }
                    mStream.Position = 0;
                    AssetExporter.Add(path, mStream.ToArray());
                }
            }
        }
        public void WriteToFileForce(string path)
        {
            Serialize();
            using (MemoryStream mStream = new())
            {
                using (StreamWriter writer = new StreamWriter(mStream, null, -1, true))
                {
                    foreach (var line in FileLines)
                    {
                        writer.WriteLine(line);
                    }
                }
                mStream.Position = 0;
                lock (AssetExporter.Cache)
                {
                    AssetExporter.Cache[path.Replace('\\','/').Replace("//","/")] = mStream.ToArray();
                }
            }
            //File.WriteAllLines(path, FileLines);
        }
        public void Serialize()
        {
            FileLines = new List<string>();
            FileLines.Add(GetFileDescriptor());

            for (int i = 0; i < ExternalResourceList.Count; i++)
            {
                ExternalResource res = ExternalResourceList[i];
                FileLines.Add($"[ext_resource path=\"{res.Path}\" type=\"{res.Type}\" id={i + 1}]");
            }
            for (int i = 0; i < InternalResourceList.Count; i++)
            {
                InternalResource res = InternalResourceList[i];
                FileLines.Add($"[sub_resource type=\"{res.Type}\" id={i + 1}]");
                foreach (string line in res.Lines)
                {
                    FileLines.Add(line);
                }
            }

            WriteExtra(FileLines);
        }
        public void SaveToFile(string path)
        {
            if (!AssetExporter.Check(path))
            {
                using (MemoryStream mStream = new())
                {
                    using (StreamWriter writer = new StreamWriter(mStream, null, -1, true))
                    {
                        foreach (var line in FileLines)
                        {
                            writer.WriteLine(line);
                        }    
                    }
                    mStream.Position = 0;
                    AssetExporter.Add(path, mStream.ToArray());
                }
            }
        }

        public virtual string GetFileDescriptor() { return "invalid"; }
        public virtual void WriteExtra(List<string> FileLines) { }

        public class ExternalResource
        {
            public string Path = string.Empty;
            public string Type = "Resource";

            public ExternalResource(string path)
            {
                Path = path;
            }
            public ExternalResource(string path, string t)
            {
                Path = path;
                Type = t;
            }

            public void SetAsTexture() { Type = ExportGodot.Texture2D; }
            public void SetAsMaterial() { Type = ExportGodot.StandardMaterial3D; }
            public void SetAsPackedScene() { Type = "PackedScene"; }
            public void SetAsSpatial() { Type = ExportGodot.Node3D; }
            public void SetAsShader() { Type = "Shader"; }
            public void SetAsScript() { Type = "Script"; }
            public void SetAsAudio() { Type = "AudioStream"; }
            public void SetAsAnimation() { Type = "Animation"; }
            public void SetAsArrayMesh() { Type = "ArrayMesh"; }
        }
        public class InternalResource
        {
            public string Type = "Resource";
            public List<string> Lines = new List<string>();

            public void CreateMaterial()
            {
                Type = ExportGodot.StandardMaterial3D;
            }
            public void CreateShaderMaterial()
            {
                Type = ExportGodot.ShaderMaterial;
            }
            public void CreateStandardMaterial()
            {
                Lines.Add($"vertex_color_use_as_albedo = true");
                Lines.Add($"vertex_color_is_srgb = true");
                Lines.Add($"{ExportGodot.materialCullMode} = 2");
                Lines.Add($"backlight_enabled = true");
                Lines.Add($"backlight = Color(1, 1, 1, 1)");
            }
            public void CreateSolidMaterial(float R, float G, float B, float A)
            {
                Lines.Add($"albedo_color = Color( {R.ToText()}, {G.ToText()}, {B.ToText()}, {A.ToText()} )");
                Lines.Add($"{ExportGodot.materialCullMode} = 2");
                Lines.Add($"metallic_specular = 0.0");
            }

            public void CreateMaterialNoVColor()
            {
                Type = ExportGodot.StandardMaterial3D;
                Lines.Add($"{ExportGodot.materialCullMode} = 2");
                Lines.Add($"metallic_specular = 0.0");
            }

            public void CreateBoxShape()
            {
                Type = ExportGodot.BoxShape3D;
                Lines.Add("extents = Vector3( 1, 1, 1 )");
            }
            public void CreateBoxShape(float X, float Y, float Z)
            {
                Type = ExportGodot.BoxShape3D;
                Lines.Add($"extents = Vector3( {X.ToText()}, {Y.ToText()}, {Z.ToText()} )");
            }

            public void CreateWorldEnvironment()
            {
                Type = "Environment";
                Lines.Add("background_mode = 1");
                //Lines.Add(ExportGodot.ambientLightSource);
                //Lines.Add("ambient_light_color = Color( 1, 1, 1, 1 )");
                //Lines.Add("ambient_light_energy = 1.5");
            }
            public void CreateWorldEnvironmentLight(float R, float G, float B, float Power)
            {
                Lines.Add(ExportGodot.ambientLightSource);
                Lines.Add($"ambient_light_color = Color( {R.ToText()}, {G.ToText()}, {B.ToText()}, 1 )");
                Lines.Add($"ambient_light_energy = {Power.ToText()}");
            }
            public void CreateWorldEnvironment(bool fog, float R, float G, float B)
            {
                Type = "Environment";
                Lines.Add("background_mode = 1");
                if (!fog) return;
                /* looks ok, but doesn't work with skydome
                Lines.Add($"fog_enabled = true");
                Lines.Add($"fog_light_color = Color( {R.ToText()}, {G.ToText()}, {B.ToText()}, 1 )");
                Lines.Add($"fog_density = 0.0005");
                */
                Lines.Add($"volumetric_fog_enabled = true");
                Lines.Add($"volumetric_fog_emission = Color( {R.ToText()}, {G.ToText()}, {B.ToText()}, 1 )");
                Lines.Add($"volumetric_fog_density = 0.005");
            }

            public void CreateCurve3D()
            {
                Type = "Curve3D";
            }
        }

    }
}
