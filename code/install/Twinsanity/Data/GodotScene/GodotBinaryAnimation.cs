using System;
using System.Collections.Generic;
using Twinsanity;
using Twinsanity.Items;

namespace RehabSetup
{
    public class GodotBinaryAnimation : GodotBinaryResourceFile
    {

        public override string ResType => "Animation";

        public GodotBinaryAnimation()
        {

        }

        public GodotBinaryAnimation(Animation Anim, List<Pos> RigAddRot, List<uint> RigJointParent)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            res.Add("resource_name", DefaultHashes.ToName(SectionType.Animation, Anim.ID));
            if (Anim.TotalFrames == 0 && Anim.FacialAnimationTotalFrames == 0) return;
            float FrameStep = 0.02f * 2f; // All animations are 25 FPS
            uint AllFrames = Anim.TotalFrames;
            if (AllFrames == 0)
            {
                AllFrames = Anim.FacialAnimationTotalFrames;
            }
            if (AllFrames == 0) return;
            res.Add("length", FrameStep * AllFrames);
            res.Add("step", FrameStep);

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

                    List<float> ListPos = new List<float>();
                    List<float> ListRot = new List<float>();
                    List<float> ListScale = new List<float>();
                    for (int t = 0; t < animFrames; t++)
                    {
                        //System.Numerics.Matrix4x4.Decompose(boneTransform[t][j].Item1, out var scale, out var rot, out var pos);
                        var pos = boneTransform[t][j].Item1;
                        var rot = boneTransform[t][j].Item2;
                        var scale = boneTransform[t][j].Item3;
                        ListPos.Add(timeStamps[t]);
                        ListPos.Add(1);
                        ListPos.Add(pos.X);
                        ListPos.Add(pos.Y);
                        ListPos.Add(pos.Z);
                        ListRot.Add(timeStamps[t]);
                        ListRot.Add(1);
                        ListRot.Add(rot.X);
                        ListRot.Add(rot.Y);
                        ListRot.Add(rot.Z);
                        ListRot.Add(rot.W);
                        ListScale.Add(timeStamps[t]);
                        ListScale.Add(1);
                        ListScale.Add(scale.X);
                        ListScale.Add(scale.Y);
                        ListScale.Add(scale.Z);
                    }

                    NodePath node = new NodePath(".", $"joint{j}");
                    res.Add($"tracks/{i}/type", "position_3d");
                    res.Add($"tracks/{i}/path", node);
                    res.Add($"tracks/{i}/keys", ListPos.ToArray());
                    i++;
                    res.Add($"tracks/{i}/type", "rotation_3d");
                    res.Add($"tracks/{i}/path", node);
                    res.Add($"tracks/{i}/keys", ListRot.ToArray());
                    i++;
                    res.Add($"tracks/{i}/type", "scale_3d");
                    res.Add($"tracks/{i}/path", node);
                    res.Add($"tracks/{i}/keys", ListScale.ToArray());
                    i++;
                }
            }

            // Blend shape animations
            if (Anim.FacialAnimationTotalFrames != 0)
            {
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
                    NodePath node = new NodePath("BlendSkin", $"blend_shapes/morph_{shape}");
                    res.Add($"tracks/{i + shape}/type", "value");
                    res.Add($"tracks/{i + shape}/path", node);

                    var dict = new Dictionary<object, object>();
                    List<float> times = new List<float>();
                    for (int t = 0; t < blendFrames; t++)
                    {
                        times.Add(t * FrameStep);
                    }
                    dict.Add("times", times.ToArray());
                    List<object> values = new List<object>();
                    for (int t = 0; t < blendFrames; t++)
                    {
                        values.Add(blendTransform[t][shape]);
                    }
                    dict.Add("values", values.ToArray());
                    res.Add($"tracks/{i + shape}/keys", dict);
                }
            }

            Resources.Add(res);
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

        public GodotBinaryAnimation(GraphicsInfo rig, uint shapesAmount)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            res.Add("resource_name", "RESET");
            res.Add("length", 0.001f);

            Dictionary<int, System.Numerics.Matrix4x4> RestPoses = new();
            GodotUtil.ComputeTposeTransform(rig, 0, System.Numerics.Matrix4x4.Identity, ref RestPoses);

            int i = 0;
            // Bone keys
            for (int j = 0; j < rig.Joints.Length; j++)
            {
                List<float> ListPos = new List<float>();
                List<float> ListRot = new List<float>();
                List<float> ListScale = new List<float>();
                System.Numerics.Matrix4x4.Decompose(RestPoses[j], out System.Numerics.Vector3 scale, out System.Numerics.Quaternion rot, out System.Numerics.Vector3 pos);
                ListPos.Add(0f);
                ListPos.Add(1);
                ListPos.Add(pos.X);
                ListPos.Add(pos.Y);
                ListPos.Add(pos.Z);
                ListRot.Add(0f);
                ListRot.Add(1);
                ListRot.Add(rot.X);
                ListRot.Add(rot.Y);
                ListRot.Add(rot.Z);
                ListRot.Add(rot.W);
                ListScale.Add(0f);
                ListScale.Add(1);
                ListScale.Add(scale.X);
                ListScale.Add(scale.Y);
                ListScale.Add(scale.Z);

                NodePath node = new NodePath(".", $"joint{j}");
                res.Add($"tracks/{i}/type", "position_3d");
                res.Add($"tracks/{i}/path", node);
                res.Add($"tracks/{i}/keys", ListPos.ToArray());
                i++;
                res.Add($"tracks/{i}/type", "rotation_3d");
                res.Add($"tracks/{i}/path", node);
                res.Add($"tracks/{i}/keys", ListRot.ToArray());
                i++;
                res.Add($"tracks/{i}/type", "scale_3d");
                res.Add($"tracks/{i}/path", node);
                res.Add($"tracks/{i}/keys", ListScale.ToArray());
                i++;
            }

            // Blend shape keys
            for (int shape = 0; shape < shapesAmount; shape++)
            {
                NodePath node = new NodePath("BlendSkin", $"blend_shapes/morph_{shape}");
                res.Add($"tracks/{i + shape}/type", "value");
                res.Add($"tracks/{i + shape}/path", node);

                var dict = new Dictionary<object, object>();
                List<float> times = new List<float>() { 0f };
                dict.Add("times", times.ToArray());
                List<object> values = new List<object>() { 0f };
                dict.Add("values", values.ToArray());
                res.Add($"tracks/{i + shape}/keys", dict);
            }

            Resources.Add(res);
        }


    }
}