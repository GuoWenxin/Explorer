using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Explorer
{
    class Util
    {
        /// <summary>排序向下小三角 </summary>
        public const char SORT_DOWN = (char) 0x25bc;
        /// <summary>排序向上小三角 </summary>
        public const char SORT_UP = (char)0x25b2;

        //所给路径中所对应的文件大小
        public static long FileSize(string filePath)
        {
            //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        /// <summary>
        /// 获取指定路径的大小
        /// </summary>
        /// <param name="dirPath">路径</param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            long len = 0;

            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(dirPath))
            {
                //查询文件的大小
                len = FileSize(dirPath);
            }
            else
            {
                //定义一个DirectoryInfo对象
                try
                {

                    DirectoryInfo di = new DirectoryInfo(dirPath);

                    //通过GetFiles方法，获取di目录中的所有文件的大小
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        len += fi.Length;
                    }
                    //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                    DirectoryInfo[] dis = di.GetDirectories();
                    if (dis.Length > 0)
                    {
                        for (int i = 0; i < dis.Length; i++)
                        {
                            len += GetDirectoryLength(dis[i].FullName);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return len;
        }

        public static string FormatSize(long size)
        {
            if (size > 1024 * 1024*1024)
            {
                float gb = size / (1024*1024 * 1024.0f);
                return gb.ToString("0.00") + "GB";
            }
            if (size>1024*1024)
            {
                float mb = size /(1024* 1024.0f);
                return mb.ToString("0.00") + "MB";
            }
            if (size>1024)
            {
                float kb = size/1024.0f;
                return kb.ToString("0.00") + "KB";
            }
            return size + "B";
        }

        public static string GetExtension(string fileName)
        {
            int index = fileName.LastIndexOf('.');
            if (index!=-1)
            {
                return fileName.Substring(index + 1);
            }
            return "";
        }

        public static int GetFileExtesionIndex(string extension)
        {
            int index = 0;
            if (extension.Equals("zip", StringComparison.CurrentCultureIgnoreCase) || extension.Equals("rar", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.ZIP;
            }
            else if (extension.Equals("doc", StringComparison.CurrentCultureIgnoreCase) || extension.Equals("docx", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.WORD;
            }
            else if (extension.Equals("xls", StringComparison.CurrentCultureIgnoreCase) || extension.Equals("xlsx", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.EXCEL;
            }
            else if (extension.Equals("ppt", StringComparison.CurrentCultureIgnoreCase) || extension.Equals("pptx", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.PPT;
            }
            else if (extension.Equals("pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.PDF;
            }
            else if (extension.Equals("apk", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.APK;
            }
            else if (extension.Equals("ipa", StringComparison.CurrentCultureIgnoreCase))
            {
                index = (int)ImageFileType.IPA;
            }
            else
            {
                index = (int)ImageFileType.FILE;
            }
            return index;
        }
    }
}
