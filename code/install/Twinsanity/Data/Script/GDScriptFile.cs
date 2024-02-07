using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Twinsanity;

namespace RehabSetup
{
    public static class GDScriptFile
    {

        public static void WriteToFile(Script script, string path)
        {
            if (File.Exists(path)) return;
            var scriptText = Serialize(script);
            try
            {
                if (File.Exists(path)) return;
                File.WriteAllLines(path, scriptText.FileLines);
            }
            catch
            {
                // race condition
            }
        }

        public static GDScript Serialize(Script script)
        {
            var scriptText = new GDScript();
            scriptText.Serialize(script);
            return scriptText;            
        }

        public static void WriteToFile(CustomAgent script, string path, string actorName)
        {
            for (int i = 0; i < script.scriptIds.Count; i++)
            {
                try
                {
                    var scr_name = DefaultHashes.Hash_Scripts[script.scriptIds[i]];
                    string scriptPath = $"{path}{actorName}_{scr_name}.gd";
                    if (File.Exists(scriptPath)) return;
                    var scriptText = Serialize(script.scriptIds[i], script.agentLabAdditionsList[i], actorName);
                    File.WriteAllLines(scriptPath, scriptText.FileLines);
                }
                catch
                {
                    // race condition
                }
            }
            
        }

        public static GDScript Serialize(ushort id, CustomAgent.AgentLabAdditions script, string actorName)
        {
            var scriptText = new GDScript();
            scriptText.Serialize(id, script, actorName);
            return scriptText;            
        }
        

    }

    public class GDScript
    {
        public List<string> FileLines = new List<string>();

        public void Serialize(Script script)
        {
            var scriptName = DefaultHashes.Hash_Scripts[script.ID];
            FileLines = new List<string>(){
                "extends ALabScript",
                $"class_name {scriptName}",
                "",
            };

            // todo behaviour starter
            if (script.Main == null) return;
            var main = script.Main;
            
            FileLines.AddRange(new List<string>(){
                "func _init():",
                $"\tUnit = Unit{main.StartUnit}",
                "",
            });

            var unit = main.scriptState1;
            int unitID = 0;
            while (unit != null)
            {
                FileLines.Add($"func Unit{unitID}():");
                if (unit.scriptIndexOrSlot != -1)
                {
                    if (unit.IsSlot)
                    {
                        FileLines.Add($"\tME.Scripts[{unit.scriptIndexOrSlot}].OnRun(ME)");
                    }
                    else
                    {
                        var linkScriptName = DefaultHashes.Hash_Scripts[(uint)unit.scriptIndexOrSlot];
                        FileLines.Add($"\t{linkScriptName}.new().OnRun(ME)");
                    }
                }
                if (unit.type1 != null)
                {
                    var pack = unit.type1;
                    // todo 5 enums
                    if (pack.Translates) FileLines.Add($"\tME.CTRLPACK.Translates = true");
                    if (pack.Rotates) FileLines.Add($"\tME.CTRLPACK.Rotates = true");
                    if (pack.UsesPhysics) FileLines.Add($"\tME.CTRLPACK.UsesPhysics = true");
                    if (pack.UsesRotator) FileLines.Add($"\tME.CTRLPACK.UsesRotator = true");
                    if (pack.UsesInterpolator) FileLines.Add($"\tME.CTRLPACK.UsesInterpolator = true");
                    if (pack.InterpolatesAngles) FileLines.Add($"\tME.CTRLPACK.InterpolatesAngles = true");
                    if (pack.TranslationContinues) FileLines.Add($"\tME.CTRLPACK.TranslationContinues = true");
                    if (pack.YawFaces) FileLines.Add($"\tME.CTRLPACK.YawFaces = true");
                    if (pack.PitchFaces) FileLines.Add($"\tME.CTRLPACK.PitchFaces = true");
                    if (pack.OrientsPredicts) FileLines.Add($"\tME.CTRLPACK.OrientsPredicts = true");
                    if (pack.TracksDestination) FileLines.Add($"\tME.CTRLPACK.TracksDestination = true");
                    if (pack.KeyIsLocal) FileLines.Add($"\tME.CTRLPACK.KeyIsLocal = true");
                    if (pack.ContRotatesInWorldSpace) FileLines.Add($"\tME.CTRLPACK.ContRotates = true");
                    if (pack.Stalls) FileLines.Add($"\tME.CTRLPACK.Stalls = true");
                    for (int p = 0; p < ControlPacketNames.Length; p++)
                    {
                        if (pack.bytes.Count > p)
                        {
                            if (pack.bytes[p] != 255)
                            {
                                if (pack.bytes[p] > 127)
                                {
                                    int fIndex = pack.bytes[p] - 128;
                                    FileLines.Add($"\tME.CTRLPACK.{ControlPacketNames[p]} = ME.RegFloat[{fIndex}]");
                                }
                                else
                                {
                                    FileLines.Add($"\tME.CTRLPACK.{ControlPacketNames[p]} = {pack.floats[pack.bytes[p]].ToText()}");
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                var link = unit.scriptStateBody;
                int linkID = 0;
                while (link != null)
                {
                    var cond = link.condition;
                    string NotGate = cond.NotGate ? "true" : "false";
                    string PerceptName = $"Percept_{(DefaultEnums.ConditionID)cond.VTableIndex}";
                    string elseAdd = string.Empty;
                    if (linkID != 0)
                    {
                        elseAdd = "el";
                    }
                    FileLines.Add($"\tPercepts.append({PerceptName}.new({NotGate}, {cond.Parameter}, {cond.Interval.ToText()}, {cond.Threshold.ToText()}, ME, func():");

                    var command = link.command;
                    while (command != null)
                    {
                        string ActionName = $"Action_{(DefaultEnums.CommandID)command.VTableIndex}";
                        FileLines.Add($"\t\t{ActionName}.run(ME)");
                        command = command.nextCommand;
                    }
                    

                    FileLines.Add($"\t\tUnit = Unit{link.scriptStateListIndex} ))");
                    //FileLines.Add($"\t\tUnit.call()");
                    //FileLines.Add($"\t\t))");
                    link = link.nextScriptStateBody;
                    linkID++;
                }
                if (linkID == 0)
                {
                    FileLines.Add($"\tpass");
                }

                FileLines.Add(string.Empty);
                unit = unit.nextState;
                unitID++;
            }

            

        }

        public void Serialize(ushort scriptID, CustomAgent.AgentLabAdditions script, string actorName)
        {
            var scr_name = DefaultHashes.Hash_Scripts[scriptID];
            var scriptName = $"{actorName}_{scr_name}";
            FileLines = new List<string>(){
                "extends ALabScript",
                $"class_name {scriptName}",
                "",
            };

            if (script.scriptCommandsAmount == 0) return;
            
            FileLines.AddRange(new List<string>(){
                "func _init():",
                $"\tUnit = Unit0",
                "",
                "func Unit0():",
                "\tPercepts.append(Percept_Next.new(false, 0, 0, 0.5, ME, func():",
            });
            var command = script.scriptCommand;
            for (int a = 0; a < script.scriptCommandsAmount; a++)
            {
                string ActionName = $"Action_{(DefaultEnums.CommandID)command.VTableIndex}";
                FileLines.Add($"\t\t{ActionName}.run(ME)");
                command = command.nextCommand;
            }
            FileLines.Add("\t\t))");
            

        }

        static string[] ControlPacketNames = new string[] {
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

    }
}
