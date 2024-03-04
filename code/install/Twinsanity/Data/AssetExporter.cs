using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Twinsanity;
using System.Diagnostics;

namespace RehabSetup
{
    public class AssetExporter
    {
        public int FilesLeft = 0;
        public int TotalFiles = 0;
        public bool Exporting = true;
        public bool isISO = false;
        public bool isPAL = false;
        public bool isJPN = false;
        public bool isPS2 = false;
        public bool isDemo = false;
        public string InputPath = string.Empty; // folder path to game files
        public string OutputPath = "import";//string.Empty;
        BackgroundWorker Worker;
        Dictionary<string, List<string>> FilePaths;
        List<string> XMVPaths;
        public XISO ISO;
        public ISO9660 ISO_PS2;
        public string ISOpath = string.Empty;
        public string GodotPath = string.Empty;
        public string ZipPath => Rehab.RehabGame.DataPath + "DataRehab";

        public int VideosLeft = 0;
        public int TotalVideos = 0;
        public int LevelsLeft = 0;
        public int TotalLevels = 0;
        public int PSMLeft = 0;
        public int TotalPSM = 0;
        int AddPercent = 0;
        public ProcessStages Stage = ProcessStages.Prepare;

        static List<string> IgnoreTXT = new List<string>() { "command.txt", "levelselect.txt" };
        static List<string> FirstXMV = new List<string>() { "ttident.xmv" };
            //"h01_a.xmv", "h02_n.xmv", "ttident.xmv", "vivendi.xmv", };

        static List<string> XISO_Ignore_Ext = new List<string>() {
            ".ptl", ".txt", ".xbe", ".xmh", ".xmv", ".psf", ".dir", "", ".geom", ".geo", ".ma2", ".su2",
        };

        static List<string> XISO_Ignore_Name = new List<string>(){
            "dsstdfx.bin",
        };

        static List<string> ISO_Allow_Ext = new List<string>() {
            ".rm2", ".sm2", ".psm", ".psf", ".ptc", ".bin", ".bd", ".bh", ".mb", ".mh", ".msb", ".msh", ".txt"
        };
        static List<string> XISO_Allow_Ext = new List<string>() {
            ".rmx", ".smx", ".psm", ".psf", ".ptc", ".bin", ".xwb", ".txt"
        };

        public event EventHandler<int> WorkerProgressChanged;
        public event EventHandler WorkerFinished;

        public static Dictionary<string, (uint, uint, byte[])> BufferFiles = new(); // Files from ISO (name => offset, size, data)
        public byte[] BufferBD = null; // full CRASH.BD ref on PS2, or full ISO data on XBOX
        public static Dictionary<string, byte[]> Cache = new(); // all exported files

        public enum ProcessStages
        {
            Prepare = 0,
            ExtractISO = 1,
            ExtractAssets = 2,
            InstallMods = 3,
            PackAssets = 4,
            End = 5,
        }

