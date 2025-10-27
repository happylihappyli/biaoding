


using Microsoft.VisualBasic.FileIO;
using System.IO.MemoryMappedFiles;

namespace Test1
{
    public class Main_File
    {
        public static bool isUtf8(string str_file)
        {

            byte[] bytes = File.ReadAllBytes(str_file);

            int index = 0;
            if (bytes.Length > 3)
            {
                if (bytes[0] == 239 && bytes[1] == 187 && bytes[2] == 191)
                {
                    return true;
                }
            }
            return false;

            for (int i = index; i < bytes.Length; i++)
            {
                byte b1 = bytes[i];

                if ((b1 & 0x80) == 0x00)
                {
                    continue;
                }
                else if ((b1 & 0xC0) == 0xC0)
                {
                    if (i + 1 < bytes.Length)
                    {
                        byte b2 = bytes[i + 1];
                        if ((b2 & 0x80) == 0x80)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if ((b1 & 0xE0) == 0xE0)
                {
                    if (i + 2 < bytes.Length)
                    {
                        byte b2 = bytes[i + 1];
                        byte b3 = bytes[i + 2];
                        if ((b2 & 0x80) == 0x80 && (b3 & 0x80) == 0x80)
                        {
                            i += 2;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if ((b1 & 0xF0) == 0xF0)
                {
                    if (i + 3 < bytes.Length)
                    {
                        byte b2 = bytes[i + 1];
                        byte b3 = bytes[i + 2];
                        byte b4 = bytes[i + 3];
                        if ((b2 & 0x80) == 0x80 && (b3 & 0x80) == 0x80 && (b4 & 0x80) == 0x80)
                        {
                            i += 3;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        public static void 删除文件(FileInfo file)
        {
            try
            {
                // 将文件移动到回收站
                FileSystem.DeleteFile(file.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                Console.WriteLine($"文件 '{file.FullName}' 已成功移动到回收站。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
            }
        }

        public static byte[] GetImageBytes(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, image.RawFormat);
                    return memoryStream.ToArray();
                }
            }
        }

        public static void save_share_memory(string memory_name, byte[] bytes,long size)
        {
            var mmf = MemoryMappedFile.CreateOrOpen(memory_name, size, MemoryMappedFileAccess.ReadWrite);
            var viewAccessor = mmf.CreateViewAccessor(0, size);
            viewAccessor.Write(0, bytes.Length);
            viewAccessor.WriteArray<byte>(0, bytes, 0, bytes.Length);
        }
    }
}
