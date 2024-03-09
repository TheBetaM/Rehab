using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twinsanity;

namespace RehabSetup
{
    public class GodotSceneFileTwinsanity : GodotSceneFile
    {
        public static GodotSceneFileTwinsanity Create(string Name, int InstanceID = -1, string targetType = ExportGodot.Node3D)
        {
            GodotSceneFileTwinsanity ModelScene = new GodotSceneFileTwinsanity();
            Node RootNode = new Node(Name, targetType);
            RootNode.InstanceID = InstanceID;
            ModelScene.Nodes.Add(RootNode);
            return ModelScene;
        }

        public int CodeResourceID_Container_Instance = 0;
        public int CodeResourceID_Container_Trigger = 0;
        public int CodeResourceID_Container_Camera = 0;
        public int CodeResourceID_Scene = 0;
        public int WorldEnvResourceID = 0;

        public void AddLights(SceneryData Scene)
        {
            // todo fog, clear color
            int LightCount = Scene.LightsAmbient.Count + Scene.LightsDirectional.Count + Scene.LightsNegative.Count + Scene.LightsPoint.Count;
            if (LightCount <= 0) return;

            Node LightsNode = new Node($"Lights", ExportGodot.Node3D);
            LightsNode.KeyValues.Add("parent", ".");
            //LightsNode.Lines.Add("visible = false"); // temp
            Nodes.Add(LightsNode);

            InternalResource EnvData = new InternalResource();
            switch (Scene.FogPreset)
            {
                default:
                case 1: // no fog
                    EnvData.CreateWorldEnvironment();
                    break;
                case 0: // purple
                    EnvData.CreateWorldEnvironment(true, 0.5f, 0, 1);
                    break;
                case 2: // light blue
                    EnvData.CreateWorldEnvironment(true, 0.5f, 0.5f, 1);
                    break;
                case 3: // green
                    EnvData.CreateWorldEnvironment(true, 0, 1, 0);
                    break;
                case 4: // grey
                    EnvData.CreateWorldEnvironment(true, 0.5f, 0.5f, 0.5f);
                    break;
                case 5: // beige
                    EnvData.CreateWorldEnvironment(true, 0.5f, 0.5f, 0);
                    break;
            }

            if (Scene.LightsAmbient.Count > 0)
            {
                // todo: more than one ambient light, which is rare
                SceneryData.LightAmbient Light = Scene.LightsAmbient[0];
                
                EnvData.CreateWorldEnvironmentLight(Light.Color_R, Light.Color_G, Light.Color_B, Light.Radius * 1f);

                // adds WorldEnvironment node
                //Node LightsHolderNode = new Node($"WorldEnvironment", "WorldEnvironment");
                //LightsHolderNode.KeyValues.Add("parent", $"{LightsNode.Name}");
                //LightsHolderNode.Lines.Add($"environment = SubResource( {InternalResourceList.Count} )");
                //Nodes.Add(LightsHolderNode);
            }
            if (Scene.LightsDirectional.Count > 0)
            {
                // adds DirectionalLight's
                Node LightsHolderNode = new Node($"Directional", ExportGodot.Node3D);
                LightsHolderNode.KeyValues.Add("parent", $"{LightsNode.Name}");
                Nodes.Add(LightsHolderNode);

                for (int i = 0; i < Scene.LightsDirectional.Count; i++)
                {
                    SceneryData.LightDirectional Light = Scene.LightsDirectional[i];

                    Node LightNode = new Node($"DirectionalLight{i}", "DirectionalLight3D");
                    LightNode.KeyValues.Add("parent", $"{LightsNode.Name}/{LightsHolderNode.Name}");
                    LightNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Light.Position.X).ToText()}, {Light.Position.Y.ToText()}, {Light.Position.Z.ToText()} )");
                    LightNode.Lines.Add($"rotation = Vector3( {(-Light.Vector3.X).ToText()}, {(-Light.Vector3.Y).ToText()}, {(-Light.Vector3.Z).ToText()} )");
                    LightNode.Lines.Add($"light_cull_mask = 0");
                    LightNode.Lines.Add($"light_color = Color( {Light.Color_R.ToText()}, {Light.Color_G.ToText()}, {Light.Color_B.ToText()}, 1 )");
                    LightNode.Lines.Add($"light_energy = {(Light.Radius * 1f).ToText()}");
                    LightNode.Lines.Add($"light_specular = 0");
                    //LightNode.Lines.Add($"spot_angle = 180");
                    LightNode.Lines.Add($"light_bake_mode = 0");
                    //LightNode.Lines.Add($"shadow_enabled = true");
                    LightNode.Lines.Add($"shadow_normal_bias = 0"); // remove when normals are done?
                    LightNode.Lines.Add($"layers = 0");
                    Nodes.Add(LightNode);
                }
            }
            if (Scene.LightsNegative.Count > 0)
            {
                Node LightsHolderNode = new Node($"Spot", ExportGodot.Node3D);
                LightsHolderNode.KeyValues.Add("parent", $"{LightsNode.Name}");
                Nodes.Add(LightsHolderNode);

                // adds SpotLight's
                for (int i = 0; i < Scene.LightsNegative.Count; i++)
                {
                    SceneryData.LightNegative Light = Scene.LightsNegative[i];

                    Node LightNode = new Node($"SpotLight{i}", "SpotLight3D");
                    LightNode.KeyValues.Add("parent", $"{LightsNode.Name}/{LightsHolderNode.Name}");
                    LightNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Light.Position.X).ToText()}, {Light.Position.Y.ToText()}, {Light.Position.Z.ToText()} )");
                    LightNode.Lines.Add($"rotation = Vector3( {(-Light.Vector3.X).ToText()}, {(-Light.Vector3.Y).ToText()}, {(-Light.Vector3.Z).ToText()} )");
                    LightNode.Lines.Add($"light_cull_mask = 0");
                    LightNode.Lines.Add($"light_color = Color( {Light.Color_R.ToText()}, {Light.Color_G.ToText()}, {Light.Color_B.ToText()}, 1 )");
                    LightNode.Lines.Add($"light_energy = {(Light.Radius * 5f).ToText()}");
                    LightNode.Lines.Add($"light_specular = 0");
                    //LightNode.Lines.Add($"light_negative = true");
                    LightNode.Lines.Add($"spot_range = {(Light.Radius * 5f).ToText()}");
                    LightNode.Lines.Add($"spot_attenuation = 0.5");
                    LightNode.Lines.Add($"spot_angle_attenuation = 0.5");
                    LightNode.Lines.Add($"light_bake_mode = 0");
                    //LightNode.Lines.Add($"shadow_enabled = true");
                    LightNode.Lines.Add($"shadow_normal_bias = 0"); // remove when normals are done?
                    LightNode.Lines.Add($"layers = 0");
                    Nodes.Add(LightNode);
                }
            }
            if (Scene.LightsPoint.Count > 0)
            {
                Node LightsHolderNode = new Node($"Point", ExportGodot.Node3D);
                LightsHolderNode.KeyValues.Add("parent", $"{LightsNode.Name}");
                Nodes.Add(LightsHolderNode);

                // adds OmniLight's
                for (int i = 0; i < Scene.LightsPoint.Count; i++)
                {
                    SceneryData.LightPoint Light = Scene.LightsPoint[i];

                    Node LightNode = new Node($"PointLight{i}", "OmniLight3D");
                    LightNode.KeyValues.Add("parent", $"{LightsNode.Name}/{LightsHolderNode.Name}");
                    LightNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Light.Position.X).ToText()}, {Light.Position.Y.ToText()}, {Light.Position.Z.ToText()} )");
                    LightNode.Lines.Add($"light_cull_mask = 0");
                    LightNode.Lines.Add($"light_color = Color( {Light.Color_R.ToText()}, {Light.Color_G.ToText()}, {Light.Color_B.ToText()}, 1 )");
                    LightNode.Lines.Add($"light_energy = {(Light.Radius * 0.021f).ToText()}");
                    LightNode.Lines.Add($"light_specular = 0");
                    LightNode.Lines.Add($"omni_range = {(Light.Radius * 0.0938f).ToText()}");
                    LightNode.Lines.Add($"omni_attenuation = -1");
                    LightNode.Lines.Add($"light_bake_mode = 0");
                    //LightNode.Lines.Add($"shadow_enabled = true");
                    LightNode.Lines.Add($"shadow_normal_bias = 0"); // remove when normals are done?
                    LightNode.Lines.Add($"layers = 0");
                    Nodes.Add(LightNode);
                }
            }

            InternalResourceList.Add(EnvData);
            WorldEnvResourceID = InternalResourceList.Count;
        }

        public void AddLinks(ChunkLinks Links)
        {
            bool HasLinks = Links.Links.Count > 0;
            if (!HasLinks) return;

            ExternalResource LinkCodeRes = new ExternalResource($"res://code/Containers/ChunkLink{ExportGodot.ScriptExt}");
            LinkCodeRes.SetAsScript();
            ExternalResourceList.Add(LinkCodeRes);
            int LinkCode = ExternalResourceList.Count;

            Node LinksNode = new Node($"Links", ExportGodot.Node3D);
            LinksNode.KeyValues.Add("parent", ".");
            Nodes.Add(LinksNode);

            for (int i = 0; i < Links.Links.Count; i++)
            {
                ChunkLinks.ChunkLink Link = Links.Links[i];

                //ExternalResource LinkChunkRes = new ExternalResource($"../Levels/{Link.Path.Replace('\\', '_')}.tscn");
                //LinkChunkRes.SetAsPackedScene();
                //ExternalResourceList.Add(LinkChunkRes);
                //int LinkChunkID = ExternalResourceList.Count;

                Node LinkNode = new Node($"Link{i}-{Link.Path.Replace('\\', '_')}", ExportGodot.Node3D);
                LinkNode.KeyValues.Add("parent", LinksNode.Name);
                LinkNode.Lines.Add($"script = ExtResource( {LinkCode} )");
                //LinkNode.Lines.Add($"Chunk = ExtResource( {LinkChunkID} )");
                LinkNode.Lines.Add($"ChunkPath = \"Levels/{Link.Path.Replace('\\', '_')}.tscn\"");
                LinkNode.Lines.Add($"ChunkName = \"{Link.Path.Replace('\\', '_')}\"");
                if (!Link.WallIsEnabled) LinkNode.Lines.Add($"IsDisabled = true");
                if (!Link.IsVisible) 
                {
                    LinkNode.Lines.Add($"SpawnInvisible = true");
                    //LinkNode.Lines.Add($"visible = false");
                }
                //LinkNode.Lines.Add($"Flags = {Link.Flags}");
                Nodes.Add(LinkNode);

                Node ChunkHolder = new Node("ChunkHolder", ExportGodot.Node3D);
                ChunkHolder.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}");
                ChunkHolder.Lines.Add($"transform = {MatrixToTransform(Link.ChunkMatrix)}");
                Nodes.Add(ChunkHolder);

                //Node ChunkNode = new Node($"{Link.Path.Replace('\\', '_')}", ExportGodot.Node3D);
                //LinkNode.KeyValues.Add("parent", $"{LinksNode.Name}/{ChunkHolder.Name}");
                //LinkNode.KeyValues.Add("instance_placeholder", $"../Levels/{Link.Path.Replace('\\', '_')}.tscn");
                //Nodes.Add(ChunkNode);

                //Node ObjectHolder = new Node("ObjectHolder", ExportGodot.Node3D);
                //ObjectHolder.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}");
                //ObjectHolder.Lines.Add($"transform = {MatrixToTransform(Link.ObjectMatrix)}");
                //Nodes.Add(ObjectHolder);

                if (Link.HasWall)
                {
                    InternalResource Shape = new InternalResource();
                    Shape.Type = ExportGodot.ConvexPolygonShape3D;

                    StringBuilder ShapeArray = new StringBuilder();
                    ShapeArray.Append("points = PoolVector3Array( ");

                    Pos[] MeshPoints = Link.LoadWall;

                    for (int f = 0; f < MeshPoints.Length; f++)
                    {
                        ShapeArray.Append($"{(-MeshPoints[f].X).ToText()}, {MeshPoints[f].Y.ToText()}, {MeshPoints[f].Z.ToText()}, ");
                    }

                    ShapeArray.Remove(ShapeArray.Length - 2, 2);
                    ShapeArray.Append(" ) ");
                    Shape.Lines.Add(ShapeArray.ToString());
                    InternalResourceList.Add(Shape);
                    int LinkShapeID = InternalResourceList.Count;

                    Node LinkAreaNode = new Node($"EnterTrigger", ExportGodot.Area3D);
                    LinkAreaNode.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}");
                    //LinkAreaNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Pos_X).ToText()}, {Pos_Y.ToText()}, {Pos_Z.ToText()} )");
                    //PosNode.Lines.Add($"rotation = Vector3( {RotX.ToText()}, {RotY.ToText()}, {RotZ.ToText()} )");
                    Nodes.Add(LinkAreaNode);
                    Node LinkColNode = new Node($"EnterTriggerShape", ExportGodot.CollisionShape3D);
                    LinkColNode.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}/{LinkAreaNode.Name}");
                    LinkColNode.Lines.Add($"shape = SubResource( {LinkShapeID} )");
                    Nodes.Add(LinkColNode);
                }

                if (Link.TreeRoot != null)
                {
                    Node LoadTriggerHolder = new Node("LoadTriggers", ExportGodot.Node3D);
                    LoadTriggerHolder.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}");
                    Nodes.Add(LoadTriggerHolder);
                    uint LinkTriggerID = 0;

                    ChunkLinks.ChunkLink.LinkTree Leaf = Link.TreeRoot;
                    while (Leaf != null)
                    {
                        InternalResource Shape = new InternalResource();
                        Shape.Type = ExportGodot.ConvexPolygonShape3D;

                        StringBuilder ShapeArray = new StringBuilder();
                        ShapeArray.Append("points = PoolVector3Array( ");

                        Pos[] MeshPoints = Leaf.LoadArea;

                        for (int f = 0; f < MeshPoints.Length; f++)
                        {
                            ShapeArray.Append($"{(-MeshPoints[f].X).ToText()}, {MeshPoints[f].Y.ToText()}, {MeshPoints[f].Z.ToText()}, ");
                        }

                        ShapeArray.Remove(ShapeArray.Length - 2, 2);
                        ShapeArray.Append(" ) ");
                        Shape.Lines.Add(ShapeArray.ToString());
                        InternalResourceList.Add(Shape);
                        int LinkShapeID = InternalResourceList.Count;

                        Node LinkLeafNode = new Node($"LoadTrigger_{LinkTriggerID}", ExportGodot.Area3D);
                        LinkLeafNode.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}/{LoadTriggerHolder.Name}");
                        //LinkLeafNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Pos_X).ToText()}, {Pos_Y.ToText()}, {Pos_Z.ToText()} )");
                        //PosNode.Lines.Add($"rotation = Vector3( {RotX.ToText()}, {RotY.ToText()}, {RotZ.ToText()} )");
                        Nodes.Add(LinkLeafNode);
                        Node LinkLeafColNode = new Node($"LoadTriggerCollision", ExportGodot.CollisionShape3D);
                        LinkLeafColNode.KeyValues.Add("parent", $"{LinksNode.Name}/{LinkNode.Name}/{LoadTriggerHolder.Name}/{LinkLeafNode.Name}");
                        LinkLeafColNode.Lines.Add($"shape = SubResource( {InternalResourceList.Count} )");
                        Nodes.Add(LinkLeafColNode);

                        Leaf = Leaf.Next;
                        LinkTriggerID++;
                    }
                }
            }
        }

        public void AddDynamicScenery(DynamicSceneryData Scene, string path, Dictionary<uint, string> ExportedTextures)
        {
            // todo: more collision data?
            if (Scene.Models.Count == 0) return;
            TwinsSection rigid_sec = Scene.Parent.GetItem<TwinsSection>(6).GetItem<TwinsSection>(6);
            TwinsSection targetFile = Scene.Parent;
            SceneryData SceneData = targetFile.GetItem<SceneryData>(0);
            string SceneName = $"{SceneData.ChunkName.Replace('\\', '_')}-DynamicScenery";
            string DirPath = $"{path}\\Scenery\\";
            //string ModelFilePath = System.IO.Path.GetFileNameWithoutExtension(path);

            Node RootNode = new Node($"DynamicScenery", ExportGodot.Node3D);
            RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(RootNode);

            for (int i = 0; i < Scene.Models.Count; i++)
            {
                DynamicSceneryData.DynamicSceneryModel ThisModel = Scene.Models[i];

                Node ColNode = new Node($"AnimBody{i}", "AnimatableBody3D");
                bool HasPos = ThisModel.AnimPosX != null || ThisModel.AnimPosY != null || ThisModel.AnimPosZ != null;
                bool HasRot = ThisModel.AnimRotX != null || ThisModel.AnimRotY != null || ThisModel.AnimRotZ != null;
                ColNode.KeyValues.Add("parent", RootNode.Name);
                //ColNode.Lines.Add($"mode = 1"); // static
                ColNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-ThisModel.WorldPosition.X).ToText()}, {ThisModel.WorldPosition.Y.ToText()}, {ThisModel.WorldPosition.Z.ToText()} )");
                ColNode.Lines.Add($"sync_to_physics=false"); //fixes animation bug
                Nodes.Add(ColNode);

                // Export DAE and textures
                RigidModel RigidModelCont = rigid_sec.GetItem<RigidModel>(ThisModel.ModelID);
                uint Hash = ExportGodot.ExportModelResource(RigidModelCont, path, ExportedTextures);
                Add_InstancedScene($"../Mesh/{DefaultHashes.RigidToName(ThisModel.ModelID, Hash)}", $"{RootNode.Name}/{ColNode.Name}");

                if (Scene.Models[i].GI_Types.Count == 0) continue;

                for (int g = 0; g < Scene.Models[i].GI_Types.Count; g++)
                {
                    DynamicSceneryData.GI_Type3 Coll = Scene.Models[i].GI_Types[g];

                    if (Coll.Vertices.Count == 0) continue;

                    InternalResource Shape = new InternalResource();
                    Shape.Type = ExportGodot.ConvexPolygonShape3D;

                    StringBuilder ShapeArray = new StringBuilder();
                    ShapeArray.Append("points=PoolVector3Array(");

                    Pos[] MeshPoints = Coll.Vertices.ToArray();

                    for (int f = 0; f < MeshPoints.Length; f++)
                    {
                        ShapeArray.Append($"{(-MeshPoints[f].X).ToText()}, {MeshPoints[f].Y.ToText()}, {MeshPoints[f].Z.ToText()}, ");
                    }

                    ShapeArray.Remove(ShapeArray.Length - 2, 2);
                    ShapeArray.Append(")");
                    Shape.Lines.Add(ShapeArray.ToString());
                    InternalResourceList.Add(Shape);
                    int ShapeID = InternalResourceList.Count;

                    Node ShapeNode = new Node($"CollisionShape{g}", ExportGodot.CollisionShape3D);
                    ShapeNode.KeyValues.Add("parent", $"{RootNode.Name}/{ColNode.Name}");
                    ShapeNode.Lines.Add($"shape=SubResource({ ShapeID })");
                    Nodes.Add(ShapeNode);
                }

                if (ThisModel.FrameCount == 0) continue;

                // Export RESET animation
                InternalResource ResetRes = new InternalResource();
                ResetRes.Lines.Add($"length=0.001");
                ResetRes.Type = "Animation";
                StringBuilder ReStrpos = new();
                StringBuilder ReStrrot = new();

                System.Numerics.Quaternion resquat = System.Numerics.Quaternion.CreateFromYawPitchRoll(ThisModel.WorldRotation.Y, ThisModel.WorldRotation.X, ThisModel.WorldRotation.Z);

                ReStrpos.Append($"0,1,{(-ThisModel.WorldPosition.X).ToText()},{ThisModel.WorldPosition.Y.ToText()},{ThisModel.WorldPosition.Z.ToText()}");
                ReStrrot.Append($"0,1,{resquat.X.ToText()},{resquat.Y.ToText()},{resquat.Z.ToText()},{resquat.W.ToText()}");
                ResetRes.Lines.Add($"tracks/0/type=\"position_3d\"");
                ResetRes.Lines.Add($"tracks/0/path=NodePath(\".\")");
                ResetRes.Lines.Add($"tracks/0/keys=PackedFloat32Array({ReStrpos.ToString()})");
                ResetRes.Lines.Add($"tracks/1/type=\"rotation_3d\"");
                ResetRes.Lines.Add($"tracks/1/path=NodePath(\".\")");
                ResetRes.Lines.Add($"tracks/1/keys=PackedFloat32Array({ReStrrot.ToString()})");
                InternalResourceList.Add(ResetRes);
                int ResetResID = InternalResourceList.Count;

                // Export movement animation
                InternalResource AnimRes = new InternalResource();
                float FrameStep = 0.02f * 2f; // All animations are 25 FPS
                int AllFrames = ThisModel.FrameCount;
                List<float> timeStamps = new();
                for (int t = 0; t < AllFrames; t++)
                {
                    timeStamps.Add(t * FrameStep);
                }
                AnimRes.Type = "Animation";
                AnimRes.Lines.Add($"length={(FrameStep * AllFrames).ToText()}");
                AnimRes.Lines.Add($"step={FrameStep.ToText()}");
                AnimRes.Lines.Add($"loop_mode=1");
                StringBuilder Strpos = new();
                StringBuilder Strrot = new();
                Strpos.Append($"0, 1, {(-ThisModel.WorldPosition.X).ToText()}, {ThisModel.WorldPosition.Y.ToText()}, {ThisModel.WorldPosition.Z.ToText()}, ");
                Strrot.Append($"0, 1, {resquat.X.ToText()}, {resquat.Y.ToText()}, {resquat.Z.ToText()}, {resquat.W.ToText()}, ");
                for (int f = 1; f < ThisModel.FrameCount; f++)
                {
                    Strpos.Append($"{timeStamps[f].ToText()}, 1, ");
                    Strrot.Append($"{timeStamps[f].ToText()}, 1, ");

                    if (ThisModel.AnimPosX != null)
                        Strpos.Append($"{(-ThisModel.AnimPosX[f]).ToText()}, ");
                    else
                        Strpos.Append($"{(-ThisModel.WorldPosition.X).ToText()}, ");
                    if (ThisModel.AnimPosY != null)
                        Strpos.Append($"{ThisModel.AnimPosY[f].ToText()}, ");
                    else
                        Strpos.Append($"{ThisModel.WorldPosition.Y.ToText()}, ");
                    if (ThisModel.AnimPosZ != null)
                        Strpos.Append($"{ThisModel.AnimPosZ[f].ToText()}, ");
                    else
                        Strpos.Append($"{ThisModel.WorldPosition.Z.ToText()}, ");

                    System.Numerics.Quaternion quat = System.Numerics.Quaternion.Identity;

                    if (ThisModel.AnimRotX != null)
                        quat.X = ThisModel.AnimRotX[f];
                    else
                        quat.X = ThisModel.WorldRotation.X;
                    if (ThisModel.AnimRotY != null)
                        quat.Y = ThisModel.AnimRotY[f];
                    else
                        quat.Y = ThisModel.WorldRotation.Y;
                    if (ThisModel.AnimRotZ != null)
                        quat.Z = ThisModel.AnimRotZ[f];
                    else
                        quat.Z = ThisModel.WorldRotation.Z;
                    if (ThisModel.AnimRotW != null)
                        quat.W = ThisModel.AnimRotW[f];
                    else
                        quat.W = ThisModel.WorldRotation.W;
                    
                    quat = System.Numerics.Quaternion.CreateFromYawPitchRoll(quat.Y, quat.X, quat.Z);

                    Strrot.Append($"{quat.X.ToText()},{quat.Y.ToText()},{quat.Z.ToText()},{quat.W.ToText()}, ");
                }
                Strpos.Remove(Strpos.Length - 2, 2);
                Strrot.Remove(Strrot.Length - 2, 2);
                AnimRes.Lines.Add($"tracks/0/type=\"position_3d\"");
                AnimRes.Lines.Add($"tracks/0/path=NodePath(\".\")");
                AnimRes.Lines.Add($"tracks/0/keys=PackedFloat32Array({Strpos.ToString()})");
                AnimRes.Lines.Add($"tracks/1/type=\"rotation_3d\"");
                AnimRes.Lines.Add($"tracks/1/path=NodePath(\".\")");
                AnimRes.Lines.Add($"tracks/1/keys=PackedFloat32Array({Strrot.ToString()})");
                InternalResourceList.Add(AnimRes);
                int AnimResID = InternalResourceList.Count;

                InternalResource AnimLib = new InternalResource();
                AnimLib.Type = "AnimationLibrary";
                AnimLib.Lines.Add("_data = {");
                AnimLib.Lines.Add($"\"RESET\": SubResource({ResetResID}),");
                AnimLib.Lines.Add($"\"anim\": SubResource({AnimResID}),");
                AnimLib.Lines.Add("}");
                InternalResourceList.Add(AnimLib);

                Node AnimNode = new Node($"AnimationPlayer", "AnimationPlayer");
                AnimNode.KeyValues.Add("parent", $"{RootNode.Name}/{ColNode.Name}");
                AnimNode.Lines.Add($"root_node=NodePath(\"..\")");
                AnimNode.Lines.Add($"autoplay=\"anim\"");
                AnimNode.Lines.Add($"playback_process_mode=0"); // physics
                AnimNode.Lines.Add("libraries={");
                AnimNode.Lines.Add($"\"\": SubResource({InternalResourceList.Count})");
                AnimNode.Lines.Add("}");
                Nodes.Add(AnimNode);

            }
        }

        public void AddSkydome(Skydome Scene, string path, Dictionary<uint, string> ExportedTextures)
        {
            if (Scene.ModelIDs.Length <= 0) return;
            TwinsSection rigid_sec = Scene.Parent.Parent.GetItem<TwinsSection>(6);

            Nodes[0].Lines.Add("scale=Vector3(100,100,100)");

            for (int i = 0; i < Scene.ModelIDs.Length; i++)
            {
                // Export DAE and textures
                RigidModel RigidModelCont = rigid_sec.GetItem<RigidModel>(Scene.ModelIDs[i]);
                uint Hash = ExportGodot.ExportModelResource(RigidModelCont, path, ExportedTextures);
                Add_InstancedScene($"../Mesh/{DefaultHashes.RigidToName(Scene.ModelIDs[i], Hash)}", $".");
                Nodes[i + 1].Lines.Add($"cast_shadow=0"); // no need for skydome to cast shadows? can always tune for clouds or sth afterwards
            }
        }

        public void AddLODModel(LodModel Scene, string path, Dictionary<uint, string> ExportedTextures)
        {
            // todo LOD distance
            TwinsSection rigid_sec = Scene.Parent.Parent.GetItem<TwinsSection>(6);

            for (int i = 0; i < Scene.ModelsAmount; i++)
            {
                // Export DAE and textures
                RigidModel RigidModelCont = rigid_sec.GetItem<RigidModel>(Scene.LODModelIDs[i]);
                uint Hash = ExportGodot.ExportModelResource(RigidModelCont, path, ExportedTextures);
                Add_InstancedScene($"../Mesh/{DefaultHashes.RigidToName(Scene.LODModelIDs[i], Hash)}", $".");
                
                if (i != 0)
                {
                    Nodes.Last().Lines.Add("visible = false");
                }
            }
        }

        public void AddScenery(SceneryData Scene, string path, Dictionary<uint, string> ExportedTextures)
        {
            TwinsSection scene_rigid_sec = Scene.Parent.GetItem<TwinsSection>(6).GetItem<TwinsSection>(6);
            TwinsSection scene_lod_sec = Scene.Parent.GetItem<TwinsSection>(6).GetItem<TwinsSection>(7);
            TwinsSection scene_sky_sec = Scene.Parent.GetItem<TwinsSection>(6).GetItem<TwinsSection>(8);

            // Export skydome
            if ((Scene.SkydomeID != 0 || Scene.ParentFile.Type == TwinsFile.FileType.DemoSM2) && scene_sky_sec.ContainsItem(Scene.SkydomeID))
            {
                Skydome SkydomeCont = scene_sky_sec.GetItem<Skydome>(Scene.SkydomeID);
                ExportGodot.ExportSkydome(SkydomeCont, path, ExportedTextures);
            }

            // Export scenery
            Node RootNode = new Node("SceneryRoot", ExportGodot.Node3D);
            RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(RootNode);

            uint NodeID = 0;
            Dictionary<uint, (int, uint)> ExportedModels = new Dictionary<uint, (int, uint)>();
            Dictionary<uint, int> ExportedLODs = new Dictionary<uint, int>();
            ParseSceneryTree(Scene.SceneryRoot, scene_rigid_sec, scene_lod_sec, path, false, ref NodeID, "SceneryRoot", ExportedModels, ExportedLODs, ExportedTextures);
        }

        public void ParseSceneryTree(SceneryData.SceneryStruct Node, TwinsSection ModelSection, TwinsSection LODSection, 
            string path, bool SceneOnly, ref uint NodeID, string ParentNodeName, Dictionary<uint, (int, uint)> ExportedModels, Dictionary<uint, int> ExportedLODs,
            Dictionary<uint, string> ExportedTextures)
        {
            for (int a = 0; a < Node.Model.Models.Count; a++)
            {
                Add_InstancedSceneryModel(Node.Model.Models[a], path, ParentNodeName, ref NodeID, ModelSection, LODSection, ExportedModels, ExportedLODs, ExportedTextures);
            }

            uint ParentNodeID = NodeID;
            Node TreeNode = new Node($"node_{NodeID}", ExportGodot.Node3D);
            TreeNode.KeyValues.Add("parent", ParentNodeName);
            Nodes.Add(TreeNode);
            NodeID++;

            for (int i = 0; i < Node.Links.Length; i++)
            {
                if (Node.Links[i] is SceneryData.SceneryStruct n)
                {
                    ParseSceneryTree(n, ModelSection, LODSection, path, SceneOnly, ref NodeID, $"{ParentNodeName}/node_{ParentNodeID}", ExportedModels, ExportedLODs, ExportedTextures);
                }
                else if (Node.Links[i] is SceneryData.SceneryModelStruct Model)
                {
                    for (int a = 0; a < Model.Models.Count; a++)
                    {
                        Add_InstancedSceneryModel(Model.Models[a], path, ParentNodeName, ref NodeID, ModelSection, LODSection, ExportedModels, ExportedLODs, ExportedTextures);
                    }
                }
            }
        }

        public void AddSM(TwinsFile targetFile, string path,  bool SceneOnly, bool IncludeSkydome = true)
        {
            SceneryData Scene = targetFile.GetItem<SceneryData>(0);
            ChunkLinks Links = targetFile.GetItem<ChunkLinks>(5);

            #region Export Textures
            TwinsSection tex_sec = targetFile.GetItem<TwinsSection>(6).GetItem<TwinsSection>(0);
            string TexPath = $"{path}\\Textures\\";
            Dictionary<uint, string> ExportedTextures = new();
            if (tex_sec.Type == SectionType.TextureX)
            {
                foreach (var item in tex_sec.Records)
                {
                    TextureX tex = (TextureX)item;
                    if (DefaultHashes.DupeTextureIDs.Contains(tex.ID))
                        ExportedTextures[tex.ID] = ExtractTextureX(tex, TexPath, true);
                    else
                        ExtractTextureX(tex, TexPath, false);
                }
            }
            else
            {
                foreach (var item in tex_sec.Records)
                {
                    Texture tex = (Texture)item;
                    if (DefaultHashes.DupeTextureIDs.Contains(tex.ID))
                        ExportedTextures[tex.ID] = ExtractTexture(tex, TexPath, true);
                    else
                        ExtractTexture(tex, TexPath, false);
                }
            }
            #endregion

            string SceneryFilePath = $"{Scene.ChunkName.Replace('\\', '_')}-Scenery";
            if (!SceneOnly)
            {
                ExportGodot.ExportScenery(Scene, path, ExportedTextures);
            }
            Add_InstancedScene($"../Scenery/{SceneryFilePath}", $".");
            Nodes.Last().Lines.Add("metadata/_edit_lock_ = true"); // prevents scenery from being selected in editor (easier for level editing)

            AddLights(Scene);
            AddLinks(Links);
        }

        public void AddCollisionData(ColData Data, string path, string ModelFilePath, bool SceneOnly = false)
        {
            string DirPath = $"{path}\\Scenery\\";
            //string ModelFilePath = System.IO.Path.GetFileNameWithoutExtension(path);
            // optional collision visuals
            /*
            // Export DAE
            ExportCollada.ClearCache();
            ExportCollada.LoadModel(Data);
            if (!SceneOnly)
            {
                ExportCollada.ExportModel($"{DirPath}{ModelFilePath}");
            }

            ExternalResource ModelFileReference = new ExternalResource(ModelFilePath);
            ModelFileReference.SetAsPackedScene();
            ExternalResourceList.Add(ModelFileReference);
            */

            List<int> SurfaceLayers = new List<int>();
            for (int g = 0; g < Data.Tris.Count; g++)
            {
                if (!SurfaceLayers.Contains(Data.Tris[g].Surface))
                {
                    SurfaceLayers.Add(Data.Tris[g].Surface);
                }
            }
            if (SurfaceLayers.Count == 0) return;

            //Directory.CreateDirectory($"{System.IO.Path.GetDirectoryName(path)}\\Code\\Containers\\");
            //ExportGodot.ContainerWriter.ExportContainer_CollisionData($"{System.IO.Path.GetDirectoryName(path)}\\Code\\Containers\\CollisionSurface{ExportGodot.ScriptExt}");
            //ExternalResource Code_Container_Resource_Collision = new ExternalResource($"../Code/Containers/CollisionSurface{ExportGodot.ScriptExt}");
            //Code_Container_Resource_Collision.SetAsScript();
            //ExternalResourceList.Add(Code_Container_Resource_Collision);
            //int ColDataCode = ExternalResourceList.Count;

            for (uint i = 0; i < 28; i++)
            {
                if (!SurfaceLayers.Contains((int)i)) continue;
                string MatName = $"{DefaultHashes.ToName(SectionType.CollisionSurface, i)}";

                //InternalResource Material = new InternalResource();
                //Material.CreateSolidMaterial(CollisionColors[i].R/255f, CollisionColors[i].G/255f, CollisionColors[i].B/255f, 1f);
                //InternalResourceList.Add(Material);

                Node GeomNode = new Node($"{MatName}", ExportGodot.Node3D);
                GeomNode.KeyValues.Add("parent", ".");
                //GeomNode.Lines.Add($"{ExportGodot.materialOverride}/0 = SubResource( {InternalResourceList.Count} )");
                Nodes.Add(GeomNode);

                //ExternalResource SurfData = new ExternalResource($"../Surfaces/{DefaultHashes.ToName(SectionType.CollisionSurface, i)}.tres");
                //ExternalResourceList.Add(SurfData);
                //int SurfDataID = ExternalResourceList.Count;

                Node BodyNode = new Node($"StaticBody", ExportGodot.StaticBody3D);
                BodyNode.KeyValues.Add("parent", GeomNode.Name);

                switch ((DefaultEnums.SurfaceTypes)i)
                {
                    default:
                        break;
                    case DefaultEnums.SurfaceTypes.SURF_FALL_THRU_DEATH: // non-solid death trigger
                    case DefaultEnums.SurfaceTypes.SURF_NORMAL_WATER: // ripples only
                    case DefaultEnums.SurfaceTypes.SURF_CAMERA_BLOCKING: // solid collision for camera only
                    case DefaultEnums.SurfaceTypes.SURF_NONSOLID_ELECTRIC_DEATH: // non-solid death trigger
                    case DefaultEnums.SurfaceTypes.SURF_BLOCK_AI_ONLY: // solid collision for non-player objects only
                        // temp?
                        BodyNode.Lines.Add($"collision_layer=0");
                        break;
                }
                //BodyNode.Lines.Add($"collision_layer = 3");
                //BodyNode.Lines.Add($"collision_mask = 3");
                //BodyNode.Lines.Add($"physics_material_override = SubResource( 3 )");
                //BodyNode.Lines.Add($"constant_linear_velocity = Vector3( 1, 2, 3 )");
                //BodyNode.Lines.Add($"constant_angular_velocity = Vector3( 1, 2, 3 )");
                //BodyNode.Lines.Add($"script = ExtResource( {ColDataCode} )");
                //BodyNode.Lines.Add($"SurfaceData = ExtResource( { SurfDataID } )");
                Nodes.Add(BodyNode);

                int ShapeID = 0;
                GodotBinaryCollisionShape shape = new GodotBinaryCollisionShape(Data, (int)i);
                string ShapeFilePath = $"{DirPath}{ModelFilePath}_{(DefaultEnums.SurfaceTypes)i}.res";
                shape.WriteToFile(ShapeFilePath);

                ExternalResource ShapeRes = new ExternalResource($"{ModelFilePath}_{(DefaultEnums.SurfaceTypes)i}.res", shape.ResType);
                ExternalResourceList.Add(ShapeRes);
                ShapeID = ExternalResourceList.Count;

                Node ShapeNode = new Node("CollisionShape", ExportGodot.CollisionShape3D);
                ShapeNode.KeyValues.Add("parent", $"{GeomNode.Name}/{BodyNode.Name}");
                ShapeNode.Lines.Add($"shape=ExtResource({ ShapeID })");
                Nodes.Add(ShapeNode);

            }
            
        }

        public void AddParticleData(ParticleData Parts, string path, bool SceneOnly = false)
        {
            // todo: particle definitions as packed scenes, particle instance data
            if (Parts.ParticleInstances.Count <= 0 || Parts.ParticleTypes.Count <= 0) return;

            Node RootNode = new Node($"Particles", ExportGodot.Node3D);
            RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(RootNode);

            for (int i = 0; i < Parts.ParticleInstances.Count; i++)
            {
                ParticleData.ParticleSystemInstance Inst = Parts.ParticleInstances[i];

                Node PosNode = new Node($"{Inst.Name}_{i}", ExportGodot.Node3D);
                PosNode.KeyValues.Add("parent", RootNode.Name);
                PosNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Inst.Position.X).ToText()}, {Inst.Position.Y.ToText()}, {Inst.Position.Z.ToText()} )");
                System.Numerics.Matrix4x4 mat = System.Numerics.Matrix4x4.Identity;
                mat *= System.Numerics.Matrix4x4.CreateRotationX((Inst.Rot_X / 65536f) * (float)(2f * Math.PI));
                mat *= System.Numerics.Matrix4x4.CreateRotationY((-Inst.Rot_Y / 65536f) * (float)(2f * Math.PI));
                mat *= System.Numerics.Matrix4x4.CreateRotationZ((-Inst.Rot_Z / 65536f) * (float)(2f * Math.PI));
                System.Numerics.Matrix4x4.Decompose(mat, out var tscale, out var trot, out var tpos);
                PosNode.Lines.Add($"quaternion = Quaternion( {trot.X.ToText()}, {trot.Y.ToText()}, {trot.Z.ToText()}, {trot.W.ToText()} )");
                //float RotX = (float)((Inst.Rot_X / 65535f) * (2f * Math.PI));
                //float RotY = (float)((-Inst.Rot_Y / 65535f) * (2f * Math.PI));
                //float RotZ = (float)((-Inst.Rot_Z / 65535f) * (2f * Math.PI));
                //PosNode.Lines.Add($"rotation = Vector3( {RotX.ToText()}, {RotY.ToText()}, {RotZ.ToText()} )");
                Nodes.Add(PosNode);
            }

        }

        public void AddOGI(GraphicsInfo GI, string path, Dictionary<uint, string> ExportedTextures)
        {
            // todo: additional collision data?
            TwinsSection rigid_sec = GI.Parent.Parent.Parent.GetItem<TwinsSection>(11).GetItem<TwinsSection>(3);
            TwinsSection skin_sec = GI.Parent.Parent.Parent.GetItem<TwinsSection>(11).GetItem<TwinsSection>(4);

            Node ColNode = new Node($"RigidBody", ExportGodot.RigidBody3D);
            ColNode.KeyValues.Add("parent", $".");
            ColNode.Lines.Add($"mode=1"); // static
            ColNode.Lines.Add($"freeze=true"); // static
            Nodes.Add(ColNode);

            // Export skeleton
            Node RootNode = new Node($"Armature", "Skeleton");
            RootNode.KeyValues.Add("parent", "RigidBody");
            GodotUtil.GetRestPose(RootNode, GI);
            Nodes.Add(RootNode);
            uint BlendShapeCount = 0;

            // Export models
            if (skin_sec.Type == SectionType.SkinX)
            {
                if (GI.BlendSkinID != 0)
                {
                    BlendShapeCount = DefaultHashes.BlendShapeCounts[GI.BlendSkinID];
                    string ModelFilePath = $"../Skins/BlendSkin_{DefaultHashes.ToName(SectionType.BlendSkin, GI.BlendSkinID)}";
                    Add_InstancedScene(ModelFilePath, $"RigidBody/{RootNode.Name}");

                    // adjusting name for blend shape animation access
                    Nodes.Last().Name = "BlendSkin";
                }
                if (GI.SkinID != 0)
                {
                    string ModelFilePath = $"../Skins/Skin_{DefaultHashes.ToName(SectionType.Skin, GI.SkinID)}";
                    Add_InstancedScene(ModelFilePath, $"RigidBody/{RootNode.Name}");
                }
            }
            else
            {
                if (GI.BlendSkinID != 0)
                {
                    BlendShapeCount = DefaultHashes.BlendShapeCounts[GI.BlendSkinID];
                    string ModelFilePath = $"../Skins/BlendSkin_{DefaultHashes.ToName(SectionType.BlendSkin, GI.BlendSkinID)}";
                    Add_InstancedScene(ModelFilePath, $"RigidBody/{RootNode.Name}");

                    // adjusting name for blend shape animation access
                    Nodes.Last().Name = "BlendSkin";
                }
                if (GI.SkinID != 0)
                {
                    string ModelFilePath = $"../Skins/Skin_{DefaultHashes.ToName(SectionType.Skin, GI.SkinID)}";
                    Add_InstancedScene(ModelFilePath, $"RigidBody/{RootNode.Name}");
                }
            }

            // Export RESET animation
            GodotBinaryAnimation ResetAnim = new GodotBinaryAnimation(GI, BlendShapeCount);
            string AnimPath = $"{path}\\Rigs\\RigRESET_{DefaultHashes.ToName(SectionType.OGI, GI.ID)}.res";
            ResetAnim.WriteToFile(AnimPath);

            // Attachments
            Dictionary<uint, Node> AttachNodes = new();

            if (GI.CollisionData.Length != 0)
            {
                
                ExternalResource Code_Bone = new ExternalResource($"res://code/Containers/BoneCollisionShape3D{ExportGodot.ScriptExt}");
                Code_Bone.SetAsScript();
                ExternalResourceList.Add(Code_Bone);
                int Code_BoneID = ExternalResourceList.Count;

                for (int a = 0; a < GI.CollisionData.Length; a++)
                {
                    uint jointID = GI.CollisionDataRelated[a];
                    /*
                    if (jointID != 255 && !AttachNodes.ContainsKey(jointID))
                    {
                        var attach = new Node($"attach{jointID}", "BoneAttachment3D");
                        attach.KeyValues.Add("parent", "RigidBody/Armature");
                        attach.Lines.Add($"bone_name = \"joint{jointID}\"");
                        attach.Lines.Add($"bone_idx = {jointID}");
                        AttachNodes.Add(jointID, attach);
                        Nodes.Add(attach);
                    }
                    */

                    InternalResource Shape = new InternalResource();
                    Shape.Type = ExportGodot.ConvexPolygonShape3D;

                    StringBuilder ShapeArray = new StringBuilder();
                    ShapeArray.Append("points=PoolVector3Array(");

                    TwinsVector4[] MeshPoints = GI.CollisionData[a].UnkVectors1;

                    for (int f = 0; f < MeshPoints.Length; f++)
                    {
                        ShapeArray.Append($"{(-MeshPoints[f].X).ToText()},{MeshPoints[f].Y.ToText()},{MeshPoints[f].Z.ToText()}, ");
                    }

                    ShapeArray.Remove(ShapeArray.Length - 2, 2);
                    ShapeArray.Append(")");
                    Shape.Lines.Add(ShapeArray.ToString());
                    InternalResourceList.Add(Shape);
                    int ShapeID = InternalResourceList.Count;

                    Node ShapeNode = new Node($"CollisionShape{a}", ExportGodot.CollisionShape3D);
                    ShapeNode.KeyValues.Add("parent", $"RigidBody");
                    ShapeNode.Lines.Add($"script=ExtResource({Code_BoneID})");
                    ShapeNode.Lines.Add($"shape=SubResource({ ShapeID })");
                    ShapeNode.Lines.Add($"bone={jointID}");
                    /*
                    if (jointID == 255)
                    {
                        ShapeNode.KeyValues.Add("parent", $"RigidBody");
                    }
                    else
                    {
                        ShapeNode.KeyValues.Add("parent", $"RigidBody/Armature/attach{jointID}");
                    }
                    */
                    //ShapeNode.Lines.Add($"disabled = true");
                    Nodes.Add(ShapeNode);
                }
            }

            foreach (var pair in GI.ModelIDs)
            {
                uint jointID = pair.Value.JointIndex;
                uint modelID = pair.Value.ModelID;
                if (!AttachNodes.ContainsKey(jointID))
                {
                    var attach = new Node($"attach{jointID}", "BoneAttachment3D");
                    attach.KeyValues.Add("parent", "RigidBody/Armature");
                    attach.Lines.Add($"bone_name=\"joint{jointID}\"");
                    attach.Lines.Add($"bone_idx={jointID}");
                    AttachNodes.Add(jointID, attach);
                    Nodes.Add(attach);
                }

                string ModelFilePath = $"../Mesh/";
                //RigidModel model = rigid_sec.GetItem<RigidModel>(modelID);
                //uint Hash = ExportGodot.ExportModelResource(model, path);
                //string outName = DefaultHashes.RigidToName(model.ID, Hash);
                //ModelFilePath += outName;
                
                if (DefaultHashes.Hash_RigidModels.ContainsKey(modelID))
                {
                    string outName = DefaultHashes.RigidToName(modelID, 0);
                    ModelFilePath += outName;
                }
                else
                {
                    RigidModel model = rigid_sec.GetItem<RigidModel>(modelID);
                    uint Hash = ExportGodot.ExportModelResource(model, path, ExportedTextures);
                    string outName = DefaultHashes.RigidToName(model.ID, Hash);
                    ModelFilePath += outName;
                }
                

                Add_InstancedScene(ModelFilePath, $"RigidBody/Armature/attach{jointID}");
            }
            for (int a = 0; a < GI.ExitPoints.Length; a++)
            {
                uint jointID = GI.ExitPoints[a].ParentJointIndex;
                if (!AttachNodes.ContainsKey(jointID))
                {
                    var attach = new Node($"attach{jointID}", "BoneAttachment3D");
                    attach.KeyValues.Add("parent", "RigidBody/Armature");
                    attach.Lines.Add($"bone_name=\"joint{jointID}\"");
                    attach.Lines.Add($"bone_idx={jointID}");
                    AttachNodes.Add(jointID, attach);
                    Nodes.Add(attach);
                }
                
                // todo change to new calculation?
                Node ExitNode = new Node($"ExitPoint{GI.ExitPoints[a].ID}", ExportGodot.Node3D);
                ExitNode.KeyValues.Add("parent", $"RigidBody/Armature/attach{jointID}");
                ExitNode.Lines.Add($"transform={MatrixToTransform(GI.ExitPoints[a].Matrix)}");
                Nodes.Add(ExitNode);
            }

            for (int a = 0; a < GI.Joints.Length; a++)
            {
                if (GI.Joints[a].ReactJointID != 255)
                {
                    // kind of a roundabout way of doing Joint-ID, but that way there's no need for code container
                    uint jointID = (uint)a;
                    if (!AttachNodes.ContainsKey(jointID))
                    {
                        var attach = new Node($"attach{jointID}", "BoneAttachment3D");
                        attach.KeyValues.Add("parent", "RigidBody/Armature");
                        attach.Lines.Add($"bone_name=\"joint{jointID}\"");
                        attach.Lines.Add($"bone_idx={jointID}");
                        AttachNodes.Add(jointID, attach);
                        Nodes.Add(attach);
                    }

                    Node ExitNode = new Node($"JointID-{GI.Joints[a].ReactJointID}", ExportGodot.Node3D);
                    ExitNode.KeyValues.Add("parent", $"RigidBody/Armature/attach{jointID}");
                    Nodes.Add(ExitNode);
                }
            }

            Node AnimNode = new Node($"AnimationPlayer", "AnimationPlayer");
            AnimNode.KeyValues.Add("parent", $".");
            AnimNode.Lines.Add($"root_node=NodePath(\"../RigidBody/Armature\")");
            Nodes.Add(AnimNode);

        }

        static readonly string[] GameObjectTypeScripts = new string[] {
            "Character",
            "Pickup",
            "Crate",
            "Creature",
            "Furniture",
            "ChiChiGrass",
            "PayGate",
            "Foofie",
            "Projectile",
        };

        public void AddGameObject(GameObject Agent, string path, bool SceneOnly = false)
        {
            //TwinsSection gi_sec = Agent.Parent.Parent.GetItem<TwinsSection>(3);
            //TwinsSection obj_sec = Agent.Parent.Parent.GetItem<TwinsSection>(0);
            //TwinsSection ca_sec = Agent.Parent.Parent.GetItem<TwinsSection>(4);
            //TwinsSection scr_sec = Agent.Parent.Parent.GetItem<TwinsSection>(1);
            byte AgentType = (byte)(Agent.UnkBitfield >> 0x14 & 0xFF);
            byte AgentUnkTypeValue = (byte)(Agent.UnkBitfield >> 0xC & 0xFF);
            byte AgentJointIDs = (byte)(Agent.UnkBitfield >> 0x6 & 0x3F);
            byte AgentExitPoints = (byte)(Agent.UnkBitfield & 0x3F);

            ExternalResource AgentObjectCode = new ExternalResource($"res://code/ALab/Agent{GameObjectTypeScripts[AgentType]}{ExportGodot.ScriptExt}");
            AgentObjectCode.SetAsScript();
            ExternalResourceList.Add(AgentObjectCode);
            int AgentObjectCodeID = ExternalResourceList.Count;
            Nodes[0].Type = ExportGodot.RigidBody3D;
            Nodes[0].Lines.Add($"script=ExtResource({AgentObjectCodeID})");
            if (AgentType == 0)
            {
                Nodes[0].Type = ExportGodot.CharacterBody3D;
            }
            else
            {
                Nodes[0].Lines.Add($"mode=1"); // static
                Nodes[0].Lines.Add($"freeze=true"); // static
            }
            Nodes[0].Lines.Add($"Type={AgentType}");
            Nodes[0].Lines.Add($"UnkTypeValue={AgentUnkTypeValue}");
            Nodes[0].Lines.Add($"JointIDCount={AgentJointIDs}");
            Nodes[0].Lines.Add($"ExitPointCount={AgentExitPoints}");

            #region Template Resource
            
            //InternalResource TemplateRes = new InternalResource();
            //TemplateRes.Lines.Add($"script = ExtResource( { TemplateCodeID } )");
            //TemplateRes.Lines.Add($"Flags = { Agent.PUI32 }");
            //TemplateRes.Lines.Add($"Bitfield = { Agent.PHeader }");
            /*
            if (Agent.instIntegerList.Count != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"Regint = [ ");
                for (int a = 0; a < Agent.instIntegerList.Count - 1; a++)
                {
                    IntReg.Append($"{Agent.instIntegerList[a]}, ");
                }
                IntReg.Append($"{Agent.instIntegerList.Last()} ]");
                //TemplateRes.Lines.Add(IntReg.ToString());
            }
            if (Agent.instFlagsList.Count != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"RegAngle = [ ");
                for (int a = 0; a < Agent.instFlagsList.Count - 1; a++)
                {
                    IntReg.Append($"{Agent.instFlagsList[a]}, ");
                }
                IntReg.Append($"{Agent.instFlagsList.Last()} ]");
                //TemplateRes.Lines.Add(IntReg.ToString());
            }
            if (Agent.instFloatsList.Count != 0)
            {
                StringBuilder IntReg = new StringBuilder();
                IntReg.Append($"RegFloat = [ ");
                for (int a = 0; a < Agent.instFloatsList.Count - 1; a++)
                {
                    IntReg.Append($"{Agent.instFloatsList[a].ToText()}, ");
                }
                IntReg.Append($"{Agent.instFloatsList.Last().ToText()} ]");
                //TemplateRes.Lines.Add(IntReg.ToString());
            }
            */
            //InternalResourceList.Add(TemplateRes);
            //int TemplateResID = InternalResourceList.Count;
            
            #endregion

            #region Spawn Actions
            // todo
            /*
            List<int> SpawnActions = new List<int>();
            if (Agent.scriptCommandsAmount != 0)
            {
                for (int i = 0; i < Agent.scriptCommands.Count; i++)
                {
                    Script.MainScript.ScriptCommand Ptr = Agent.scriptCommands[i];

                    InternalResource ActionRes = new InternalResource();
                    ActionRes.Lines.Add($"script = ExtResource( {ALabActionCodeID} )");
                    ActionRes.Lines.Add($"ActionID = {Ptr.VTableIndex}");
                    if (Ptr.arguments.Count != 0)
                    {
                        StringBuilder ArgList = new StringBuilder();
                        ArgList.Append($"Arg = [ ");
                        for (int arg = 0; arg < Ptr.arguments.Count - 1; arg++)
                        {
                            ArgList.Append($"{Ptr.arguments[arg]}, ");
                        }
                        ArgList.Append($"{Ptr.arguments.Last()} ]");
                        ActionRes.Lines.Add(ArgList.ToString());
                    }
                    InternalResourceList.Add(ActionRes);
                    SpawnActions.Add(InternalResourceList.Count);
                }
            }
            */
            #endregion

            //Nodes[0].Lines.Add($"Template = SubResource( {TemplateResID} )");
            
            /*
            if (SpawnActions.Count != 0)
            {
                StringBuilder ActionsList = new StringBuilder();
                ActionsList.Append($"SpawnActions = [ ");
                for (int a = 0; a < SpawnActions.Count - 1; a++)
                {
                    ActionsList.Append($"SubResource( {SpawnActions[a]} ), ");
                }
                ActionsList.Append($"SubResource( {SpawnActions.Last()} ) ]");
                Nodes[0].Lines.Add(ActionsList.ToString());
            }
            */

            // optional attachment node
            Node ObjRootNode = new Node($"Children", ExportGodot.Node3D);
            ObjRootNode.KeyValues.Add("parent", ".");
            Nodes.Add(ObjRootNode);

            if (Agent.Objects.Count != 0)
            {
                List<int> ObjectList = new List<int>();
                for (int i = 0; i < Agent.Objects.Count; i++)
                {
                    if (Agent.Objects[i] != 65535 && Agent.Objects[i] != Agent.ID)
                    {
                        string ModelFilePath = $"../Actors/{DefaultHashes.ToName(SectionType.Object, Agent.Objects[i])}";
                        ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}.tscn");
                        ModelFileReference.SetAsPackedScene();
                        ExternalResourceList.Add(ModelFileReference);

                        // instantiate object (optional)
                        //Add_InstancedScene(ModelFilePath, ObjRootNode.Name);
                        //Nodes.Last().Lines.Add("visible = false");
                        //Nodes.Last().Lines.Add("process_mode = 4");

                        ObjectList.Add(ExternalResourceList.Count);
                    }
                }
                
                if (ObjectList.Count != 0)
                {
                    StringBuilder SubobjectList = new StringBuilder();
                    SubobjectList.Append($"SubActorsScenes = [ ");
                    for (int a = 0; a < ObjectList.Count - 1; a++)
                    {
                        SubobjectList.Append($"ExtResource({ObjectList[a]}), ");
                    }
                    SubobjectList.Append($"ExtResource({ObjectList.Last()}) ]");
                    Nodes[0].Lines.Add(SubobjectList.ToString());
                }
                
            }

            Dictionary<int, int> AnimList = new Dictionary<int, int>();
            if (Agent.Anims.Count != 0)
            {
                string Extension = ".res";
                for (int i = 0; i < Agent.Anims.Count; i++)
                {
                    if (Agent.Anims[i] != 65535 && !AnimList.ContainsKey(Agent.Anims[i]))
                    {
                        string AnimFilePath = $"../Animations/{DefaultHashes.ToName(SectionType.Animation, Agent.Anims[i])}";
                        ExternalResource AnimFileReference = new ExternalResource($"{AnimFilePath}{Extension}");
                        //AnimFileReference.SetAsAnimation();
                        ExternalResourceList.Add(AnimFileReference);

                        AnimList.Add(Agent.Anims[i], ExternalResourceList.Count);
                    }
                }

                /*
                if (AnimList.Count != 0)
                {
                    StringBuilder AnimListLine = new StringBuilder();
                    AnimListLine.Append($"Animations = [ ");
                    foreach (KeyValuePair<int, int> Pair in AnimList)
                    {
                        AnimListLine.Append($"ExtResource( {Pair.Value} ),");
                    }
                    AnimListLine.Remove(AnimListLine.Length - 1, 1);
                    AnimListLine.Append($" ]");
                    Nodes[0].Lines.Add(AnimListLine.ToString());
                }
                */
            }

            Dictionary<int, int> ModelList = new Dictionary<int, int>();
            Dictionary<int, int> OGI_IndexList = new Dictionary<int, int>();
            if (Agent.OGIs.Count != 0)
            {
                int OGI_Index = 0;
                // optional attachment node
                Node ModelRootNode = new Node($"Models", ExportGodot.Node3D);
                ModelRootNode.KeyValues.Add("parent", ".");
                Nodes.Add(ModelRootNode);
                
                for (int i = 0; i < Agent.OGIs.Count; i++)
                {
                    if (Agent.OGIs[i] != 65535 && !ModelList.ContainsKey(Agent.OGIs[i]))
                    {
                        string ModelFilePath = $"../Rigs/Rig_{DefaultHashes.ToName(SectionType.OGI, Agent.OGIs[i])}";
                        //ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}.tscn");
                        //ModelFileReference.SetAsPackedScene();
                        //ExternalResourceList.Add(ModelFileReference);

                        string Extension = ".res";
                        ExternalResource ResetAnimRef = new ExternalResource($"../Rigs/RigRESET_{DefaultHashes.ToName(SectionType.OGI, Agent.OGIs[i])}{Extension}");
                        ResetAnimRef.SetAsAnimation();
                        ExternalResourceList.Add(ResetAnimRef);
                        int ResetAnimRefID = ExternalResourceList.Count;

                        // instantiate model (optional)
                        Add_InstancedScene(ModelFilePath, ModelRootNode.Name);
                        OGI_IndexList.Add(Agent.OGIs[i], OGI_Index);
                        OGI_Index++;
                        if (i != 0)
                        {
                            Nodes.Last().Lines.Add("visible=false");
                            Nodes.Last().Lines.Add("process_mode=4");
                        }
                        // adding animation clips to player (optional)
                        List<(ushort, ushort)> OGI_AnimAdd = new List<(ushort, ushort)>();
                        Node AnimRefNode = null;
                        InternalResource AnimLib = null;
                        for (int a = 0; a < Agent.Anims.Count; a++)
                        {
                            if (Agent.Anims[a] != 65535 && Agent.OGIs[a] == Agent.OGIs[i] && !OGI_AnimAdd.Contains((Agent.Anims[a], Agent.OGIs[a])))
                            {
                                if (AnimRefNode == null)
                                {
                                    AnimLib = new InternalResource();
                                    AnimLib.Type = "AnimationLibrary";
                                    AnimLib.Lines.Add("_data = {");
                                    AnimLib.Lines.Add($"\"RESET\": ExtResource( {ResetAnimRefID} ),");
                                    AnimLib.Lines.Add("}");
                                    InternalResourceList.Add(AnimLib);
                                    AnimRefNode = new Node("AnimationPlayer");
                                    AnimRefNode.KeyValues.Add("index", "1");
                                    AnimRefNode.KeyValues.Add("parent", $"Models/Rig_{DefaultHashes.ToName(SectionType.OGI, Agent.OGIs[a])}");
                                    AnimRefNode.Lines.Add("libraries = {");
                                    AnimRefNode.Lines.Add($"\"\": SubResource( {InternalResourceList.Count} )");
                                    AnimRefNode.Lines.Add("}");
                                    Editables.Add($"Models/Rig_{DefaultHashes.ToName(SectionType.OGI, Agent.OGIs[a])}");
                                }
                                else
                                {
                                    AnimLib.Lines[AnimLib.Lines.Count - 2] = AnimLib.Lines[AnimLib.Lines.Count - 2] + ",";
                                }
                                AnimLib.Lines.Insert(AnimLib.Lines.Count - 1, $"\"{DefaultHashes.ToName(SectionType.Animation, Agent.Anims[a])}\": ExtResource( {AnimList[Agent.Anims[a]]} )");
                                OGI_AnimAdd.Add((Agent.Anims[a], Agent.OGIs[a]));
                            }
                        }
                        if (AnimRefNode != null)
                        {
                            Nodes.Add(AnimRefNode);
                        }

                        ModelList.Add(Agent.OGIs[i], ExternalResourceList.Count);
                    }
                }

                /*
                if (ModelList.Count != 0)
                {
                    StringBuilder OGIList = new StringBuilder();
                    OGIList.Append($"OGIs = [ ");
                    foreach (KeyValuePair<int, int> Pair in ModelList)
                    {
                        OGIList.Append($"ExtResource( {Pair.Value} ),");
                    }
                    OGIList.Remove(OGIList.Length - 1, 1);
                    OGIList.Append($" ]");
                    Nodes[0].Lines.Add(OGIList.ToString());
                }
                */
            }

            List<int> CustomAgentScripts = new List<int>();
            if (Agent.cCM.Count != 0)
            {
                // todo
                /*
                List<int> CustomAgentIDs = new List<int>();
                for (int i = 0; i < Agent.cCM.Count; i++)
                {
                    ExternalResource ScriptRes = new ExternalResource($"../CustomAgents/{DefaultHashes.ToName(SectionType.CustomAgent, Agent.cCM[i])}.tres");
                    ExternalResourceList.Add(ScriptRes);
                    CustomAgentIDs.Add(ExternalResourceList.Count);

                    CustomAgent CA_Cont = ca_sec.GetItem<CustomAgent>(Agent.cCM[i]);
                    for (int a = 0; a < CA_Cont.scriptIds.Count; a++)
                    {
                        CustomAgentScripts.Add(CA_Cont.scriptIds[a]);
                    }
                }

                StringBuilder CustomAgentList = new StringBuilder();
                CustomAgentList.Append($"CustomAgents = [ ");
                for (int a = 0; a < CustomAgentIDs.Count - 1; a++)
                {
                    CustomAgentList.Append($"ExtResource( {CustomAgentIDs[a]} ), ");
                }
                CustomAgentList.Append($"ExtResource( {CustomAgentIDs.Last()} ) ]");
                Nodes[0].Lines.Add(CustomAgentList.ToString());
                */
            }

            if (Agent.Sounds.Count != 0)
            {
                string Extension = ".res";
                Dictionary<int, int> SoundIDs = new Dictionary<int, int>();
                for (int i = 0; i < Agent.Sounds.Count; i++)
                {
                    if (Agent.Sounds[i] != 65535 && !SoundIDs.ContainsKey(Agent.Sounds[i]))
                    {
                        ExternalResource SoundRes = new ExternalResource($"../Sounds/{DefaultHashes.ToName(SectionType.SE, Agent.Sounds[i])}{Extension}");
                        SoundRes.SetAsAudio();
                        ExternalResourceList.Add(SoundRes);
                        SoundIDs.Add(Agent.Sounds[i], ExternalResourceList.Count);
                    }
                }

                StringBuilder AudioList = new StringBuilder();
                AudioList.Append($"Sounds = [ ");
                for (int a = 0; a < Agent.Sounds.Count - 1; a++)
                {
                    if (Agent.Sounds[a] == 65535)
                    {
                        AudioList.Append($"null, ");
                    }
                    else
                    {
                        AudioList.Append($"ExtResource( {SoundIDs[Agent.Sounds[a]]} ), ");
                    }
                }
                if (Agent.Sounds.Last() == 65535)
                {
                    AudioList.Append($"null ]");
                }
                else
                {
                    AudioList.Append($"ExtResource( {SoundIDs[Agent.Sounds.Last()]} ) ]");
                }
                Nodes[0].Lines.Add(AudioList.ToString());
            }

            /*
            if (ExportGodot.ExportScripts)
            {
                Dictionary<int, int> ScriptIDs = new Dictionary<int, int>();
                if (Agent.Scripts.Count != 0)
                {
                    for (int i = 0; i < Agent.Scripts.Count; i++)
                    {
                        if (Agent.Scripts[i] != 65535 && !ScriptIDs.ContainsKey(Agent.Scripts[i]))
                        {
                            AddObjectScript(Agent, scr_sec, Agent.Scripts[i]);
                            ScriptIDs.Add(Agent.Scripts[i], InternalResourceList.Count);
                        }
                    }

                    StringBuilder AudioList = new StringBuilder();
                    AudioList.Append($"Scripts = [ ");
                    for (int a = 0; a < Agent.Scripts.Count - 1; a++)
                    {
                        if (Agent.Scripts[a] == 65535)
                        {
                            AudioList.Append($"null, ");
                        }
                        else
                        {
                            AudioList.Append($"SubResource( {ScriptIDs[Agent.Scripts[a]]} ), ");
                        }
                    }
                    if (Agent.Scripts.Last() == 65535)
                    {
                        AudioList.Append($"null ]");
                    }
                    else
                    {
                        AudioList.Append($"SubResource( {ScriptIDs[Agent.Scripts.Last()]} ) ]");
                    }
                    Nodes[0].Lines.Add(AudioList.ToString());
                }
                if (Agent.UI32.Count != 0)
                {
                    StringBuilder MessList = new StringBuilder();
                    MessList.Append("Messages = { ");           
                    for (int i = 0; i < Agent.UI32.Count; i++)
                    {
                        uint Message = Agent.UI32[i];
                        ushort script = (ushort)((Message >> 0xA) & 0x3FFF);
                        ushort arg = (ushort)(Message & 0x3FF);
                        ushort caller = (ushort)((Message >> 0x18 & 0x1));

                        if (script != 65535 && !ScriptIDs.ContainsKey(script))
                        {
                            AddObjectScript(Agent, scr_sec, script);
                            ScriptIDs.Add(script, InternalResourceList.Count);
                        }
                        MessList.Append($"{arg} : SubResource( {ScriptIDs[script]} ), "); 
                        
                    }
                    MessList.Remove(MessList.Length - 2, 2);
                    MessList.Append("}");  
                    Nodes[0].Lines.Add(MessList.ToString());
                }
            }
            */

            if (Agent.OGIs.Count != 0 && Agent.Anims.Count != 0 && AnimList.Count != 0 && ModelList.Count != 0)
            {
                
                Nodes[0].Lines.Add("ModelActions = [ { ");
                for (int i = 0; i < Agent.OGIs.Count - 1; i++)
                {
                    StringBuilder ModelActionList = new StringBuilder();
                    if (Agent.OGIs[i] != 65535)
                    {
                        ModelActionList.Append($"{OGI_IndexList[Agent.OGIs[i]]}");
                    }
                    else
                    {
                        ModelActionList.Append($"null");
                    }
                    ModelActionList.Append($": ");
                    if (Agent.Anims[i] != 65535)
                    {
                        var animName = DefaultHashes.ToName(SectionType.Animation, Agent.Anims[i]);
                        ModelActionList.Append($"\"{animName}\"");
                    }
                    else
                    {
                        ModelActionList.Append($"null");
                    }
                    Nodes[0].Lines.Add(ModelActionList.ToString());
                    Nodes[0].Lines.Add("}, {");
                }
                if (Agent.OGIs.Last() != 65535)
                {
                    if (Agent.Anims.Last() != 65535)
                    {
                        var animName = DefaultHashes.ToName(SectionType.Animation, Agent.Anims.Last());
                        Nodes[0].Lines.Add($"{OGI_IndexList[Agent.OGIs.Last()]}: \"{animName}\"");
                    }
                    else
                    {
                        Nodes[0].Lines.Add($"{OGI_IndexList[Agent.OGIs.Last()]}: null");
                    }
                }
                else
                {
                    if (Agent.Anims.Last() != 65535)
                    {
                        var animName = DefaultHashes.ToName(SectionType.Animation, Agent.Anims.Last());
                        Nodes[0].Lines.Add($"null: \"{animName}\"");
                    }
                    else
                    {
                        Nodes[0].Lines.Add($"null: null");
                    }
                }
                Nodes[0].Lines.Add("} ]");
            }

            // optional shadow attachment node
            //Node ShadowsRootNode = new Node($"Shadows", ExportGodot.Node3D);
            //ShadowsRootNode.KeyValues.Add("parent", ".");
            //Nodes.Add(ShadowsRootNode);

            // not required, but might aswell
            //Node AudioNode = new Node($"AudioStreamPlayer3D", "AudioStreamPlayer3D");
            //AudioNode.KeyValues.Add("parent", ".");
            //Nodes.Add(AudioNode);
        }

        void AddObjectScript(GameObject Agent, TwinsSection scr_sec, ushort scriptID)
        {
            /*
            uint TargetScript = scriptID;
            Script target = null;
            bool isStarter = false;
            bool isCustom = false;
            if (DefaultHashes.CustomAgentScripts.Contains(scriptID))
            {
                isCustom = true;
            }
            if (!isCustom && TargetScript % 2 == 0)
            {
                target = scr_sec.GetItem<Script>(TargetScript);
                TargetScript = (uint)(target.Header.pairs[0].mainScriptIndex - 1);
                isStarter = true;
            }
            string hashName = DefaultHashes.Hash_Scripts[TargetScript];
            if (isCustom)
            {
                hashName = $"{DefaultHashes.ToName(SectionType.Object, Agent.ID)}_{hashName}";
            }
            ExternalResource ScriptCode = new ExternalResource($"../Scripts/{hashName}{ExportGodot.ScriptExt}");
            ScriptCode.SetAsScript();
            ExternalResourceList.Add(ScriptCode);
            InternalResource ScriptRes = new InternalResource();
            ScriptRes.Lines.Add($"script = ExtResource( {ExternalResourceList.Count} )");
            if (isStarter)
            {
                ScriptRes.Lines.Add($"Priority = {target.mask}");
                if (target.Header.pairs.Count > 1)
                {
                    StringBuilder PartList = new StringBuilder();
                    PartList.Append($"ParticipantTypes = [ ");
                    for (int a = 1; a < target.Header.pairs.Count - 1; a++)
                    {
                        var partType = 0;
                        if (target.Header.pairs[a].AssignType == Script.HeaderScript.AssignTypeID.HUMAN_PLAYER)
                        {
                            partType = 1;
                        }
                        else if (target.Header.pairs[a].AssignType == Script.HeaderScript.AssignTypeID.ORIGINATOR)
                        {
                            partType = 2;
                        }
                        else if (target.Header.pairs[a].AssignType == Script.HeaderScript.AssignTypeID.GLOBAL_AGENT)
                        {
                            partType = 3;
                            if (target.Header.pairs[a].ObjectID != 0)
                            {
                                partType = 4;
                            }
                        }
                        PartList.Append($"{partType}, ");
                    }
                    var partTypeA = 0;
                    if (target.Header.pairs.Last().AssignType == Script.HeaderScript.AssignTypeID.HUMAN_PLAYER)
                    {
                        partTypeA = 1;
                    }
                    else if (target.Header.pairs.Last().AssignType == Script.HeaderScript.AssignTypeID.ORIGINATOR)
                    {
                        partTypeA = 2;
                    }
                    else if (target.Header.pairs.Last().AssignType == Script.HeaderScript.AssignTypeID.GLOBAL_AGENT)
                    {
                        partTypeA = 3;
                        if (target.Header.pairs.Last().ObjectID != 0)
                        {
                            partTypeA = 4;
                        }
                    }
                    PartList.Append($"{partTypeA} ]");
                    ScriptRes.Lines.Add(PartList.ToString());
                }
            }
            InternalResourceList.Add(ScriptRes);
            */
        }

        public void AddAIPositions(TwinsSection Section, uint SectionID, string RootNodeName)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                AIPosition Pos = (AIPosition)Section.Records[i];

                Node PosNode = new Node($"AI_Node_{SectionID}_{i}", ExportGodot.Marker3D);
                PosNode.KeyValues.Add("parent", RootNodeName);
                //PosNode.Lines.Add($"script = ExtResource( {CodeResourceID_Container_AIPathNode} )");
                PosNode.Lines.Add($"{ExportGodot.transformPosition}=Vector3({(-Pos.Pos.X).ToText()},{Pos.Pos.Y.ToText()},{Pos.Pos.Z.ToText()})");
                PosNode.Lines.Add($"Weight={Pos.Pos.W.ToText()}");
                PosNode.Lines.Add($"Type={(ushort)Pos.Node}");
                Nodes.Add(PosNode);
            }
        }
        public void AddAIPath(TwinsSection Section, TwinsSection PositionSection, uint SectionID, string RootNodeName)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                AIPath Pos = (AIPath)Section.Records[i];

                AIPosition Pos1 = PositionSection.GetItem<AIPosition>(Pos.Arg[0]);
                AIPosition Pos2 = PositionSection.GetItem<AIPosition>(Pos.Arg[1]);

                InternalResource Curve = new InternalResource();
                Curve.CreateCurve3D();
                Curve.Lines.Add("_data = {");
                Curve.Lines.Add($"\"points\": PoolVector3Array(0,0,0,0,0,0,{(-Pos1.Pos.X).ToText()}," +
                    $"{Pos1.Pos.Y.ToText()},{Pos1.Pos.Z.ToText()},0,0,0,0,0,0," +
                    $"{(-Pos2.Pos.X).ToText()},{Pos2.Pos.Y.ToText()},{Pos2.Pos.Z.ToText()}),");
                Curve.Lines.Add("\"tilts\": PoolRealArray(0,0)");
                Curve.Lines.Add("}");
                InternalResourceList.Add(Curve);

                Node PosNode = new Node($"AI_Path_{SectionID}_{i}", ExportGodot.Path3D);
                PosNode.KeyValues.Add("parent", RootNodeName);
                //PosNode.Lines.Add($"script = ExtResource( {CodeResourceID_Container_AIPath} )");
                PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");
                PosNode.Lines.Add($"Param1={Pos.Arg[2]}");
                PosNode.Lines.Add($"Param2={Pos.Arg[3]}");
                PosNode.Lines.Add($"Param3={Pos.Arg[4]}");
                Nodes.Add(PosNode);
            }

        }
        public void AddPosition(TwinsSection Section, uint SectionID, string RootNodeName)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                Position Pos = (Position)Section.Records[i];

                Node PosNode = new Node($"Point_{SectionID}_{i}", ExportGodot.Marker3D);
                PosNode.KeyValues.Add("parent", RootNodeName);
                PosNode.Lines.Add($"{ExportGodot.transformPosition}=Vector3({(-Pos.Pos.X).ToText()},{Pos.Pos.Y.ToText()},{Pos.Pos.Z.ToText()})");
                Nodes.Add(PosNode);
            }
        }
        public void AddPath(TwinsSection Section, TwinsSection PositionSection, uint SectionID, string RootNodeName)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                Twinsanity.Path Pos = (Twinsanity.Path)Section.Records[i];

                InternalResource Curve = new InternalResource();
                Curve.CreateCurve3D();
                Curve.Lines.Add("_data = {");

                string points = string.Empty;
                string tilts = string.Empty;
                for (int a = 0; a < Pos.Positions.Count - 1; a++)
                {
                    points += $"0,0,0,0,0,0,{(-Pos.Positions[a].X).ToText()}," +
                        $"{Pos.Positions[a].Y.ToText()}," +
                        $"{Pos.Positions[a].Z.ToText()},";
                    tilts += "0,";
                }
                points += $"0,0,0,0,0,0,{(-Pos.Positions[Pos.Positions.Count - 1].X).ToText()}," +
                    $"{Pos.Positions[Pos.Positions.Count - 1].Y.ToText()}," +
                    $"{Pos.Positions[Pos.Positions.Count - 1].Z.ToText()}";
                Curve.Lines.Add($"\"points\": PoolVector3Array({points}),");
                Curve.Lines.Add($"\"tilts\": PoolRealArray({tilts}0)");
                Curve.Lines.Add("}");
                InternalResourceList.Add(Curve);

                Node PosNode = new Node($"Path_{SectionID}_{i}", ExportGodot.Path3D);
                PosNode.KeyValues.Add("parent", RootNodeName);
                //PosNode.Lines.Add($"script = ExtResource( {CodeResourceID_Container_AgentPath} )");
                PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");

                if (Pos.Params.Count != 0)
                {
                    StringBuilder PathParamList = new StringBuilder();
                    PathParamList.Append($"Params=[");
                    for (int a = 0; a < Pos.Params.Count - 1; a++)
                    {
                        PathParamList.Append($"{Pos.Params[a].P1.ToText()},{Pos.Params[a].P2.ToText()},");
                    }
                    PathParamList.Append($"{Pos.Params.Last().P1.ToText()},{Pos.Params.Last().P2.ToText()}]");
                    PosNode.Lines.Add(PathParamList.ToString());
                }

                Nodes.Add(PosNode);
            }

        }
        public void AddTrigger(TwinsSection Section, uint SectionID, string RootNodeName)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                Trigger Pos = (Trigger)Section.Records[i];

                InternalResource LinkShapeData = new InternalResource();
                LinkShapeData.CreateBoxShape(Pos.Coords[2].X, Pos.Coords[2].Y, Pos.Coords[2].Z);
                InternalResourceList.Add(LinkShapeData);

                Node TrigNode = new Node($"Trigger_{SectionID}_{i}", ExportGodot.Area3D);
                TrigNode.Groups.Add($"InstanceLayer{SectionID}");
                TrigNode.KeyValues.Add("parent", $"{RootNodeName}");

                TrigNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Pos.Coords[1].X).ToText()}, {Pos.Coords[1].Y.ToText()}, {Pos.Coords[1].Z.ToText()} )");

                Pos TriggerRot = Pos.Coords[0];
                TriggerRot.X = (float)(TriggerRot.X * Math.PI);
                TriggerRot.Y = (float)(TriggerRot.Y * Math.PI);
                TriggerRot.Z = (float)(TriggerRot.Z * Math.PI);

                TrigNode.Lines.Add($"rotation = Vector3( {TriggerRot.X.ToText()}, {(TriggerRot.Y).ToText()}, {(TriggerRot.Z).ToText()} )"); // or quaternion with W?

                TrigNode.Lines.Add($"script = ExtResource ( {CodeResourceID_Container_Trigger} )");
                TrigNode.Lines.Add("Messages = {");
                TrigNode.Lines.Add($"{Pos.Arg1}: {Pos.Arg1_Used.ToString().ToLower()},");
                TrigNode.Lines.Add($"{Pos.Arg2}: {Pos.Arg2_Used.ToString().ToLower()},");
                TrigNode.Lines.Add($"{Pos.Arg3}: {Pos.Arg3_Used.ToString().ToLower()},");
                TrigNode.Lines.Add($"{Pos.Arg4}: {Pos.Arg4_Used.ToString().ToLower()}");
                TrigNode.Lines.Add("}");

                bool[] TrigMask = Pos.Mask;
                StringBuilder MaskLine = new StringBuilder();
                MaskLine.Append($"Mask = [ ");
                for (int a = 0; a < TrigMask.Length - 1; a++)
                {
                    MaskLine.Append($"{TrigMask[a].ToString().ToLower()}, ");
                }
                MaskLine.Append($"{TrigMask.Last().ToString().ToLower()} ]");
                TrigNode.Lines.Add(MaskLine.ToString());

                if (Pos.Instances.Count != 0)
                {
                    StringBuilder TrigRefs = new StringBuilder();
                    TrigRefs.Append($"InstanceRefs = [ ");
                    for (int a = 0; a < Pos.Instances.Count - 1; a++)
                    {
                        TrigRefs.Append($"NodePath(\"../../../Instances/Instance_{SectionID}_{Pos.Instances[a]}\"), ");
                    }
                    TrigRefs.Append($"NodePath(\"../../../Instances/Instance_{SectionID}_{Pos.Instances.Last()}\") ]");
                    TrigNode.Lines.Add(TrigRefs.ToString());
                }

                TrigNode.Lines.Add($"SomeFloat = {Pos.SomeFloat.ToText()}");
                TrigNode.Lines.Add($"SectionHead = {Pos.SectionHead}");

                Nodes.Add(TrigNode);

                Node TrigColNode = new Node($"TriggerCollision", ExportGodot.CollisionShape3D);
                TrigColNode.KeyValues.Add("parent", $"{RootNodeName}/{TrigNode.Name}");
                TrigColNode.Lines.Add($"shape = SubResource( {InternalResourceList.Count} )");
                Nodes.Add(TrigColNode);
            }
        }
        public void AddCamera(TwinsSection Section, uint SectionID, string RootNodeName)
        {
            // todo: camera data

            for (int i = 0; i < Section.Records.Count; i++)
            {
                Camera Pos = (Camera)Section.Records[i];

                InternalResource TrigShapeData = new InternalResource();
                TrigShapeData.CreateBoxShape(Pos.Coords[2].X, Pos.Coords[2].Y, Pos.Coords[2].Z);
                InternalResourceList.Add(TrigShapeData);

                Node TrigNode = new Node($"CameraTrigger_{SectionID}_{i}", ExportGodot.Area3D);
                TrigNode.Groups.Add($"InstanceLayer{SectionID}");
                TrigNode.KeyValues.Add("parent", $"{RootNodeName}");

                TrigNode.Lines.Add($"script = ExtResource ( {CodeResourceID_Container_Camera} )");

                if (Pos.Instances.Count != 0)
                {
                    StringBuilder TrigRefs = new StringBuilder();
                    TrigRefs.Append($"InstanceRefs = [ ");
                    for (int a = 0; a < Pos.Instances.Count - 1; a++)
                    {
                        TrigRefs.Append($"NodePath(\"../../../Instances/Instance_{SectionID}_{Pos.Instances[a]}\"), ");
                    }
                    TrigRefs.Append($"NodePath(\"../../../Instances/Instance_{SectionID}_{Pos.Instances.Last()}\") ]");
                    TrigNode.Lines.Add(TrigRefs.ToString());
                }

                TrigNode.Lines.Add($"SomeFloat = {Pos.SomeFloat.ToText()}");
                TrigNode.Lines.Add($"SectionHead = {Pos.SectionHead}");

                bool[] TrigMask = Pos.Mask;
                StringBuilder MaskLine = new StringBuilder();
                MaskLine.Append($"Mask = [ ");
                for (int a = 0; a < TrigMask.Length - 1; a++)
                {
                    MaskLine.Append($"{TrigMask[a].ToString().ToLower()}, ");
                }
                MaskLine.Append($"{TrigMask.Last().ToString().ToLower()} ]");
                TrigNode.Lines.Add(MaskLine.ToString());

                TrigNode.Lines.Add($"Camera1Type = {Pos.CameraType1}");
                TrigNode.Lines.Add($"Camera2Type = {Pos.CameraType2}");
                TrigNode.Lines.Add($"CamHeader = {Pos.CamHeader}");
                TrigNode.Lines.Add($"CamHeader2 = {Pos.CamHeader2}");
                TrigNode.Lines.Add($"UnkShort = {Pos.UnkShort}");

                TrigNode.Lines.Add($"UnkFloat1 = {Pos.UnkFloat1.ToText()}");
                if ((Pos.CamHeader & (1 << 8)) != 0 || (Pos.CamHeader & (1 << 28)) != 0)
                {
                    TrigNode.Lines.Add($"UnkCoords1 = Vector2( {Pos.UnkCoords1.X.ToText()}, {Pos.UnkCoords1.Y.ToText()} )");
                    TrigNode.Lines.Add($"UnkCoords2 = Vector2( {Pos.UnkCoords1.Z.ToText()}, {Pos.UnkCoords1.W.ToText()} )");
                    TrigNode.Lines.Add($"UnkCoords3 = Vector2( {Pos.UnkCoords2.X.ToText()}, {Pos.UnkCoords2.Y.ToText()} )");
                    TrigNode.Lines.Add($"UnkCoords4 = Vector2( {Pos.UnkCoords2.Z.ToText()}, {Pos.UnkCoords2.W.ToText()} )");
                }
                if ((Pos.CamHeader & (1 << 9)) != 0 || (Pos.CamHeader & (1 << 10)) != 0)
                {
                    TrigNode.Lines.Add($"UnkFloat2 = {Pos.UnkFloat2.GetValueOrDefault().ToText()}");
                    TrigNode.Lines.Add($"UnkFloat3 = {Pos.UnkFloat3.GetValueOrDefault().ToText()}");
                }
                if ((Pos.CamHeader & (1 << 7)) != 0)
                {
                    TrigNode.Lines.Add($"UnkUInt1 = {Pos.UnkUInt1.GetValueOrDefault()}");
                    TrigNode.Lines.Add($"UnkUInt2 = {Pos.UnkUInt2.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 2)) != 0)
                {
                    TrigNode.Lines.Add($"UnkUInt3 = {Pos.UnkUInt3.GetValueOrDefault()}");
                    TrigNode.Lines.Add($"UnkUInt4 = {Pos.UnkUInt4.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 6)) != 0)
                {
                    TrigNode.Lines.Add($"UnkInt5 = {Pos.UnkInt5.GetValueOrDefault()}");
                    TrigNode.Lines.Add($"UnkInt6 = {Pos.UnkInt6.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 3)) != 0)
                {
                    TrigNode.Lines.Add($"UnkFloat4 = {Pos.UnkFloat4.GetValueOrDefault().ToText()}");
                    TrigNode.Lines.Add($"UnkFloat5 = {Pos.UnkFloat5.GetValueOrDefault().ToText()}");
                }
                if ((Pos.CamHeader & (1 << 12)) != 0)
                {
                    TrigNode.Lines.Add($"UnkFloat6 = {Pos.UnkFloat6.GetValueOrDefault().ToText()}");
                }
                if ((Pos.CamHeader & (1 << 13)) != 0)
                {
                    TrigNode.Lines.Add($"UnkFloat7 = {Pos.UnkFloat7.GetValueOrDefault().ToText()}");
                }
                if ((Pos.CamHeader & (1 << 15)) != 0)
                {
                    TrigNode.Lines.Add($"UnkUInt7 = {Pos.UnkUInt7.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 16)) != 0)
                {
                    TrigNode.Lines.Add($"UnkInt8 = {Pos.UnkInt8.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 17)) != 0)
                {
                    TrigNode.Lines.Add($"UnkUInt9 = {Pos.UnkUInt9.GetValueOrDefault()}");
                }
                if ((Pos.CamHeader & (1 << 18)) != 0)
                {
                    TrigNode.Lines.Add($"UnkFloat8 = {Pos.UnkFloat8.GetValueOrDefault().ToText()}");
                }

                TrigNode.Lines.Add($"UnkByte = {Pos.UnkByte}");
                

                Nodes.Add(TrigNode);

                Node TrigColNode = new Node($"TriggerCollision", ExportGodot.CollisionShape3D);
                TrigColNode.KeyValues.Add("parent", $"{RootNodeName}/{TrigNode.Name}");
                TrigColNode.Lines.Add($"{ExportGodot.transformPosition} = Vector3( {(-Pos.Coords[1].X).ToText()}, {Pos.Coords[1].Y.ToText()}, {Pos.Coords[1].Z.ToText()} )");

                Pos TriggerRot = Pos.Coords[0];
                TriggerRot.X = (float)(TriggerRot.X * Math.PI);
                TriggerRot.Y = (float)(TriggerRot.Y * Math.PI);
                TriggerRot.Z = (float)(TriggerRot.Z * Math.PI);

                TrigColNode.Lines.Add($"rotation = Vector3( {TriggerRot.X.ToText()}, {(TriggerRot.Y).ToText()}, {(TriggerRot.Z).ToText()} )"); // or quaternion with W?
                TrigColNode.Lines.Add($"shape = SubResource( {InternalResourceList.Count} )");
                Nodes.Add(TrigColNode);

                AddCameraData($"{RootNodeName}/{TrigNode.Name}", (Camera.CameraType)Pos.CameraType1, Pos.Cameras[0], 0);
                AddCameraData($"{RootNodeName}/{TrigNode.Name}", (Camera.CameraType)Pos.CameraType2, Pos.Cameras[1], 1);

            }
        }
        void AddCameraData(string RootNodeName, Camera.CameraType CamType, object CamObject, int camID)
        {
            switch (CamType)
            {
                default:
                    break;
                case Camera.CameraType.Point:
                    {
                        Camera.Camera_Point CamPoint = (Camera.Camera_Point)CamObject;
                        Node PointNode = new Node($"Cam{camID}_CameraPoint", ExportGodot.Marker3D);
                        PointNode.Lines.Add($"{ExportGodot.transformPosition}=Vector3({(-CamPoint.unkVector.X).ToText()},{CamPoint.unkVector.Y.ToText()},{CamPoint.unkVector.Z.ToText()})");
                        PointNode.KeyValues.Add("parent", $"{RootNodeName}");
                        Nodes.Add(PointNode);
                        break;
                    }
                case Camera.CameraType.Point2:
                    {
                        Camera.Camera_Point2 CamPoint = (Camera.Camera_Point2)CamObject;
                        Node PointNode = new Node($"Cam{camID}_CameraPoint2", ExportGodot.Marker3D);
                        PointNode.Lines.Add($"{ExportGodot.transformPosition}=Vector3({(-CamPoint.unkVector.X).ToText()},{CamPoint.unkVector.Y.ToText()},{CamPoint.unkVector.Z.ToText()})");
                        PointNode.KeyValues.Add("parent", $"{RootNodeName}");
                        Nodes.Add(PointNode);
                        break;
                    }
                case Camera.CameraType.Line:
                    {
                        Camera.Camera_Line CamLine = (Camera.Camera_Line)CamObject;
                        InternalResource Curve = new InternalResource();
                        Curve.CreateCurve3D();
                        Curve.Lines.Add("_data = {");
                        Curve.Lines.Add($"\"points\": PoolVector3Array(0,0,0,0,0,0,{(-CamLine.unkBoundingBoxVector1.X).ToText()}," +
                            $"{CamLine.unkBoundingBoxVector1.Y.ToText()},{CamLine.unkBoundingBoxVector1.Z.ToText()},0,0,0,0,0,0," +
                            $"{(-CamLine.unkBoundingBoxVector2.X).ToText()},{CamLine.unkBoundingBoxVector2.Y.ToText()},{CamLine.unkBoundingBoxVector2.Z.ToText()}),");
                        Curve.Lines.Add("\"tilts\": PoolRealArray(0,0)");
                        Curve.Lines.Add("}");
                        InternalResourceList.Add(Curve);

                        Node PosNode = new Node($"Cam{camID}_CameraLine", ExportGodot.Path3D);
                        PosNode.KeyValues.Add("parent", $"{RootNodeName}");
                        PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");
                        Nodes.Add(PosNode);
                        break;
                    }
                case Camera.CameraType.Line2:
                    {
                        Camera.Camera_Line2 CamLine = (Camera.Camera_Line2)CamObject;
                        InternalResource Curve = new InternalResource();
                        Curve.CreateCurve3D();
                        Curve.Lines.Add("_data = {");
                        Curve.Lines.Add($"\"points\": PoolVector3Array(0,0,0,0,0,0,{(-CamLine.unkBoundingBoxVector1.X).ToText()}," +
                            $"{CamLine.unkBoundingBoxVector1.Y.ToText()},{CamLine.unkBoundingBoxVector1.Z.ToText()},0,0,0,0,0,0," +
                            $"{(-CamLine.unkBoundingBoxVector2.X).ToText()},{CamLine.unkBoundingBoxVector2.Y.ToText()},{CamLine.unkBoundingBoxVector2.Z.ToText()}),");
                        Curve.Lines.Add("\"tilts\": PoolRealArray(0,0)");
                        Curve.Lines.Add("}");
                        InternalResourceList.Add(Curve);

                        Node PosNode = new Node($"Cam{camID}_CameraLine", ExportGodot.Path3D);
                        PosNode.KeyValues.Add("parent", $"{RootNodeName}");
                        PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");
                        Nodes.Add(PosNode);
                        break;
                    }
                case Camera.CameraType.Path:
                    {
                        Camera.Camera_Path CamPath = (Camera.Camera_Path)CamObject;

                        InternalResource Curve = new InternalResource();
                        Curve.CreateCurve3D();
                        Curve.Lines.Add("_data = {");

                        string points = string.Empty;
                        string tilts = string.Empty;
                        for (int a = 0; a < CamPath.unkVectors.Length - 1; a++)
                        {
                            points += $"0,0,0,0,0,0,{(-CamPath.unkVectors[a].X).ToText()}," +
                                $"{CamPath.unkVectors[a].Y.ToText()}," +
                                $"{CamPath.unkVectors[a].Z.ToText()},";
                            tilts += "0,";
                        }
                        points += $"0,0,0,0,0,0,{(-CamPath.unkVectors[CamPath.unkVectors.Length - 1].X).ToText()}," +
                            $"{CamPath.unkVectors[CamPath.unkVectors.Length - 1].Y.ToText()}," +
                            $"{CamPath.unkVectors[CamPath.unkVectors.Length - 1].Z.ToText()} ";
                        Curve.Lines.Add($"\"points\": PoolVector3Array({points}),");
                        Curve.Lines.Add($"\"tilts\": PoolRealArray({tilts}0)");
                        Curve.Lines.Add("}");
                        InternalResourceList.Add(Curve);

                        Node PosNode = new Node($"Cam{camID}_CameraPath", ExportGodot.Path3D);
                        PosNode.KeyValues.Add("parent", $"{RootNodeName}");
                        PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");
                        Nodes.Add(PosNode);
                        break;
                    }
                case Camera.CameraType.Spline:
                    {
                        // todo every other vector is a rotation
                        Camera.Camera_Spline CamPath = (Camera.Camera_Spline)CamObject;

                        InternalResource Curve = new InternalResource();
                        Curve.CreateCurve3D();
                        Curve.Lines.Add("_data = {");

                        string points = string.Empty;
                        string tilts = string.Empty;
                        for (int a = 0; a < CamPath.unkVectors.Length - 2; a++)
                        {
                            if (a % 2 == 0)
                            {
                                points += $"0,0,0,0,0,0,{(-CamPath.unkVectors[a].X).ToText()}," +
                                    $"{CamPath.unkVectors[a].Y.ToText()}," +
                                    $"{CamPath.unkVectors[a].Z.ToText()},";
                                tilts += "0,";
                            }
                        }
                        points += $"0,0,0,0,0,0,{(-CamPath.unkVectors[CamPath.unkVectors.Length - 2].X).ToText()}," +
                            $"{CamPath.unkVectors[CamPath.unkVectors.Length - 2].Y.ToText()}," +
                            $"{CamPath.unkVectors[CamPath.unkVectors.Length - 2].Z.ToText()} ";
                        Curve.Lines.Add($"\"points\": PoolVector3Array({points}),");
                        Curve.Lines.Add($"\"tilts\": PoolRealArray({tilts}0)");
                        Curve.Lines.Add("}");
                        InternalResourceList.Add(Curve);

                        Node PosNode = new Node($"Cam{camID}_CameraSpline", ExportGodot.Path3D);
                        PosNode.KeyValues.Add("parent", $"{RootNodeName}");
                        PosNode.Lines.Add($"curve=SubResource({InternalResourceList.Count})");
                        Nodes.Add(PosNode);
                        break;
                    }
                case Camera.CameraType.Zone:
                    {
                        Camera.Camera_Zone CamPath = (Camera.Camera_Zone)CamObject;
                        break;
                    }
                case Camera.CameraType.Boss:
                    {
                        Camera.Camera_Boss CamPath = (Camera.Camera_Boss)CamObject;
                        break;
                    }
            }
        }

        public void AddInstance(TwinsSection Section, uint SectionID, Dictionary<int, int> ImportedObjects, bool AllowGlobal, string RootNodeName, TwinsSection PositionSection, TwinsSection PathSection, Dictionary<uint, int> PathsExported)
        {
            for (int i = 0; i < Section.Records.Count; i++)
            {
                Instance Inst = (Instance)Section.Records[i];

                Node HolderNode = new Node($"Instance_{SectionID}_{i}", ExportGodot.Marker3D);
                HolderNode.KeyValues.Add("parent", $"{RootNodeName}");
                HolderNode.Groups.Add($"InstanceLayer{SectionID}");

                HolderNode.Lines.Add($"script=ExtResource({CodeResourceID_Container_Instance})");

                int PrefabResID = -1;
                if (ImportedObjects.ContainsKey(Inst.ObjectID))
                {
                    PrefabResID = ImportedObjects[Inst.ObjectID];
                }
                else
                {
                    ExternalResource PrefabRes = new ExternalResource($"../Actors/{DefaultHashes.ToName(SectionType.Object, Inst.ObjectID)}{ExportGodot.SceneExtension}");
                    PrefabRes.SetAsPackedScene();
                    ExternalResourceList.Add(PrefabRes);
                    PrefabResID = ExternalResourceList.Count;
                    ImportedObjects.Add(Inst.ObjectID, PrefabResID);
                }
                HolderNode.Lines.Add($"Prefab=ExtResource({PrefabResID})");

                if (Inst.ScriptID != -1)
                {
                    HolderNode.Lines.Add($"OutlineCrate=true");
                    //ExternalResource ScriptRes = new ExternalResource($"../Scripts/{DefaultHashes.ToName(SectionType.Script, (uint)Inst.ScriptID)}.tres");
                    //ExternalResource ScriptRes = new ExternalResource($"../Scripts/{DefaultHashes.ToName(SectionType.Script, (uint)Inst.ScriptID)}.tres");
                    //ExternalResourceList.Add(ScriptRes);
                    //HolderNode.Lines.Add($"InstanceScript = ExtResource ( {ExternalResourceList.Count} )");
                }

                if (Inst.RefList != -1)
                {
                    HolderNode.Lines.Add($"RefList={Inst.RefList}");
                }

                if (Inst.PathIDs.Count != 0)
                {
                    StringBuilder InstRefs = new StringBuilder();
                    InstRefs.Append($"LinkPath = [ ");
                    for (int a = 0; a < Inst.PathIDs.Count - 1; a++)
                    {
                        InstRefs.Append($"NodePath(\"../../Locators/Paths/Path_{SectionID}_{Inst.PathIDs[a]}\"), ");
                    }
                    InstRefs.Append($"NodePath(\"../../Locators/Paths/Path_{SectionID}_{Inst.PathIDs.Last()}\") ]");
                    HolderNode.Lines.Add(InstRefs.ToString());
                }
                if (Inst.PositionIDs.Count != 0)
                {
                    StringBuilder InstRefs = new StringBuilder();
                    InstRefs.Append($"LinkPoint = [ ");
                    for (int a = 0; a < Inst.PositionIDs.Count - 1; a++)
                    {
                        InstRefs.Append($"NodePath(\"../../Locators/Points/Point_{SectionID}_{Inst.PositionIDs[a]}\"), ");
                    }
                    InstRefs.Append($"NodePath(\"../../Locators/Points/Point_{SectionID}_{Inst.PositionIDs.Last()}\") ]");
                    HolderNode.Lines.Add(InstRefs.ToString());
                }
                if (Inst.InstanceIDs.Count != 0)
                {
                    StringBuilder InstRefs = new StringBuilder();
                    InstRefs.Append($"LinkInstance = [ ");
                    for (int a = 0; a < Inst.InstanceIDs.Count - 1; a++)
                    {
                        InstRefs.Append($"NodePath(\"../Instance_{SectionID}_{Inst.InstanceIDs[a]}\"), ");
                    }
                    InstRefs.Append($"NodePath(\"../Instance_{SectionID}_{Inst.InstanceIDs.Last()}\") ]");
                    HolderNode.Lines.Add(InstRefs.ToString());
                }
                if (Inst.UnkI321.Count != 0)
                {
                    StringBuilder AngleReg = new StringBuilder();
                    AngleReg.Append($"RegAngle = [ ");
                    for (int a = 0; a < Inst.UnkI321.Count - 1; a++)
                    {
                        AngleReg.Append($"{Inst.UnkI321[a]}, ");
                    }
                    AngleReg.Append($"{Inst.UnkI321.Last()} ]");
                    HolderNode.Lines.Add(AngleReg.ToString());
                }
                if (Inst.UnkI322.Count != 0)
                {
                    StringBuilder FloatReg = new StringBuilder();
                    FloatReg.Append($"RegFloat = [ ");
                    for (int a = 0; a < Inst.UnkI322.Count - 1; a++)
                    {
                        FloatReg.Append($"{Inst.UnkI322[a].ToText()}, ");
                    }
                    FloatReg.Append($"{Inst.UnkI322.Last().ToText()} ]");
                    HolderNode.Lines.Add(FloatReg.ToString());
                }
                if (Inst.UnkI323.Count != 0)
                {
                    StringBuilder IntReg = new StringBuilder();
                    IntReg.Append($"RegInt = [ ");
                    for (int a = 0; a < Inst.UnkI323.Count - 1; a++)
                    {
                        IntReg.Append($"{Inst.UnkI323[a]}, ");
                    }
                    IntReg.Append($"{Inst.UnkI323.Last()} ]");
                    HolderNode.Lines.Add(IntReg.ToString());
                }

                HolderNode.Lines.Add($"{ExportGodot.transformPosition}=Vector3({(-Inst.Pos.X).ToText()},{Inst.Pos.Y.ToText()},{Inst.Pos.Z.ToText()})");
                System.Numerics.Matrix4x4 mat = System.Numerics.Matrix4x4.Identity;
                mat *= System.Numerics.Matrix4x4.CreateRotationX((Inst.RotX / 65536f) * (float)(2f * Math.PI));
                mat *= System.Numerics.Matrix4x4.CreateRotationY((-Inst.RotY / 65536f) * (float)(2f * Math.PI));
                mat *= System.Numerics.Matrix4x4.CreateRotationZ((-Inst.RotZ / 65536f) * (float)(2f * Math.PI));
                System.Numerics.Matrix4x4.Decompose(mat, out var tscale, out var trot, out var tpos);
                HolderNode.Lines.Add($"quaternion=Quaternion({trot.X.ToText()},{trot.Y.ToText()},{trot.Z.ToText()},{trot.W.ToText()})");
                //float RotX = (float)((Inst.RotX / 65535f) * (2f * Math.PI));
                //float RotY = (float)((-Inst.RotY / 65535f) * (2f * Math.PI));
                //float RotZ = (float)((-Inst.RotZ / 65535f) * (2f * Math.PI));
                //HolderNode.Lines.Add($"rotation = Vector3( {RotX.ToText()}, {RotY.ToText()}, {RotZ.ToText()} )");

                Nodes.Add(HolderNode);

                #region Instantiate GameObject (optional)
                /*
                Node InstNode = new Node($"{DefaultHashes.ToName(SectionType.Object, Inst.ObjectID)}");
                InstNode.InstanceID = PrefabResID;
                if (PrefabResID == -1)
                {
                    InstNode.SetAsNode3D();
                }
                InstNode.KeyValues.Add("parent", $"{RootNodeName}/{HolderNode.Name}");
                Nodes.Add(InstNode);
                */
                #endregion

            }
        }

        public void AddRM(TwinsSection Cont, string path, bool SceneOnly = false, bool AllowGlobal = false)
        {
            bool ExportingSounds = true;
            string SceneName = $"Scene_RM";
            Dictionary<int, int> ImportedObjects = new Dictionary<int, int>();

            #region Export Code
            ExternalResource Code_Container_Scene = new ExternalResource($"res://code/Containers/ChunkScene{ExportGodot.ScriptExt}");
            Code_Container_Scene .SetAsScript();
            ExternalResourceList.Add(Code_Container_Scene);
            CodeResourceID_Scene = ExternalResourceList.Count;
            ExternalResource Code_Container_Resource_Instance = new ExternalResource($"res://code/Containers/ActorInstance{ExportGodot.ScriptExt}");
            Code_Container_Resource_Instance.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_Instance);
            CodeResourceID_Container_Instance = ExternalResourceList.Count;
            ExternalResource Code_Container_Resource_Trigger = new ExternalResource($"res://code/Containers/TriggerVolume{ExportGodot.ScriptExt}");
            Code_Container_Resource_Trigger.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_Trigger);
            CodeResourceID_Container_Trigger = ExternalResourceList.Count;
            ExternalResource Code_Container_Resource_Camera = new ExternalResource($"res://code/Containers/CameraTriggerVolume{ExportGodot.ScriptExt}");
            Code_Container_Resource_Camera.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_Camera);
            CodeResourceID_Container_Camera = ExternalResourceList.Count;
            /*
            ExternalResource Code_Container_Resource_AIPath = new ExternalResource($"../Code/Containers/AIPath{ExportGodot.ScriptExt}");
            Code_Container_Resource_AIPath.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_AIPath);
            CodeResourceID_Container_AIPath = ExternalResourceList.Count;
            ExternalResource Code_Container_Resource_AIPathNode = new ExternalResource($"../Code/Containers/AIPathNode{ExportGodot.ScriptExt}");
            Code_Container_Resource_AIPathNode.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_AIPathNode);
            CodeResourceID_Container_AIPathNode = ExternalResourceList.Count;
            ExternalResource Code_Container_Resource_AgentPath = new ExternalResource($"../Code/Containers/AgentPath{ExportGodot.ScriptExt}");
            Code_Container_Resource_AgentPath.SetAsScript();
            ExternalResourceList.Add(Code_Container_Resource_AgentPath);
            CodeResourceID_Container_AgentPath = ExternalResourceList.Count;
            */
            #endregion

            Nodes[0].Lines.Add($"script = ExtResource( {CodeResourceID_Scene} )");
            
            #region Export Graphics
            TwinsSection tex_sec = Cont.GetItem<TwinsSection>(11).GetItem<TwinsSection>(0);
            TwinsSection model_sec = Cont.GetItem<TwinsSection>(11).GetItem<TwinsSection>(2);
            TwinsSection rigid_sec = Cont.GetItem<TwinsSection>(11).GetItem<TwinsSection>(3);
            TwinsSection skin_sec = Cont.GetItem<TwinsSection>(11).GetItem<TwinsSection>(4);
            TwinsSection bskin_sec = Cont.GetItem<TwinsSection>(11).GetItem<TwinsSection>(5);
            string TexPath = $"{path}\\Textures\\";
            string MeshPath = $"{path}\\Mesh\\";
            Dictionary<uint, string> ExportedTextures = new();
            if (tex_sec.Type == SectionType.TextureX)
            {
                foreach (var item in tex_sec.Records)
                {
                    TextureX tex = (TextureX)item;
                    if (DefaultHashes.DupeTextureIDs.Contains(tex.ID))
                        ExportedTextures[tex.ID] = ExtractTextureX(tex, TexPath, true);
                    else
                        ExtractTextureX(tex, TexPath, false);
                }
                foreach (var item in model_sec.Records)
                {
                    ModelX model = (ModelX)item;
                    GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(model);
                    ModelResource.WriteResourceToBuffer();
                    string MeshName = $"Mesh{DefaultHashes.ModelToName(item.ID, 0)}.res";
                    ModelResource.WriteBufferToFile($"{MeshPath}{MeshName}");
                }
                foreach (var item in skin_sec.Records)
                {
                    SkinX skin = (SkinX)item;
                    ExportGodot.ExportSkinXResource(skin, path, ExportedTextures);
                }
                foreach (var item in bskin_sec.Records)
                {
                    BlendSkinX skin = (BlendSkinX)item;
                    ExportGodot.ExportBlendSkinXResource(skin, path, ExportedTextures);
                }
            }
            else
            {
                foreach (var item in tex_sec.Records)
                {
                    Texture tex = (Texture)item;
                    if (DefaultHashes.DupeTextureIDs.Contains(tex.ID))
                        ExportedTextures[tex.ID] = ExtractTexture(tex, TexPath, true);
                    else
                        ExtractTexture(tex, TexPath, false);
                }
                foreach (var item in model_sec.Records)
                {
                    Model model = (Model)item;
                    GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(model);
                    ModelResource.WriteResourceToBuffer();
                    string MeshName = $"Mesh{DefaultHashes.ModelToName(item.ID, 0)}.res";
                    ModelResource.WriteBufferToFile($"{MeshPath}{MeshName}");
                }
                foreach (var item in skin_sec.Records)
                {
                    Skin skin = (Skin)item;
                    ExportGodot.ExportSkinResource(skin, path, ExportedTextures);
                }
                foreach (var item in bskin_sec.Records)
                {
                    BlendSkin skin = (BlendSkin)item;
                    ExportGodot.ExportBlendSkinResource(skin, path, ExportedTextures);
                }
            }
            foreach (var item in rigid_sec.Records)
            {
                RigidModel model = (RigidModel)item;
                ExportGodot.ExportModelResource(model, path, ExportedTextures);
            }
            #endregion

            AddParticleData(Cont.GetItem<ParticleData>(8), path, SceneOnly);

            #region Export Resources
            // Export GameObjects / OGIs
            TwinsSection obj_sec = Cont.GetItem<TwinsSection>(10).GetItem<TwinsSection>(0);
            for (int i = 0; i < obj_sec.Records.Count; i++)
            {
                uint ObjectID = obj_sec.Records[i].ID;
                string ObjectName = $"{DefaultHashes.ToName(SectionType.Object, ObjectID)}";
                ExportGodot.ExportGameObject(obj_sec.GetItem<GameObject>(ObjectID), path, SceneOnly);
            }

            // Export Scripts
            /*
            if (ExportGodot.ExportScripts)
            {
                TwinsSection scr_sec = Cont.GetItem<TwinsSection>(10).GetItem<TwinsSection>(1);
                for (int i = 0; i < scr_sec.Records.Count; i++)
                {
                    uint ScriptID = scr_sec.Records[i].ID;
                    string ScriptName = $"Script";
                    ExportGodot.ExportScript(scr_sec.GetItem<Script>(ScriptID), $"{System.IO.Path.GetDirectoryName(path)}\\{ScriptName}.dae");
                }
            }
            */

            // Export Animations
            TwinsSection anim_sec = Cont.GetItem<TwinsSection>(10).GetItem<TwinsSection>(2);
            for (int i = 0; i < anim_sec.Records.Count; i++)
            {
                uint ObjectID = anim_sec.Records[i].ID;
                string ObjectName = DefaultHashes.ToName(SectionType.Animation, ObjectID);
                ExportGodot.ExportAnimation(anim_sec.GetItem<Animation>(ObjectID), path);
            }

            // Export OGis
            TwinsSection ogi_sec = Cont.GetItem<TwinsSection>(10).GetItem<TwinsSection>(3);
            for (int i = 0; i < ogi_sec.Records.Count; i++)
            {
                uint OGID = ogi_sec.Records[i].ID;
                string ObjectName = $"Rig_{DefaultHashes.ToName(SectionType.OGI, OGID)}";
                ExportGodot.ExportOGI(ogi_sec.GetItem<GraphicsInfo>(OGID), path, ExportedTextures);
            }

            // Export CustomAgents
            /*
            if (ExportGodot.ExportScripts)
            {
                TwinsSection ca_sec = Cont.GetItem<TwinsSection>(10).GetItem<TwinsSection>(4);
                for (int i = 0; i < ca_sec.Records.Count; i++)
                {
                    uint ObjectID = ca_sec.Records[i].ID;
                    string ObjectName = $"{DefaultHashes.ToName(SectionType.CustomAgent, ObjectID)}";
                    ExportGodot.ExportCustomAgent(ca_sec.GetItem<CustomAgent>(ObjectID), $"{System.IO.Path.GetDirectoryName(path)}\\{ObjectName}.dae", ObjectName);
                }
            }
            */
            #endregion

            Node Locators_RootNode = new Node($"Locators", ExportGodot.Node3D);
            Locators_RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(Locators_RootNode);
            Node Paths_RootNode = new Node($"Paths", ExportGodot.Node3D);
            Paths_RootNode.KeyValues.Add("parent", Locators_RootNode.Name);
            Nodes.Add(Paths_RootNode);
            Node AIPath_RootNode = new Node($"PathsAI", ExportGodot.Node3D);
            AIPath_RootNode.KeyValues.Add("parent", Locators_RootNode.Name);
            Nodes.Add(AIPath_RootNode);
            Node Points_RootNode = new Node($"Points", ExportGodot.Node3D);
            Points_RootNode.KeyValues.Add("parent", Locators_RootNode.Name);
            Nodes.Add(Points_RootNode);
            Node AIPos_RootNode = new Node($"PointsAI", ExportGodot.Node3D);
            AIPos_RootNode.KeyValues.Add("parent", Locators_RootNode.Name);
            Nodes.Add(AIPos_RootNode);

            Node Volumes_RootNode = new Node($"Volumes", ExportGodot.Node3D);
            Volumes_RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(Volumes_RootNode);
            Node Triggers_RootNode = new Node($"Triggers", ExportGodot.Node3D);
            Triggers_RootNode.KeyValues.Add("parent", Volumes_RootNode.Name);
            Nodes.Add(Triggers_RootNode);
            Node CameraTriggers_RootNode = new Node($"CameraTriggers", ExportGodot.Node3D);
            CameraTriggers_RootNode.KeyValues.Add("parent", Volumes_RootNode.Name);
            Nodes.Add(CameraTriggers_RootNode);

            Node Instances_RootNode = new Node($"Instances", ExportGodot.Node3D);
            Instances_RootNode.KeyValues.Add("parent", ".");
            Nodes.Add(Instances_RootNode);

            for (uint i = 0; i < 8; i++)
            {
                if (Cont.ContainsItem(i))
                {
                    Dictionary<uint, int> PathsExported = new Dictionary<uint, int>();

                    TwinsSection Section = Cont.GetItem<TwinsSection>(i);
                    //if (Section.ContainsItem(0) && Section.Records.Count != 0)
                    //{
                        //TwinsSection TemplateSection = Section.GetItem<TwinsSection>(0);
                        //for (int a = 0; a < TemplateSection.Records.Count; a++)
                        //{
                            //ExportGodot.ExportInstanceTemplate(TemplateSection.GetItem<InstanceTemplate>(TemplateSection.Records[a].ID), path);
                        //}
                    //}
                    if (Section.ContainsItem(1) && Section.GetItem<TwinsSection>(1).Records.Count != 0)
                    {
                        AddAIPositions(Section.GetItem<TwinsSection>(1), i, $"{Locators_RootNode.Name}/{AIPos_RootNode.Name}");
                    }
                    if (Section.ContainsItem(2) && Section.GetItem<TwinsSection>(2).Records.Count != 0)
                    {
                        AddAIPath(Section.GetItem<TwinsSection>(2), Section.GetItem<TwinsSection>(1), i, $"{Locators_RootNode.Name}/{AIPath_RootNode.Name}");
                    }
                    if (Section.ContainsItem(3) && Section.GetItem<TwinsSection>(3).Records.Count != 0)
                    {
                        AddPosition(Section.GetItem<TwinsSection>(3), i, $"{Locators_RootNode.Name}/{Points_RootNode.Name}");
                    }
                    if (Section.ContainsItem(4) && Section.GetItem<TwinsSection>(4).Records.Count != 0)
                    {
                        AddPath(Section.GetItem<TwinsSection>(4), Section.GetItem<TwinsSection>(3), i, $"{Locators_RootNode.Name}/{Paths_RootNode.Name}");
                    }
                    //if (Section.ContainsItem(5) && Section.GetItem<TwinsSection>(5).Records.Count != 0)
                    //{
                        //TwinsSection SurfaceSection = Section.GetItem<TwinsSection>(5);
                        //for (int a = 0; a < SurfaceSection.Records.Count; a++)
                        //{
                            //ExportGodot.ExportCollisionSurface(SurfaceSection.GetItem<CollisionSurface>(SurfaceSection.Records[a].ID), path);
                        //}
                    //}
                    if (Section.ContainsItem(6) && Section.GetItem<TwinsSection>(6).Records.Count != 0)
                    {
                        AddInstance(Section.GetItem<TwinsSection>(6), i, ImportedObjects, AllowGlobal, $"{Instances_RootNode.Name}", Section.GetItem<TwinsSection>(3), Section.GetItem<TwinsSection>(4), PathsExported);
                    }
                    if (Section.ContainsItem(7) && Section.GetItem<TwinsSection>(7).Records.Count != 0)
                    {
                        AddTrigger(Section.GetItem<TwinsSection>(7), i, $"{Volumes_RootNode.Name}/{Triggers_RootNode.Name}");
                    }
                    if (Section.ContainsItem(8) && Section.GetItem<TwinsSection>(8).Records.Count != 0)
                    {
                        AddCamera(Section.GetItem<TwinsSection>(8), i, $"{Volumes_RootNode.Name}/{CameraTriggers_RootNode.Name}");
                    }
                }
            }

            if (ExportingSounds)
            {
                ExportSounds(Cont, path);
            }

        }

        public void ExportSounds(TwinsSection Cont, string path)
        {
            string SoundExt = ".res";
            TwinsSection CodeSection = Cont.GetItem<TwinsSection>(10);
            bool IsXbox = CodeSection.Type == SectionType.CodeX;
            if (CodeSection.ContainsItem(6))
            {
                TwinsSection Section = CodeSection.GetItem<TwinsSection>(6);
                for (int a = 0; a < Section.Records.Count; a++)
                {
                    string SoundPath = $"{path}\\Sounds\\{DefaultHashes.ToName(SectionType.SE, Section.Records[a].ID).Replace("/","\\")}{SoundExt}";
                    if (IsXbox)
                    {
                        SoundEffectX SFX = Section.GetItem<SoundEffectX>(Section.Records[a].ID);
                        if (!AssetExporter.Check(SoundPath))
                        {
                            GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(SFX);
                            wav.WriteToFile(SoundPath);
                        }
                    }
                    else
                    {
                        SoundEffect SFX = Section.GetItem<SoundEffect>(Section.Records[a].ID);
                        if (!AssetExporter.Check(SoundPath))
                        {
                            GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(SFX);
                            wav.WriteToFile(SoundPath);
                        }
                    }
                }
            }
            string DirPath_English = "English";
            string DirPath_French = "French";
            string DirPath_German = "German";
            string DirPath_Spanish = "Spanish";
            string DirPath_Italian = "Italian";
            //string DirPath_Japanese = "Japanese";
            string DirPath = DirPath_English;
            for (uint i = 7; i < 12; i++)
            {
                switch (i)
                {
                    case 7: DirPath = DirPath_English; break;
                    case 8: DirPath = DirPath_French; break;
                    case 9: DirPath = DirPath_German; break;
                    case 10: DirPath = DirPath_Spanish; break;
                    case 11: DirPath = DirPath_Italian; break;
                }
                if (CodeSection.ContainsItem(i))
                {
                    TwinsSection Section = CodeSection.GetItem<TwinsSection>(i);
                    for (int a = 0; a < Section.Records.Count; a++)
                    {
                        string SoundPath = $"{path}\\Sounds\\{DefaultHashes.ToName(SectionType.SE, Section.Records[a].ID).Replace("/","\\").Replace(DirPath_English, DirPath)}{SoundExt}";
                        if (IsXbox)
                        {
                            SoundEffectX SFX = Section.GetItem<SoundEffectX>(Section.Records[a].ID);
                            if (!AssetExporter.Check(SoundPath))
                            {
                                GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(SFX);
                                wav.WriteToFile(SoundPath);
                            }
                        }
                        else
                        {
                            SoundEffect SFX = Section.GetItem<SoundEffect>(Section.Records[a].ID);
                            if (!AssetExporter.Check(SoundPath))
                            {
                                GodotBinaryAudioStreamWAV wav = new GodotBinaryAudioStreamWAV(SFX);
                                wav.WriteToFile(SoundPath);
                            }
                        }
                    }
                }
            }
        }

        public void AddRigidModelResource(RigidModel MCont, string path, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Mesh\\";
            string Extension = ".res";
            TwinsSection mesh_sec = MCont.Parent.Parent.GetItem<TwinsSection>(2);
            string MeshName;

            // Export resource
            if (DefaultHashes.Hash_Models.ContainsKey(MCont.MeshID))
            {
                MeshName = $"Mesh{DefaultHashes.ModelToName(MCont.MeshID, 0)}{Extension}";
            }
            else
            {
                GodotBinaryArrayMesh ModelResource;
                if (mesh_sec.Type == SectionType.ModelX)
                {
                    ModelX Cont = mesh_sec.GetItem<ModelX>(MCont.MeshID);
                    ModelResource = new GodotBinaryArrayMesh(Cont);
                }
                else
                {
                    Model Cont = mesh_sec.GetItem<Model>(MCont.MeshID);
                    ModelResource = new GodotBinaryArrayMesh(Cont);
                }
                ModelResource.WriteResourceToBuffer();
                uint MeshHash = ModelResource.WriteBuffer.GetSequenceHashCode();
                MeshName = $"Mesh{DefaultHashes.ModelToName(MCont.MeshID, MeshHash)}{Extension}";
                if (!AssetExporter.Check($"{DirPath}{MeshName}"))
                    ModelResource.WriteBufferToFile($"{DirPath}{MeshName}");
            }

            ExternalResource ModelFileReference = new ExternalResource(MeshName);
            ModelFileReference.SetAsArrayMesh();
            ExternalResourceList.Add(ModelFileReference);

            // Export materials and textures
            TwinsSection mat_sec = MCont.Parent.Parent.GetItem<TwinsSection>(1);

            List<Material> Materials = new List<Material>();
            for (int i = 0; i < MCont.MaterialIDs.Length; i++)
            {
                Material Mat = mat_sec.GetItem<Material>(MCont.MaterialIDs[i]);
                Materials.Add(Mat);
            }
            AddMaterialsTextures(path, Materials, ExportedTextures);

            // Create mesh node
            //Node GeomNode = new Node($"mesh", ExportGodot.MeshInstance3D);
            //GeomNode.KeyValues.Add("parent", ".");
            //GeomNode.Lines.Add($"layers = 3"); // for lights
            Nodes[0].Lines.Add($"mesh=ExtResource(1)");
            for (int i = 0; i < MCont.MaterialIDs.Length; i++)
            {
                Nodes[0].Lines.Add($"{ExportGodot.materialOverride}/{i}=ExtResource({i + 2})");
            }
            //Nodes.Add(GeomNode);
        }
        public void AddSkinResource(Skin Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Skins\\";
            //Directory.CreateDirectory(DirPath);
            string ModelFilePath = $"Skin_{DefaultHashes.ToName(SectionType.Skin, Cont.ID)}";
            string Extension = ".res";

            // Export resource
            if (!AssetExporter.Check($"{DirPath}{ModelFilePath}{Extension}"))
            {
                GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(Cont);
                ModelResource.WriteToFile($"{DirPath}{ModelFilePath}{Extension}");
            }

            ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}{Extension}");
            ModelFileReference.SetAsArrayMesh();
            ExternalResourceList.Add(ModelFileReference);

            // Export materials and textures
            TwinsSection mat_sec = Cont.Parent.Parent.GetItem<TwinsSection>(1);

            List<Material> Materials = new List<Material>();
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Material Mat = mat_sec.GetItem<Material>(Cont.SubModels[i].MaterialID);
                Materials.Add(Mat);
            }
            AddMaterialsTextures(path, Materials, ExportedTextures);

            // Create mesh node
            //Node GeomNode = new Node($"mesh", ExportGodot.MeshInstance3D);
            //GeomNode.KeyValues.Add("parent", ".");
            //GeomNode.Lines.Add($"layers = 3"); // for lights
            Nodes[0].Lines.Add($"mesh=ExtResource(1)");
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Nodes[0].Lines.Add($"{ExportGodot.materialOverride}/{i}=ExtResource({i + 2})");
            }
            //Nodes.Add(GeomNode);
        }
        public void AddSkinXResource(SkinX Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Skins\\";
            //Directory.CreateDirectory(DirPath);
            string ModelFilePath = $"Skin_{DefaultHashes.ToName(SectionType.Skin, Cont.ID)}";
            string Extension = ".res";

            // Export resource
            if (!AssetExporter.Check($"{DirPath}{ModelFilePath}{Extension}"))
            {
                GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(Cont);
                ModelResource.WriteToFile($"{DirPath}{ModelFilePath}{Extension}");
            }

            ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}{Extension}");
            ModelFileReference.SetAsArrayMesh();
            ExternalResourceList.Add(ModelFileReference);

            // Export materials and textures
            TwinsSection tex_sec = Cont.Parent.Parent.GetItem<TwinsSection>(0);
            TwinsSection mat_sec = Cont.Parent.Parent.GetItem<TwinsSection>(1);

            List<Material> Materials = new List<Material>();
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Material Mat = mat_sec.GetItem<Material>(Cont.SubModels[i].MaterialID);
                Materials.Add(Mat);
            }
            AddMaterialsTextures(path, Materials, ExportedTextures);

            // Create mesh node
            //Node GeomNode = new Node($"mesh", ExportGodot.MeshInstance3D);
            //GeomNode.KeyValues.Add("parent", ".");
            //GeomNode.Lines.Add($"layers = 3"); // for lights
            Nodes[0].Lines.Add($"mesh=ExtResource(1)");
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Nodes[0].Lines.Add($"{ExportGodot.materialOverride}/{i}=ExtResource({i + 2})");
            }
            //Nodes.Add(GeomNode);
        }
        public void AddBlendSkinResource(BlendSkin Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Skins\\";
            //Directory.CreateDirectory(DirPath);
            string ModelFilePath = $"BlendSkin_{DefaultHashes.ToName(SectionType.BlendSkin, Cont.ID)}";
            string Extension = ".res";

            // Export resource
            if (!AssetExporter.Check($"{DirPath}{ModelFilePath}{Extension}"))
            {
                GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(Cont);
                ModelResource.WriteToFile($"{DirPath}{ModelFilePath}{Extension}");
            } 

            ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}{Extension}");
            ModelFileReference.SetAsArrayMesh();
            ExternalResourceList.Add(ModelFileReference);

            // Export materials and textures
            TwinsSection mat_sec = Cont.Parent.Parent.GetItem<TwinsSection>(1);

            List<Material> Materials = new List<Material>();
            for (int i = 0; i < Cont.Models.Length; i++)
            {
                Material Mat = mat_sec.GetItem<Material>(Cont.Models[i].MaterialID);
                Materials.Add(Mat);
            }
            AddMaterialsTextures(path, Materials, ExportedTextures);

            // Create mesh node
            //Node GeomNode = new Node($"mesh", ExportGodot.MeshInstance3D);
            //GeomNode.KeyValues.Add("parent", ".");
            //GeomNode.Lines.Add($"layers = 3"); // for lights
            Nodes[0].Lines.Add($"mesh=ExtResource(1)");
            for (int i = 0; i < Cont.Models.Length; i++)
            {
                Nodes[0].Lines.Add($"{ExportGodot.materialOverride}/{i}=ExtResource({i + 2})");
            }
            //Nodes.Add(GeomNode);
        }
        public void AddBlendSkinXResource(BlendSkinX Cont, string path, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Skins\\";
            //Directory.CreateDirectory(DirPath);
            string ModelFilePath = $"BlendSkin_{DefaultHashes.ToName(SectionType.BlendSkin, Cont.ID)}";
            string Extension = ".res";

            // Export resource
            if (!AssetExporter.Check($"{DirPath}{ModelFilePath}{Extension}"))
            {
                GodotBinaryArrayMesh ModelResource = new GodotBinaryArrayMesh(Cont);
                ModelResource.WriteToFile($"{DirPath}{ModelFilePath}{Extension}");
            } 

            ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}{Extension}");
            ModelFileReference.SetAsArrayMesh();
            ExternalResourceList.Add(ModelFileReference);

            // Export materials and textures
            TwinsSection mat_sec = Cont.Parent.Parent.GetItem<TwinsSection>(1);

            List<Material> Materials = new List<Material>();
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Material Mat = mat_sec.GetItem<Material>(Cont.SubModels[i].MaterialID);
                Materials.Add(Mat);
            }
            AddMaterialsTextures(path, Materials, ExportedTextures);

            // Create mesh node
            //Node GeomNode = new Node($"mesh", ExportGodot.MeshInstance3D);
            //GeomNode.KeyValues.Add("parent", ".");
            //GeomNode.Lines.Add($"layers = 3"); // for lights
            Nodes[0].Lines.Add($"mesh=ExtResource(1)");
            for (int i = 0; i < Cont.SubModels.Count; i++)
            {
                Nodes[0].Lines.Add($"{ExportGodot.materialOverride}/{i}=ExtResource({i + 2})");
            }
            //Nodes.Add(GeomNode);
        }

        public void AddMaterialsTextures(string path, List<Material> Materials, Dictionary<uint, string> ExportedTextures)
        {
            string DirPath = $"{path}\\Materials\\";
            string TexPath = $"{path}\\Textures\\";
            //Directory.CreateDirectory(DirPath);
            //Directory.CreateDirectory(TexPath);

            List<string> MaterialFileNames = new List<string>();
            for (int mat = 0; mat < Materials.Count; mat++)
            {
                var MaterialFile = new GodotResourceFile();
                int ExtraShaderID = -1;
                string ExtraShaderType = "";
                for (int i = 0; i < Materials[mat].Shaders.Count; i++)
                {
                    TwinsShader shader = Materials[mat].Shaders[i];
                    string shaderAdd = "";
                    bool ExtraShaderNeeded = true;
                    InternalResource TargetResource = MaterialFile.Resource;
                    if (i != 0)
                    {
                        TargetResource = new InternalResource();
                    }

                    // or shader.AlphaValueToBeComparedTo?
                    //int render_priority = Math.Clamp(Materials[mat].Unknown + (shader.AlphaValueToBeComparedTo - 128), -127, 128);
                    int render_priority = Math.Clamp((int)(shader.UnkVector2.W - 128f) + Materials[mat].Unknown + i, -127, 128);
                    //int render_priority = Math.Clamp(i, -127, 128);
                    TargetResource.Lines.Add($"render_priority={render_priority}");
                    //TargetResource.Lines.Add($"render_priority = {shader.FixedAlphaValue - 128}");
                    //TargetResource.Lines.Add($"render_priority = {-(shader.FixedAlphaValue - 127)}");

                    /* currently setting all materials under a custom shader because of draw order
                    if (shader.UnkVector3.Z != 0 || shader.UnkVector3.W != 0 || 
                        shader.ShaderType == 16 || shader.ShaderType == 12 || shader.ShaderType == 22 ||
                        shader.ShaderType == 23 || shader.ShaderType == 26 ||
                        (shader.ABlending == TwinsShader.AlphaBlending.ON && shader.AlphaRegSettingsIndex == 2))
                    {
                        ExtraShaderNeeded = true;
                    }
                    */

                    if (ExtraShaderNeeded)
                    {
                        string shaderName = "TexScroll";
                        if (shader.ABlending == TwinsShader.AlphaBlending.ON)
                        {
                            //shaderName = "TexScrollAlphaBlend";
                            if (shader.AlphaRegSettingsIndex == 0)
                            {
                                shaderName = "TexScrollAlphaBlendMix";
                            }
                            else if (shader.AlphaRegSettingsIndex == 1)
                            {
                                shaderName = "TexScrollAlphaBlendAdd";
                            }
                            else if (shader.AlphaRegSettingsIndex == 2)
                            {
                                shaderName = "TexScrollAlphaBlendSub";
                            }
                            else
                            {
                                throw new NotImplementedException("Unknown blending tyype! Should not exist.");
                            }
                        }
                        else if (shader.ATest == TwinsShader.AlphaTest.ON)
                        {
                            shaderName = "TexScrollAlphaTest";
                        }
                        if (ExtraShaderID == -1 || ExtraShaderType != shaderName)
                        {
                            switch (shader.ShaderType)
                            {
                                default: break;
                                // Unlit shaders
                                case 1:
                                case 10:
                                case 11:
                                case 13:
                                case 18:
                                case 19:
                                case 21:
                                case 22:
                                case 23:
                                case 26:
                                case 27:
                                    shaderName = "Unlit" + shaderName;
                                    break;
                                case 16:
                                    shaderName = "Reflect" + shaderName;
                                    break;
                            }
                            //ExternalResource ShResource = new ExternalResource($"../Shaders/{shaderName}.gdshader");
                            ExternalResource ShResource = new ExternalResource($"res://shaders/{shaderName}.gdshader");
                            ShResource.SetAsShader();
                            MaterialFile.ExternalResourceList.Add(ShResource);
                            TargetResource.Lines.Add($"shader=ExtResource({MaterialFile.ExternalResourceList.Count})");
                            ExtraShaderID = MaterialFile.ExternalResourceList.Count;
                            ExtraShaderType = shaderName;
                        }
                        else
                        {
                            TargetResource.Lines.Add($"shader=ExtResource({ExtraShaderID})");
                        }

                        shaderAdd = "shader_parameter/";
                    }

                    switch (shader.ShaderType)
                    {
                        default: break;
                        case 12:
                        case 22:
                        // Lit/Unlit environment map
                        {
                            TargetResource.Lines.Add($"{shaderAdd}envmap=true");
                        }
                        break;
                        case 16:
                        // Lit reflection surface
                        {
                            TargetResource.Lines.Add($"{shaderAdd}reflects=true");
                            TargetResource.Lines.Add($"{shaderAdd}reflectDist={shader.FloatParam[0].ToText()}");
                        }
                        break;
                        case 15:
                        case 21:
                        // Lit/Unlit glossy/metallic/chrome
                        {
                            
                        }
                        break;
                        case 23:
                        case 26:
                        // Unlit Cloth/Grass/Tree/Mist/Cobweb deformation
                        {
                            TargetResource.Lines.Add($"{shaderAdd}deform=true");
                            TargetResource.Lines.Add($"{shaderAdd}deformspeed=Vector2({shader.FloatParam[0].ToText()},{(shader.FloatParam[1]).ToText()})");
                            if (shader.ShaderType == 26)
                            {
                                TargetResource.Lines.Add($"{shaderAdd}deformspeed2=Vector2({shader.FloatParam[2].ToText()},{(shader.FloatParam[3]).ToText()})");
                            }
                        }
                        break;
                        case 27:
                        // Unlit 2D billboard always facing camera?
                        {
                            //TargetResource.Lines.Add($"{shaderAdd}billboard_mode = 2");
                        }
                        break;
                    }
                    
                    if (!ExtraShaderNeeded)
                    {
                        switch (shader.ShaderType)
                        {
                            default: break;
                            // Unlit shaders
                            case 1:
                            case 10:
                            case 11:
                            case 13:
                            case 18:
                            case 19:
                            case 21:
                            case 22:
                            case 23:
                            case 26:
                            case 27:
                                TargetResource.Lines.Add($"shading_mode=0");
                            break;
                        }
                        if (shader.ABlending == TwinsShader.AlphaBlending.ON)
                        {
                            TargetResource.Lines.Add(ExportGodot.materialTransparency);
                            TargetResource.Lines.Add($"{ExportGodot.materialBlendMode}=1");
                            TargetResource.Lines.Add(ExportGodot.materialDepthDrawMode);
                        }
                        else if (shader.ATest == TwinsShader.AlphaTest.ON)
                        {
                            TargetResource.Lines.Add(ExportGodot.materialTransparency);
                            TargetResource.Lines.Add(ExportGodot.materialDepthDrawMode);
                        }
                    }
                    else
                    {
                        if (shader.UnkVector3.Z != 0 || shader.UnkVector3.W != 0)
                        {
                            // also dictated by Val2 - X/Val3 - Y (ex. bees have X scrolling values, but disabled)
                            float X = shader.UnkVector3.Z;
                            float Y = -shader.UnkVector3.W;
                            if (shader.UnkVal2 == 0)
                            {
                                X = 0f;
                            }
                            if (shader.UnkVal3 == 0)
                            {
                                Y = 0f;
                            }
                            TargetResource.Lines.Add($"{shaderAdd}speed=Vector2({X.ToText()},{Y.ToText()})");
                        }
                        if (shader.ABlending == TwinsShader.AlphaBlending.ON && shader.AlphaRegSettingsIndex == 2)
                        {
                            // edge case: inverted colors for some reason
                            //TargetResource.Lines.Add($"{shaderAdd}swapcolors = true");
                        }
                    }

                    if (shader.ShaderType == 15 || shader.ShaderType == 21)
                    {
                        TargetResource.Lines.Add($"{shaderAdd}metallic_specular=0.5");
                    }
                    else
                    {
                        TargetResource.Lines.Add($"{shaderAdd}metallic_specular=0.0");
                    }

                    if (shader.TxtMapping == TwinsShader.TextureMapping.ON)
                    {
                        string OutName;
                        if (!DefaultHashes.DupeTextureIDs.Contains(shader.TextureId))
                        {
                            OutName = $"{DefaultHashes.TexToName(shader.TextureId, 0)}.res";
                        }
                        else
                        {
                            OutName = ExportedTextures[shader.TextureId];
                        }

                        ExternalResource TexResource = new ExternalResource($"../Textures/{OutName}");
                        TexResource.SetAsTexture();
                        MaterialFile.ExternalResourceList.Add(TexResource);

                        TargetResource.Lines.Add($"{shaderAdd}albedo_texture=ExtResource({MaterialFile.ExternalResourceList.Count})");
                    }
                   
                    if (!ExtraShaderNeeded)
                    {
                        TargetResource.CreateMaterial();
                        TargetResource.CreateStandardMaterial();
                    }
                    else
                    {
                        TargetResource.CreateShaderMaterial();
                    }
                    
                    if (i != 0)
                    {
                        MaterialFile.InternalResourceList.Add(TargetResource);
                        if (i == 1)
                        {
                            MaterialFile.Resource.Lines.Add($"next_pass=SubResource({MaterialFile.InternalResourceList.Count})");
                        }
                        else
                        {
                            MaterialFile.InternalResourceList[MaterialFile.InternalResourceList.Count - 2].Lines.Add($"next_pass=SubResource({MaterialFile.InternalResourceList.Count})");
                        }
                    }
                }
                // Re-hashing material due to ID collisions
                MaterialFile.Serialize();
                string MatHash = MaterialFile.FileLines.GetSequenceHashCode().ToString("X8");
                string MatName = $"Mat{Materials[mat].ID.ToString("X8")}-{Materials[mat].Name.Replace("\0", "")}-{MatHash}";
                string MatPath = $"{DirPath}{MatName}.tres";
                if (!AssetExporter.Check(MatPath))
                    MaterialFile.WriteToFile(MatPath);
                MaterialFileNames.Add($"../Materials/{MatName}.tres");
            }

            for (int i = 0; i < MaterialFileNames.Count; i++)
            {
                ExternalResource MatFile = new ExternalResource(MaterialFileNames[i]);
                MatFile.SetAsMaterial();
                ExternalResourceList.Add(MatFile);
            }

        }
        public string ExtractTextureX(TextureX Tex, string DirPath, bool ReHash)
        {
            // Duplicate names are skipped to save time where possible
            int TWidth = Tex.Width;
            int THeight = Tex.Height;
            List<Color> Texture = new List<Color>(Tex.RawData);
            string Path;
            string Extenstion = ".res";

            if (ReHash)
            {
                // Rehashing Textures due to ID collisions
                uint Hash = Texture.GetSequenceHashCode();
                Path = $"{DefaultHashes.TexToName(Tex.ID, Hash)}{Extenstion}";
                if (AssetExporter.Check($"{DirPath}\\{Path}")) return Path;
            }
            else
            {
                Path = $"{DefaultHashes.ToName(SectionType.Texture, Tex.ID)}{Extenstion}";
                if (AssetExporter.Check($"{DirPath}\\{Path}"))
                {
                    return Path;
                }
            }

            GodotBinaryImageTexture TexRes = new GodotBinaryImageTexture(Tex);
            if (!AssetExporter.Check($"{DirPath}\\{Path}"))
                TexRes.WriteToFile($"{DirPath}\\{Path}");
            return Path;
        }

        public void Add_InstancedScene(string ModelFilePath, string parentNodePath)
        {
            ExternalResource ModelFileReference = new ExternalResource($"{ModelFilePath}{ExportGodot.SceneExtension}");
            ModelFileReference.SetAsPackedScene();
            ExternalResourceList.Add(ModelFileReference);

            Node ModelNode = new Node($"{ModelFilePath.Split('/').Last()}");
            ModelNode.InstanceID = ExternalResourceList.Count;
            ModelNode.KeyValues.Add("parent", parentNodePath);
            Nodes.Add(ModelNode);
        }

        void Add_InstancedSceneryModel(SceneryData.ScenerySubModel Model, string path, string ParentNodeName, 
            ref uint NodeID, TwinsSection ModelSection, TwinsSection LODSection, Dictionary<uint, (int, uint)> ExportedModels, Dictionary<uint, int> ExportedLODs,
            Dictionary<uint, string> ExportedTextures)
        {
            if (Model.isSpecial && ExportGodot.ExportLODs)
            {
                if (ExportedLODs.ContainsKey(Model.ModelID))
                {
                    Node ModelNode = new Node($"LODModel_{Model.ModelID.ToString("X8")}_{NodeID}");
                    ModelNode.InstanceID = ExportedLODs[Model.ModelID];
                    ModelNode.KeyValues.Add("parent", ParentNodeName);
                    Nodes.Add(ModelNode);
                }
                else
                {
                    string ModelName = $"LODModel_{Model.ModelID.ToString("X8")}";
                    LodModel LODCont = LODSection.GetItem<LodModel>(Model.ModelID);
                    string Hash = ExportGodot.ExportLODModel(LODCont, path, ExportedTextures);
                    Add_InstancedScene($"../LODs/{ModelName}_{Hash}", ParentNodeName);
                    ExportedLODs.Add(Model.ModelID, ExternalResourceList.Count);
                }
            }
            else
            {
                uint ModelID = Model.ModelID;
                if (Model.isSpecial && !ExportGodot.ExportLODs)
                {
                    LodModel LODCont = LODSection.GetItem<LodModel>(Model.ModelID);
                    ModelID = LODCont.LODModelIDs[0];
                }

                if (ExportedModels.ContainsKey(ModelID))
                {
                    string outName = DefaultHashes.RigidToName(ModelID, ExportedModels[ModelID].Item2);
                    Node ModelNode = new Node($"{outName}_{NodeID}");
                    ModelNode.InstanceID = ExportedModels[ModelID].Item1;
                    ModelNode.KeyValues.Add("parent", ParentNodeName);
                    Nodes.Add(ModelNode);
                }
                else
                {
                    RigidModel RigidCont = ModelSection.GetItem<RigidModel>(ModelID);
                    uint Hash = ExportGodot.ExportModelResource(RigidCont, path, ExportedTextures);
                    Add_InstancedScene($"../Mesh/{DefaultHashes.RigidToName(ModelID, Hash)}", ParentNodeName);
                    ExportedModels.Add(ModelID, (ExternalResourceList.Count, Hash));
                }
            }

            Nodes.Last().Name += $"_{NodeID}";
            Nodes.Last().Lines.Add($"transform={MatrixToTransform(Model.ModelMatrix)}");
            Nodes.Last().Lines.Add($"cast_shadow=0"); // original game doesn't cast shadows, remove if needed

            NodeID++;
        }

        public static string MatrixToTransform(Pos[] Matrix)
        {
            //Transform( 1, 0, 0 | 0, 1, 0 | 0, 0, 1 | 0, 0, 0 )");
            // godot doesn't like strings like 5,960464E-08, has to be 5.960464e-08
            List<string> Values = new List<string>();
            Values.Add(Matrix[0].X.ToText());
            Values.Add((-Matrix[1].X).ToText());
            Values.Add((-Matrix[2].X).ToText());

            Values.Add((-Matrix[0].Y).ToText());
            Values.Add(Matrix[1].Y.ToText());
            Values.Add(Matrix[2].Y.ToText());

            Values.Add((-Matrix[0].Z).ToText());
            Values.Add(Matrix[1].Z.ToText());
            Values.Add(Matrix[2].Z.ToText());

            Values.Add((-Matrix[3].X).ToText());
            Values.Add(Matrix[3].Y.ToText());
            Values.Add(Matrix[3].Z.ToText());

            string outMatrix = $"{ExportGodot.Transform3D}(";
            for (int i = 0; i < Values.Count - 1; i++)
            {
                outMatrix += $"{Values[i]},";
            }
            outMatrix += $"{Values[Values.Count - 1]})";
            return outMatrix;
        }

        public static string ExtractTexture(Texture Tex, string DirPath, bool ReHash)
        {
            // Duplicate names are skipped to save time where possible
            List<Color> Texture = new List<Color>(Tex.RawData);
            string Path;
            string Extenstion = ".res";

            if (ReHash)
            {
                // Rehashing Textures due to ID collisions
                uint Hash = Texture.GetSequenceHashCode();
                Path = $"{DefaultHashes.TexToName(Tex.ID, Hash)}{Extenstion}";
                if (AssetExporter.Check($"{DirPath}\\{Path}")) return Path;
            }
            else
            {
                Path = $"{DefaultHashes.ToName(SectionType.Texture, Tex.ID)}{Extenstion}";
                if (AssetExporter.Check($"{DirPath}\\{Path}"))
                {
                    return Path;
                }
            }
            
            GodotBinaryImageTexture TexRes = new GodotBinaryImageTexture(Tex);
            if (!AssetExporter.Check($"{DirPath}\\{Path}"))
                TexRes.WriteToFile($"{DirPath}\\{Path}");
            
            return Path;
        }

    }
}
