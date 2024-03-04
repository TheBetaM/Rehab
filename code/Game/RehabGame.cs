using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
namespace Rehab;

public static class RehabGame
{
    public static bool Dev = false; // development mode flag
    public static bool DemoMode = true;
    public static Dictionary<string, List<int>> ChunkData = new();
    public static int Progress = 0;
    public static int Lives = 4;
    public static int Fruit = 0;
    public static int Crystals = 0;
    public static int LevelID = -1;
    public static Dictionary<int, List<int>> Gems = new();
    public static int PlayerMode = 0;
    public static int PlayerCharacterType = 0;
    public static string SavePointChunk;
    public static Vector3 SavePointPos;
    public static Vector3 SavePointRot;
    public static string CheckPointChunk;
    public static Vector3 CheckPointPos;
    public static Vector3 CheckPointRot;
    public static bool InvertCameraX = false;
    public static bool InvertCameraY = false;
    public static bool UseMouseCamera = true;
    public static VoiceLanguage VoiceLang = VoiceLanguage.English;
    public static string AssetsPath = "res://import/";
    public static string ConfigPath = "user://rehab.cfg";
    public static string DataPath = OS.GetExecutablePath();
    public static List<ModInfo> ModsInstalled = new List<ModInfo>();
    public static GameMode Mode = GameMode.Explorer;

    public static void Init(){
        if (OS.GetName() == "Android")
        {
            //DataPath = OS.GetUserDataDir() + "/";
            DataPath = "/storage/emulated/0/Rehab/Packs";
        }
        else
        {
            var PathSplit = DataPath.Split("/");
            var PacksPath = "";
            var PathID = 0;
            foreach (var i in PathSplit)
            {
                PathID++;
                if (PathID < PathSplit.Length)
                {
                    PacksPath += i + "/";
                }
            }
            PacksPath += "Packs/";
            DataPath = PacksPath;
            if (!DirAccess.DirExistsAbsolute(PacksPath))
                DirAccess.MakeDirAbsolute(PacksPath);
        }

        string locale = OS.GetLocaleLanguage();
        if (TranslationServer.GetLoadedLocales().Contains(locale))
        {
            TranslationServer.SetLocale(locale);
        }
    }

    public static void ResetGame(){
        Fruit = 0;
        Lives = 4;
        Progress = 0;
        Crystals = 0;
        LevelID = -1;
    }

    public static void AddWumpa(int amount){
        Fruit += amount;
        if (Fruit > 99)
        {
            Fruit = 0;
            AddLives(amount);
        }
        if (Fruit < 0) Fruit = 0;
	    RehabScene.GameHUD.AnimateWumpa();
	    RehabScene.GameHUD.UpdateWumpa();
    }

    public static void AddLives(int amount){
        Lives += amount;
        if (Lives > 99) Lives = 99;
        if (Lives < 0)
        {
            Lives = 0;
            RehabScene.Root.ForceGameOver();
        }
        RehabScene.GameHUD.AnimateLife();
	    RehabScene.GameHUD.UpdateLives();
    }

    public static void AddGem(int gem){
        RehabScene.GameHUD.AnimateGem(gem);
        if (Gems.ContainsKey(LevelID))
        {
            if (!Gems[LevelID].Contains(gem))
                Gems[LevelID].Add(gem);
        }
        else
        {
            Gems.Add(LevelID, new() {gem});
        }
    }

    public static void AddCrystal(){
        Crystals++;
        RehabScene.GameHUD.AnimateCrystal();
    }

    public static void DisplayHUD(){
        RehabScene.GameHUD.UpdateWumpa();
	    RehabScene.GameHUD.UpdateLives();
    }

    public static void DisplayMessage(string text){
        RehabScene.GameHUD.FlashMessage(text);
    }

    public static void SetLevelID(int id){
        LevelID = id;
    }

    public static string GetVoicePath(){
        switch (VoiceLang)
        {
            default:
            case VoiceLanguage.English:
                return "English";
            case VoiceLanguage.French:
                return "French";
            case VoiceLanguage.German:
                return "German";
            case VoiceLanguage.Italian:
                return "Italian";
            case VoiceLanguage.Spanish:
                return "Spanish";
            case VoiceLanguage.Japanese:
                return "Japanese";
        }
    }

    public static void SetupMods()
    {
        var conf = RehabScene.Root.GetNode<ConfigHandler>("ConfigHandler");
        var dir = DirAccess.Open(AssetsPath + "Mods/");
        if (dir != null)
        {
            dir.ListDirBegin();
            var file_name = dir.GetNext();
            while (file_name != "")
            {
                if (!dir.CurrentIsDir())
                {
                    var Dict = conf.LoadModInfo(AssetsPath + "Mods/" + file_name);
                    if (Dict.ContainsKey("name"))
                    {
                        ModInfo mod = new ModInfo();
                        mod.Name = (string)Dict["name"];
                        if (Dict.ContainsKey("IsPAL"))
                            mod.IsPAL = (bool)Dict["IsPAL"];
                        ModsInstalled.Add(mod);
                    }
                }
                file_name = dir.GetNext();
            }
        }
    }

