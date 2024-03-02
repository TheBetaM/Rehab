using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twinsanity;
using Twinsanity.Items;

namespace RehabSetup
{
    public static class ExportGodot
    {

        public static void ExportScenery(SceneryData Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Scenery\\");
            string SceneName = $"{Cont.ChunkName.Replace('\\','_')}-Scenery";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            TwinsSection targetFile = Cont.Parent;
            DynamicSceneryData DynScenery = targetFile.GetItem<DynamicSceneryData>(4);
            if (DynScenery != null && DynScenery.Models.Count != 0)
            {
                ModelScene.AddDynamicScenery(DynScenery, path, ExportedTextures);
            }
            ModelScene.AddScenery(Cont, path, ExportedTextures);
            ModelScene.WriteToFile($"{path}\\Scenery\\{SceneName}{SceneExtension}");
        }

        public static string ExportLODModel(LodModel Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\LODs\\");
            string SceneName = $"LODModel_{Cont.ID.ToString("X8")}";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddLODModel(Cont, path, ExportedTextures);
            // Re-hashing LOD due to ID collisions
            ModelScene.Serialize();
            string Hash = ModelScene.FileLines.GetSequenceHashCode().ToString("X8");
            string outPath = $"{path}\\LODs\\{SceneName}_{Hash}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return Hash;
            ModelScene.SaveToFile(outPath);
            return Hash;
        }

        public static void ExportSkydome(Skydome Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Skydomes\\");
            string hashName = DefaultHashes.SkyToName(Cont);
            string SceneName = $"Skydome_{hashName}";
            string outPath = $"{path}\\Skydomes\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddSkydome(Cont, path, ExportedTextures);
            ModelScene.WriteToFile(outPath);
        }

        public static void ExportDynamicScenery(DynamicSceneryData Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\DynamicScenery\\");
            TwinsSection targetFile = Cont.Parent;
            SceneryData Scene = targetFile.GetItem<SceneryData>(0);
            string SceneName = $"{Scene.ChunkName.Replace('\\', '_')}-DynamicScenery";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddDynamicScenery(Cont, path, ExportedTextures);
            ModelScene.WriteToFile($"{path}\\DynamicScenery\\{SceneName}{SceneExtension}");
        }

        public static void ExportSM(TwinsFile targetFile, string path, bool SceneOnly = false)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Levels\\");
            SceneryData Scene = targetFile.GetItem<SceneryData>(0);
            string SceneName = $"{Scene.ChunkName.Replace('\\', '_')}-SM";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddSM(targetFile, path, SceneOnly);
            ModelScene.WriteToFile($"{path}\\Levels\\{SceneName}{SceneExtension}");
        }

