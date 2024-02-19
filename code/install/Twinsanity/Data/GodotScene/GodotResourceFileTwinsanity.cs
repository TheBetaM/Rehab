using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Twinsanity;
using Twinsanity.Items;

namespace RehabSetup
{
    public class GodotResourceFileTwinsanity : GodotResourceFile
    {

        public static GodotResourceFileTwinsanity Create(string Name)
        {
            GodotResourceFileTwinsanity Res = new GodotResourceFileTwinsanity();
            return Res;
        }

        public void AddCollisionSurface(CollisionSurface Surf, string path)
        {
            // todo: particle scene references

            //string CodePath = $"{System.IO.Path.GetDirectoryName(path)}\\Code\\Containers\\CollisionSurfaceData.gd";
            //ExportGodot.ContainerWriter.ExportContainer_Surface(CodePath);
            //ExternalResource CodeRef = new ExternalResource($"../Code/Containers/CollisionSurfaceData.gd");
            //CodeRef.SetAsScript();
            //ExternalResourceList.Add(CodeRef);

            Resource.Lines.Add($"script = ExtResource ( { ExternalResourceList.Count } )");

            if (Surf.Sound_1 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_1)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound1 = ExtResource( {ExternalResourceList.Count} )");
            }
            if (Surf.Sound_2 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_2)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound2 = ExtResource( {ExternalResourceList.Count} )");
            }
            if (Surf.Sound_3 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_3)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound3 = ExtResource( {ExternalResourceList.Count} )");
            }
            if (Surf.Sound_4 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_4)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound4 = ExtResource( {ExternalResourceList.Count} )");
            }
            if (Surf.Sound_5 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_5)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound5 = ExtResource( {ExternalResourceList.Count} )");
            }
            if (Surf.Sound_6 != 65535)
            {
                ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Surf.Sound_6)}.wav");
                SoundRes.SetAsAudio();
                ExternalResourceList.Add(SoundRes);
                Resource.Lines.Add($"Sound6 = ExtResource( {ExternalResourceList.Count} )");
            }

        }

        public void AddInstanceTemplate(InstanceTemplate Temp, string path)
        {
            //string CodePath = $"{System.IO.Path.GetDirectoryName(path)}\\Code\\Containers\\InstanceTemplate.gd";
            //ExportGodot.ContainerWriter.ExportContainer_Template(CodePath);
            //ExternalResource CodeRef = new ExternalResource($"../Code/Containers/InstanceTemplate.gd");
            //CodeRef.SetAsScript();
            //ExternalResourceList.Add(CodeRef);
            //int CodeID = ExternalResourceList.Count;

            ExternalResource PrefabRef = new ExternalResource($"../Actors/{DefaultHashes.ToName(SectionType.Object, Temp.ObjectID)}{ExportGodot.SceneExtension}");
            PrefabRef.SetAsPackedScene();
            ExternalResourceList.Add(PrefabRef);
            int PrefabID = ExternalResourceList.Count;

            //Resource.Lines.Add($"script = ExtResource ( { CodeID } )");
            Resource.Lines.Add($"Prefab = ExtResource ( { PrefabID } )");
            Resource.Lines.Add($"Name = { Temp.Name }");
            //Resource.Lines.Add($"TemplateID = { Temp.ID }");
            Resource.Lines.Add($"Flags = { Temp.Properties }");
            Resource.Lines.Add($"Bitfield = { Temp.Bitfield }");
            Resource.Lines.Add($"HeaderInt1 = { Temp.HeaderInt1 }");
            Resource.Lines.Add($"HeaderInt2 = { Temp.HeaderInt2 }");
            Resource.Lines.Add($"HeaderInt3 = { Temp.HeaderInt3 }");
            Resource.Lines.Add($"UnkShort = { Temp.UnkShort }");
            Resource.Lines.Add($"Flag1 = { Temp.UnkFlags[0] }");
            Resource.Lines.Add($"Flag2 = { Temp.UnkFlags[1] }");
            if (Temp.UnkFlags.Length > 2)
            {
                Resource.Lines.Add($"Flag3 = {Temp.UnkFlags[2]}");
                Resource.Lines.Add($"Flag4 = {Temp.UnkFlags[3]}");
                Resource.Lines.Add($"Flag5 = {Temp.UnkFlags[4]}");
                Resource.Lines.Add($"Flag6 = {Temp.UnkFlags[5]}");
            }
            if (Temp.Ints.Length != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"IntRegisters = [ ");
                for (int a = 0; a < Temp.Ints.Length - 1; a++)
                {
                    IntReg.Append($"{Temp.Ints[a]}, ");
                }
                IntReg.Append($"{Temp.Ints.Last()} ]");
                Resource.Lines.Add(IntReg.ToString());
            }
            if (Temp.Flags.Length != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"AngleRegisters = [ ");
                for (int a = 0; a < Temp.Flags.Length - 1; a++)
                {
                    IntReg.Append($"{Temp.Flags[a]}, ");
                }
                IntReg.Append($"{Temp.Flags.Last()} ]");
                Resource.Lines.Add(IntReg.ToString());
            }
            if (Temp.Floats.Length != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"FloatRegisters = [ ");
                for (int a = 0; a < Temp.Floats.Length - 1; a++)
                {
                    IntReg.Append($"{Temp.Floats[a].ToText()}, ");
                }
                IntReg.Append($"{Temp.Floats.Last().ToText()} ]");
                Resource.Lines.Add(IntReg.ToString());
            }

        }

        public static string[] ControlPacketNames = new string[]
        {
            "Selector_SyncIndex",
            "KeyIndex_FocusData",
            "MoveSpeed_RiseHeight",
            "TurnSpeed",
            "RawPosX",
            "RawPosY",
            "RawPosZ",
            "RawAngsX_Pitch",
            "RawAngsY_Yaw",
            "RawAngsZ_Roll",
            "Delay",
            "Duration_Curvy_Homepower",
            "TumbleData",
            "SpinData",
            "TwistData",
            "SqrTolerance_RandRange",
            "Power_Gravity_Banking",
            "Damping_SpeedLim_Braking",
            "ACDist_RTOpt_ShiftFreq",
            "DecDist_PhysOpt_Shift",
            "Bounce_BankLimit",
            "SyncUnit",
            "JointIndex",
        };

        public void AddAnimation(Animation Anim, string path, List<Pos> RigAddRot, List<uint> RigJointParent)
        {
            float FrameStep = 0.02f * 2f; // All animations are 25 FPS
            Resource.Lines.Add($"resource_name = \"{DefaultHashes.ToName(SectionType.Animation, Anim.ID)}\"");
            Resource.Type = "Animation";
            if (Anim.TotalFrames == 0 && Anim.FacialAnimationTotalFrames == 0) return;

            uint AllFrames = Anim.TotalFrames;
            if (AllFrames == 0)
            {
                AllFrames = Anim.FacialAnimationTotalFrames;
            }
            if (AllFrames == 0) return;
            Resource.Lines.Add($"length = {(FrameStep * AllFrames).ToText()}");
            Resource.Lines.Add($"step = {FrameStep.ToText()}");

            int i = 0;
            // Bone animations
            ushort animFrames = Anim.TotalFrames;
            if (animFrames != 0)
            {
                List<Dictionary<int, (System.Numerics.Vector3, System.Numerics.Vector4, System.Numerics.Vector3)>> boneTransform = new();
                for (int frame = 0; frame < animFrames; frame++)
                {
                    var frameTransform = new Dictionary<int, (System.Numerics.Vector3, System.Numerics.Vector4, System.Numerics.Vector3)>();
                    GenerateAnimFrane(Anim, 0, frame, animFrames, new System.Numerics.Vector3(1, 1, 1), RigAddRot, RigJointParent, ref frameTransform);
                    boneTransform.Add(frameTransform);
                }
                // note: in Godot 3 it's a transform track, and in Godot 4 it's a pos/rot/scale track
                List<float> timeStamps = new();
                for (int t = 0; t < animFrames; t++)
                {
                    timeStamps.Add(t * FrameStep);
                }
                for (int j = 0; j < Anim.JointsSettings.Count; j++)
                {
                    if (!boneTransform[0].ContainsKey(j))
                    {
                        // if there are more joints in the animation than the skeleton, the extra ones won't get animated...
                        continue;
                    }
                    StringBuilder Strpos = new();
                    StringBuilder Strrot = new();
                    StringBuilder Strscale = new();
                    for (int t = 0; t < animFrames; t++)
                    {
                        //System.Numerics.Matrix4x4.Decompose(boneTransform[t][j].Item1, out var scale, out var rot, out var pos);
                        var pos = boneTransform[t][j].Item1;
                        var rot = boneTransform[t][j].Item2;
                        var scale = boneTransform[t][j].Item3;
                        Strpos.Append($"{timeStamps[t].ToText()}, 1, {pos.X.ToText()}, {pos.Y.ToText()}, {pos.Z.ToText()}, ");
                        Strrot.Append($"{timeStamps[t].ToText()}, 1, {rot.X.ToText()}, {rot.Y.ToText()}, {rot.Z.ToText()}, {rot.W.ToText()}, ");
                        Strscale.Append($"{timeStamps[t].ToText()}, 1, {scale.X.ToText()}, {scale.Y.ToText()}, {scale.Z.ToText()}, ");
                    }
                    Strpos.Remove(Strpos.Length - 2, 2);
                    Strrot.Remove(Strrot.Length - 2, 2);
                    Strscale.Remove(Strscale.Length - 2, 2);
                    //Resource.Lines.Add($"tracks/{i}/type = \"transform\"");
                    Resource.Lines.Add($"tracks/{i}/type = \"position_3d\"");
                    Resource.Lines.Add($"tracks/{i}/imported = false");
                    Resource.Lines.Add($"tracks/{i}/enabled = true");
                    Resource.Lines.Add($"tracks/{i}/path = NodePath(\".:joint{j}\")");
                    Resource.Lines.Add($"tracks/{i}/interp = 1");
                    Resource.Lines.Add($"tracks/{i}/loop_wrap = true");
                    Resource.Lines.Add($"tracks/{i}/keys = PackedFloat32Array({Strpos.ToString()})");
                    i++;
                    Resource.Lines.Add($"tracks/{i}/type = \"rotation_3d\"");
                    Resource.Lines.Add($"tracks/{i}/imported = false");
                    Resource.Lines.Add($"tracks/{i}/enabled = true");
                    Resource.Lines.Add($"tracks/{i}/path = NodePath(\".:joint{j}\")");
                    Resource.Lines.Add($"tracks/{i}/interp = 1");
                    Resource.Lines.Add($"tracks/{i}/loop_wrap = true");
                    Resource.Lines.Add($"tracks/{i}/keys = PackedFloat32Array({Strrot.ToString()})");
                    i++;
                    Resource.Lines.Add($"tracks/{i}/type = \"scale_3d\"");
                    Resource.Lines.Add($"tracks/{i}/imported = false");
                    Resource.Lines.Add($"tracks/{i}/enabled = true");
                    Resource.Lines.Add($"tracks/{i}/path = NodePath(\".:joint{j}\")");
                    Resource.Lines.Add($"tracks/{i}/interp = 1");
                    Resource.Lines.Add($"tracks/{i}/loop_wrap = true");
                    Resource.Lines.Add($"tracks/{i}/keys = PackedFloat32Array({Strscale.ToString()})");
                    i++;
                }
            }

            // Blend shape animations
            if (Anim.FacialAnimationTotalFrames == 0) return;
            var jointSetting = Anim.FacialJointsSettings[0];
            var shapesAmount = ((jointSetting.Flags >> 0x8) & 0xf);
            ushort blendFrames = Anim.FacialAnimationTotalFrames;
            List<float[]> blendTransform = new();
            for (int frame = 0; frame < blendFrames; frame++)
            {
                blendTransform.Add(GodotUtil.GetFacialAnimationTransform(Anim, frame));
            }
            for (int shape = 0; shape < shapesAmount; shape++)
            {
                Resource.Lines.Add($"tracks/{i + shape}/type = \"value\"");
                //Resource.Lines.Add($"tracks/{i + shape}/type = \"blend_shape\"");
                Resource.Lines.Add($"tracks/{i + shape}/imported = false");
                Resource.Lines.Add($"tracks/{i + shape}/enabled = true");
                //Resource.Lines.Add($"tracks/{i + shape}/path = NodePath(\"BlendSkin/Mesh2:blend_shapes/morph_{shape}\")");
                Resource.Lines.Add($"tracks/{i + shape}/path = NodePath(\"BlendSkin:blend_shapes/morph_{shape}\")");
                Resource.Lines.Add($"tracks/{i + shape}/interp = 1");
                Resource.Lines.Add($"tracks/{i + shape}/loop_wrap = true");
                Resource.Lines.Add($"tracks/{i + shape}/keys = " + "{");
                StringBuilder times = new();
                for (int t = 0; t < blendFrames; t++)
                {
                    times.Append($"{(t * FrameStep).ToText()}, ");
                }
                times.Remove(times.Length - 2, 2);
                StringBuilder values = new();
                for (int t = 0; t < blendFrames; t++)
                {
                    values.Append($"{blendTransform[t][shape].ToText()}, ");
                }
                values.Remove(values.Length - 2, 2);
                Resource.Lines.Add($"\"times\": PackedFloat32Array({times.ToString()}),");
                Resource.Lines.Add($"\"values\": [{values.ToString()}]");
                //Resource.Lines.Add($"\"blend_shapes\": [{values.ToString()}]");
                Resource.Lines.Add("}");
            }

            
        }

        void GenerateAnimFrane(Animation Anim, int j, int frame, ushort animFrames, System.Numerics.Vector3 parentScale, List<Pos> RigAddRot, List<uint> RigJointParent,
            ref Dictionary<int, (System.Numerics.Vector3, System.Numerics.Vector4, System.Numerics.Vector3)> frameTransform)
        {
            if (j > Anim.JointsSettings.Count - 1)
            {
                return;
            }
            Pos AddRotPos = new Pos(0, 0, 0, 1);
            if (j < RigAddRot.Count)
            {
                AddRotPos = RigAddRot[j];
            }
            int nextFrame = frame;
            if (frame < animFrames - 1)
            {
                nextFrame = frame + 1;
            }
            GodotUtil.GetMainAnimationTransform(Anim, j, frame, nextFrame, parentScale, AddRotPos, out var pos, out var rot, out var scale, out var rawScale);
            frameTransform.Add(j, (pos, rot, scale));
            for (int i = 0; i < RigJointParent.Count; i++)
            {
                if (RigJointParent[i] == j)
                {
                    GenerateAnimFrane(Anim, i, frame, animFrames, rawScale, RigAddRot, RigJointParent, ref frameTransform);
                }
            }
        }

        public void AddResetAnimation(GraphicsInfo rig, string path, uint shapesAmount)
        {
            Resource.Lines.Add($"length=0.001");
            Resource.Type = "Animation";

            Dictionary<int, System.Numerics.Matrix4x4> RestPoses = new();

            GodotUtil.ComputeTposeTransform(rig, 0, System.Numerics.Matrix4x4.Identity, ref RestPoses);

            int i = 0;
            // Bone keys
            for (int j = 0; j < rig.Joints.Length; j++)
            {
                StringBuilder Strpos = new();
                StringBuilder Strrot = new();
                StringBuilder Strscale = new();
                System.Numerics.Matrix4x4.Decompose(RestPoses[j], out System.Numerics.Vector3 scale, out System.Numerics.Quaternion rot, out System.Numerics.Vector3 pos);
                Strpos.Append($"0,1,{pos.X.ToText()},{pos.Y.ToText()},{pos.Z.ToText()}, ");
                Strrot.Append($"0,1,{rot.X.ToText()},{rot.Y.ToText()},{rot.Z.ToText()},{rot.W.ToText()}, ");
                Strscale.Append($"0,1,{scale.X.ToText()},{scale.Y.ToText()},{scale.Z.ToText()}, ");
                Strpos.Remove(Strpos.Length - 2, 2);
                Strrot.Remove(Strrot.Length - 2, 2);
                Strscale.Remove(Strscale.Length - 2, 2);
                //Resource.Lines.Add($"tracks/{i}/type = \"transform\"");
                Resource.Lines.Add($"tracks/{i}/type=\"position_3d\"");
                //Resource.Lines.Add($"tracks/{i}/imported=false");
                //Resource.Lines.Add($"tracks/{i}/enabled=true");
                Resource.Lines.Add($"tracks/{i}/path=NodePath(\".:joint{j}\")");
                //Resource.Lines.Add($"tracks/{i}/interp=1");
                //Resource.Lines.Add($"tracks/{i}/loop_wrap=true");
                Resource.Lines.Add($"tracks/{i}/keys=PackedFloat32Array({Strpos.ToString()})");
                i++;
                Resource.Lines.Add($"tracks/{i}/type=\"rotation_3d\"");
                //Resource.Lines.Add($"tracks/{i}/imported=false");
                //Resource.Lines.Add($"tracks/{i}/enabled=true");
                Resource.Lines.Add($"tracks/{i}/path=NodePath(\".:joint{j}\")");
                //Resource.Lines.Add($"tracks/{i}/interp=1");
                //Resource.Lines.Add($"tracks/{i}/loop_wrap=true");
                Resource.Lines.Add($"tracks/{i}/keys=PackedFloat32Array({Strrot.ToString()})");
                i++;
                Resource.Lines.Add($"tracks/{i}/type=\"scale_3d\"");
                //Resource.Lines.Add($"tracks/{i}/imported = false");
                //Resource.Lines.Add($"tracks/{i}/enabled = true");
                Resource.Lines.Add($"tracks/{i}/path=NodePath(\".:joint{j}\")");
                //Resource.Lines.Add($"tracks/{i}/interp = 1");
                //Resource.Lines.Add($"tracks/{i}/loop_wrap = true");
                Resource.Lines.Add($"tracks/{i}/keys=PackedFloat32Array({Strscale.ToString()})");
                i++;
            }

            // Blend shape keys
            for (int shape = 0; shape < shapesAmount; shape++)
            {
                Resource.Lines.Add($"tracks/{i + shape}/type=\"value\"");
                //Resource.Lines.Add($"tracks/{i + shape}/type = \"blend_shape\"");
                //Resource.Lines.Add($"tracks/{i + shape}/imported = false");
                //Resource.Lines.Add($"tracks/{i + shape}/enabled = true");
                //Resource.Lines.Add($"tracks/{i + shape}/path = NodePath(\"BlendSkin/Mesh2:blend_shapes/morph_{shape}\")");
                Resource.Lines.Add($"tracks/{i + shape}/path=NodePath(\"BlendSkin:blend_shapes/morph_{shape}\")");
                //Resource.Lines.Add($"tracks/{i + shape}/interp = 1");
                //Resource.Lines.Add($"tracks/{i + shape}/loop_wrap = true");
                Resource.Lines.Add($"tracks/{i + shape}/keys=" + "{");
                Resource.Lines.Add($"\"times\": PackedFloat32Array(0),");
                Resource.Lines.Add($"\"values\": [0]");
                //Resource.Lines.Add($"\"blend_shapes\": [0]");
                Resource.Lines.Add("}");
            }

            
        }

        #region XBOX

        public void AddModelX(ModelX Model)
        {
            Resource.Type = "ArrayMesh";

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                ModelX.SubModel Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                int vertexcount = 0;
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        Indices.Append(string.Format("{0}, {1}, {2}, ", vertexcount + ((a & 0x1) == 0x1 ? a + 1 : a + 0), vertexcount + ((a & 0x1) == 0x1 ? a + 0 : a + 1), vertexcount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    vertexcount += (int)Sub.GroupList[g];
                }
                for (int x = 0; x < Sub.VData.Count; x++)
                {
                    ModelX.VertexData VData = Sub.VData[x];
                    Vertices.Append($"{(-VData.X).ToText()}, {VData.Y.ToText()}, {VData.Z.ToText()}, ");
                    UVs.Append($"{VData.UV_X.ToText()}, {(-VData.UV_Y).ToText()}, ");

                    int R = VData.R;
                    int G = VData.G;
                    int B = VData.B;
                    int A = VData.A;
                    Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                }
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                Indices.Remove(Indices.Length - 2, 2);
                Indices.Append($")");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                Resource.Lines.Add($"null,"); // normals (todo)
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add($"null,"); // bones
                Resource.Lines.Add($"null,"); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                Resource.Lines.Add($"\"morph_arrays\":[]");
                Resource.Lines.Add("}");
            }
        }

        public void AddSkinX(SkinX Model)
        {
            Resource.Type = "ArrayMesh";

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                SkinX.SubModel Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                StringBuilder Bones = new StringBuilder($"IntArray(");
                StringBuilder Weights = new StringBuilder($"PoolRealArray(");
                int vertexcount = 0;
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        Indices.Append(string.Format("{0}, {1}, {2}, ", vertexcount + ((a & 0x1) == 0x1 ? a + 1 : a + 0), vertexcount + ((a & 0x1) == 0x1 ? a + 0 : a + 1), vertexcount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    vertexcount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                for (int x = 0; x < Sub.VData.Count; x++)
                {
                    SkinX.VertexData VData = Sub.VData[x];
                    Vertices.Append($"{(-VData.X).ToText()}, {VData.Y.ToText()}, {VData.Z.ToText()}, ");
                    UVs.Append($"{VData.UV_X.ToText()}, {(-VData.UV_Y).ToText()}, ");
                    int R = VData.R;
                    int G = VData.G;
                    int B = VData.B;
                    int A = VData.A;
                    Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                    Weights.Append($"{VData.Weight1.ToText()}, {VData.Weight2.ToText()}, {VData.Weight3.ToText()}, 0.0, ");

                    ushort Bone1 = 0;
                    ushort Bone2 = 0;
                    ushort Bone3 = 0;
                    int Joint1 = (VData.Joint1 - 16) / 4;
                    int Joint2 = (VData.Joint2 - 16) / 4;
                    int Joint3 = (VData.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = (ushort)Sub.GroupJoints[GroupID][Joint1];
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = (ushort)Sub.GroupJoints[GroupID][Joint2];
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = (ushort)Sub.GroupJoints[GroupID][Joint3];
                    }
                    Bones.Append($"{Bone1}, {Bone2}, {Bone3}, 0, ");
                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }
                }
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                Indices.Remove(Indices.Length - 2, 2);
                Indices.Append($")");
                Bones.Remove(Bones.Length - 2, 2);
                Bones.Append($"),");
                Weights.Remove(Weights.Length - 2, 2);
                Weights.Append($"),");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                Resource.Lines.Add($"null,"); // normals (todo)
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add(Bones.ToString()); // bones
                Resource.Lines.Add(Weights.ToString()); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                Resource.Lines.Add($"\"morph_arrays\":[]");
                Resource.Lines.Add("}");
            }
        }

        public void AddBlendSkinX(BlendSkinX Model)
        {
            bool ExportBlendShapes = false; // Blend shapes are super buggy in Godot right now... (export code and data is 100% fine)
            Resource.Type = "ArrayMesh";

            if (ExportBlendShapes)
            {
                //Resource.Lines.Add($"blend_shape_mode = 0");
                StringBuilder Shapes = new StringBuilder();
                Shapes.Append($"blend_shape/names = StringArray(");
                for (int a = 0; a < Model.BlendShapeCount; a++)
                {
                    Shapes.Append($"\"shape{a}\", ");
                }
                Shapes.Remove(Shapes.Length - 2, 2);
                Shapes.Append($")");
                Resource.Lines.Add(Shapes.ToString());
                //Resource.Lines.Add($"blend_shape/mode = 0");
            }

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                BlendSkinX.SubModel Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                StringBuilder Bones = new StringBuilder($"IntArray(");
                StringBuilder Weights = new StringBuilder($"PoolRealArray(");
                int vertexcount = 0;
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        Indices.Append(string.Format("{0}, {1}, {2}, ", vertexcount + ((a & 0x1) == 0x1 ? a + 1 : a + 0), vertexcount + ((a & 0x1) == 0x1 ? a + 0 : a + 1), vertexcount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    vertexcount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                for (int x = 0; x < Sub.VData.Count; x++)
                {
                    BlendSkinX.VertexData VData = Sub.VData[x];
                    Vertices.Append($"{(-VData.X).ToText()}, {VData.Y.ToText()}, {VData.Z.ToText()}, ");
                    UVs.Append($"{VData.UV_X.ToText()}, {(-VData.UV_Y).ToText()}, ");
                    int R = VData.R;
                    int G = VData.G;
                    int B = VData.B;
                    int A = VData.A;
                    Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                    Weights.Append($"{VData.Weight1.ToText()}, {VData.Weight2.ToText()}, {VData.Weight3.ToText()}, 0.0, ");

                    ushort Bone1 = 0;
                    ushort Bone2 = 0;
                    ushort Bone3 = 0;
                    int Joint1 = (VData.Joint1 - 16) / 4;
                    int Joint2 = (VData.Joint2 - 16) / 4;
                    int Joint3 = (VData.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = (ushort)Sub.GroupJoints[GroupID][Joint1];
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = (ushort)Sub.GroupJoints[GroupID][Joint2];
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = (ushort)Sub.GroupJoints[GroupID][Joint3];
                    }
                    Bones.Append($"{Bone1}, {Bone2}, {Bone3}, 0, ");
                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }
                }
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                Indices.Remove(Indices.Length - 2, 2);
                Indices.Append($")");
                Bones.Remove(Bones.Length - 2, 2);
                Bones.Append($"),");
                Weights.Remove(Weights.Length - 2, 2);
                Weights.Append($"),");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                Resource.Lines.Add($"null,"); // normals (todo)
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add(Bones.ToString()); // bones
                Resource.Lines.Add(Weights.ToString()); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                if (ExportBlendShapes)
                {
                    Resource.Lines.Add($"\"morph_arrays\":[");
                    for (int a = 0; a < Model.BlendShapeCount; a++)
                    {
                        StringBuilder Shape = new StringBuilder();
                        Shape.Append($"Vector3Array(");
                        for (int x = 0; x < Sub.VData.Count; x++)
                        {
                            BlendSkinX.VertexData VData = Sub.VData[x];
                            Shape.Append($"{(-VData.BlendShapes[a].X).ToText()}, {VData.BlendShapes[a].Y.ToText()}, {VData.BlendShapes[a].Z.ToText()}, ");
                        }
                        Shape.Remove(Shape.Length - 2, 2);
                        Shape.Append($"),");

                        // unfortunately the rest of the model data has to be duplicated here
                        Resource.Lines.Add($"[");
                        Resource.Lines.Add(Shape.ToString());
                        Resource.Lines.Add($"null,");
                        Resource.Lines.Add($"null,");
                        Resource.Lines.Add(Colors.ToString());
                        Resource.Lines.Add(UVs.ToString());
                        Resource.Lines.Add($"null,");
                        Resource.Lines.Add($"null,");
                        Resource.Lines.Add($"null,");
                        if (a != Model.BlendShapeCount - 1)
                        {
                            Resource.Lines.Add($"],");
                        }
                    }
                    Resource.Lines.Add($"]");
                    Resource.Lines.Add($"]");
                }
                else
                {
                    Resource.Lines.Add($"\"morph_arrays\":[]");
                }
                Resource.Lines.Add("}");
            }
        }

        public void AddAudioStreamIMA(XWB.Sound sound)
        {
            // didn't work, comes out garbled, probably because of Godot's IMA decoder being different
            Resource.Type = "AudioStreamWAV";

            //byte[] SoundData = IMA_ADPCM.IMA_Decoder.XboxToIma(sound.SoundData, sound.Channels);

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PoolByteArray( ");
            for (int i = 0; i < sound.SoundData.Length; i++)
            {
                Data.Append($"{sound.SoundData[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 2");
            Resource.Lines.Add($"mix_rate = {sound.SampleRate}");
            if (sound.Channels == 2)
            {
                Resource.Lines.Add("stereo = true");
            }

        }

        public void AddAudioStreamFromFile(string path, bool stereo, uint sampleRate)
        {
            Resource.Type = "AudioStreamWAV";

            byte[] filebytes = File.ReadAllBytes(path);

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PoolByteArray( ");
            for (int i = 0x2c; i < filebytes.Length; i++)
            {
                Data.Append($"{filebytes[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 1");
            Resource.Lines.Add($"mix_rate = {sampleRate}");
            if (stereo)
            {
                Resource.Lines.Add("stereo = true");
            }

        }

        public void AddAudioStream(SoundEffectX sound)
        {
            Resource.Type = "AudioStreamWAV";

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PackedByteArray(");
            for (int i = 0; i < sound.SoundData.Length; i++)
            {
                Data.Append($"{sound.SoundData[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 1");
            Resource.Lines.Add($"mix_rate = {sound.Freq}");

        }

        public void AddTextureX(TextureX tex)
        {
            Resource.Type = "Image";

            StringBuilder Data = new StringBuilder();
            Resource.Lines.Add("data = {");
            Data.Append("\"data\": PackedByteArray( ");
            for (int i = 0; i < tex.RawData.Length; i++)
            {
                Data.Append($"{tex.RawData[i].R}, {tex.RawData[i].G}, {tex.RawData[i].B}, {tex.RawData[i].A}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append("),");
            Resource.Lines.Add(Data.ToString());
            Resource.Lines.Add($"\"format\": \"RGBA8\",");
            Resource.Lines.Add($"\"height\": {tex.Height},");
            Resource.Lines.Add($"\"mipmaps\": false,");
            Resource.Lines.Add($"\"width\": {tex.Width}");
            Resource.Lines.Add("}");
        }

        public void AddNewModelX(ModelX Model)
        {
            // New ArrayMesh format in Godot 4
            Resource.Type = "ArrayMesh";

            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                bool hasIndices = false;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                foreach (var vert in Sub.VData)
                {
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    //byte[] NX = BitConverter.GetBytes(vert.NX);
                    //byte[] NY = BitConverter.GetBytes(vert.NY);
                    //byte[] NZ = BitConverter.GetBytes(vert.NZ);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                    Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                    Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                    //Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                    AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                    AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                    AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                Resource.Lines.Add($"\"format\": {0x80000101B},"); // 4121 no normals / 4123 normals
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.SubModels.Count - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
        }

        public void AddNewSkinX(SkinX Model)
        {
            // New ArrayMesh format in Godot 4
            Resource.Type = "ArrayMesh";

            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                StringBuilder SkinData = new StringBuilder();
                bool hasIndices = false;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                foreach (var vert in Sub.VData)
                {
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    byte[] Bone1 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone2 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone3 = new byte[2] { 0x00, 0x00 };
                    int Joint1 = (vert.Joint1 - 16) / 4;
                    int Joint2 = (vert.Joint2 - 16) / 4;
                    int Joint3 = (vert.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint1]);
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint2]);
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint3]);
                    }
                    ushort ConvWeight1 = (ushort)(vert.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                    Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                    Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                    //Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                    AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                    AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                    AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                    SkinData.Append($"{Bone1[0]}, {Bone1[1]}, {Bone2[0]}, {Bone2[1]}, {Bone3[0]}, {Bone3[1]}, 0, 0, ");
                    SkinData.Append($"{Weight1[0]}, {Weight1[1]}, {Weight2[0]}, {Weight2[1]}, {Weight3[0]}, {Weight3[1]}, 0, 0, ");
                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);
                SkinData.Remove(SkinData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                Resource.Lines.Add($"\"format\": {0x800001C1B},"); // 7193 or 0x1C19 no normals, 7195 or 0x1C1B normals
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"skin_data\": PackedByteArray({SkinData.ToString()}),");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.SubModels.Count - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
        }

        public void AddNewBlendSkinX(BlendSkinX Model)
        {
            // New ArrayMesh format in Godot 4
            bool ExportBlendShapes = true;
            Resource.Type = "ArrayMesh";

            if (ExportBlendShapes)
            {
                StringBuilder Shapes = new StringBuilder();
                Shapes.Append($"_blend_shape_names = PackedStringArray(");
                for (int a = 0; a < Model.BlendShapeCount; a++)
                {
                    Shapes.Append($"\"morph_{a}\", ");
                }
                Shapes.Remove(Shapes.Length - 2, 2);
                Shapes.Append($")");
                Resource.Lines.Add(Shapes.ToString());
            }
            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                StringBuilder SkinData = new StringBuilder();
                StringBuilder BlendShapeData = new StringBuilder();
                bool hasIndices = false;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                foreach (var vert in Sub.VData)
                {
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    byte[] Bone1 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone2 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone3 = new byte[2] { 0x00, 0x00 };
                    int Joint1 = (vert.Joint1 - 16) / 4;
                    int Joint2 = (vert.Joint2 - 16) / 4;
                    int Joint3 = (vert.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint1]);
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint2]);
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint3]);
                    }
                    ushort ConvWeight1 = (ushort)(vert.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                    Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                    Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                    //Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                    AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                    AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                    AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                    SkinData.Append($"{Bone1[0]}, {Bone1[1]}, {Bone2[0]}, {Bone2[1]}, {Bone3[0]}, {Bone3[1]}, 0, 0, ");
                    SkinData.Append($"{Weight1[0]}, {Weight1[1]}, {Weight2[0]}, {Weight2[1]}, {Weight3[0]}, {Weight3[1]}, 0, 0, ");
                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);
                SkinData.Remove(SkinData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                if (ExportBlendShapes)
                {
                    for (int a = 0; a < Model.BlendShapeCount; a++)
                    {
                        for (int x = 0; x < Sub.VData.Count; x++)
                        {
                            float BS_X = -Sub.VData[x].BlendShapes[a].X;
                            float BS_Y = Sub.VData[x].BlendShapes[a].Y;
                            float BS_Z = Sub.VData[x].BlendShapes[a].Z;
                            byte[] BSX = BitConverter.GetBytes(BS_X);
                            byte[] BSY = BitConverter.GetBytes(BS_Y);
                            byte[] BSZ = BitConverter.GetBytes(BS_Z);
                            //byte NX = (byte)(Sub.VData[x].NX * 127);
                            //byte NY = (byte)(Sub.VData[x].NY * 127);
                            //byte NZ = (byte)(Sub.VData[x].NZ * 127);
                            BlendShapeData.Append($"{BSX[0]}, {BSX[1]}, {BSX[2]}, {BSX[3]}, ");
                            BlendShapeData.Append($"{BSY[0]}, {BSY[1]}, {BSY[2]}, {BSY[3]}, ");
                            BlendShapeData.Append($"{BSZ[0]}, {BSZ[1]}, {BSZ[2]}, {BSZ[3]}, ");
                            //BlendShapeData.Append($"255, {NX}, {NY}, {NZ}, ");
                        }
                        for (int x = 0; x < Sub.VData.Count; x++)
                        {
                            byte NX = (byte)(Sub.VData[x].NX * 127);
                            byte NY = (byte)(Sub.VData[x].NY * 127);
                            byte NZ = (byte)(Sub.VData[x].NZ * 127);
                            BlendShapeData.Append($"255, {NX}, {NY}, {NZ}, ");
                        }
                    }
                    BlendShapeData.Remove(BlendShapeData.Length - 2, 2);
                    Resource.Lines.Add($"\"blend_shapes\": PackedByteArray({BlendShapeData.ToString()}),");
                }
                Resource.Lines.Add($"\"format\": {0x800001C1B},"); // 7193 no normals, 7195 normals
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"skin_data\": PackedByteArray({SkinData.ToString()}),");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.SubModels.Count - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
            if (ExportBlendShapes)
            {
                Resource.Lines.Add($"blend_shape_mode = 0");
            }
        }


        #endregion

        #region PS2

        public void AddModel(Model Model)
        {
            Resource.Type = "ArrayMesh";

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                Model.SubModel Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Normals = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                bool hasIndices = false;
                int refIndex = 0;
                List<int> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add(refIndex);
                                idx.Add(refIndex + 1);
                                idx.Add(refIndex + 2);
                            }
                            else
                            {
                                idx.Add(refIndex + 1);
                                idx.Add(refIndex);
                                idx.Add(refIndex + 2);
                            }
                        }
                        ++refIndex;
                    }
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    Indices.Append(string.Format("{0}, ", idx[a]));
                    hasIndices = true;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    Vertices.Append($"{(-vert.X).ToText()}, {vert.Y.ToText()}, {vert.Z.ToText()}, ");
                    Normals.Append($"{(-vert.NX).ToText()}, {vert.NY.ToText()}, {vert.NZ.ToText()}, ");
                    UVs.Append($"{vert.U.ToText()}, {vert.V.ToText()}, ");
                    int R = Math.Min(vert.R + vert.ER, 255);
                    int G = Math.Min(vert.G + vert.EG, 255);
                    int B = Math.Min(vert.B + vert.EB, 255);
                    int A = Math.Min(vert.A + vert.EA, 255);
                    Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                Indices.Append($")");
                Normals.Remove(Normals.Length - 2, 2);
                Normals.Append($"),");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                Resource.Lines.Add(Normals.ToString()); // normals
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add($"null,"); // bones
                Resource.Lines.Add($"null,"); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                Resource.Lines.Add($"\"morph_arrays\":[]");
                Resource.Lines.Add("}");
            }
        }

        public void AddSkin(Skin Model)
        {
            Resource.Type = "ArrayMesh";

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                Skin.SubModel Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Normals = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                StringBuilder Bones = new StringBuilder($"IntArray(");
                StringBuilder Weights = new StringBuilder($"PoolRealArray(");
                bool hasIndices = false;
                int refIndex = 0;
                List<int> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add(refIndex);
                                idx.Add(refIndex + 1);
                                idx.Add(refIndex + 2);
                            }
                            else
                            {
                                idx.Add(refIndex + 1);
                                idx.Add(refIndex);
                                idx.Add(refIndex + 2);
                            }
                        }
                        ++refIndex;
                    }
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    Indices.Append(string.Format("{0}, ", idx[a]));
                    hasIndices = true;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    Vertices.Append($"{(-vert.X).ToText()}, {vert.Y.ToText()}, {vert.Z.ToText()}, ");
                    //Normals.Append($"{(-vert.NX).ToText()}, {vert.NY.ToText()}, {vert.NZ.ToText()}, ");
                    UVs.Append($"{vert.U.ToText()}, {(-vert.V).ToText()}, ");
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                    Weights.Append($"{vert.Joint.Weight1.ToText()}, {vert.Joint.Weight2.ToText()}, {vert.Joint.Weight3.ToText()}, 0.0, ");
                    Bones.Append($"{vert.Joint.JointIndex1}, {vert.Joint.JointIndex2}, {vert.Joint.JointIndex3}, 0, ");
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                Indices.Append($")");
                //Normals.Remove(Normals.Length - 2, 2);
                //Normals.Append($"),");
                Bones.Remove(Bones.Length - 2, 2);
                Bones.Append($"),");
                Weights.Remove(Weights.Length - 2, 2);
                Weights.Append($"),");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                //Resource.Lines.Add(Normals.ToString()); // normals
                Resource.Lines.Add($"null,"); // todo normals
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add(Bones.ToString()); // bones
                Resource.Lines.Add(Weights.ToString()); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                Resource.Lines.Add($"\"morph_arrays\":[]");
                Resource.Lines.Add("}");
            }
        }

        public void AddBlendSkin(BlendSkin Model)
        {
            bool ExportBlendShapes = false; // Blend shapes are super buggy in Godot right now... (export code and data is 100% fine)
            Resource.Type = "ArrayMesh";

            if (ExportBlendShapes)
            {
                //Resource.Lines.Add($"blend_shape_mode = 0");
                StringBuilder Shapes = new StringBuilder();
                Shapes.Append($"blend_shape/names = StringArray(");
                for (int a = 0; a < Model.Models[0].SubModels[0].BlendShapes.Length; a++)
                {
                    Shapes.Append($"\"shape{a}\", ");
                }
                Shapes.Remove(Shapes.Length - 2, 2);
                Shapes.Append($")");
                Resource.Lines.Add(Shapes.ToString());
                //Resource.Lines.Add($"blend_shape/mode = 0");
            }

            for (int i = 0; i < Model.Models.Length; i++)
            {
                StringBuilder Vertices = new StringBuilder($"Vector3Array(");
                StringBuilder Normals = new StringBuilder($"Vector3Array(");
                StringBuilder Colors = new StringBuilder($"ColorArray(");
                StringBuilder UVs = new StringBuilder($"Vector2Array(");
                StringBuilder Indices = new StringBuilder($"IntArray(");
                StringBuilder Bones = new StringBuilder($"IntArray(");
                StringBuilder Weights = new StringBuilder($"PoolRealArray(");
                bool hasIndices = false;
                int refIndex = 0;
                List<int> idx = new();
                foreach (var Sub in Model.Models[i].SubModels)
                {
                    for (var j = 0; j < Sub.Vertexes.Count; ++j)
                    {
                        if (j < Sub.Vertexes.Count - 2)
                        {
                            if (Sub.Vertexes[j + 2].Conn)
                            {
                                if ((/*offset +*/ j) % 2 == 0)
                                {
                                    idx.Add(refIndex);
                                    idx.Add(refIndex + 1);
                                    idx.Add(refIndex + 2);
                                }
                                else
                                {
                                    idx.Add(refIndex + 1);
                                    idx.Add(refIndex);
                                    idx.Add(refIndex + 2);
                                }
                            }
                            ++refIndex;
                        }
                    }
                    refIndex += 2;
                    foreach (var vert in Sub.Vertexes)
                    {
                        Vertices.Append($"{(-vert.X).ToText()}, {vert.Y.ToText()}, {vert.Z.ToText()}, ");
                        //Normals.Append($"{(-vert.NX).ToText()}, {vert.NY.ToText()}, {vert.NZ.ToText()}, ");
                        UVs.Append($"{vert.U.ToText()}, {(-vert.V).ToText()}, ");
                        int R = vert.R;
                        int G = vert.G;
                        int B = vert.B;
                        int A = vert.A;
                        Colors.Append($"{(R / 255f).ToText()}, {(G / 255f).ToText()}, {(B / 255f).ToText()}, {(A / 255f).ToText()}, ");
                        Weights.Append($"{vert.Joint.Weight1.ToText()}, {vert.Joint.Weight2.ToText()}, {vert.Joint.Weight3.ToText()}, 0.0, ");
                        Bones.Append($"{vert.Joint.JointIndex1}, {vert.Joint.JointIndex2}, {vert.Joint.JointIndex3}, 0, ");
                    }
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    Indices.Append(string.Format("{0}, ", idx[a]));
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                Vertices.Append($"),");
                Colors.Remove(Colors.Length - 2, 2);
                Colors.Append($"),");
                UVs.Remove(UVs.Length - 2, 2);
                UVs.Append($"),");
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                Indices.Append($")");
                //Normals.Remove(Normals.Length - 2, 2);
                //Normals.Append($"),");
                Bones.Remove(Bones.Length - 2, 2);
                Bones.Append($"),");
                Weights.Remove(Weights.Length - 2, 2);
                Weights.Append($"),");

                Resource.Lines.Add("surfaces/" + i + " = {");
                Resource.Lines.Add($"\"primitive\":4,");
                Resource.Lines.Add($"\"arrays\":[");
                Resource.Lines.Add(Vertices.ToString()); // vertices 
                //Resource.Lines.Add(Normals.ToString()); // normals
                Resource.Lines.Add($"null,"); // todo normals
                Resource.Lines.Add($"null,"); // tangents
                Resource.Lines.Add(Colors.ToString()); // colors
                Resource.Lines.Add(UVs.ToString()); // uv 1
                Resource.Lines.Add($"null,"); // uv 2
                Resource.Lines.Add(Bones.ToString()); // bones
                Resource.Lines.Add(Weights.ToString()); // weights
                Resource.Lines.Add(Indices.ToString()); // indices
                Resource.Lines.Add($"],");
                if (ExportBlendShapes)
                {
                    Resource.Lines.Add($"\"morph_arrays\":[");
                    for (int a = 0; a < Model.BlendShapeCount; a++)
                    {
                        StringBuilder Shape = new StringBuilder();
                        Shape.Append($"Vector3Array(");
                        for (int s1 = 0; s1 < Model.Models[i].SubModels.Length; s1++)
                        {
                            var Sub = Model.Models[i].SubModels[s1];
                            for (int x = 0; x < Sub.Vertexes.Count; x++)
                            {
                                BlendSkin.BlendShapeVertex bs = Sub.BlendShapes[a].ShapeVertecies[x];
                                Shape.Append($"{(Sub.Vertexes[x].X - bs.Offset.X).ToText()}, {Sub.Vertexes[x].Y + bs.Offset.Y.ToText()}, {Sub.Vertexes[x].Z + bs.Offset.Z.ToText()}, ");
                            }
                        }
                        Shape.Remove(Shape.Length - 2, 2);
                        Shape.Append($"),");

                        // unfortunately the rest of the model data has to be duplicated here
                        Resource.Lines.Add($"[");
                        Resource.Lines.Add(Shape.ToString());
                        //Resource.Lines.Add(Normals.ToString()); // normals
                        Resource.Lines.Add($"null,"); // todo normals
                        Resource.Lines.Add($"null,"); // tangents
                        Resource.Lines.Add(Colors.ToString()); // colors
                        Resource.Lines.Add(UVs.ToString()); // uv 1
                        Resource.Lines.Add($"null,"); // uv 2
                        Resource.Lines.Add(Bones.ToString()); // bones
                        Resource.Lines.Add(Weights.ToString()); // weights
                        Resource.Lines.Add(Indices.ToString()); // indices
                        if (a != Model.BlendShapeCount - 1)
                        {
                            Resource.Lines.Add($"],");
                        }
                    }
                    Resource.Lines.Add($"]");
                    Resource.Lines.Add($"]");
                }
                else
                {
                    Resource.Lines.Add($"\"morph_arrays\":[]");
                }
                Resource.Lines.Add("}");
            }
        }

        public void AddTexture(Texture tex)
        {
            Resource.Type = "Image";

            StringBuilder Data = new StringBuilder();
            Resource.Lines.Add("data = {");
            Data.Append("\"data\": PackedByteArray( ");
            for (int i = 0; i < tex.RawData.Length; i++)
            {
                Data.Append($"{tex.RawData[i].R}, {tex.RawData[i].G}, {tex.RawData[i].B}, {tex.RawData[i].A}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append("),");
            Resource.Lines.Add(Data.ToString());
            Resource.Lines.Add($"\"format\": \"RGBA8\",");
            Resource.Lines.Add($"\"height\": {tex.Height},");
            Resource.Lines.Add($"\"mipmaps\": false,");
            Resource.Lines.Add($"\"width\": {tex.Width}");
            Resource.Lines.Add("}");
        }

        public void AddRawTexture(List<Color> tex, int Width, int Height)
        {
            Resource.Type = "Image";

            StringBuilder Data = new StringBuilder();
            Resource.Lines.Add("data = {");
            Data.Append("\"data\": PackedByteArray( ");

            /*
            int c = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    BMP.SetPixel(x, y, Textures[i][c]);
                    c++;
                }
            }
            */

            for (int i = 0; i < tex.Count; i++)
            {
                Data.Append($"{tex[i].R}, {tex[i].G}, {tex[i].B}, {tex[i].A}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append("),");
            Resource.Lines.Add(Data.ToString());
            Resource.Lines.Add($"\"format\": \"RGBA8\",");
            Resource.Lines.Add($"\"height\": {Height},");
            Resource.Lines.Add($"\"mipmaps\": false,");
            Resource.Lines.Add($"\"width\": {Width}");
            Resource.Lines.Add("}");
        }

        public void AddCombinedTexture(List<List<Color>> Textures, List<int> Widths, List<int> Heights)
        {
            Resource.Type = "Image";

            StringBuilder Data = new StringBuilder();
            Resource.Lines.Add("data = {");
            Data.Append("\"data\": PackedByteArray( ");

            int TexCount = Textures.Count;
            int ogWidth = Widths[0];
            int ogHeight = Heights[0];
            int maxWidth = ogWidth;
            int maxHeight = ogHeight;

            if (TexCount > 1)
            {
                maxWidth += ogWidth;
                if (TexCount > 2)
                {
                    maxWidth += ogWidth;
                }
                if (TexCount > 3)
                {
                    maxWidth += ogWidth;
                }
            }
            int rows = (TexCount / 4);
            if (rows == 0)
                rows = 1;

            maxHeight = maxHeight * rows;

            int ptc = 0;
            for (int y = 0; y < maxHeight; y++)
            {
                int maxptc = 4;
                if (TexCount > 4 && y >= maxHeight / 2)
                {
                    ptc = 4;
                    maxptc = TexCount;
                }
                else
                {
                    ptc = 0;
                }
                while (ptc < TexCount && ptc < maxptc)
                {
                    int c = y * ogWidth;
                    if (ptc >= 4)
                    {
                        c = (y - (maxHeight / 2)) * ogWidth;
                    }
                    for (int x = 0; x < ogWidth; x++)
                    {
                        if (c < Textures[ptc].Count)
                        {
                            Data.Append($"{Textures[ptc][c].R}, {Textures[ptc][c].G}, {Textures[ptc][c].B}, {Textures[ptc][c].A}, ");
                        }
                        else
                        {
                            Data.Append($"0, 0, 0, 255, ");
                        }
                        c++;
                    }
                    ptc++;
                }
            }

            Data.Remove(Data.Length - 2, 1);
            Data.Append("),");
            Resource.Lines.Add(Data.ToString());
            Resource.Lines.Add($"\"format\": \"RGBA8\",");
            Resource.Lines.Add($"\"height\": {maxHeight},");
            Resource.Lines.Add($"\"mipmaps\": false,");
            Resource.Lines.Add($"\"width\": {maxWidth}");
            Resource.Lines.Add("}");
        }

        public void AddAudioStream(SoundEffect sound)
        {
            Resource.Type = "AudioStreamWAV";

            byte[] RawData = new byte[sound.SoundSize];
            Array.Copy(sound.Parent.ExtraData, sound.SoundOffset, RawData, 0, sound.SoundSize);
            byte[] SoundData = RIFF.SaveRiff(ADPCM.ToPCMMono(RawData, (int)sound.SoundSize), 1, sound.Freq);

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PackedByteArray(");
            for (int i = 0; i < SoundData.Length; i++)
            {
                Data.Append($"{SoundData[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 1");
            Resource.Lines.Add($"mix_rate = {sound.Freq}");

        }

        public void AddRawAudioStream(byte[] RawData, int Freq, short Channels, bool loop)
        {
            Resource.Type = "AudioStreamWAV";

            byte[] SoundData = RIFF.SaveRiff(RawData, Channels, Freq);

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PackedByteArray(");
            for (int i = 0; i < SoundData.Length; i++)
            {
                Data.Append($"{SoundData[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 1");
            if (Channels == 2)
            {
                Resource.Lines.Add("stereo = true");
            }
            if (loop)
            {
                Resource.Lines.Add($"loop_mode = 1");
                if (Channels == 2)
                {
                    Resource.Lines.Add($"loop_begin = 16"); // there's a pop noise at the start without this
                    Resource.Lines.Add($"loop_end = {(int)(RawData.Length / 4)}");
                }
                else
                {
                    Resource.Lines.Add($"loop_begin = 32"); // there's a pop noise at the start without this
                    Resource.Lines.Add($"loop_end = {(int)(RawData.Length / 2)}");
                }
            }
            Resource.Lines.Add($"mix_rate = {Freq}");

        }

        public void AddRawAudioStreamPS2(byte[] RawData, int Freq, short Channels, int inter, bool loop)
        {
            Resource.Type = "AudioStreamWAV";

            byte[] SoundData;
            if (Channels == 2)
            {
                SoundData = RIFF.SaveRiff(ADPCM.ToPCMStereo(RawData, RawData.Length, inter), 2, Freq);
            }
            else
            {
                SoundData = RIFF.SaveRiff(ADPCM.ToPCMMono(RawData, RawData.Length), 1, Freq);
            }

            StringBuilder Data = new StringBuilder();
            Data.Append("data = PackedByteArray(");
            for (int i = 0; i < SoundData.Length; i++)
            {
                Data.Append($"{SoundData[i]}, ");
            }
            Data.Remove(Data.Length - 2, 1);
            Data.Append(")");
            Resource.Lines.Add(Data.ToString());

            Resource.Lines.Add("format = 1");
            if (Channels == 2)
            {
                Resource.Lines.Add("stereo = true");
            }
            if (loop)
            {
                Resource.Lines.Add($"loop_mode = 1");
                if (Channels == 2)
                {
                    Resource.Lines.Add($"loop_begin = 16"); // there's a pop noise at the start without this
                    Resource.Lines.Add($"loop_end = {(int)(RawData.Length / 4)}");
                }
                else
                {
                    Resource.Lines.Add($"loop_begin = 32"); // there's a pop noise at the start without this
                    Resource.Lines.Add($"loop_end = {(int)(RawData.Length / 2)}");
                }
            }
            Resource.Lines.Add($"mix_rate = {Freq}");

        }

        public void AddNewModel(Model Model)
        {
            // New ArrayMesh format in Godot 4
            Resource.Type = "ArrayMesh";

            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                bool hasIndices = false;
                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)(refIndex + 2));
                            }
                            else
                            {
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 2));
                            }
                        }
                        ++refIndex;
                    }
                    VertCount++;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.U);
                    byte[] UV_Y = BitConverter.GetBytes(vert.V);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    //byte[] NX = BitConverter.GetBytes(-vert.NX);
                    //byte[] NY = BitConverter.GetBytes(vert.NY);
                    //byte[] NZ = BitConverter.GetBytes(vert.NZ);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                    Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                    Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                    //Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                    AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                    AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                    AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                }
                foreach (var vert in Sub.Vertexes)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    Vertices.Append($"255, {NX}, {NY}, {NZ}, ");
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                Resource.Lines.Add($"\"format\": {0x80000101B},"); // 4121 or 0x1019 no normals / 4123 or 0x101B normals
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.SubModels.Count - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
        }

        public void AddNewSkin(Skin Model)
        {
            // New ArrayMesh format in Godot 4
            Resource.Type = "ArrayMesh";

            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var Sub = Model.SubModels[i];
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                StringBuilder SkinData = new StringBuilder();
                bool hasIndices = false;
                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)(refIndex + 2));
                            }
                            else
                            {
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 2));
                            }
                        }
                        ++refIndex;
                    }
                    VertCount++;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    int R = vert.R;
                    int G = vert.G;
                    int B = vert.B;
                    int A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.U);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.V);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    byte[] Bone1 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex1);
                    byte[] Bone2 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex2);
                    byte[] Bone3 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex3);
                    ushort ConvWeight1 = (ushort)(vert.Joint.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Joint.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Joint.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                    Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                    Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                    AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                    AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                    AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                    SkinData.Append($"{Bone1[0]}, {Bone1[1]}, {Bone2[0]}, {Bone2[1]}, {Bone3[0]}, {Bone3[1]}, 0, 0, ");
                    SkinData.Append($"{Weight1[0]}, {Weight1[1]}, {Weight2[0]}, {Weight2[1]}, {Weight3[0]}, {Weight3[1]}, 0, 0, ");
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);
                SkinData.Remove(SkinData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                Resource.Lines.Add($"\"format\": {0x800001C19},");
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"skin_data\": PackedByteArray({SkinData.ToString()}),");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.SubModels.Count - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
        }

        public void AddNewBlendSkin(BlendSkin Model)
        {
            // New ArrayMesh format in Godot 4
            bool ExportBlendShapes = true;
            Resource.Type = "ArrayMesh";

            if (ExportBlendShapes)
            {
                StringBuilder Shapes = new StringBuilder();
                Shapes.Append($"_blend_shape_names = PackedStringArray(");
                for (int a = 0; a < Model.BlendShapeCount; a++)
                {
                    Shapes.Append($"\"morph_{a}\", ");
                }
                Shapes.Remove(Shapes.Length - 2, 2);
                Shapes.Append($")");
                Resource.Lines.Add(Shapes.ToString());
            }
            Resource.Lines.Add("_surfaces = [{");

            for (int i = 0; i < Model.Models.Length; i++)
            {
                StringBuilder Vertices = new StringBuilder();
                StringBuilder Indices = new StringBuilder();
                StringBuilder AttributeData = new StringBuilder();
                StringBuilder SkinData = new StringBuilder();
                StringBuilder BlendShapeData = new StringBuilder();
                bool hasIndices = false;
                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                foreach (var Sub in Model.Models[i].SubModels)
                {
                    for (var j = 0; j < Sub.Vertexes.Count; ++j)
                    {
                        if (j < Sub.Vertexes.Count - 2)
                        {
                            if (Sub.Vertexes[j + 2].Conn)
                            {
                                if ((/*offset +*/ j) % 2 == 0)
                                {
                                    idx.Add((ushort)refIndex);
                                    idx.Add((ushort)(refIndex + 1));
                                    idx.Add((ushort)(refIndex + 2));
                                }
                                else
                                {
                                    idx.Add((ushort)(refIndex + 1));
                                    idx.Add((ushort)refIndex);
                                    idx.Add((ushort)(refIndex + 2));
                                }
                            }
                            ++refIndex;
                        }
                        VertCount++;
                    }
                    refIndex += 2;
                    foreach (var vert in Sub.Vertexes)
                    {
                        int R = vert.R;
                        int G = vert.G;
                        int B = vert.B;
                        int A = vert.A;
                        byte[] UV_X = BitConverter.GetBytes(vert.U);
                        byte[] UV_Y = BitConverter.GetBytes(-vert.V);
                        byte[] X = BitConverter.GetBytes(-vert.X);
                        byte[] Y = BitConverter.GetBytes(vert.Y);
                        byte[] Z = BitConverter.GetBytes(vert.Z);
                        byte[] Bone1 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex1);
                        byte[] Bone2 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex2);
                        byte[] Bone3 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex3);
                        ushort ConvWeight1 = (ushort)(vert.Joint.Weight1 * 65535);
                        ushort ConvWeight2 = (ushort)(vert.Joint.Weight2 * 65535);
                        ushort ConvWeight3 = (ushort)(vert.Joint.Weight3 * 65535);
                        byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                        byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                        byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                        if (-vert.X < MinX)
                            MinX = -vert.X;
                        if (-vert.X > MaxX)
                            MaxX = -vert.X;
                        if (vert.Y < MinY)
                            MinY = vert.Y;
                        if (vert.Y > MaxY)
                            MaxY = vert.Y;
                        if (vert.Z < MinZ)
                            MinZ = vert.Z;
                        if (vert.Z > MaxZ)
                            MaxZ = vert.Z;
                        Vertices.Append($"{X[0]}, {X[1]}, {X[2]}, {X[3]}, ");
                        Vertices.Append($"{Y[0]}, {Y[1]}, {Y[2]}, {Y[3]}, ");
                        Vertices.Append($"{Z[0]}, {Z[1]}, {Z[2]}, {Z[3]}, ");
                        AttributeData.Append($"{R}, {G}, {B}, {A}, ");
                        AttributeData.Append($"{UV_X[0]}, {UV_X[1]}, {UV_X[2]}, {UV_X[3]}, ");
                        AttributeData.Append($"{UV_Y[0]}, {UV_Y[1]}, {UV_Y[2]}, {UV_Y[3]}, ");
                        SkinData.Append($"{Bone1[0]}, {Bone1[1]}, {Bone2[0]}, {Bone2[1]}, {Bone3[0]}, {Bone3[1]}, 0, 0, ");
                        SkinData.Append($"{Weight1[0]}, {Weight1[1]}, {Weight2[0]}, {Weight2[1]}, {Weight3[0]}, {Weight3[1]}, 0, 0, ");
                    }
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    Indices.Append($"{id[0]}, {id[1]}, ");
                    hasIndices = true;
                }
                
                Vertices.Remove(Vertices.Length - 2, 2);
                if (hasIndices)
                {
                    Indices.Remove(Indices.Length - 2, 2);
                }
                AttributeData.Remove(AttributeData.Length - 2, 2);
                SkinData.Remove(SkinData.Length - 2, 2);

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                string AABB_X = (MinX).ToText();
                string AABB_Y = (MinY).ToText();
                string AABB_Z = (MinZ).ToText();
                string AABB_W = Math.Abs((MaxX - MinX)).ToText();
                string AABB_H = Math.Abs((MaxY - MinY)).ToText();
                string AABB_D = Math.Abs((MaxZ - MinZ)).ToText();
                Resource.Lines.Add($"\"aabb\": AABB({AABB_X}, {AABB_Y}, {AABB_Z}, {AABB_W}, {AABB_H}, {AABB_D}),");
                Resource.Lines.Add($"\"attribute_data\": PackedByteArray({AttributeData.ToString()}),");
                if (ExportBlendShapes)
                {
                    for (int a = 0; a < Model.BlendShapeCount; a++)
                    {
                        for (int s1 = 0; s1 < Model.Models[i].SubModels.Length; s1++)
                        {
                            var Sub = Model.Models[i].SubModels[s1];
                            for (int x = 0; x < Sub.Vertexes.Count; x++)
                            {
                                BlendSkin.BlendShapeVertex bs = Sub.BlendShapes[a].ShapeVertecies[x];
                                float BS_X = -Sub.Vertexes[x].X - bs.Offset.X;
                                float BS_Y = Sub.Vertexes[x].Y + bs.Offset.Y;
                                float BS_Z = Sub.Vertexes[x].Z + bs.Offset.Z;
                                byte[] BSX = BitConverter.GetBytes(BS_X);
                                byte[] BSY = BitConverter.GetBytes(BS_Y);
                                byte[] BSZ = BitConverter.GetBytes(BS_Z);
                                BlendShapeData.Append($"{BSX[0]}, {BSX[1]}, {BSX[2]}, {BSX[3]}, ");
                                BlendShapeData.Append($"{BSY[0]}, {BSY[1]}, {BSY[2]}, {BSY[3]}, ");
                                BlendShapeData.Append($"{BSZ[0]}, {BSZ[1]}, {BSZ[2]}, {BSZ[3]}, ");
                            }
                        }
                    }
                    BlendShapeData.Remove(BlendShapeData.Length - 2, 2);
                    Resource.Lines.Add($"\"blend_shapes\": PackedByteArray({BlendShapeData.ToString()}),");
                }
                Resource.Lines.Add($"\"format\": {0x800001C19},");
                Resource.Lines.Add($"\"index_count\": {idx.Count},");
                Resource.Lines.Add($"\"index_data\": PackedByteArray({Indices.ToString()}),");
                Resource.Lines.Add($"\"primitive\": 3,");
                Resource.Lines.Add($"\"skin_data\": PackedByteArray({SkinData.ToString()}),");
                Resource.Lines.Add($"\"vertex_count\": {VertCount},");
                Resource.Lines.Add($"\"vertex_data\": PackedByteArray({Vertices.ToString()})");

                if (i != Model.Models.Length - 1)
                {
                    Resource.Lines.Add("}, {");
                }
            }

            Resource.Lines.Add("}]");
            if (ExportBlendShapes)
            {
                Resource.Lines.Add($"blend_shape_mode = 0");
            }
        }

        #endregion

        [Flags]
        public enum ArrayFormatFlags{
            Vertex = 1,
            Normal = 2,
            Tangent = 4,
            Color = 8,
            UV = 16,
            UV2 = 32,
            Custom0 = 64,
            Custom1 = 128,
            Custom2 = 256,
            Custom3 = 512,
            Bones = 1024,
            Weights = 2048,
            Index = 4096,
        }

    }
}
