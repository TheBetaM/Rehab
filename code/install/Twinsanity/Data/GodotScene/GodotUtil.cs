using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Twinsanity;

namespace RehabSetup
{
    public static class GodotUtil
    {

        public static void GetRestPose(GodotSceneFile.Node RootNode, GraphicsInfo GI)
        {
            Dictionary<int, Matrix4x4> RestPoses = new();

            ComputeTposeTransform(GI, 0, Matrix4x4.Identity, ref RestPoses);

            for (int i = 0; i < GI.Joints.Length; i++)
            {
                int ParentJoint = (int)GI.Joints[i].ParentJointIndex;
                if (ParentJoint == 255) ParentJoint = -1;

                string RestPoseString = MatrixToTransform(RestPoses[i]);
                Matrix4x4.Decompose(RestPoses[i], out Vector3 scale, out Quaternion rot, out Vector3 pos);

                RootNode.Lines.Add($"bones/{i}/name=\"joint{i}\"");
                RootNode.Lines.Add($"bones/{i}/parent={ParentJoint}");
                RootNode.Lines.Add($"bones/{i}/rest={RestPoseString}");
                //RootNode.Lines.Add($"bones/{i}/enabled=true");
                RootNode.Lines.Add($"bones/{i}/position=Vector3({pos.X.ToText()},{pos.Y.ToText()},{pos.Z.ToText()})");
                RootNode.Lines.Add($"bones/{i}/rotation=Quaternion({rot.X.ToText()},{rot.Y.ToText()},{rot.Z.ToText()},{rot.W.ToText()})");
                RootNode.Lines.Add($"bones/{i}/scale=Vector3({scale.X.ToText()},{scale.Y.ToText()},{scale.Z.ToText()})");
                //RootNode.Lines.Add($"bones/{i}/bound_children = [  ]");
            }


        }

        public static string MatrixToTransform(Matrix4x4 Matrix)
        {
            //Transform( 1, 0, 0 | 0, 1, 0 | 0, 0, 1 | 0, 0, 0 )");
            // godot doesn't like strings like 5,960464E-08, has to be 5.960464e-08
            List<string> Values = new List<string>();
            Values.Add((Matrix.M11).ToText());
            Values.Add((Matrix.M21).ToText());
            Values.Add((Matrix.M31).ToText());

            Values.Add((Matrix.M12).ToText());
            Values.Add((Matrix.M22).ToText());
            Values.Add((Matrix.M32).ToText());

            Values.Add((Matrix.M13).ToText());
            Values.Add((Matrix.M23).ToText());
            Values.Add((Matrix.M33).ToText());

            Values.Add(Matrix.M41.ToText());
            Values.Add(Matrix.M42.ToText());
            Values.Add(Matrix.M43.ToText());

            string outMatrix = $"{ExportGodot.Transform3D}(";
            for (int i = 0; i < Values.Count - 1; i++)
            {
                outMatrix += $"{Values[i]},";
            }
            outMatrix += $"{Values[Values.Count - 1]})";
            return outMatrix;
        }

        public static void ComputeTposeTransform(GraphicsInfo graphicsInfo, uint index, Matrix4x4 parentTransform, ref Dictionary<int, Matrix4x4> RestPoses)
        {
            var joint = graphicsInfo.Joints[index];
            var localRot = Matrix4x4.CreateFromQuaternion(new Quaternion(
                -graphicsInfo.Joints[index].Matrix[2].X,
                graphicsInfo.Joints[index].Matrix[2].Y,
                graphicsInfo.Joints[index].Matrix[2].Z,
                -graphicsInfo.Joints[index].Matrix[2].W));
            var localTranslate = Matrix4x4.CreateTranslation(
                    -graphicsInfo.Joints[index].Matrix[0].X,
                    graphicsInfo.Joints[index].Matrix[0].Y,
                    graphicsInfo.Joints[index].Matrix[0].Z);
            var localTransform = localRot * localTranslate;
            var jointTransform = localTransform;// * parentTransform;
            var addRot = new Quaternion(graphicsInfo.Joints[index].Matrix[4].X, 
                -graphicsInfo.Joints[index].Matrix[4].Y,
                -graphicsInfo.Joints[index].Matrix[4].Z,
                graphicsInfo.Joints[index].Matrix[4].W);
            //var rotMat = Matrix4x4.CreateFromQuaternion(addRot);
            var bindMat = jointTransform;
            //var skeleton = graphicsInfo.Skeleton;
            //Matrix4x4.Decompose(bindMat, out Vector3 scale, out Quaternion rot, out Vector3 pos);
            RestPoses.Add((int)index, bindMat);
            //skeleton.BindPose.Set((int)index, bindMat.Translation, bindMat.ExtractRotation());
            //Matrix4x4.Invert(bindMat, out Matrix4x4 invbindMat);
            //skeleton.InverseBindPose.Set((int)index, bindMat.ExtractTranslation(), bindMat.ExtractRotation());
            foreach (var c in joint.Children)
            {
                ComputeTposeTransform(graphicsInfo, c.JointIndex, jointTransform, ref RestPoses);
            }
        }