    public static Dictionary<int, string> MusicPaths = new Dictionary<int, string>(){
        [0] = "undefined",
        [1] = "DemoHub",
	    [7] = "4_6_Twisted_Docamok",
        [8] = "3_4B_Cortex_Amberley",
        [9] = "2_8_Embryo_Boss_Fight",
        [10] = "1_7_Worm_Chase",
        [27] = "1_1_Nsanity_Island",
        [28] = "1_2_Cavern_Catastrophe",
        [29] = "1_3_Totem_Hokem",
        [30] = "1_4_Mechabandicoot",
        [31] = "1_5_River_Boat_section",
        [32] = "1_6_Totem_God_Boss_Fight",
        [33] = "2_1_Ice_Lab_MT",
        [34] = "2_2_Ice_Climb",
        [35] = "2_3_Uka_Uka_Ice_Creature",
        [36] = "2_5_Boat_Chase",
        [37] = "3_1_Madame_Amberly_School",
        [38] = "3_1_Madame_Amberly_nolaugh",
        [40] = "3_3_Dingodile_Mini_Boss",
        [41] = "3_5_Rooftop_Rampage",
        [53] = "2_1A_Ice_Lab_MT_FASTER",
        [54] = "2_4_Humiliskate",
        [55] = "2_6_Ngin_Mini_Boss_Fight",
        [56] = "2_7_Henchmania",
        [57] = "3_2_Broiler_Room_Doom_2",
        [58] = "3_4A_Crash_Amberley",
        [59] = "3_6_Amberly_Boss_Fight",
        [60] = "4_0_Level_4_Hub",
        [61] = "4_1_Rockslide_Rumble",
        [62] = "4_2_Twisted_Insanity",
        [63] = "4_3_Twins_Compound",
        [64] = "4_4_Twins_Boss_Fight",
        [77] = "LO7_boiler_fan_room_bg_stereo",
        [78] = "LO7_boiler_main_room_bg_stereo",
        [79] = "LO7_boiler_met_spin_rm_bg_stereo",
        [80] = "LO7_boiler_vent_room_bg_stereo",
        [89] = "ocean_waves_stereo",
        [90] = "Jungle_Ambience_Stereo_1",
        [91] = "LO8_outside_ambience_stereo",
        [92] = "LO8_nitro_flood_loop_stereo",
        [103] = "L12_lava_pool_bg_stereo",
        [104] = "L12_lava_cave_bg_stereo",
        [105] = "L12_hanger_bg_stereo",
        [106] = "LO7_tunnel_bg_stereo",
        [107] = "LO7_Newboiler_main_room_bg_stereo",
        [108] = "LO7_Newboiler_met_spin_rm_bg_stereo",
        [109] = "LO7_Newboiler_vent_room_bg_stereo",
        [110] = "LO7_Newboiler_fan_room_bg_stereo",
        [111] = "L09_Amb_Rooftops_Stereo",
        [112] = "H02_Amb_ColdWind_Stereo",
        [113] = "H04_Amb_Wind_Stereo",
        [114] = "L01_Amb_Cave_Stereo",
        [115] = "L02_Amb_Cavern_Stereo",
        [116] = "L04_Amb_IceCavern_Stereo",
        [117] = "L06_Amb_CrowsNest_Stere",
        [118] = "L06_Amb_Henchmania_Stereo",
        [119] = "L06_Amb_ShipInt_Stereo",
        [120] = "L07_Amb_FanRoom_Stereo",
        [121] = "L07_Amb_MainRoom_Stereo",
        [122] = "L07_Amb_MetRoom_Stereo",
        [123] = "L07_Amb_VentRoom_Stereo",
        [124] = "Gen_Amb_Airship_Int_Stereo",
        [136] = "1_8NativeChase",
        [137] = "LO8_UProomtone_stereo",
        [139] = "L05_Chickens",
        [140] = "B01_MechEndfall",
    };

    public enum VoiceLanguage
    {
        English = 0,
        German = 1,
        Spanish = 2,
        French = 3,
        Italian = 4,
        Japanese = 5,
    }

    public enum GameMode
    {
        Explorer = 0,
        Cutscene = 1,
        Minigame = 2,
    }

    public class ModInfo
    {
        public string Name;
        public bool IsPAL;
    }
}