        public static void ExportCollisionData(ColData Cont, string path, string SceneName, bool SceneOnly = false)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Scenery\\");
            //string SceneName = $"{System.IO.Path.GetFileNameWithoutExtension(path)}";
            // optional for collision model visuals
            //GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, 1);
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddCollisionData(Cont, path, SceneName, SceneOnly);
            ModelScene.WriteToFile($"{path}\\Scenery\\{SceneName}{SceneExtension}");
        }

        public static void ExportOGI(GraphicsInfo Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Rigs\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"Rig_{hashName}";
            string outPath = $"{path}\\Rigs\\{SceneName}{SceneExtension}";
            bool forceWrite = false;
            if (AssetExporter.Check(outPath))
            {
                // Some models were packed incorrectly, so we have to pick and choose which file to get them from
                if (!DefaultHashes.OGI_Override.Contains(Cont.ID)) return;
                if (!Cont.ParentFile.FileName.ToLower().Contains(DefaultHashes.OGI_OverrideFile[Cont.ID])) return;
                forceWrite = true;
            }
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddOGI(Cont, path, ExportedTextures);
            if (forceWrite)
            {
                ModelScene.WriteToFileForce(outPath);
            }
            else
            {
                ModelScene.WriteToFile(outPath);
            }
        }

        public static void ExportGameObject(GameObject Cont, string path, bool SceneOnly = false)
        {
            // Default's GameObjects have some parameters that equivalents in chunks don't have, so it should be extracted first
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Actors\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"{hashName}";
            string outPath = $"{path}\\Actors\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddGameObject(Cont, path, SceneOnly);
            ModelScene.WriteToFile(outPath);
        }

        public static void ExportParticleData(ParticleData Cont, string path, bool SceneOnly = false)
        {
            string SceneName = $"Scene_ParticleData";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);
            ModelScene.AddParticleData(Cont, path, SceneOnly);
            ModelScene.WriteToFile($"{path}\\{SceneName}{SceneExtension}");
        }

        public static void ExportRM(TwinsFile targetFile, string path, bool AllowGlobal, TwinsFile file_Default, bool SceneOnly = false)
        {
            string SceneName = $"Scene_RM";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);

            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Levels\\");
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Scenery\\");
            ExportCollisionData(targetFile.GetItem<ColData>(9), path, $"Scene-Collision", SceneOnly);
            ModelScene.Add_InstancedScene($"../Scenery/Scene-Collision", $".");
            //ModelScene.Nodes.Last().Lines.Add("visible = false");

            ModelScene.AddRM(targetFile, path, SceneOnly, AllowGlobal);
            if (!targetFile.FileName.ToLower().Contains("default.rm"))
            {
                ModelScene.WriteToFile($"{path}\\Levels\\{SceneName}{SceneExtension}");
            }

            if (AllowGlobal && file_Default != null)
            {
                // Exports default resources
                GodotSceneFileTwinsanity Default = GodotSceneFileTwinsanity.Create("Default");
                Default.AddRM(file_Default, path, SceneOnly);
            }
        }

        public static void ExportAnimation(Animation Cont, string path)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Animations\\");
            string Extension = ".res";
            string outPath = $"{path}\\Animations\\{DefaultHashes.ToName(Cont.ParentType, Cont.ID)}{Extension}";
            if (AssetExporter.Check(outPath)) return;

            List<Pos> RigAddRot = new List<Pos>();
            List<uint> RigJointParent = new List<uint>();
            if (Cont.Parent.Parent.ContainsItem(0))
            {
                TwinsSection obj_section = Cont.Parent.Parent.GetItem<TwinsSection>(0);
                TwinsSection ogi_section = Cont.Parent.Parent.GetItem<TwinsSection>(3);
                if (obj_section.Records.Count > 0 && ogi_section.Records.Count != 0)
                {
                    bool foundAnim = false;
                    ushort OgiID = 0;
                    foreach (var item in obj_section.Records)
                    {
                        if (item is GameObject obj)
                        {
                            for (int i = 0; i < obj.Anims.Count; i++)
                            {
                                if (obj.Anims[i] == Cont.ID && obj.OGIs[i] != 65535)
                                {
                                    foundAnim = true;
                                    OgiID = obj.OGIs[i];
                                    break;
                                }
                            }
                        }
                        if (foundAnim)
                        {
                            break;
                        }
                    }
                    if (foundAnim && ogi_section.ContainsItem(OgiID))
                    {
                        GraphicsInfo ogi = ogi_section.GetItem<GraphicsInfo>(OgiID);
                        for (int i = 0; i < ogi.Joints.Length; i++)
                        {
                            RigAddRot.Add(ogi.Joints[i].Matrix[4]);
                            RigJointParent.Add(ogi.Joints[i].ParentJointIndex);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"MODEL NOT FOUND FOR ANIM {Cont.ID}");
                    }
                }
            }

            GodotBinaryAnimation Res = new GodotBinaryAnimation(Cont, RigAddRot, RigJointParent);
            if (AssetExporter.Check(outPath)) return;
            Res.WriteToFile(outPath);
        }

        public static void ExportFull(TwinsFile file_RM, TwinsFile file_SM, string path, bool AllowGlobal, TwinsFile file_Default, bool IncludeSkydome = true, bool SceneOnly = false)
        {
            SceneryData Scene = file_SM.GetItem<SceneryData>(0);
            string SceneName = $"{Scene.ChunkName.Replace('\\', '_')}";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName);

            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Levels\\");
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Scenery\\");
            ExportCollisionData(file_RM.GetItem<ColData>(9), path, $"{SceneName}-Collision", SceneOnly);
            ModelScene.Add_InstancedScene($"../Scenery/{SceneName}-Collision", $".");
            ModelScene.Nodes.Last().Lines.Add("visible = false");

            //Stopwatch Timer = new Stopwatch();
            //Timer.Start();
            if (AllowGlobal && file_Default != null)
            {
                // Exports default resources
                GodotSceneFileTwinsanity Default = GodotSceneFileTwinsanity.Create("Default");
                Default.AddRM(file_Default, path, SceneOnly);
                //Debug.WriteLine($"Default: {Timer.Elapsed}");
                //Timer.Restart();
            }

            ModelScene.AddSM(file_SM, path, SceneOnly, IncludeSkydome);
            //Debug.WriteLine($"SM: {Timer.Elapsed}");
            //Timer.Restart();

            ModelScene.AddRM(file_RM, path, SceneOnly, AllowGlobal);
            //Debug.WriteLine($"RM: {Timer.Elapsed}");
            //Timer.Restart();

            SceneryData Scenery = file_SM.GetItem<SceneryData>(0);
            bool SetSkyDome = false;
            if (file_SM.Type == TwinsFile.FileType.DemoSM2)
            {
                TwinsSection scene_sky_sec = Scenery.Parent.GetItem<TwinsSection>(6).GetItem<TwinsSection>(8);
                if (scene_sky_sec.ContainsItem(Scenery.SkydomeID)) SetSkyDome = true;
            }
            else
            {
                if (Scenery.SkydomeID != 0) SetSkyDome = true;
            }
            if (SetSkyDome && ModelScene.Nodes[0].Lines.Count != 0)
            {
                ModelScene.Nodes[0].Lines.Add($"SkydomePath = \"Skydomes/Skydome_{DefaultHashes.SkyToName(Scene.SkydomeID)}.tscn\"");
            }
            if (ModelScene.WorldEnvResourceID != 0)
            {
                ModelScene.Nodes[0].Lines.Add($"WorldEnv = SubResource( {ModelScene.WorldEnvResourceID} )");
            }

            ModelScene.WriteToFile($"{path}\\Levels\\{SceneName}{SceneExtension}");
            
            //Timer.Stop();
        }

        public static void ExportBIN(TwinsFile targetFile, string path)
        {
            string SoundExt = ".res";
            // Three menu sound effects
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Sounds\\");
            TwinsSection section = targetFile.GetItem<TwinsSection>(0);
            foreach (TwinsItem item in section.Records)
            {
                string SoundPath = $"{path}\\Sounds\\{DefaultHashes.ToName(SectionType.SE, item.ID)}{SoundExt}";
                //Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SoundPath));
                if (!AssetExporter.Check(SoundPath))
                {
                    if (item is SoundEffectX sfx)
                    {
                        GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(sfx);
                        wav.WriteToFile(SoundPath);
                    }
                    else if (item is SoundEffect sound)
                    {
                        GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(sound);
                        wav.WriteToFile(SoundPath);
                    }
                }
                
            }
        }

        public static void ExportXWB(TwinsFile targetFile, string path)
        {
            string FolderName = "GlobalVO";
            if (targetFile.FileName.Contains("Music")) FolderName = "Music";
            else if (targetFile.FileName.Contains("French")) return; //FolderName = "GlobalVO_French";
            else if (targetFile.FileName.Contains("German")) return; //FolderName = "GlobalVO_German";
            else if (targetFile.FileName.Contains("Italian")) return; //FolderName = "GlobalVO_Italian";

            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Sounds\\{FolderName}\\");
            TwinsSection section = targetFile.GetItem<TwinsSection>(0);
            foreach (TwinsItem item in section.Records)
            {
                XWB.Sound sfx = (XWB.Sound)item;
                string SoundPath = $"{path}\\Sounds\\{FolderName}\\{sfx.FileName}";
                string ResPath = $"{SoundPath}.res";
                string TResPath = $"{SoundPath}.tres";
                string WavPath = $"{SoundPath}.wav";
                GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(sfx, FolderName == "Music");
                wav.WriteToFile(ResPath);
                sfx.SoundData = null;
                wav = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void ExportMB(TwinsFile targetFile, string path)
        {
            string FolderName = "GlobalVO";
            if (targetFile.FileName.ToUpper().Contains("MUSIC")) FolderName = "Music";
            else if (targetFile.FileName.ToUpper().Contains("FRENCH")) return; //FolderName = "GlobalVO_French";
            else if (targetFile.FileName.ToUpper().Contains("GERMAN")) return; //FolderName = "GlobalVO_German";
            else if (targetFile.FileName.ToUpper().Contains("ITALIAN")) return; //FolderName = "GlobalVO_Italian";
            else if (targetFile.FileName.ToUpper().Contains("SPANISH")) return; //FolderName = "GlobalVO_Spanish";
            else if (targetFile.FileName.ToUpper().Contains("JAPANESE")) return; //FolderName = "GlobalVO_Japanese";

            bool undefinedDone = false;
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Sounds\\{FolderName}\\");
            TwinsSection section = targetFile.GetItem<TwinsSection>(0);
            foreach (TwinsItem item in section.Records)
            {
                MusicBank.Sound sfxHolder = (MusicBank.Sound)item;
                MusicHash.Track sfx = sfxHolder.track;
                if (sfx.Type >= 2) continue;
                if (sfx.Name == "undefined" && undefinedDone) continue;
                string SoundPath = $"{path}\\Sounds\\{FolderName}\\{sfx.Name}";
                string ResPath = $"{SoundPath}.res";
                if (sfx.Name == "undefined") undefinedDone = true;
                GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(sfx, FolderName == "Music");
                wav.WriteToFile(ResPath);
                sfx.SoundData = null;
                sfxHolder.track = null;
                wav = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void ExportMSB(TwinsFile targetFile, string path)
        {
            TwinsSection section = targetFile.GetItem<TwinsSection>(0);
            foreach (TwinsItem item in section.Records)
            {
                MusicBankDemo.Sound sfxHolder = (MusicBankDemo.Sound)item;
                MusicHashDemo.Track sfx = sfxHolder.track;
                string SoundPath = $"{path}\\Sounds\\{sfx.Name}";
                string ResPath = $"{SoundPath}.res";
                if (sfx.isStereo && sfx.Size < 0x1000) continue;
                GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(sfx, false);
                wav.WriteToFile(ResPath);
                sfx.SoundData = null;
                sfxHolder.track = null;
                wav = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        static string[] DemoIcons = [
            "1up-crash",
            "1up-cortex",
            "wumpa_icon",
            "crystal_icon",
            "relic_icon",
            "gem_greyed",
            "gem-blue",
            "gem-clear",
            "gem-green",
            "gem-purple",
            "gem-red",
            "gem-yellow",
        ];

        public static void ExportPSM(TwinsFile targetFile, string path, bool isPTC = false, bool isPSF = false)
        {
            string OutName = path;
            List<int> Widths = new List<int>();
            List<int> Heights = new List<int>();
            List<List<Color>> Textures = new List<List<Color>>();
            List<string> Names = new List<string>();
            bool MultiExport = false;
            string Extension = ".res";

            if (!isPTC && !isPSF)
            {
                if (targetFile.FileName.ToLower().EndsWith("icons.psm"))
                {
                    //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Textures\\Icons\\");
                    OutName = $"{path}\\Textures\\Icons";
                    MultiExport = true;
                }
                else
                {
                    string BaseFolder = "Extras";
                    string BaseName = targetFile.FileName;
                    if (BaseName.Contains("Language"))
                    {
                        if (BaseName.Contains("loading1"))
                        {
                            BaseName = BaseName.Replace("loading1", "Loading1");
                        }
                        BaseFolder = "Language";
                        // simplified asset paths for Titles
                        BaseName = BaseName.Replace("American", "English"); 
                        BaseName = BaseName.Replace("Japanese", "English");
                    }
                    else if (BaseName.Contains("Startup"))
                    {
                        BaseFolder = "Startup";
                    }
                    string OrigPath = BaseName.Replace(".psm", Extension).Replace(".PSM", Extension);
                    string ExtrasPath = $"{path}\\Textures\\{BaseFolder}\\";
                    int ExtrasStart = OrigPath.IndexOf(BaseFolder) + (BaseFolder.Length + 1);
                    string RelativePath = OrigPath.Substring(ExtrasStart);
                    OutName = $"{ExtrasPath}{RelativePath}";
                    //Directory.CreateDirectory(System.IO.Path.GetDirectoryName(OutName));
                }

                TwinsSection section = targetFile.GetItem<TwinsSection>(0);
                for (int i = 0; i < section.Records.Count; i++)
                {
                    TwinsPTC tptc = (TwinsPTC)section.Records[i];
                    if (tptc.TextureX != null)
                    {
                        Widths.Add(tptc.TextureX.Width);
                        Heights.Add(tptc.TextureX.Height);
                        Textures.Add(new List<Color>(tptc.TextureX.RawData));
                    }
                    else
                    {
                        Widths.Add(tptc.Texture.Width);
                        Heights.Add(tptc.Texture.Height);
                        Textures.Add(new List<Color>(tptc.Texture.RawData));
                    }
                    if (tptc.Material != null)
                    {
                        string mName = tptc.Material.Name.Replace("\0", "");
                        if (string.IsNullOrWhiteSpace(mName))
                        {
                            mName = DemoIcons[i];
                        }
                        Names.Add(mName);
                    }
                    else
                    {
                        Names.Add(string.Empty);
                    }
                }
            }
            else if (isPSF)
            {
                //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Textures\\Startup\\");

                TwinsSection section = targetFile.GetItem<TwinsSection>(0);
                for (int i = 0; i < section.Records.Count; i++)
                {
                    TwinsPTC tptc = (TwinsPTC)section.Records[i];
                    if (tptc.TextureX != null)
                    {
                        Widths.Add(tptc.TextureX.Width);
                        Heights.Add(tptc.TextureX.Height);
                        Textures.Add(new List<Color>(tptc.TextureX.RawData));
                    }
                    else
                    {
                        Widths.Add(tptc.Texture.Width);
                        Heights.Add(tptc.Texture.Height);
                        Textures.Add(new List<Color>(tptc.Texture.RawData));
                    }
                    Names.Add(string.Empty);
                }

                OutName = $"{path}\\Textures\\Startup\\Font_{System.IO.Path.GetFileNameWithoutExtension(targetFile.FileName)}{Extension}";
            }
            else
            {
                //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Textures\\Startup\\");

                TwinsPTC tptc = targetFile.GetItem<TwinsPTC>(0);
                if (tptc.TextureX != null)
                {
                    Widths.Add(tptc.TextureX.Width);
                    Heights.Add(tptc.TextureX.Height);
                    Textures.Add(new List<Color>(tptc.TextureX.RawData));
                }
                else
                {
                    Widths.Add(tptc.Texture.Width);
                    Heights.Add(tptc.Texture.Height);
                    Textures.Add(new List<Color>(tptc.Texture.RawData));
                }
                
                Names.Add(string.Empty);

                OutName = $"{path}\\Textures\\Startup\\{System.IO.Path.GetFileNameWithoutExtension(targetFile.FileName)}{Extension}";
            }

            int TexCount = Textures.Count;

            if (!MultiExport)
            {
                GodotBinaryImageTexture TexRes = new(Textures, Widths, Heights);
                if (!AssetExporter.Check(OutName))
                    TexRes.WriteToFile(OutName);
            }
            else
            {
                for (int i = 0; i < TexCount; i++)
                {
                    string TexName = $"{OutName}\\{Names[i]}.res";
                    GodotBinaryImageTexture TexRes = new(Textures[i], Widths[i], Heights[i]);
                    if (!AssetExporter.Check(TexName))
                        TexRes.WriteToFile(TexName);
                }
            }

        }

        public static uint ExportModelResource(RigidModel Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Mesh\\");
            string SceneName = $"Model";
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, -1, ExportGodot.MeshInstance3D);
            ModelScene.AddRigidModelResource(Cont, path, ExportedTextures);
            // Re-hashing RigidModel due to ID collisions
            ModelScene.Serialize();
            uint Hash = ModelScene.FileLines.GetSequenceHashCode();
            string outName = DefaultHashes.RigidToName(Cont.ID, Hash);
            string outPath = $"{path}\\Mesh\\{outName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return Hash;
            ModelScene.Nodes[0].Name = outName;
            ModelScene.SaveToFile(outPath);
            return Hash;
        }
        public static void ExportSkinXResource(SkinX Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Skins\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"Skin_{hashName}";
            string outPath = $"{path}\\Skins\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, -1, ExportGodot.MeshInstance3D);
            ModelScene.AddSkinXResource(Cont, path, ExportedTextures);
            ModelScene.WriteToFile(outPath);
        }
        public static void ExportSkinResource(Skin Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Skins\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"Skin_{hashName}";
            string outPath = $"{path}\\Skins\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, -1, ExportGodot.MeshInstance3D);
            ModelScene.AddSkinResource(Cont, path, ExportedTextures);
            ModelScene.WriteToFile(outPath);
        }
        public static void ExportBlendSkinResource(BlendSkin Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Skins\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"BlendSkin_{hashName}";
            string outPath = $"{path}\\Skins\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, -1, ExportGodot.MeshInstance3D);
            ModelScene.AddBlendSkinResource(Cont, path, ExportedTextures);
            ModelScene.WriteToFile(outPath);
            //ExportGLTF.Export(Cont, $"{System.IO.Path.GetDirectoryName(path)}\\Skins\\{SceneName}.glb");
        }
        public static void ExportBlendSkinXResource(BlendSkinX Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Skins\\");
            string hashName = DefaultHashes.ToName(Cont.ParentType, Cont.ID);
            string SceneName = $"BlendSkin_{hashName}";
            string outPath = $"{path}\\Skins\\{SceneName}{SceneExtension}";
            if (AssetExporter.Check(outPath)) return;
            GodotSceneFileTwinsanity ModelScene = GodotSceneFileTwinsanity.Create(SceneName, -1, ExportGodot.MeshInstance3D);
            ModelScene.AddBlendSkinXResource(Cont, path, ExportedTextures);
            ModelScene.WriteToFile(outPath);
            //ExportGLTF.Export(Cont, $"{System.IO.Path.GetDirectoryName(path)}\\Skins\\{SceneName}.glb");
        }

        #region Constants

        public const bool ExportLODs = false; // set to false to use only the highest detail model and ignore the original LOD stuff
        public const bool ExportEditableObjects = false;
        public const string ScriptExt = ".cs"; // or .gd
        public const string SceneExtension = ".tscn"; //".escn"

        public const uint Format = 3;
        public const string Node3D = "Node3D";
        public const string StandardMaterial3D = "StandardMaterial3D";
        public const string ShaderMaterial = "ShaderMaterial";
        public const string MeshInstance3D = "MeshInstance3D";
        public const string ConvexPolygonShape3D = "ConvexPolygonShape3D";
        public const string ConcavePolygonShape3D = "ConcavePolygonShape3D";
        public const string RigidBody3D = "RigidBody3D";
        public const string StaticBody3D = "StaticBody3D";
        public const string CollisionShape3D = "CollisionShape3D";
        public const string BoxShape3D = "BoxShape3D";
        public const string Area3D = "Area3D";
        public const string CharacterBody3D = "CharacterBody3D";
        public const string Transform3D = "Transform3D";
        public const string Marker3D = "Marker3D";
        public const string materialOverride = "surface_material_override";
        public const string materialCullMode = "cull_mode";
        public const string materialBlendMode = "blend_mode";
        public const string materialTransparency = "transparency = 4"; // depth pre-pass
        public const string materialScissor = "transparency = 2";
        public const string materialDepthDrawMode = "";
        public const string Texture2D = "Texture2D";
        public const string Path3D = "Path3D";
        public const string ambientLightSource = "ambient_light_source = 2";
        public const string transformPosition = "position";
        
        #endregion

        #region Helpers
        public static string ToText(this float f)
        {
            return f.ToString().ToLower().Replace(',', '.');
        }

        public static uint GetSequenceHashCode(this List<string> sequence)
        {
            Crc32 crc = new Crc32();
            const uint seed = 487;
            const uint modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current * modifier) + crc.Get(Encoding.ASCII.GetBytes(item)));
            }
        }

        public static uint GetSequenceHashCode(this List<Color> sequence)
        {
            Crc32 crc = new Crc32();
            const uint seed = 487;
            const uint modifier = 31;

            unchecked
            {
                return sequence.Aggregate(seed, (current, item) =>
                    (current * modifier) + crc.Get(new byte[4] {item.R, item.G, item.B, item.A}) );
            }
        }

        public static uint GetSequenceHashCode(this byte[] sequence)
        {
            Crc32 crc = new Crc32();
            return crc.Get(sequence);
        }
        #endregion

    }

#region CRC32
    /// <summary>
    /// Performs 32-bit reversed cyclic redundancy checks.
    /// </summary>
    public class Crc32
    {
#region Constants
        /// <summary>
        /// Generator polynomial (modulo 2) for the reversed CRC32 algorithm. 
        /// </summary>
        private const UInt32 s_generator = 0xEDB88320;
#endregion

#region Constructors
        /// <summary>
        /// Creates a new instance of the Crc32 class.
        /// </summary>
        public Crc32()
        {
            // Constructs the checksum lookup table. Used to optimize the checksum.
            m_checksumTable = Enumerable.Range(0, 256).Select(i =>
            {
                var tableEntry = (uint)i;
                for (var j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0)
                        ? (s_generator ^ (tableEntry >> 1))
                        : (tableEntry >> 1);
                }
                return tableEntry;
            }).ToArray();
        }
#endregion

#region Methods
        /// <summary>
        /// Calculates the checksum of the byte stream.
        /// </summary>
        /// <param name="byteStream">The byte stream to calculate the checksum for.</param>
        /// <returns>A 32-bit reversed checksum.</returns>
        public UInt32 Get<T>(IEnumerable<T> byteStream)
        {
            try
            {
                // Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
            }
            catch (FormatException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (InvalidCastException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
            catch (OverflowException e)
            {
                throw new Exception("Could not read the stream out as bytes.", e);
            }
        }
#endregion

#region Fields
        /// <summary>
        /// Contains a cache of calculated checksum chunks.
        /// </summary>
        private readonly UInt32[] m_checksumTable;

#endregion
    }
#endregion
}