        public static void GetMainAnimationTransform(Animation Data, int jointIndex, int curFrame, int nextFrame,
        Vector3 parentScale, 
        Pos addRotVectorPos, out Vector3 outPos, out Vector4 outRot, out Vector3 outScale, out Vector3 rawScale)
        {
            Vector3 translation = new Vector3();
            Vector3 scale = new Vector3();
            var jointSetting = Data.JointsSettings[jointIndex];
            var useAddRot = (jointSetting.Flags >> 0xC & 0x1) != 0;
            bool useParentScale = (jointSetting.Flags >> 0xD & 0x1) != 0;
            var transformIndex = jointSetting.TransformationIndex;
            int currentFrameTransformIndex = jointSetting.AnimatedTransformIndex;
            var nextFrameTransformIndex = jointSetting.AnimatedTransformIndex;
            var transformChoice = jointSetting.TransformationChoice;
            var translateXChoice = (transformChoice & 0x1) == 0;
            var translateYChoice = (transformChoice & 0x2) == 0;
            var translateZChoice = (transformChoice & 0x4) == 0;
            var rotXChoice = (transformChoice & 0x8) == 0;
            var rotYChoice = (transformChoice & 0x10) == 0;
            var rotZChoice = (transformChoice & 0x20) == 0;
            var scaleXChoice = (transformChoice & 0x40) == 0;
            var scaleYChoice = (transformChoice & 0x80) == 0;
            var scaleZChoice = (transformChoice & 0x100) == 0;

            var endRotX1 = 0.0f;
            var endRotY1 = 0.0f;
            var endRotZ1 = 0.0f;
            var endRotX2 = 0.0f;
            var endRotY2 = 0.0f;
            var endRotZ2 = 0.0f;


            if (translateXChoice)
            {
                var x1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                translation.X = -x1;
                nextFrameTransformIndex++;
            }
            else
            {
                translation.X = -Data.StaticTransforms[transformIndex++].Value;
            }

            if (translateYChoice)
            {
                var y1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                translation.Y = y1;
                nextFrameTransformIndex++;
            }
            else
            {
                translation.Y = Data.StaticTransforms[transformIndex++].Value;
            }

            if (translateZChoice)
            {
                var z1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                translation.Z = z1;
                nextFrameTransformIndex++;
            }
            else
            {
                translation.Z = Data.StaticTransforms[transformIndex++].Value;
            }

            if (rotXChoice)
            {
                var rot1 = Data.AnimatedTransforms[curFrame].GetPureOffset(currentFrameTransformIndex++) * 16;
                var rot2 = Data.AnimatedTransforms[nextFrame].GetPureOffset(nextFrameTransformIndex++) * 16;
                var diff = rot1 - rot2;
                if (diff < -0x8000)
                {
                    rot1 += 0x10000;
                }
                if (diff > 0x8000)
                {
                    rot1 -= 0x10000;
                }
                var rot1Rad = rot1 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                var rot2Rad = rot2 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                endRotX1 = rot1Rad;
                endRotX2 = rot2Rad;
            }
            else
            {
                var rot = Data.StaticTransforms[transformIndex++].GetRot(false);
                endRotX1 = rot;
                endRotX2 = rot;
            }

            if (rotYChoice)
            {
                var rot1 = Data.AnimatedTransforms[curFrame].GetPureOffset(currentFrameTransformIndex++) * 16;
                var rot2 = Data.AnimatedTransforms[nextFrame].GetPureOffset(nextFrameTransformIndex++) * 16;
                var diff = rot1 - rot2;
                if (diff < -0x8000)
                {
                    rot1 += 0x10000;
                }
                if (diff > 0x8000)
                {
                    rot1 -= 0x10000;
                }
                var rot1Rad = rot1 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                var rot2Rad = rot2 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                endRotY1 = rot1Rad;
                endRotY2 = rot2Rad;
            }
            else
            {
                var rot = Data.StaticTransforms[transformIndex++].GetRot(false);
                endRotY1 = rot;
                endRotY2 = rot;
            }

            if (rotZChoice)
            {
                var rot1 = Data.AnimatedTransforms[curFrame].GetPureOffset(currentFrameTransformIndex++) * 16;
                var rot2 = Data.AnimatedTransforms[nextFrame].GetPureOffset(nextFrameTransformIndex++) * 16;
                var diff = rot1 - rot2;
                if (diff < -0x8000)
                {
                    rot1 += 0x10000;
                }
                if (diff > 0x8000)
                {
                    rot1 -= 0x10000;
                }
                var rot1Rad = rot1 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                var rot2Rad = rot2 / (float)(ushort.MaxValue + 1) * (float)Math.PI * 2;
                endRotZ1 = rot1Rad;
                endRotZ2 = rot2Rad;
            }
            else
            {
                var rot = Data.StaticTransforms[transformIndex++].GetRot(false);
                endRotZ1 = rot;
                endRotZ2 = rot;
            }

            if (scaleXChoice)
            {
                var x1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                scale.X = x1;
                nextFrameTransformIndex++;
            }
            else
            {
                scale.X = Data.StaticTransforms[transformIndex++].Value;
            }

            if (scaleYChoice)
            {
                var y1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                scale.Y = y1;
                nextFrameTransformIndex++;
            }
            else
            {
                scale.Y = Data.StaticTransforms[transformIndex++].Value;
            }

            if (scaleZChoice)
            {
                var z1 = Data.AnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                scale.Z = z1;
                nextFrameTransformIndex++;
            }
            else
            {
                scale.Z = Data.StaticTransforms[transformIndex++].Value;
            }

            var rotX = Matrix4x4.CreateRotationX(endRotX1);
            var rotY = Matrix4x4.CreateRotationY(endRotY1);
            var rotZ = Matrix4x4.CreateRotationZ(endRotZ1);
            var endRot = rotX * rotY * rotZ;
            var rotQuat = Quaternion.CreateFromRotationMatrix(endRot);
            rotQuat = new Quaternion(-rotQuat.X, rotQuat.Y, rotQuat.Z, -rotQuat.W);
            //var rotQuat = Quaternion.CreateFromYawPitchRoll(endRotY1, endRotX1, -endRotZ1);
            var addRotQuat = new Quaternion(-addRotVectorPos.X, addRotVectorPos.Y, addRotVectorPos.Z, -addRotVectorPos.W);
            var mulQuat = rotQuat;
            if (useAddRot)
                mulQuat = Quaternion.Multiply(addRotQuat, rotQuat);
            //var mulQuat = rotQuat;
            //mulQuat = Quaternion.Normalize(mulQuat);

            var resultScale = scale;
            if (useParentScale)
                resultScale = new Vector3(scale.X / parentScale.X, scale.Y / parentScale.Y, scale.Z / parentScale.Z);;
            if (float.IsNaN(resultScale.X))
                resultScale.X = 0f;
            if (float.IsNaN(resultScale.Y))
                resultScale.Y = 0f;
            if (float.IsNaN(resultScale.Z))
                resultScale.Z = 0f;

            var localRot = Matrix4x4.CreateFromQuaternion(mulQuat);
            var localTranslate = Matrix4x4.CreateTranslation(translation);
            //var localScale = Matrix4x4.CreateScale(resultScale);
            var localTransform = localRot * localTranslate;

            Matrix4x4.Decompose(localTransform, out var tscale, out var rotFix, out var tpos);
            outPos = tpos;
            //outRot = Vector4.Normalize(new Vector4(rotFix.X, rotFix.Y, rotFix.Z, rotFix.W));
            outRot = new Vector4(rotFix.X, rotFix.Y, rotFix.Z, rotFix.W);
            outScale = resultScale;
            rawScale = scale;
            return;
        }

