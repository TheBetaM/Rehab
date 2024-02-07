using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Twinsanity;
using Twinsanity.Items;

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
        public string OutputPath = string.Empty; // should end in a file name
        BackgroundWorker Worker;
        Dictionary<string, List<string>> FilePaths;
        List<string> XMVPaths;
        public XISO ISO;
        public ISO9660 ISO_PS2;
        public string ISOpath = string.Empty;
        public string GodotPath = string.Empty;
        public string ZipPath = AppDomain.CurrentDomain.BaseDirectory + "\\Packs\\RehabData.pcz";
        public bool PackingAssets = true;
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

        public event EventHandler<int> WorkerProgressChanged;
        public event EventHandler WorkerFinished;

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
            ISO_PS2 = new ISO9660();
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
            Console.WriteLine("Preparing...");

            Stage = ProcessStages.ExtractISO;

            if (isISO)
            {
                Console.WriteLine("Extracting ISO...");
                DirPath = ISO_Extract_Path;
                if (isPS2)
                {
                    await ISO_PS2.ExportISO(ISOpath, ISO_Extract_Path);
                }
                else
                {
                    await ISO.ExportISO(ISOpath, ISO_Extract_Path);
                }
            }

            Stage = ProcessStages.ExtractAssets;
            Console.WriteLine("Extracting assets...");

            #region Extract Assets
            DirectoryInfo Dir = new DirectoryInfo(DirPath);
            FilePaths = new Dictionary<string, List<string>>();
            Recursive_Batch(Dir, FilePaths);

            if (isPS2)
            {
                Task? BDTask = null;
                foreach (string Path in FilePaths[".bd"])
                {
                    BDTask = ExportBD(Path, DirPath);
                }
                if (BDTask != null)
                    await BDTask;
                
                Dir = new DirectoryInfo(DirPath);
                FilePaths = new Dictionary<string, List<string>>();
                Recursive_Batch(Dir, FilePaths);
            }

            Task? DefaultTask = null;
            foreach (string Path in FilePaths[RMext])
            {
                if (Path.ToLower().EndsWith($"default{RMext}"))
                {
                    DefaultTask = ExportDefault(Path);
                    LevelsLeft++;
                    TotalLevels++;
                    break;
                }
            }
            if (DefaultTask != null)
                await DefaultTask;

            IList<Task> TaskList = new List<Task>();
            foreach (string Path in FilePaths[RMext])
            {
                if (!Path.ToLower().EndsWith($"default{RMext}"))
                {
                    TaskList.Add(ExportLevel(Path));
                    LevelsLeft++;
                    TotalLevels++;
                }
            }
            foreach (string Path in FilePaths[".psm"])
            {
                if (!Path.ToLower().Contains("extras"))
                {
                    TaskList.Add(ExportPSM(Path));
                    PSMLeft++;
                    TotalPSM++;
                }
            }
            foreach (string Path in FilePaths[".psf"])
            {
                TaskList.Add(ExportPSF(Path));
                PSMLeft++;
                TotalPSM++;
            }
            foreach (string Path in FilePaths[".ptc"])
            {
                TaskList.Add(ExportPTC(Path));
                PSMLeft++;
                TotalPSM++;
            }
            if (isPS2)
            {
                foreach (string Path in FilePaths[".mb"])
                {
                    TaskList.Add(ExportMB(Path));
                }
            }
            else
            {
                foreach (string Path in FilePaths[".xwb"])
                {
                    TaskList.Add(ExportXWB(Path));
                }
            }
            foreach (string Path in FilePaths[".bin"])
            {
                if (Path.ToLower().Contains("frontend"))
                    TaskList.Add(ExportFrontend(Path));
            }
            TotalFiles += TaskList.Count;
            FilesLeft += TaskList.Count;

            await Task.WhenAll(TaskList);
            TaskList.Clear();
            #endregion

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Stage = ProcessStages.InstallMods;
            //Console.WriteLine("Installing mods...");

            //await CopyFilesAsync();

            if (PackingAssets)
            {
                Stage = ProcessStages.PackAssets;
                Console.WriteLine("Packing assets...");

                await PackAssets(DirPath);
            }

            Stage = ProcessStages.End;
            Console.WriteLine("Finishing up...");
            
            await Cleanup(DirPath);

            Console.WriteLine("Complete!");
            Exporting = false;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void Recursive_Batch(DirectoryInfo dir, Dictionary<string, List<string>> paths)
        {
            foreach (DirectoryInfo di in dir.EnumerateDirectories())
            {
                Recursive_Batch(di, paths);
            }
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                string ext = file.Extension.ToLower().Replace(";1", "");
                if (!paths.ContainsKey(ext))
                {
                    paths.Add(ext, new List<string>() { file.FullName });
                }
                else
                {
                    paths[ext].Add(file.FullName);
                }
            }
        }

        async Task ExportLevel(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile RM = new TwinsFile();
                    TwinsFile SM = new TwinsFile();

                    RM.LoadFile(inName, isPS2 ? TwinsFile.FileType.RM2 : TwinsFile.FileType.RMX);
                    string SMpath = inName.Replace(".rm", ".sm");
                    SM.LoadFile(SMpath, isPS2 ? TwinsFile.FileType.SM2 : TwinsFile.FileType.SMX);

                    ExportGodot.ExportFull(RM, SM, OutputPath, true, null, true, false);

                    LevelsLeft--;
                    FilesLeft--;
                }
                );
        }

        async Task ExportDefault(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile RM = new TwinsFile();
                    RM.LoadFile(inName, isPS2 ? TwinsFile.FileType.RM2 : TwinsFile.FileType.RMX);
                    ExportGodot.ExportRM(RM, OutputPath, true, null);
                    LevelsLeft--;
                    FilesLeft--;
                }
                );
        }

        async Task ExportFrontend(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile BIN = new TwinsFile();
                    BIN.LoadFile(inName, isPS2 ? TwinsFile.FileType.BIN : TwinsFile.FileType.BIN_XBOX);
                    ExportGodot.ExportBIN(BIN, OutputPath);
                    FilesLeft--;
                }
                );
        }

        async Task ExportPSM(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFile(inName, isPS2 ? TwinsFile.FileType.PSM : TwinsFile.FileType.PSM_XBOX);
                    ExportGodot.ExportPSM(PSM, OutputPath);
                    PSMLeft--;
                    FilesLeft--;
                }
                );
        }

        async Task ExportPTC(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFile(inName, isPS2 ? TwinsFile.FileType.PTC : TwinsFile.FileType.PTC_XBOX);
                    ExportGodot.ExportPSM(PSM, OutputPath, true);
                    PSMLeft--;
                    FilesLeft--;
                }
                );
        }

        async Task ExportPSF(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile PSM = new TwinsFile();
                    PSM.LoadFile(inName, isPS2 ? TwinsFile.FileType.PSF : TwinsFile.FileType.PSF_XBOX);
                    ExportGodot.ExportPSM(PSM, OutputPath, false, true);
                    PSMLeft--;
                    FilesLeft--;
                }
                );
        }

        async Task ExportXWB(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile XWB = new TwinsFile();
                    XWB.LoadFile(inName, TwinsFile.FileType.XWB);
                    ExportGodot.ExportXWB(XWB, OutputPath);
                    FilesLeft--;
                }
                );
        }

        async Task ExportTXT(string inName)
        {
            await Task.Run(
                () =>
                {
                    string BaseFolder = "Language";
                    string BaseName = inName;
                    BaseName = BaseName.Replace("American", "English"); // simplified asset paths
                    string LangPath = $"{System.IO.Path.GetDirectoryName(OutputPath)}\\{BaseFolder}\\";
                    int LangStart = inName.IndexOf(BaseFolder) + (BaseFolder.Length + 1);
                    string RelativePath = inName.Substring(LangStart);
                    string OutName = $"{LangPath}{RelativePath}";
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(OutName));
                    File.Copy(inName, OutName, true);
                    FilesLeft--;
                }
                );
        }

        async Task ExportXMV()
        {
            await Task.Run(
                () =>
                {
                    string OutDir = $"{System.IO.Path.GetDirectoryName(OutputPath)}\\Movies\\";
                    Directory.CreateDirectory(OutDir);
                    for (int i = 0; i < XMVPaths.Count; i++)
                    {
                        string ExtConv = System.IO.Path.ChangeExtension(XMVPaths[i], ".ogv");
                        string OutPath = $"{OutDir}{System.IO.Path.GetFileName(ExtConv)}";
                        string args = $"\"{XMVPaths[i]}\" -o \"{OutPath}\" ";

                        Process XMVProcess = new Process();
                        XMVProcess.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"ffmpeg2theora-0.30.exe";
                        XMVProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //Debug.WriteLine(args);
                        XMVProcess.StartInfo.Arguments = args;
                        XMVProcess.StartInfo.UseShellExecute = false;
                        XMVProcess.StartInfo.RedirectStandardOutput = true;
                        XMVProcess.StartInfo.CreateNoWindow = true;
                        XMVProcess.Start();

                        //Debug.WriteLine(XMVProcess.StandardOutput.ReadToEnd());

                        XMVProcess.WaitForExit();

                        VideosLeft--;
                        FilesLeft--;
                    }
                }
                );
        }

        async Task ExportMB(string inName)
        {
            await Task.Run(
                () =>
                {
                    TwinsFile Hash = new TwinsFile();
                    Hash.LoadFile(inName.Replace("mb","mh").Replace("MB","MH"), TwinsFile.FileType.MH);
                    TwinsFile MB = new TwinsFile();
                    MB.musicHash = (Twinsanity.Items.MusicHash)Hash.Records[0];
                    MB.LoadFile(inName, TwinsFile.FileType.MB);
                    ExportGodot.ExportMB(MB, OutputPath);
                    FilesLeft--;
                }
                );
        }

        async Task ExportBD(string inName, string IsoExtrPath)
        {
            BD_Archive BD = new BD_Archive();
            await BD.ExtractAsync(inName, inName.Replace("bd","bh").Replace("BD","BH"), IsoExtrPath);
        }

        async Task PackAssets(string IsoExtrPath)
        {
            await Task.Run(
                () =>
                {
                    if (isISO)
                    {
                        // Cleanup first, to not go over 4 GB
                        Directory.Delete(IsoExtrPath, true);
                    }
                    ZipFile.CreateFromDirectory(System.IO.Path.GetDirectoryName(OutputPath), ZipPath, CompressionLevel.Fastest, true);
                    //ZipFile.CreateFromDirectory(System.IO.Path.GetDirectoryName(OutputPath), ZipPath, CompressionLevel.NoCompression, true);
                }
                );
        }

        async Task Cleanup(string IsoExtrPath)
        {
            await Task.Run(
                () =>
                {
                    /*
                    if (isISO)
                    {
                        // Cleanup
                        Directory.Delete(IsoExtrPath, true);
                    }
                    */
                    if (PackingAssets)
                    {
                        Directory.Delete(System.IO.Path.GetDirectoryName(OutputPath), true);
                    }
                }
                );
        }

        async Task CopyFilesAsync()
        {
            DirectoryInfo di = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\Mods\\Base\\");
            string outputPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Rehab\\";

            Dictionary<string, string> CopyList = new Dictionary<string, string>();
            string pathparent = outputPath;
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                Directory.CreateDirectory(pathparent + dir.Name);
                foreach (FileInfo file in dir.EnumerateFiles())
                    CopyList.Add(file.FullName, pathparent + dir.Name + @"\" + file.Name);
                Recursive_ListFiles(dir, pathparent + dir.Name + @"\", ref CopyList);
            }
            foreach (FileInfo file in di.EnumerateFiles())
                CopyList.Add(file.FullName, pathparent + file.Name);

            IList<Task> writeTaskList = new List<Task>();
            foreach (KeyValuePair<string, string> Path in CopyList)
            {
                writeTaskList.Add(CopyFileAsync(Path.Key, Path.Value));
            }
            await Task.WhenAll(writeTaskList);
            writeTaskList.Clear();
        }
        async Task CopyFileAsync(string from, string to)
        {
            using (Stream source = File.Open(from, FileMode.Open))
            {
                using (Stream destination = File.Create(to))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }

        void Recursive_ListFiles(DirectoryInfo di, string pathparent, ref Dictionary<string, string> Paths)
        {
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                Directory.CreateDirectory(pathparent + dir.Name);
                string pathchild = pathparent + dir.Name + @"\";
                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    Paths.Add(file.FullName, pathchild + file.Name);
                }
                Recursive_ListFiles(dir, pathchild, ref Paths);
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

        public void RunGame()
        {
            // todo
            /*
            string args = $"--path Rehab Frontend/FE_LevelSelect.tscn";

            Process GodotProcess = new Process();
            GodotProcess.StartInfo.FileName = GodotPath;
            GodotProcess.StartInfo.Arguments = args;
            GodotProcess.Start();
            */
        }

    }
}
