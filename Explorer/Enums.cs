using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public enum ImageFileType
    {
        /// <summary>我的电脑图标 </summary>
        COMPUTER=0,
        /// <summary>磁盘图标 </summary>
        DRIVE=1,
        /// <summary>文件夹图标 </summary>
        DIRECTORY,
        /// <summary>通用文件图标 </summary>
        FILE,
        /// <summary>压缩文件图标 </summary>
        ZIP,
        /// <summary>Word图标 </summary>
        WORD,
        /// <summary>excel图标 </summary>
        EXCEL,
        /// <summary>PPT图标 </summary>
        PPT,
        /// <summary>pdf图标 </summary>
        PDF,
        /// <summary>APK图标 </summary>
        APK,
        /// <summary>IPA图标 </summary>
        IPA,
    }

    public enum FolderType
    {
        COMPUTER,
        Driver,
        DIRECTORY,
    }
    public enum InfoType
    {
        File,
        DIRECTORY,
    }
}