        public static float[] GetFacialAnimationTransform(Animation Data, int curFrame)
        {
            var jointSetting = Data.FacialJointsSettings[0];
            var shapesAmount = ((jointSetting.Flags >> 0x8) & 0xf);
            var shapeWeights = new float[shapesAmount];
            var transformIndex = jointSetting.TransformationIndex;
            var currentFrameTransformIndex = jointSetting.AnimatedTransformIndex;
            var nextFrameTransformIndex = jointSetting.AnimatedTransformIndex;
            var transformChoice = jointSetting.TransformationChoice;

            for (int i = 0; i < shapeWeights.Length; i++)
            {
                if ((transformChoice & 0x1) == 0)
                {
                    //var f1 = Data.FacialAnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                    //var f2 = Data.FacialAnimatedTransforms[nextFrame].GetOffset(nextFrameTransformIndex++);
                    //shapeWeights[i] = VectorFuncs.Lerp(f1, f2, frameDisplacement);
                    shapeWeights[i] = Data.FacialAnimatedTransforms[curFrame].GetOffset(currentFrameTransformIndex++);
                }
                else
                {
                    shapeWeights[i] = Data.FacialStaticTransforms[transformIndex++].Value;
                }
                transformChoice >>= 1;
            }

            return shapeWeights;
        }


    }
}