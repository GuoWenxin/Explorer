using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    class ListViewData
    {
        private static ListViewData _instance;
        public static ListViewData Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = new ListViewData();
                }
                return _instance;
            }
        }
        /// <summary>
        /// listview表头名字
        /// </summary>
        private Dictionary<int,string> listViewColumnNameDic=new Dictionary<int, string>();

        private List<ListItemInfo> fileList = new List<ListItemInfo>();
        private List<ListItemInfo> DirectoryList = new List<ListItemInfo>();
        public int CurIndex = 0;

        public string GetListViewColumnTitle(FolderType  folderType,int index)
        {
            if (folderType==FolderType.COMPUTER)
            {
                switch (index)
                {
                    case 0:
                        return "名称";
                    case 1:
                        return "文件系统";
                    case 2:
                        return "总大小";
                    case 3:
                        return "使用空间";
                    case 4:
                        return "可用空间";
                    default:
                        return "";
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        return "名称";
                    case 1:
                        return "创建日期";
                    case 2:
                        return "修改日期";
                    case 3:
                        return "类型";
                    case 4:
                        return "文件大小";

                    default:
                        return "";
                }
            }
        }

        public void SetListViewColumnName(int index, string name)
        {
            if (listViewColumnNameDic.ContainsKey(index))
            {
                listViewColumnNameDic[index] = name;
            }
            else
            {
                listViewColumnNameDic.Add(index,name);
            }
        }
        public string GetListViewColumnName(int index)
        {
            if (listViewColumnNameDic.ContainsKey(index))
            {
                return listViewColumnNameDic[index];
            }
            else
            {
                return "";
            }
        }

        public void ClearListItemInfo()
        {
            fileList.Clear();
            DirectoryList.Clear();
        }
        public void SetListItemInfo(ListItemInfo listItemInfo,InfoType type)
        {
            switch (type)
            {
                case InfoType.File:
                    fileList.Add(listItemInfo);
                    break;
                default:
                    DirectoryList.Add(listItemInfo);
                    break;
            }
            
        }

        public List<ListItemInfo> GetListItemInfo(InfoType type)
        {
            switch (type)
            {
                case InfoType.File:
                    return fileList;
                default:
                    return DirectoryList;
            }
        }

        public void SortListItemInfo(int index)
        {
            if (fileList.Count<2)
            {
                return;
            }
            CurIndex=index;
            switch (index)
            {
                case 0:
                    fileList.Sort((a,b) =>String.Compare(a.Name, b.Name, StringComparison.Ordinal));
                    DirectoryList.Sort((a,b) =>String.Compare(a.Name, b.Name, StringComparison.Ordinal));
                    break;
                case 1:
                    fileList.Sort((a, b) => a.CrtTm.CompareTo(b.CrtTm));
                    DirectoryList.Sort((a, b) => a.CrtTm.CompareTo(b.CrtTm));
                    break;
                case 2:
                    fileList.Sort((a, b) => a.ModTm.CompareTo(b.ModTm));
                    DirectoryList.Sort((a, b) => a.ModTm.CompareTo(b.ModTm));
                    break;
                case 3:
                    fileList.Sort((a, b) => String.Compare(a.Type, b.Type, StringComparison.Ordinal));
                    DirectoryList.Sort((a, b) => String.Compare(a.Type, b.Type, StringComparison.Ordinal));
                    break;
                case 4:
                    fileList.Sort((a, b) => a.Size.CompareTo(b.Size));
                    DirectoryList.Sort((a, b) => a.Size.CompareTo(b.Size));
                    break;
            }
        }
    }

    public class ListItemInfo:IComparer
    {
        public string Name;
        public DateTime CrtTm;
        public DateTime ModTm;
        public string Type;
        public long Size;

        public int Compare(object x, object y)
        {
            throw new NotImplementedException();
        }
    }

    public class ListItemInfos
    {
        
    }
}