        public AssetExporter()
        {
            FilePaths = new Dictionary<string, List<string>>();
            Worker = new BackgroundWorker();
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += Worker_DoWork;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.ProgressChanged += Worker_ProgressChanged;
            XMVPaths = new List<string>();
            ISO = new XISO();
            //ISO.IgnoreExt = XISO_Ignore_Ext;
            //ISO.IgnoreName = XISO_Ignore_Name;
            ISO.AllowExt = XISO_Allow_Ext;
            ISO_PS2 = new ISO9660();
            ISO_PS2.AllowExt = ISO_Allow_Ext;
            BufferFiles.Clear();
            BufferBD = null;
            AddPercent = 0;
            TwinsSection.ResetCache();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void StartWorker(string inPath)
        {
            InputPath = inPath;
            //OutputPath = outPath;
            Worker.RunWorkerAsync();
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (WorkerProgressChanged != null)
            {
                WorkerProgressChanged.Invoke(this, 100);
            }
            if (WorkerFinished != null)
            {
                WorkerFinished.Invoke(this, null);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (WorkerProgressChanged != null)
            {
                WorkerProgressChanged.Invoke(this, e.ProgressPercentage);
            }
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Exporting = true;
            StartExport();

            while (Exporting)
            {
                Thread.Sleep(100);
                if (TotalFiles != 0)
                {
                    int FilesPercent = (int)((1f - (FilesLeft / (float)TotalFiles)) * 100);
                    FilesPercent = Math.Clamp(FilesPercent, 0, 90);
                    Worker.ReportProgress(FilesPercent + AddPercent);
                }
            }
            Worker.ReportProgress(100);
            
        }

        async void StartExport()
        {
            string RMext = ".rmx";
            if (isPS2)
            { 
                RMext = ".rm2";
            }
            Stage = ProcessStages.Prepare;
            Stopwatch timer = new();
            timer.Start();
            Debug.WriteLine($"Preparing...");

            Stage = ProcessStages.ExtractISO;

            if (isISO)
            {
                Debug.WriteLine($"Done in {timer.Elapsed}. Extracting ISO...");
                timer.Restart();
                if (isPS2)
                {
                    await ISO_PS2.ExportISO(ISOpath);
                }
                else
                {
                    await ISO.ExportISO(ISOpath);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Stage = ProcessStages.ExtractAssets;
            AddPercent = 5;
            Debug.WriteLine($"Done in {timer.Elapsed}. Extracting default...");
            timer.Restart();
            ExportGodot.IsJPN = isJPN;

            #region Extract Assets

            if (isPS2)
            {
                byte[] bh = null;
                foreach (var pair in BufferFiles)
                {
                    if (pair.Key.ToLower().EndsWith(".bd"))
                    {
                        BufferBD = pair.Value.Item3;
                    }
                    else if (pair.Key.ToLower().EndsWith(".bh"))
                    {
                        bh = pair.Value.Item3;
                    }
                }
                await ExportBD(BufferBD, bh);
            }

            Task DefaultTask = null;
            foreach (var pair in BufferFiles)
            {
                if (pair.Key.ToLower().EndsWith($"default{RMext}"))
                {
                    DefaultTask = ExportDefault(pair);
                    LevelsLeft++;
                    TotalLevels++;
                    break;
                }
            }
            if (DefaultTask != null)
                await DefaultTask;

            Debug.WriteLine($"Done in {timer.Elapsed}. Extracting assets...");
            timer.Restart();

            IList<Task> TaskList = new List<Task>();
            foreach (var pair in BufferFiles)
            {
                if (pair.Key.ToLower().EndsWith(RMext) && !pair.Key.ToLower().EndsWith($"default{RMext}"))
                {
                    TaskList.Add(ExportLevel(pair));
                    LevelsLeft++;
                    TotalLevels++;
                }
                else if (pair.Key.ToLower().EndsWith(".psm"))
                {
                    TaskList.Add(ExportPSM(pair));
                    PSMLeft++;
                    TotalPSM++;
                }
                else if (pair.Key.ToLower().EndsWith(".psf"))
                {
                    TaskList.Add(ExportPSF(pair));
                    PSMLeft++;
                    TotalPSM++;
                }
                else if (pair.Key.ToLower().EndsWith(".ptc"))
                {
                    TaskList.Add(ExportPTC(pair));
                    PSMLeft++;
                    TotalPSM++;
                }
                else if (pair.Key.ToLower().EndsWith(".mb"))
                {
                    TaskList.Add(ExportMB(pair));
                }
                else if (pair.Key.ToLower().EndsWith(".msb") && pair.Key.ToLower().Contains("music"))
                {
                    TaskList.Add(ExportMSB(pair));
                }
                else if (pair.Key.ToLower().EndsWith(".xwb"))
                {
                    TaskList.Add(ExportXWB(pair));
                }
                else if (pair.Key.ToLower().EndsWith(".bin"))
                {
                    if (pair.Key.ToLower().Contains("frontend"))
                    {
                        TaskList.Add(ExportFrontend(pair));
                    }
                }
                else if (pair.Key.ToLower().EndsWith(".txt") && pair.Key.ToLower().Contains("language") 
                && pair.Key.ToLower().Contains("credits") && (pair.Key.ToLower().Contains("english") || pair.Key.ToLower().Contains("american")))
                {
                    TaskList.Add(ExportCredits(pair));
                    PSMLeft++;
                    TotalPSM++;
                }
            }
            TaskList.Add(ExportBuildInfo());
            PSMLeft++;
            TotalPSM++;
            TotalFiles += TaskList.Count;
            FilesLeft += TaskList.Count;

            await Task.WhenAll(TaskList);
            TaskList.Clear();
            #endregion

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stage = ProcessStages.PackAssets;
            Debug.WriteLine($"Done in {timer.Elapsed}. Packing assets...");
            timer.Restart();

            await PackAssets();

            Stage = ProcessStages.End;
            Debug.WriteLine($"Done in {timer.Elapsed}. Finishing up...");
            timer.Restart();
            
            BufferBD = null;
            Cache.Clear();
            BufferFiles.Clear();
            TwinsSection.ResetCache();
            timer.Stop();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Debug.WriteLine("Complete!");
            Exporting = false;
            
        }

        async Task ExportLevel(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    string smName = pair.Key.Replace("rm","sm").Replace("RM","SM");
                    MemoryStream RMstream = GetFile(pair);
                    MemoryStream SMstream = GetFile(smName);
                    TwinsFile RM = new TwinsFile();
                    TwinsFile SM = new TwinsFile();

                    TwinsFile.FileType TypeRM = TwinsFile.FileType.RM2;
                    TwinsFile.FileType TypeSM = TwinsFile.FileType.SM2;
                    if (isDemo)
                    {
                        TypeRM = TwinsFile.FileType.DemoRM2;
                        TypeSM = TwinsFile.FileType.DemoSM2;
                    }
                    else if (!isPS2)
                    {
                        TypeRM = TwinsFile.FileType.RMX;
                        TypeSM = TwinsFile.FileType.SMX;
                    }

                    try
                    {
                        RM.LoadFileStream(new BinaryReader(RMstream), TypeRM, pair.Key);
                        SM.LoadFileStream(new BinaryReader(SMstream), TypeSM, smName);

                        ExportGodot.ExportFull(RM, SM, OutputPath, true, null, true, false);
                    }
                    catch
                    {
                        Debug.WriteLine($"[AssetExporter] File failed: {pair.Key}");
                    }

                    LevelsLeft--;
                    FilesLeft--;
                    RMstream.Close();
                    RMstream.Dispose();
                    SMstream.Close();
                    SMstream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    BufferFiles.Remove(smName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportDefault(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile.FileType TypeRM = TwinsFile.FileType.RM2;
                    if (isDemo)
                        TypeRM = TwinsFile.FileType.DemoRM2;
                    else if (!isPS2)
                        TypeRM = TwinsFile.FileType.RMX;

                    MemoryStream RMstream = GetFile(pair);
                    TwinsFile RM = new TwinsFile();
                    RM.LoadFileStream(new BinaryReader(RMstream), TypeRM, pair.Key);
                    ExportGodot.ExportRM(RM, OutputPath, true, null);
                    LevelsLeft--;
                    FilesLeft--;
                    RMstream.Close();
                    RMstream.Dispose();
                    
                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportFrontend(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    TwinsFile BIN = new TwinsFile();
                    BIN.LoadFileStream(new BinaryReader(stream), isPS2 ? TwinsFile.FileType.BIN : TwinsFile.FileType.BIN_XBOX, pair.Key);
                    ExportGodot.ExportBIN(BIN, OutputPath);
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportPSM(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile.FileType TypePSM = TwinsFile.FileType.PSM;
                    if (isDemo)
                        TypePSM = TwinsFile.FileType.DemoPSM;
                    else if (!isPS2)
                        TypePSM = TwinsFile.FileType.PSM_XBOX;

                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), TypePSM, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportPTC(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile.FileType TypePSM = TwinsFile.FileType.PTC;
                    if (isDemo)
                        TypePSM = TwinsFile.FileType.DemoPTC;
                    else if (!isPS2)
                        TypePSM = TwinsFile.FileType.PTC_XBOX;

                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), TypePSM, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath, true);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportPSF(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile.FileType TypePSM = TwinsFile.FileType.PSF;
                    if (isDemo)
                        TypePSM = TwinsFile.FileType.DemoPSF;
                    else if (!isPS2)
                        TypePSM = TwinsFile.FileType.PSF_XBOX;

                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), TypePSM, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath, false, true);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportXWB(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    TwinsFile XWB = new TwinsFile();
                    XWB.LoadFileStream(new BinaryReader(stream), TwinsFile.FileType.XWB, pair.Key);
                    ExportGodot.ExportXWB(XWB, OutputPath);
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportMB(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    string mhName = pair.Key.Replace("mb","mh").Replace("MB","MH");
                    MemoryStream mbstream = GetFile(pair);
                    MemoryStream mhstream = GetFile(mhName);
                    TwinsFile Hash = new TwinsFile();
                    Hash.LoadFileStream(new BinaryReader(mhstream), TwinsFile.FileType.MH, mhName);
                    TwinsFile MB = new TwinsFile();
                    MB.musicHash = (Twinsanity.Items.MusicHash)Hash.Records[0];
                    MB.LoadFileStream(new BinaryReader(mbstream), TwinsFile.FileType.MB, pair.Key);
                    ExportGodot.ExportMB(MB, OutputPath);
                    FilesLeft--;
                    mbstream.Close();
                    mbstream.Dispose();
                    mhstream.Close();
                    mhstream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    BufferFiles.Remove(mhName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportMSB(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    string mhName = pair.Key.Replace("msb","msh").Replace("MSB","MSH");
                    MemoryStream mbstream = GetFile(pair);
                    MemoryStream mhstream = GetFile(mhName);
                    TwinsFile Hash = new TwinsFile();
                    Hash.LoadFileStream(new BinaryReader(mhstream), TwinsFile.FileType.MSH, mhName);
                    TwinsFile MB = new TwinsFile();
                    MB.musicHashDemo = (Twinsanity.Items.MusicHashDemo)Hash.Records[0];
                    MB.LoadFileStream(new BinaryReader(mbstream), TwinsFile.FileType.MSB, pair.Key);
                    ExportGodot.ExportMSB(MB, OutputPath);
                    FilesLeft--;
                    mbstream.Close();
                    mbstream.Dispose();
                    mhstream.Close();
                    mhstream.Dispose();

                    BufferFiles.Remove(pair.Key);
                    BufferFiles.Remove(mhName);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportCredits(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    ExportGodot.ExportCredits(stream, OutputPath);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();
                    BufferFiles.Remove(pair.Key);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportBuildInfo()
        {
            await Task.Run(
                () =>
                {
                    using (MemoryStream mStream = new())
                    {
                        string versionString = "PS2";
                        if (!isPS2) versionString = "XBOX";
                        if (isDemo) versionString = "DEMO";
                        using (StreamWriter writer = new StreamWriter(mStream, null, -1, true))
                        {
                            writer.WriteLineAsync("[mod]");
                            writer.WriteLineAsync("");
                            writer.WriteLineAsync($"name=\"Game Data ({versionString})\"");
                            writer.WriteLineAsync($"IsPS2={isPS2.ToString().ToLower()}");
                            writer.WriteLineAsync($"IsDemo={isDemo.ToString().ToLower()}");
                            writer.WriteLineAsync($"IsPAL={isPAL.ToString().ToLower()}");
                            writer.WriteLineAsync($"IsJPN={isJPN.ToString().ToLower()}");
                        }
                        mStream.Position = 0;
                        Add($"{OutputPath}/Mods/Base{versionString}.cfg", mStream.ToArray());
                    }
                    PSMLeft--;
                    FilesLeft--;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                );
        }

        async Task ExportBD(byte[] bd, byte[] bh)
        {
            BD_Archive BD = new BD_Archive();
            await BD.ExtractAsync(bd, bh);
        }

        async Task PackAssets()
        {
            string versionString = "PS2";
            if (!isPS2) versionString = "XBOX";
            if (isDemo) versionString = "DEMO";
            string path = $"{ZipPath}{versionString}.pcz";
            using (FileStream zipStream = new FileStream(path, FileMode.Create))
            {
                using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    int items = Cache.Count;
                    int itemsLeft = Cache.Count;
                    foreach (var item in Cache)
                    {
                        //var entry = zip.CreateEntry(item.Key, CompressionLevel.Fastest);
                        var entry = zip.CreateEntry(item.Key, CompressionLevel.NoCompression);
                        using (var stream = entry.Open())
                        {
                            await stream.WriteAsync(item.Value);
                        }
                        itemsLeft--;
                        AddPercent = 5 + (int)((1f - (itemsLeft / (float)items)) * 5);
                    }
                }
            }

            BufferBD = null;
            Cache.Clear();
            BufferFiles.Clear();
            TwinsSection.ResetCache();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public bool DetectXBE(string inputPath)
        {
            bool Check = false;
            try
            {
                Check = ISO.DetectXBE(inputPath);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            if (Check)
            {
                isPS2 = false;
                isDemo = false;
                isJPN = false;
                isPAL = ISO.IsPAL;
                if (inputPath.ToLower().EndsWith(".iso"))
                {
                    isISO = true;
                    ISOpath = inputPath;
                }
            }
            else
            {
                isISO = false;
                isPAL = false;
                isDemo = false;
                isPS2 = false;
                isJPN = false;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Check;
        }

        public bool DetectPS2(string inputPath)
        {
            bool Check = false;
            try
            {
                Check = ISO_PS2.DetectPS2(inputPath);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            if (Check)
            {
                isPS2 = true;
                isPAL = ISO_PS2.Version == ISO9660.GameVersion.EUR || ISO_PS2.Version == ISO9660.GameVersion.DEMO_EUR;
                isDemo = ISO_PS2.Version == ISO9660.GameVersion.DEMO_USA || ISO_PS2.Version == ISO9660.GameVersion.DEMO_EUR;
                isJPN = ISO_PS2.Version == ISO9660.GameVersion.JPN;
                if (inputPath.ToLower().EndsWith(".iso"))
                {
                    isISO = true;
                    ISOpath = inputPath;
                }
            }
            else
            {
                isISO = false;
                isPAL = false;
                isDemo = false;
                isPS2 = false;
                isJPN = false;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Check;
        }

        public MemoryStream GetFile(string name)
        {
            if (!BufferFiles.ContainsKey(name)) return null;
            if (BufferFiles[name].Item1 != 0)
            {
                return new MemoryStream(BufferBD, (int)BufferFiles[name].Item1 - 1, (int)BufferFiles[name].Item2);
            }
            else
            {
                return new MemoryStream(BufferFiles[name].Item3);
            }
        }

        public MemoryStream GetFile(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            if (!BufferFiles.ContainsKey(pair.Key)) return null;
            if (pair.Value.Item1 != 0)
            {
                return new MemoryStream(BufferBD, (int)pair.Value.Item1 - 1, (int)pair.Value.Item2);
            }
            else
            {
                return new MemoryStream(pair.Value.Item3);
            }
        }

        public static bool Check(string name)
        {
            lock (Cache)
            {
                return Cache.ContainsKey(name.Replace('\\','/').Replace("//","/"));
            }
        }
        public static void Add(string name, byte[] data)
        {
            lock (Cache)
            {
                Cache.TryAdd(name.Replace('\\','/').Replace("//","/"), data);
            }
        }

    }
}
