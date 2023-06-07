using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class ResourcesExtractor
    {
        public const uint RT_CURSOR = 0x00000001;
        public const uint RT_BITMAP = 0x00000002;
        public const uint RT_ICON = 0x00000003;
        public const uint RT_MENU = 0x00000004;
        public const uint RT_DIALOG = 0x00000005;
        public const uint RT_STRING = 0x00000006;
        public const uint RT_FONTDIR = 0x00000007;
        public const uint RT_FONT = 0x00000008;
        public const uint RT_ACCELERATOR = 0x00000009;
        public const uint RT_RCDATA = 0x0000000a;
        public const uint RT_MESSAGETABLE = 0x0000000b;

        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool EnumResourceNamesWithName(
          IntPtr hModule,
          string lpszType,
          EnumResNameDelegate lpEnumFunc,
          IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool EnumResourceNamesWithID(
          IntPtr hModule,
          uint lpszType,
          EnumResNameDelegate lpEnumFunc,
          IntPtr lParam);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FindResource(
            IntPtr hModule,
            string lpName,
            uint lpType);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FindResource(
            IntPtr hModule,
            string lpName,
            string lpType);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FindResource(
           IntPtr hModule,
           IntPtr lpName,
           IntPtr lpType,
           ushort wLanguage);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(
            IntPtr hModule,
            IntPtr hResInfo);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SizeofResource(
            IntPtr hModule,
            IntPtr hResInfo);
        private delegate bool EnumResNameDelegate(
          IntPtr hModule,
          IntPtr lpszType,
          IntPtr lpszName,
          IntPtr lParam);

        private static bool IS_INTRESOURCE(IntPtr value)
        {
            if (((uint)value) > ushort.MaxValue)
                return false;
            return true;
        }
        private static uint GET_RESOURCE_ID(IntPtr value)
        {
            if (IS_INTRESOURCE(value) == true)
                return (uint)value;
            throw new System.NotSupportedException("value is not an ID!");
        }
        private static string GET_RESOURCE_NAME(IntPtr value)
        {
            if (IS_INTRESOURCE(value) == true)
                return value.ToString();
            return Marshal.PtrToStringUni((IntPtr)value);
        }
        private string destFolder = "";
        private string cdrProgramPath;
        private int iconCount;
        private Dictionary<UInt16, List<string>> guids = new Dictionary<UInt16, List<string>>();
        public event Action<Dictionary<UInt16,List<string>>> GuidsIsLoaded;
        public ResourcesExtractor(string cdrProgramPath)
        {
            this.cdrProgramPath = cdrProgramPath;
        }
        public bool EnumIcons(IntPtr hModule,
          IntPtr lpszType,
          IntPtr lpszName,
          IntPtr lParam)
        {
            try
            {
                IntPtr hRes = FindResource(hModule, lpszName, lpszType, 0);

                //// MessageBox.Show(hRes.ToString()); // <- 0 here.

                uint size = SizeofResource(hModule, hRes);
                IntPtr pt = LoadResource(hModule, hRes);

                Bitmap bmp;
                // int sizef = sizeof(char);
                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);
                //Use este para salvar os icones
                using (MemoryStream m = new MemoryStream(bPtr))
                {
                    bmp = (Bitmap)Bitmap.FromStream(m);
                    string name = GET_RESOURCE_NAME(lpszName);
                    bmp.Save(string.Format("{0}\\{1}.png", destFolder, iconCount));
                    iconCount++;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public void SaveIcons(string destFolder)
        {
            try
            {
                this.destFolder = destFolder;
                iconCount = 1;
                IntPtr hMod = LoadLibraryEx(Path.Combine(cdrProgramPath, "CrlIcons.dll"), IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (EnumResourceNamesWithID(hMod, RT_ICON, new EnumResNameDelegate(EnumIcons), IntPtr.Zero) == false)
                {
                    Console.WriteLine("gle: {0}", Marshal.GetLastWin32Error());
                }
                FreeLibrary(hMod);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public Dictionary<UInt16,List<string>> GetGuids()
        {
            try
            {

               // text = @"C:\Program Files\Corel\CorelDRAW Graphics Suite X8\Programs64";

                IntPtr hMod = LoadLibraryEx(Path.Combine(cdrProgramPath, "CrlIcons.dll"), IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (EnumResourceNamesWithID(hMod, RT_RCDATA, new EnumResNameDelegate(EnumRes), IntPtr.Zero) == false)
                {
                    Console.WriteLine("gle: {0}", Marshal.GetLastWin32Error());
                }

                FreeLibrary(hMod);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
            return guids;
        }
        // private static Regex reg = new Regex("\0$\?<guid>(?:[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private bool EnumRes(IntPtr hModule,
          IntPtr lpszType,
          IntPtr lpszName,
          IntPtr lParam)
        {
            Console.WriteLine("Type: {0} | Name: {1} | Param: {2}",
                GET_RESOURCE_NAME(lpszType), GET_RESOURCE_NAME(lpszName), GET_RESOURCE_ID(lParam));
            //Este código está funcionando para extrair icones
            //return true;
            try
            {
                IntPtr hRes = FindResource(hModule, lpszName, lpszType, 0);

                //// MessageBox.Show(hRes.ToString()); // <- 0 here.

                uint size = SizeofResource(hModule, hRes);
                IntPtr pt = LoadResource(hModule, hRes);

                Bitmap bmp;
                // int sizef = sizeof(char);
                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);



                char[] s = { '\0', '$' };
                string t = Encoding.Unicode.GetString(bPtr);
                // //bytes divisores 2 ao 6 -- 00 00 36 00 00 00
                //// byte[] b = { 0, 0, 36, 0, 0, 0 };
                // //string teste = Encoding.Unicode.GetString(b); 
                // //int charCount = Encoding.Unicode.GetCharCount(bPtr);
                string[] slices = t.Split(s, StringSplitOptions.RemoveEmptyEntries);

                //List<string> li = new List<string>();
                //li.AddRange(slices);

                // int guidLength = 72;
                // int start = 8;
                // int position = 0;
                // position += start;
                //Dictionary<UInt16, List<string>> guids = new Dictionary<UInt16, List<string>>();
                guids.Add(0, new List<string>());
                for (int i = 0; i < slices.Length; i++)
                {
                    if (slices[i].Length > 36)
                    {
                        string c = slices[i].Substring(36, slices[i].Length - 36);
                        string guid = slices[i].Substring(0, 36);
                        UInt16 id = BitConverter.ToUInt16(Encoding.Unicode.GetBytes(c), 0);
                        if (guids.ContainsKey(id))
                            guids[id].Add(guid);
                        else
                        {
                            List<string> l = new List<string>();
                            l.Add(guid);
                            guids.Add(id, l);
                        }
                    }
                    else
                        guids[0].Add(slices[i]);
                }
                if (GuidsIsLoaded != null)
                    GuidsIsLoaded(guids);
                //t = t.Replace("\0", "");
                // File.WriteAllText(string.Format("D:\\testeRCData{0}.txt", GET_RESOURCE_NAME(lpszName)), t);

                    //Use este para salvar os icones
                    //using (MemoryStream m = new MemoryStream(bPtr))
                    //{

                    //    bmp = (Bitmap)Bitmap.FromStream(m);
                    //    string name = GET_RESOURCE_NAME(lpszName);
                    //    bmp.Save(string.Format("{0}\\{1}.png",destFolder, name));
                    //}
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return true;
        }

    }
}


