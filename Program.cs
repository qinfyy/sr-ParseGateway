using System;
using System.IO;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Newtonsoft.Json;

namespace Gate
{
    internal class Program
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenFileName
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public string lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

        static string SaveFile()
        {
            OpenFileName ofn = new OpenFileName
            {
                lStructSize = Marshal.SizeOf(typeof(OpenFileName)),
                lpstrFilter = "Json文件 (*.json)\0*.json\0所有文件 (*.*)\0*.*\0",
                lpstrFile = new string('\0', 260),
                nMaxFile = 260,
                lpstrFileTitle = new string('\0', 100),
                nMaxFileTitle = 100,
                Flags = 0x00000002 | 0x00000004 | 0x00000008 | 0x00080000,
                lpstrTitle = "保存"
            };

            if (GetSaveFileName(ofn))
            {
                return ofn.lpstrFile;
            }
            else
            {
                return null;
            }
        }

        static void Main()
        {
            string DataFile = "data.txt";
            if (!File.Exists(DataFile))
            {
                Console.WriteLine($"{DataFile} 不存在");
                Console.Write("请按任意键继续...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            string Base64EncodedData = File.ReadAllText(DataFile);
            byte[] Base64DecodedData = Convert.FromBase64String(Base64EncodedData);
            Gateserver Gateserver = Gateserver.Parser.ParseFrom(Base64DecodedData);
            string JsonOutput = JsonFormatter.ToDiagnosticString(Gateserver);
            string FormattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(JsonOutput), Formatting.Indented);
            string FilePath = SaveFile();
            if (FilePath == null)
            {
                Console.WriteLine("用户未选择保存文件");
            } 
            else
            {
                File.WriteAllText(FilePath, FormattedJson);
                Console.WriteLine($"JSON 数据已保存到文件: {FilePath}");
                Console.WriteLine("解码已完成");
            }
            Console.Write("请按任意键继续...");
            Console.ReadKey();
        }
    }
}
