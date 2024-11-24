using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Media;
using System.Diagnostics;

namespace br.com.Bonus630DevToolsBar.DrawUIExplorer.Models
{
    public class ResourcesExtractor
    {
        private IntPtr hMod;

        public const uint RT_CURSOR = 0x00000001;
        public const uint RT_BITMAP = 0x00000002;
        public const uint RT_ICON = 0x00000003;
        public const uint RT_GROUP_ICON = 0x00000003+11;
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
        static extern IntPtr FindResourceW(
            IntPtr hModule,
            IntPtr lpName,
            IntPtr lpType);
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
        public event Action<Dictionary<UInt16, List<string>>> GuidsIsLoaded;

        private  readonly byte[] PNG_SIGNATURE = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private  readonly byte[] PNG_END = new byte[] { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };
        public ResourcesExtractor(string cdrProgramPath)
        {
            this.cdrProgramPath = cdrProgramPath;
           
        }
    
        public bool EnumIcons(IntPtr hModule, IntPtr lpszType,IntPtr lpszName, IntPtr lParam)
        {
            try
            {
                IntPtr hRes = FindResourceW(hModule, lpszName, lpszType);

                Debug.WriteLine(hRes);
                return true;

                //// MessageBox.Show(hRes.ToString()); // <- 0 here.

                //uint size = SizeofResource(hModule, hRes);
                //IntPtr pt = LoadResource(hModule, hRes);

                //Bitmap bmp;
                //// int sizef = sizeof(char);
                //byte[] bPtr = new byte[size];
                //Marshal.Copy(pt, bPtr, 0, (int)size);
                ////Use este para salvar os icones
                //using (MemoryStream m = new MemoryStream(bPtr))
                //{
                //    Console.WriteLine(lpszName);
                //    bmp = (Bitmap)Bitmap.FromStream(m);
                //    string name = GET_RESOURCE_NAME(lpszName);
                //    bmp.Save(string.Format("{0}\\{1}.png", destFolder, iconCount));
                //    iconCount++;
                //    return true;
                //}
                
            }
            catch
            {
                return false;
            }
        }
        public void SaveIcons(string destFolder)
        {
            string path = Path.GetTempFileName();
            File.Copy(Path.Combine(cdrProgramPath, "CrlIcons.dll"), path,true);
          
            try
            {
                

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    int PointerPEReaderOffset = 0x3c;
                    byte[] buffer = new byte[4];

                    fs.Position = PointerPEReaderOffset;

                    fs.Read(buffer, 0, buffer.Length);

                    int PointerPEReaderValue = BitConverter.ToInt32(buffer, 0);

                    fs.Position = PointerPEReaderValue+4;

                    fs.Read(buffer, 0, 4);

                    short numberOfSection = BitConverter.ToInt16(buffer, 2);
                    fs.Position += 12;

                    fs.Read(buffer, 2, 2);
                    int sizeOfOptionalHead = BitConverter.ToInt16(buffer, 2);

                    fs.Position += 2 + sizeOfOptionalHead;
                    fs.Position += 8; //rcdata signature
                    //Cada section table tem 40 bytes

                    fs.Read(buffer, 0, 4);
                    int virtualSize = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int virtualAdress = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int sizeRdata = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int pointRdata = buffer.ToInt();

                    fs.Position += 16;

                    buffer = new byte[8];

                    fs.Read(buffer, 0, 8); //rsrc signature

                    //Console.WriteLine(Encoding.ASCII.GetString(buffer));
                    fs.Read(buffer, 0, 4);
                    int virtualSizeRc = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int virtualAdressRc = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int sizeRc = buffer.ToInt();

                    fs.Read(buffer, 0, 4);
                    int pointRc = buffer.ToInt();

                    fs.Position += 16;


                    int salt = 8;
                    int byteRead = 0;

                    buffer = new byte[1];
                    List<long> pngPosition = new List<long>();
                    List<long> pngEnd = new List<long>();

                    while ((byteRead = fs.Read(buffer, 0, 1)) > 0)
                    {
                        if (buffer[0] == PNG_SIGNATURE[0])
                        {
                            bool ispng = true;
                            for (int i = 1; i < PNG_SIGNATURE.Length; i++)
                            {
                                fs.Read(buffer, 0, 1);
                                if (buffer[0] != PNG_SIGNATURE[i])
                                {
                                    ispng = false;
                                    fs.Position -= i;
                                    break;
                                }
                            }
                            if (ispng)
                                pngPosition.Add(fs.Position - salt);
                        }
                        if (buffer[0] == PNG_END[0])
                        {
                            bool ispng = true;
                            for (int i = 1; i < PNG_END.Length; i++)
                            {
                                fs.Read(buffer, 0, 1);
                                if (buffer[0] != PNG_END[i])
                                {
                                    ispng = false;
                                    fs.Position -= i;
                                    break;
                                }
                            }
                            if (ispng)
                                pngEnd.Add(fs.Position);

                        }
                    }
                    int header = 86;
                    for (int i = 0; i < pngPosition.Count; i++)
                    {
                        string icon = string.Format("{0}\\{1}.png", destFolder, i + 1);
                        if (File.Exists(icon))
                            continue;
                        buffer = new byte[pngEnd[i] - pngPosition[i] + header];

                        fs.Position = pngPosition[i] - header;

                        fs.Read(buffer, 0, buffer.Length);

                        int num = BitConverter.ToUInt16(buffer, 0) / 7 + 1;


                        using (MemoryStream ms = new MemoryStream(buffer, header, buffer.Length - header))
                        {
                            System.Drawing.Bitmap bitmap = (Bitmap)Bitmap.FromStream(ms);
                            bitmap.Save(icon);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Dictionary<UInt16, List<string>> GetGuids()
        {
            try
            {
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
        private int findIndex(byte[] buffer, byte[] delimiter, int current)
        {
            for (int i = current; i <= buffer.Length - delimiter.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < delimiter.Length; j++)
                {
                    if (buffer[i + j] != delimiter[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return i;
            }
            return -1;
        }

        // private static Regex reg = new Regex("\0$\?<guid>(?:[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private bool EnumRes(IntPtr hModule,IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            Console.WriteLine("Type: {0} | Name: {1} | Param: {2}",
                GET_RESOURCE_NAME(lpszType), GET_RESOURCE_NAME(lpszName), GET_RESOURCE_ID(lParam));
      
            try
            {
                IntPtr hRes = FindResource(hModule, lpszName, lpszType, 0);
                uint size = SizeofResource(hModule, hRes);
                IntPtr pt = LoadResource(hModule, hRes);
                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);
                List<byte[]> buffers = new List<byte[]>();
                int index = 0;
                byte[] delimiter = new byte[4];
                Buffer.BlockCopy(bPtr, 2, delimiter, 0, 4);
               

                while (index < bPtr.Length)
                {
                    int pos = findIndex(bPtr, delimiter, index);

                    if (pos > -1)
                    {
                        buffers.Add(new byte[pos - index]);
                        Buffer.BlockCopy(bPtr, index, buffers[buffers.Count - 1], 0, pos - index);
                        index += (pos - index) + delimiter.Length;
                    }
                    else
                        index++;

                }






                //char[] s = { '\0', '$' };
                // string[] s = { "\\0$\\" };
                // string t = Encoding.Unicode.GetString(bPtr);
                // //bytes divisores 2 ao 6 -- 00 00 36 00 00 00
                //// byte[] b = { 0, 0, 36, 0, 0, 0 };
                // //string teste = Encoding.Unicode.GetString(b); 
                // //int charCount = Encoding.Unicode.GetCharCount(bPtr);
                // string[] slices = t.Split(s, StringSplitOptions.RemoveEmptyEntries);
#if DEBUG
                string text = "";
                //File.WriteAllText("C:\\Users\\bonus\\OneDrive\\Ambiente de Trabalho\\guid-string.txt", t);
#endif
                //List<string> li = new List<string>();
                //li.AddRange(slices);

                // int guidLength = 72;
                // int start = 8;
                // int position = 0;
                // position += start;
                guids.Add(0, new List<string>());
                for (int i = 0; i < buffers.Count; i++)

                {
                    if (buffers[i].Length == 76)
                    {
                        string guid = Encoding.Unicode.GetString(buffers[i], 2, 72);
                        ushort id = BitConverter.ToUInt16(buffers[i], 74);
                        if (guids.ContainsKey(id))
                            guids[id].Add(guid);
                        else
                        {
                            List<string> l = new List<string>();
                            l.Add(guid);
                            guids.Add(id, l);
                        }
                    }
                    else if (buffers[i].Length <= 2)
                    {
                        //guids[0].Add()

                    }
                    else
                    {
                        //guids[0].Add()
                        System.Diagnostics.Debug.WriteLine("guids: " + i);
                        string t = Encoding.Unicode.GetString(buffers[i]);
                        string[] slices = t.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                        //text += i.ToString("0000:") + Encoding.Unicode.GetString(buffers[i]) + "\n";
                        for (int k = 0; k < slices.Length; k++)
                        {
                            if (slices.Length > 1)
                            {
                                string c = slices[k].Substring(slices[k].Length - 1);
                                string guid = slices[k].Substring(0, slices[k].Length - 1);
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
                        }
                    }

                }


#if DEBUG
                int y = 0;
                foreach (var item in guids)
                {
                    if (item.Key > y)
                    {
                        text += y.ToString("0000") + ":" + item.Key.ToString("0000")+"\n";
                        y = item.Key;
                    }
                    y++;
                }
                File.WriteAllText("C:\\Users\\bonus\\OneDrive\\Ambiente de Trabalho\\guid-string4.txt", text);
#endif



                //for (int i = 0; i < slices.Length; i++)
                //{
                //    if (slices[i].Length > 36)
                //    {
                //        string c = slices[i].Substring(36, slices[i].Length - 36);
                //        string guid = slices[i].Substring(0, 36);
                //        UInt16 id = BitConverter.ToUInt16(Encoding.Unicode.GetBytes(c), 0);
                //        if (guids.ContainsKey(id))
                //            guids[id].Add(guid);
                //        else
                //        {
                //            List<string> l = new List<string>();
                //            l.Add(guid);
                //            guids.Add(id, l);
                //        }
                //    }
                //    else
                //        guids[0].Add(slices[i]);
                //}

#if DEBUG
                string debugger = "";

                foreach (var i in guids)
                {
                    debugger += i.Key.ToString("0000") + "\n";
                    for (int j = 0; j < i.Value.Count; j++)
                    {
                        debugger += "\t" + i.Value[j] + "\n";
                    }
                    debugger += "----------------------------------\n";
                }
                File.WriteAllText("C:\\Users\\bonus\\OneDrive\\Ambiente de Trabalho\\guid-string2.txt", debugger);

#endif
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
    public static class a
    {
        public static string ToBinaryString(this byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }
        public static string ToBinaryString(this int b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }
        public static string ToHexString(this byte b)
        {
            return b.ToString("X2");
        }
        public static string ToHexString(this int b)
        {
            return b.ToString("X2");
        }
        public static int ToInt(this byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

    }
}


