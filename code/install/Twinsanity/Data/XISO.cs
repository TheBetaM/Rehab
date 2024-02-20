using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RehabSetup
{
    public class XISO
    {
        const string XBE_Title = "twinsanity";
        const uint XBE_Region_NTSC = 7;
        const uint XBE_Region_PAL = 4;

        public string TitleID = string.Empty;
        public bool IsPAL = false;

        public bool XBE_Only = false;
        public bool ExtractFiles = false;
        public byte[] XBE_Buffer;

        long s_xbox_disc_lseek = 0;

        public string ExtractPath;

        public List<string> IgnoreExt = new();
        public List<string> IgnoreName = new();

        public bool DetectXBE(string filePath)
        {
            bool isISO = false;
            string dirPath = string.Empty;
            FileInfo? xbe = new FileInfo(filePath);
            if (xbe == null) return false;

            Stream fileStream = null;
            if (xbe.Extension.ToLower() == ".iso" || xbe.Extension.ToLower() == ".xiso")
            {
                isISO = true;
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    using (BinaryReader ireader = new BinaryReader(file))
                    {
                        ExtractPath = AppDomain.CurrentDomain.BaseDirectory;
                        int root_dir_sect = 0;
                        int root_dir_size = 0;
                        bool result = VerifyXISO(ireader, ref root_dir_sect, ref root_dir_size);
                        if (!result)
                        {
                            return false;
                        }
                        file.Seek((long)(root_dir_sect * XISO_SECTOR_SIZE + s_xbox_disc_lseek), SeekOrigin.Begin);
                        XBE_Only = true;
                        ExtractFiles = false;
                        TraverseXISO(ireader, null, root_dir_sect * XISO_SECTOR_SIZE + s_xbox_disc_lseek, "");
                        fileStream = new MemoryStream(XBE_Buffer);
                    }
                }
            }
            else
            {
                fileStream = new FileStream(xbe.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            // Based on OpenXDK
            fileStream.Seek(0x0118, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(fileStream);
            uint CertOffset = reader.ReadUInt16();
            fileStream.Seek(CertOffset, SeekOrigin.Begin);
            fileStream.Seek(CertOffset + 0x0008, SeekOrigin.Begin);
            uint CertID = reader.ReadUInt32();
            fileStream.Seek(CertOffset + 0x000C, SeekOrigin.Begin);
            byte[] CertNameUnicode = new byte[2];
            string TitleName = "";
            for (int i = 0; i < 40; i++)
            {
                CertNameUnicode[0] = reader.ReadByte();
                CertNameUnicode[1] = reader.ReadByte();
                TitleName += System.Text.Encoding.Unicode.GetString(CertNameUnicode);
            }
            fileStream.Seek(CertOffset + 0x00A0, SeekOrigin.Begin);
            uint CertRegion = reader.ReadUInt32();
            fileStream.Seek(CertOffset + 0x00AC, SeekOrigin.Begin);
            uint CertVersion = reader.ReadUInt32();

            /*
            Console.WriteLine("Cert offset: " + CertOffset.ToString("X"));
            Console.WriteLine("Cert Title ID: " + CertID);
            Console.WriteLine("Cert Region: " + CertRegion);
            Console.WriteLine("Cert Version: " + CertVersion);
            Console.WriteLine("Cert Name: " + TitleName);
            */

            TitleID = TitleName;
            IsPAL = CertRegion == XBE_Region_PAL;

            fileStream.Close();
            fileStream.Dispose();
            XBE_Buffer = null;

            if (TitleID.ToLower().Contains(XBE_Title))
            {
                return true;
            }

            return false;
        }

        public async Task ExportISO(string inputPath, string outputPath)
        {
            await Task.Run(
                () =>
                {
                    using (FileStream file = new FileStream(inputPath, FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(file))
                        {
                            XBE_Only = false;
                            ExtractFiles = true;
                            ExtractPath = outputPath;
                            //Directory.CreateDirectory(ExtractPath);
                            int root_dir_sect = 0;
                            int root_dir_size = 0;
                            bool result = VerifyXISO(reader, ref root_dir_sect, ref root_dir_size);
                            file.Seek((long)(root_dir_sect * XISO_SECTOR_SIZE + s_xbox_disc_lseek), SeekOrigin.Begin);
                            TraverseXISO(reader, null, root_dir_sect * XISO_SECTOR_SIZE + s_xbox_disc_lseek, "");
                        }
                    }
                }
                );
        }

        // extract-xiso code rewritten in C# (extracting only)
        // Original extract-xiso code credit to in@fishtank.com

        void TraverseXISO(BinaryReader file, dir_node in_dir_node, long in_dir_start, string in_path)
        {
            string path = in_path;
            long curpos;
            ushort l_offset = 0;
            
            if (in_dir_node == null) in_dir_node = new dir_node();
            dir_node dir = in_dir_node;

            read_entry:

            ushort tmp = file.ReadUInt16();
            if (tmp == XISO_PAD_SHORT)
            {
                l_offset = (ushort)(l_offset * XISO_DWORD_SIZE + (XISO_SECTOR_SIZE - (l_offset * XISO_DWORD_SIZE) % XISO_SECTOR_SIZE));
                file.BaseStream.Seek(in_dir_start + l_offset, SeekOrigin.Begin);
            }
            else
            {
                l_offset = tmp;
            }
            dir.r_offset = file.ReadUInt16();
            dir.start_sector = file.ReadUInt32();
            dir.file_size = file.ReadUInt32();
            dir.attributes = file.ReadByte();
            dir.filename_length = file.ReadByte();
            dir.filename = new string(file.ReadChars(dir.filename_length));
            //Console.WriteLine($"{dir.filename} Attr: {dir.attributes} Size: {dir.file_size}");
            if (dir.filename.StartsWith(".") || dir.filename.StartsWith("/") || dir.filename.StartsWith("\\") ) throw new Exception();

            if (l_offset != 0)
            {
                dir.left = new dir_node();
                file.BaseStream.Seek(in_dir_start + l_offset * XISO_DWORD_SIZE, SeekOrigin.Begin);
                dir.left.parent = dir;
                dir = dir.left;
                goto read_entry;
            }

            left_processed:

            if (dir.left != null) dir.left = null;
            curpos = file.BaseStream.Position;

            if ((dir.attributes & XISO_ATTRIBUTE_DIR) != 0)
            {
                path = in_path + dir.filename + "\\";
                file.BaseStream.Seek((long)dir.start_sector * XISO_SECTOR_SIZE + s_xbox_disc_lseek, SeekOrigin.Begin);

                dir_node subdir = new dir_node();
                subdir.CopyFrom(dir);
                subdir.parent = null;
                if (dir.file_size > 0)
                {
                    TraverseXISO(file, subdir, (long)dir.start_sector * XISO_SECTOR_SIZE + s_xbox_disc_lseek, path);
                }
                path = in_path;
            }
            else
            {
                file.BaseStream.Seek((long)dir.start_sector * XISO_SECTOR_SIZE + s_xbox_disc_lseek, SeekOrigin.Begin);
                string FileName = ExtractPath + path + dir.filename;
                string Ext = System.IO.Path.GetExtension(FileName).ToLower();
                if (XBE_Only && Ext == ".xbe")
                {
                    byte[] data = file.ReadBytes((int)dir.file_size);
                    XBE_Buffer = data;
                    return;
                }
                else if (ExtractFiles && !IgnoreExt.Contains(Ext) && !IgnoreName.Contains(dir.filename + Ext))
                {
                    byte[] data = file.ReadBytes((int)dir.file_size);
                    //Console.WriteLine(FileName);
                    //Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FileName));
                    //File.WriteAllBytes(FileName, data);
                    AssetExporter.BufferFiles.Add(FileName, (0, (uint)dir.file_size, data));
                    //AssetExporter.BufferFiles.Add(FileName, ((uint)file.BaseStream.Position + 1, (uint)dir.file_size, null));
                }
            }

            if (dir.r_offset != 0)
            {
                int sector = (int)((curpos - in_dir_start) / XISO_SECTOR_SIZE);
                if (dir.r_offset * XISO_DWORD_SIZE / XISO_SECTOR_SIZE > sector)
                {
                    dir.r_offset = (ushort)(sector * (XISO_SECTOR_SIZE / XISO_DWORD_SIZE) + (XISO_SECTOR_SIZE / XISO_DWORD_SIZE));
                }
                file.BaseStream.Seek(in_dir_start + dir.r_offset * XISO_DWORD_SIZE, SeekOrigin.Begin);
                l_offset = dir.r_offset;
                goto read_entry;
            }

            dir = dir.parent;
            if (dir != null)
            {
                goto left_processed;
            }

        }

        public bool VerifyXISO(BinaryReader file, ref int out_root_dir_sector, ref int out_root_dir_size)
        {
            if (file.BaseStream.Length < (long)(XISO_HEADER_OFFSET + XISO_FILETIME_SIZE + XISO_UNUSED_SIZE)) return false;
            file.BaseStream.Seek(XISO_HEADER_OFFSET, SeekOrigin.Begin);
            string HeaderCheck = new string(file.ReadChars(XISO_HEADER_DATA_LENGTH));
            if (HeaderCheck != XISO_HEADER_DATA) 
            {
                file.BaseStream.Seek((long)(XISO_HEADER_OFFSET + GLOBAL_LSEEK_OFFSET), SeekOrigin.Begin);
                HeaderCheck = new string(file.ReadChars(XISO_HEADER_DATA_LENGTH));
                if (HeaderCheck != XISO_HEADER_DATA) 
                {
                    file.BaseStream.Seek((long)(XISO_HEADER_OFFSET + XGD3_LSEEK_OFFSET), SeekOrigin.Begin);
                    HeaderCheck = new string(file.ReadChars(XISO_HEADER_DATA_LENGTH));
                    if (HeaderCheck != XISO_HEADER_DATA) 
                    {
                        return false;
                    }
                    else
                    {
                        s_xbox_disc_lseek = XGD3_LSEEK_OFFSET;
                    }
                }
                else
                {
                    s_xbox_disc_lseek = GLOBAL_LSEEK_OFFSET;
                }
            }
            else
            {
                s_xbox_disc_lseek = 0;
            }
            
            out_root_dir_sector = file.ReadInt32();
            out_root_dir_size = file.ReadInt32();

            if (out_root_dir_sector == 0 || out_root_dir_size == 0) return false;

            file.BaseStream.Seek((long)(XISO_FILETIME_SIZE + XISO_UNUSED_SIZE), SeekOrigin.Current);
            HeaderCheck = new string(file.ReadChars(XISO_HEADER_DATA_LENGTH));
            if (HeaderCheck != XISO_HEADER_DATA) return false;

            if (file.BaseStream.Length < (long)(out_root_dir_sector * XISO_SECTOR_SIZE)) return false;

            return true;
        }

        const long GLOBAL_LSEEK_OFFSET = 0xFD90000;
        const long XGD3_LSEEK_OFFSET = 0x2080000;
        const string XISO_HEADER_DATA = "MICROSOFT*XBOX*MEDIA";
        const int XISO_HEADER_DATA_LENGTH = 20;
        const int XISO_HEADER_OFFSET = 0x10000;

        const int XISO_DWORD_SIZE = 4;
        const int XISO_FILETIME_SIZE = 8;

        const int XISO_SECTOR_SIZE = 2048;
        const int XISO_UNUSED_SIZE = 0x7c8;

        const byte XISO_ATTRIBUTE_DIR = 0x10;
        const ushort XISO_PAD_SHORT = 0xffff;

        class dir_node{
            public dir_node left;
            public dir_node parent;
            public string filename;
            public ushort r_offset;
            public byte attributes;
            public byte filename_length;
            public ulong file_size;
            public ulong start_sector;

            public void CopyFrom(dir_node from)
            {
                left = from.left;
                parent = from.parent;
                filename = from.filename;
                r_offset = from.r_offset;
                attributes = from.attributes;
                filename_length = from.filename_length;
                file_size = from.file_size;
                start_sector = from.start_sector;
            }
        }

        





    }
}
