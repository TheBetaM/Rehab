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
        public bool isPS2 = false;
        public string InputPath = string.Empty; // folder path to game files
        public string OutputPath = "import\\";//string.Empty;
        BackgroundWorker Worker;
        Dictionary<string, List<string>> FilePaths;
        List<string> XMVPaths;
        public XISO ISO;
        public ISO9660 ISO_PS2;
        public string ISOpath = string.Empty;
        public string GodotPath = string.Empty;
        public string ZipPath = AppDomain.CurrentDomain.BaseDirectory + "\\Packs\\RehabData.pcz";
        public string ISO_Extract_Path = AppDomain.CurrentDomain.BaseDirectory + "Packs\\ISO\\";

        public int VideosLeft = 0;
        public int TotalVideos = 0;
        public int LevelsLeft = 0;
        public int TotalLevels = 0;
        public int PSMLeft = 0;
        public int TotalPSM = 0;
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
            ISO_PS2 = new ISO9660();
            BufferFiles.Clear();
            BufferBD = null;
            TwinsSection.ResetCache();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void StartWorker(string inPath, string outPath)
        {
            InputPath = inPath;
            OutputPath = outPath;
            Worker.RunWorkerAsync();
        }

        void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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

        void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (WorkerProgressChanged != null)
            {
                WorkerProgressChanged.Invoke(this, e.ProgressPercentage);
            }
        }

        void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            Exporting = true;
            StartExport();

            while (Exporting)
            {
                Thread.Sleep(100);
                if (TotalFiles != 0)
                {
                    Worker.ReportProgress((int)((1f - (FilesLeft / (float)TotalFiles)) * 100));
                }
            }
            
        }

        async void StartExport()
        {
            string DirPath = InputPath;
            string RMext = ".rmx";
            if (isPS2) RMext= ".rm2";
            Stage = ProcessStages.Prepare;
            Stopwatch timer = new();
            timer.Start();
            Console.WriteLine($"Preparing...");

            Stage = ProcessStages.ExtractISO;

            if (isISO)
            {
                Console.WriteLine($"Extracting ISO... {timer.Elapsed}");
                timer.Restart();
                DirPath = ISO_Extract_Path;
                if (isPS2)
                {
                    await ISO_PS2.ExportISO(ISOpath, ISO_Extract_Path);
                }
                else
                {
                    await ISO.ExportISO(ISOpath, ISO_Extract_Path);
                    Console.WriteLine($"Copying ISO to memory... {timer.Elapsed}");
                    timer.Restart();
                    BufferBD = await File.ReadAllBytesAsync(ISOpath);
                }
            }

            Stage = ProcessStages.ExtractAssets;
            Console.WriteLine($"Extracting assets... {timer.Elapsed}");
            timer.Restart();

            #region Extract Assets
            //DirectoryInfo Dir = new DirectoryInfo(DirPath);
            //FilePaths = new Dictionary<string, List<string>>();
            //Recursive_Batch(Dir, FilePaths);

            if (isPS2)
            {
                
                byte[] bh = null;
                //foreach (string Path in FilePaths[".bd"])
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
                
                //Dir = new DirectoryInfo(DirPath);
                //FilePaths = new Dictionary<string, List<string>>();
                //Recursive_Batch(Dir, FilePaths);
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

            Console.WriteLine($"Extracted default... {timer.Elapsed}");
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
                    //TaskList.Add(ExportMB(pair));
                }
                else if (pair.Key.ToLower().EndsWith(".xwb"))
                {
                    //TaskList.Add(ExportXWB(pair));
                }
                else if (pair.Key.ToLower().EndsWith(".bin"))
                {
                    if (pair.Key.ToLower().Contains("frontend"))
                        TaskList.Add(ExportFrontend(pair));
                }
            }
            TotalFiles += TaskList.Count;
            FilesLeft += TaskList.Count;

            await Task.WhenAll(TaskList);
            TaskList.Clear();
            #endregion

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stage = ProcessStages.PackAssets;
            Console.WriteLine($"Packing assets... {timer.Elapsed}");
            timer.Restart();

            await PackAssets(DirPath);

            Stage = ProcessStages.End;
            Console.WriteLine($"Finishing up... {timer.Elapsed}");
            timer.Restart();
            
            BufferBD = null;
            Cache.Clear();
            BufferFiles.Clear();
            TwinsSection.ResetCache();
            timer.Stop();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Complete!");
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

                    RM.LoadFileStream(new BinaryReader(RMstream), isPS2 ? TwinsFile.FileType.RM2 : TwinsFile.FileType.RMX, pair.Key);
                    SM.LoadFileStream(new BinaryReader(SMstream), isPS2 ? TwinsFile.FileType.SM2 : TwinsFile.FileType.SMX, smName);

                    ExportGodot.ExportFull(RM, SM, OutputPath, true, null, true, false);

                    LevelsLeft--;
                    FilesLeft--;
                    RMstream.Close();
                    RMstream.Dispose();
                    SMstream.Close();
                    SMstream.Dispose();
                }
                );
        }

        async Task ExportDefault(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream RMstream = GetFile(pair);
                    TwinsFile RM = new TwinsFile();
                    RM.LoadFileStream(new BinaryReader(RMstream), isPS2 ? TwinsFile.FileType.RM2 : TwinsFile.FileType.RMX, pair.Key);
                    ExportGodot.ExportRM(RM, OutputPath, true, null);
                    LevelsLeft--;
                    FilesLeft--;
                    RMstream.Close();
                    RMstream.Dispose();
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
                }
                );
        }

        async Task ExportPSM(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), isPS2 ? TwinsFile.FileType.PSM : TwinsFile.FileType.PSM_XBOX, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();
                }
                );
        }

        async Task ExportPTC(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), isPS2 ? TwinsFile.FileType.PTC : TwinsFile.FileType.PTC_XBOX, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath, true);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();
                }
                );
        }

        async Task ExportPSF(KeyValuePair<string, (uint, uint, byte[])> pair)
        {
            await Task.Run(
                () =>
                {
                    MemoryStream stream = GetFile(pair);
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFileStream(new BinaryReader(stream), isPS2 ? TwinsFile.FileType.PSF : TwinsFile.FileType.PSF_XBOX, pair.Key);
                    ExportGodot.ExportPSM(PSM, OutputPath, false, true);
                    PSMLeft--;
                    FilesLeft--;
                    stream.Close();
                    stream.Dispose();
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
                }
                );
        }

        async Task ExportBD(byte[] bd, byte[] bh)
        {
            BD_Archive BD = new BD_Archive();
            await BD.ExtractAsync(bd, bh);
        }

        async Task PackAssets(string IsoExtrPath)
        {
            using (MemoryStream mStream = new())
            {
                ZipArchive zip = new(mStream);
                foreach (var item in Cache)
                {
                    var entry = zip.CreateEntry(item.Key, CompressionLevel.Fastest);
                    using (var stream = entry.Open())
                    {
                        await stream.WriteAsync(item.Value);
                    }
                }

                BufferBD = null;
                Cache.Clear();
                BufferFiles.Clear();
                TwinsSection.ResetCache();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                mStream.Position = 0;
                await File.WriteAllBytesAsync(ZipPath, mStream.ToArray());
            }
        }

        public bool DetectXBE(string inputPath)
        {
            bool Check = ISO.DetectXBE(inputPath);
            if (Check)
            {
                isPS2 = false;
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
                isPS2 = false;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Check;
        }

        public bool DetectPS2(string inputPath)
        {
            bool Check = ISO_PS2.DetectPS2(inputPath);
            if (Check)
            {
                isPS2 = true;
                isPAL = ISO_PS2.Version == ISO9660.GameVersion.EUR;
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
                isPS2 = false;
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

        public static bool Check(string name) => Cache.ContainsKey(name.Replace('\\','/'));
        public static void Add(string name, byte[] data) => Cache.TryAdd(name.Replace('\\','/'), data);

    }
}
