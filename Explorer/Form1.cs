using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public partial class Form1 : Form
    {
        private const int LISTVIEECOLUMNNUM=5;

        private TreeNode CurOpenNode = null;
        private TreeNodeInfo CurSelectNode = null;
        private Dictionary<string,TreeNodeInfo> treeNodeDic=new Dictionary<string, TreeNodeInfo>();
        private ContextMenuStrip rightMenuStrip;
        private Thread addListViewThread;
        private FolderType currentFolderType=FolderType.COMPUTER;
        public Form1()
        {
            InitializeComponent();
            LogLabel.Text = "初始化中...";
            //申明一个鼠标右键点击菜单
            rightMenuStrip = new ContextMenuStrip();//1
            ToolStripItem toolStripItem1=new ToolStripButton("属性");
            toolStripItem1.Click += RightMenuItemClick;
            ToolStripItem toolStripItem2=new ToolStripButton("删除");
            toolStripItem2.Click += RightMenuItemClick;
            rightMenuStrip.Items.Add(toolStripItem1);//2
            rightMenuStrip.Items.Add(toolStripItem2); //3


            CheckForIllegalCrossThreadCalls = false;

            LoadRootInfo();
            InitListView();
            listView1.SmallImageList = SmallImageList;
            treeView1.ImageList = SmallImageList;
            treeView1.SelectedImageIndex = (int) ImageFileType.COMPUTER;
            LogLabel.Text = "初始化完成";
        }
        private void LoadRootInfo()
        {
            //加载根路径
            string rootname = "我的电脑";
            TreeNode rooTreeNode = new TreeNode(rootname);
            rooTreeNode.ImageIndex = (int) ImageFileType.COMPUTER;
            treeView1.Nodes.Add(rooTreeNode);
            //加载一级路径
            DriveInfo[] allDirves = DriveInfo.GetDrives();
            //检索计算机上的所有逻辑驱动器名称
            foreach (DriveInfo item in allDirves)
            {
                //Fixed 硬盘
                //Removable 可移动存储设备，如软盘驱动器或USB闪存驱动器。
                Console.Write(item.Name + "---" + item.DriveType);
                if (item.IsReady)
                {
                    string driveName = string.Format("{0} ({1})", item.VolumeLabel, item.Name.Replace("\\",""));
                    TreeNodeInfo nodeinfo = new TreeNodeInfo(new TreeNode(driveName), driveName);
                    nodeinfo.TreeNode.ImageIndex = (int) ImageFileType.DRIVE;
                    rooTreeNode.Nodes.Add(nodeinfo.TreeNode);
                    //treeNodeDic.Add(nodeinfo.TreeNode.FullPath, nodeinfo);
                    //加载二级路径

                    //AddDirectoryToNode(item.RootDirectory,nodeinfo.TreeNode);
                    DirectoryInfo di = item.RootDirectory;
                    DirectoryInfo[] dis = di.GetDirectories();
                    foreach (var directoryInfo in dis)
                    {
                        TreeNodeInfo childNode=new TreeNodeInfo(new TreeNode(directoryInfo.Name),directoryInfo.FullName);
                        childNode.TreeNode.ImageIndex = (int) ImageFileType.DIRECTORY;
                        nodeinfo.TreeNode.Nodes.Add(childNode.TreeNode);
                        treeNodeDic.Add(childNode.TreeNode.FullPath,childNode);

                        AddDirectoryToNode(directoryInfo,childNode.TreeNode);
                    }
                }
                else
                {
                    Console.Write("没有就绪");
                }
            }
        }
        

        private void AddChildNode(TreeNode node)
        {
            if (treeNodeDic.ContainsKey(node.FullPath))
            {

                DirectoryInfo di = new DirectoryInfo(treeNodeDic[node.FullPath].FullPath);
                AddDirectoryToNode(di,node);
            }
        }

        private void AddDirectoryToNode(DirectoryInfo di,TreeNode node)
        {
            try
            {
                //if (((int)directoryInfo.Attributes & (int)FileAttributes.ReadOnly)!=1) //判断是否是只读属性
                //文件夹是只读的不处理
                DirectoryInfo[] dis = di.GetDirectories();
                node.Nodes.Clear();
                foreach (var directoryInfos in dis)
                {
                    TreeNodeInfo childNode0 = new TreeNodeInfo(new TreeNode(directoryInfos.Name),
                                   directoryInfos.FullName);
                    childNode0.TreeNode.ImageIndex = (int) ImageFileType.DIRECTORY;
                    node.Nodes.Add(childNode0.TreeNode);
                    if (!treeNodeDic.ContainsKey(childNode0.TreeNode.FullPath))
                    {
                        treeNodeDic.Add(childNode0.TreeNode.FullPath, childNode0);
                    }
                    //try
                    {
                        DirectoryInfo[] diss = directoryInfos.GetDirectories();
                        childNode0.TreeNode.Nodes.Clear();
                        foreach (var directoryInfo in diss)
                        {
                            TreeNodeInfo childNode = new TreeNodeInfo(new TreeNode(directoryInfo.Name),
                                directoryInfo.FullName);
                            childNode.TreeNode.ImageIndex = (int) ImageFileType.DIRECTORY;
                            childNode0.TreeNode.Nodes.Add(childNode.TreeNode);
                            if (!treeNodeDic.ContainsKey(childNode.TreeNode.FullPath))
                            {
                                treeNodeDic.Add(childNode.TreeNode.FullPath, childNode);
                            }
                        }
                    }
                    //catch (Exception e)
                    {
                        //Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void TreeViewAfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.IsExpanded && CurOpenNode != node)
            {
                CurOpenNode = node;
                AddChildNode(node);
            }
            else
            {
                CurOpenNode = node.Parent;
            }
        }

        private void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.FullPath == "我的电脑")//当前选择了我的电脑根目录
            {
                currentFolderType=FolderType.COMPUTER;
                
                InitListViewDrive();
                listView1.Items.Clear();
                DriveInfo[] allDirves = DriveInfo.GetDrives();
                //检索计算机上的所有逻辑驱动器名称
                foreach (DriveInfo item in allDirves)
                {
                    //Fixed 硬盘
                    //Removable 可移动存储设备，如软盘驱动器或USB闪存驱动器。
                    if (item.IsReady)
                    {
                        string driveName = string.Format("{0} ({1})", item.VolumeLabel, item.Name.Replace("\\", ""));
                        
                        ListViewItem li=new ListViewItem();
                        li.Text = driveName;
                        li.ImageIndex = (int)ImageFileType.DRIVE;
                        li.SubItems.Add(item.DriveFormat);
                        li.SubItems.Add(Util.FormatSize(item.TotalSize));
                        li.SubItems.Add(Util.FormatSize(item.TotalSize - item.TotalFreeSpace));
                        li.SubItems.Add(Util.FormatSize(item.TotalFreeSpace));
                        if (item.TotalFreeSpace*10<item.TotalSize)//剩余空间不足10%背景显示红色
                        {
                            li.BackColor=Color.Red;
                        }
                        listView1.Items.Add(li);
                    }
                    else
                    {
                        Console.Write("没有就绪");
                    }
                }
            }
            else
            {
                InitListView();
                bool isdriveRoot = false;
                DriveInfo[] allDirves = DriveInfo.GetDrives();
                //检索计算机上的所有逻辑驱动器名称
                foreach (DriveInfo item in allDirves)
                {
                    string driveName = string.Format("{0} ({1})", item.VolumeLabel, item.Name.Replace("\\", ""));
                    if (node.Text==driveName)
                    {
                        currentFolderType=FolderType.Driver;
                        AddListItemByFullPath(item.Name);
                        isdriveRoot = true;
                        break;
                    }
                }
                if (!isdriveRoot && treeNodeDic.ContainsKey(node.FullPath) && (CurSelectNode == null || CurSelectNode.TreeNode != node))
                {
                    CurSelectNode = treeNodeDic[node.FullPath];
                    currentFolderType=FolderType.DIRECTORY;
                    AddListItem(node);
                }
            }
        }

        private void AddListItem(TreeNode node)
        {
            if (treeNodeDic.ContainsKey(node.FullPath))
            {
                AddListItemByFullPath(treeNodeDic[node.FullPath].FullPath);
            }
        }
        private void AddListItemByFullPath(string path)
        {
            if (addListViewThread != null && addListViewThread.IsAlive)
            {
                addListViewThread.Abort();
            }
            addListViewThread = new Thread(AddListItemThread);

            addListViewThread.Start(path);
        }
        private void AddListItemThread(object fullpath)
        {
            LogLabel.Text = "加载中...";
            string path = (string)fullpath;
            listView1.Items.Clear();
            try
            {

                ListViewData.Instance.ClearListItemInfo();
                DirectoryInfo di = new DirectoryInfo(path);
                DirectoryInfo[] dis = di.GetDirectories();
                FileInfo[] fis = di.GetFiles();
                int allfiles = dis.Length + fis.Length;
                int index = 0;
                foreach (var directoryInfo in dis)
                {
                    LogLabel.Text = string.Format("正在加载{2}  已完成{0}/{1}", index, allfiles, directoryInfo.Name);
                    ListItemInfo listItemInfo = new ListItemInfo {Name = directoryInfo.Name,CrtTm = directoryInfo.CreationTime,ModTm = directoryInfo.LastWriteTime,Type = "文件夹",Size = Util.GetDirectoryLength(directoryInfo.FullName) };
                    AddViewItem(listItemInfo, false);
                    ListViewData.Instance.SetListItemInfo(listItemInfo,InfoType.DIRECTORY);
                    index++;
                    Thread.Sleep(10);
                }
                foreach (var fileInfo in fis)
                {
                    LogLabel.Text = string.Format("正在加载{2}  已完成{0}/{1}", index, allfiles, fileInfo.Name);
                    ListItemInfo listItemInfo = new ListItemInfo {Name = fileInfo.Name,CrtTm = fileInfo.CreationTime,ModTm = fileInfo.LastWriteTime,Type = fileInfo.Extension.Replace(".", "").ToUpper() + "文件" ,Size = Util.FileSize(fileInfo.FullName) };
                    AddViewItem(listItemInfo);
                    ListViewData.Instance.SetListItemInfo(listItemInfo, InfoType.File);
                    index++;
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            LogLabel.Text = "加载完成";
        }

        private void AddViewItem(ListItemInfo listItemInfo,bool isfile=true)
        {
            ListViewItem li = new ListViewItem();
            li.Text = listItemInfo.Name;
            li.SubItems.Add(listItemInfo.CrtTm.ToString("yyyy-MM-dd HH:MM:ss"));
            li.SubItems.Add(listItemInfo.ModTm.ToString("yyyy-MM-dd HH:MM:ss"));
            li.SubItems.Add(listItemInfo.Type);
            long size = listItemInfo.Size;
            li.SubItems.Add(Util.FormatSize(size));
            long largeFile = 1024*1024*1024;
            if (size>largeFile)
            {
                li.BackColor=Color.Red;
            }
            if (isfile)
            {
                li.ForeColor = Color.Green;
                string extension = Util.GetExtension(listItemInfo.Name);
                if (extension!="")
                {
                    li.ImageIndex = Util.GetFileExtesionIndex(extension);
                }
                else
                {
                    li.ImageIndex = (int)ImageFileType.FILE;
                }
            }
            else
            {
                if (size==0)
                {
                    li.ForeColor=Color.Blue;
                }
                li.ImageIndex = (int) ImageFileType.DIRECTORY;
            }
            listView1.Items.Add(li);
        }

        private void InitListViewDrive()
        {
            int perwidth = listView1.Width / 10;

            int firstwidth = perwidth * 2;
            int othereitdh = listView1.Width - firstwidth * 4;
            string []names=new string[LISTVIEECOLUMNNUM];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ListViewData.Instance.GetListViewColumnTitle(FolderType.COMPUTER, i);
                ListViewData.Instance.SetListViewColumnName(i, names[i]);
            }
            
            listView1.Columns.Clear();
            listView1.Columns.Add(names[0], othereitdh);
            listView1.Columns.Add(names[1], firstwidth);
            listView1.Columns.Add(names[2], firstwidth);
            listView1.Columns.Add(names[3], firstwidth);
            listView1.Columns.Add(names[4], firstwidth);

            listView1.View = View.Details;
        }
        private void InitListView()
        {
            listView1.Columns.Clear();
            int perwidth = listView1.Width / 10;

            int mainwidth = perwidth * 3;
            int secwidth = perwidth*2;
            int otherwidth = listView1.Width - mainwidth - secwidth*3;
            string []names=new string[LISTVIEECOLUMNNUM];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ListViewData.Instance.GetListViewColumnTitle(FolderType.DIRECTORY, i);
                ListViewData.Instance.SetListViewColumnName(i, names[i]);
            }
            listView1.Columns.Add(names[0], mainwidth);
            listView1.Columns.Add(names[1], secwidth);
            listView1.Columns.Add(names[2], secwidth);
            listView1.Columns.Add(names[3], secwidth);
            listView1.Columns.Add(names[4], otherwidth);

            listView1.View=View.Details;
        }

        private void OnListViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //strip.Show(listViewtcmedicineSearch, e.Location);//鼠标右键按下弹出菜单
                rightMenuStrip.Show(listView1,e.Location);
            }
        }

        private void RightMenuItemClick(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem toolStripItem = (ToolStripItem) sender;
                var items = listView1.SelectedItems;
                foreach (var item in items)
                {
                    ListViewItem li = (ListViewItem) item;
                    bool isDirec = false;
                    DirectoryInfo selectDirectoryInfo = null;
                    FileInfo selectFileInfo = null;
                    DirectoryInfo di = new DirectoryInfo(CurSelectNode.FullPath);
                    DirectoryInfo[] dis = di.GetDirectories();
                    foreach (var directory in dis)
                    {
                        if (directory.Name==li.Text)
                        {
                            isDirec = true;
                            selectDirectoryInfo = directory;
                            break;
                        }
                    }
                    if (!isDirec)
                    {
                        FileInfo[] fis = di.GetFiles();
                        foreach (var fileInfo in fis)
                        {
                            if (fileInfo.Name==li.Text)
                            {
                                selectFileInfo = fileInfo;
                                break;
                            }
                        }
                    }
                    switch (toolStripItem.Text)
                    {
                        case "属性":
                            if (isDirec)
                            {
                                MessageBox.Show("详细属性：\n" +
                                                "名称：" + selectDirectoryInfo.Name + "\n" +
                                                "类型：文件夹\n" +
                                                "位置：" + selectDirectoryInfo.FullName + "\n" +
                                                "大小：" + Util.FormatSize(Util.GetDirectoryLength(selectDirectoryInfo.FullName)) + "\n" +
                                                "创建时间：" + selectDirectoryInfo.CreationTime.ToString("yyyy年MM月dd日,HH:MM:ss") + "\n" +
                                                "最后修改时间：" + selectDirectoryInfo.LastWriteTime.ToString("yyyy年MM月dd日,HH:MM:ss") + "\n" +
                                                "最后访问时间：" + selectDirectoryInfo.LastAccessTime.ToString("yyyy年MM月dd日,HH:MM:ss"),
                                                selectDirectoryInfo.Name + "属性",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("详细属性：\n" +
                                                "名称：" + selectFileInfo.Name + "\n" +
                                                "类型：" + selectFileInfo.Extension.Replace(".", "").ToUpper() + "文件(" + selectFileInfo.Extension + ")" + "\n" +
                                                "位置：" + selectFileInfo.FullName + "\n" +
                                                "大小：" + Util.FormatSize(Util.GetDirectoryLength(selectFileInfo.FullName)) + "\n" +
                                                "创建时间：" + selectFileInfo.CreationTime.ToString("yyyy年MM月dd日,HH:MM:ss") + "\n" +
                                                "最后修改时间：" + selectFileInfo.LastWriteTime.ToString("yyyy年MM月dd日,HH:MM:ss") + "\n" +
                                                "最后访问时间：" + selectFileInfo.LastAccessTime.ToString("yyyy年MM月dd日,HH:MM:ss"),
                                                selectFileInfo.Name + "属性",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                            }
                            break;
                        case "删除":
                            if (isDirec)
                            {
                                DialogResult dr = MessageBox.Show("确定是否删除文件夹" + selectDirectoryInfo.Name + "?", "文件夹删除",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (dr==DialogResult.OK)
                                {
                                    try
                                    {
                                        selectDirectoryInfo.Delete();
                                    }
                                    catch (Exception ee)
                                    {
                                        MessageBox.Show(ee.Message, "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                DialogResult dr = MessageBox.Show("确定是否删除文件" + selectFileInfo.Name + "?", "文件删除",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (dr == DialogResult.OK)
                                {
                                    try
                                    {
                                        selectFileInfo.Delete();
                                    }
                                    catch (Exception ee)
                                    {
                                        MessageBox.Show(ee.Message, "删除失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    finally
                                    {
                                        RefreshViewList(CurSelectNode.TreeNode);
                                    }
                                }
                            }
                            break;
                    }
                }
                
            }
        }

        private void RefreshViewList(TreeNode node)
        {
            listView1.Items.Clear();
            AddListItem(node);
        }
        private void OnMainFormSizeChangerd(object sender, EventArgs e)
        {
            switch (currentFolderType)
            {
                case FolderType.COMPUTER:
                    InitListViewDrive();
                    break;
                case FolderType.Driver:
                case FolderType.DIRECTORY:
                    InitListView();
                    break;
            }
        }

        private void OnListViewColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (currentFolderType==FolderType.COMPUTER)
            {
                return;
            }
            int index = e.Column;
            string newname = "";
            string columnName = ListViewData.Instance.GetListViewColumnName(index);
            ListViewData.Instance.SortListItemInfo(index);
            if (!columnName.Contains(Util.SORT_UP) && !columnName.Contains(Util.SORT_DOWN))//当前没排序
            {
                 newname= columnName + Util.SORT_UP;
                ShowSortViewList(0);
            }
            else
            {
                if (columnName.Contains(Util.SORT_UP))//向上排序
                {
                    newname = columnName.Replace(Util.SORT_UP, Util.SORT_DOWN);
                    ShowSortViewList(2);
                }
                else//向下排序
                {
                    newname = columnName.Replace(Util.SORT_DOWN, Util.SORT_UP);
                    ShowSortViewList(1);
                }
            }
            ListViewData.Instance.SetListViewColumnName(index, newname);
            listView1.Columns[index].Text = newname;
            ReMoveOtherCol(index);
        }

        private void ReMoveOtherCol(int curIndex)
        {
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                if (curIndex!=i)
                {
                    if (listView1.Columns[i].Text.Contains(Util.SORT_UP))
                    {
                        string newname = listView1.Columns[i].Text.Replace(Util.SORT_UP.ToString(),"");
                        ListViewData.Instance.SetListViewColumnName(i, newname);
                        listView1.Columns[i].Text = newname;
                    }
                    else if(listView1.Columns[i].Text.Contains(Util.SORT_DOWN))
                    {
                        string newname = listView1.Columns[i].Text.Replace(Util.SORT_DOWN.ToString(), "");
                        ListViewData.Instance.SetListViewColumnName(i, newname);
                        listView1.Columns[i].Text = newname;
                    }
                }
            }
        }

        private void ShowSortViewList(int sortType)
        {
            listView1.Items.Clear();
            List<ListItemInfo> listViewDatas1 = ListViewData.Instance.GetListItemInfo(InfoType.DIRECTORY);
            List<ListItemInfo> listViewDatas2 = ListViewData.Instance.GetListItemInfo(InfoType.File);
            switch (sortType)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    listViewDatas1.Reverse();
                    listViewDatas2.Reverse();
                    break;
            }
            foreach (var listItemInfo in listViewDatas1)
            {
                AddViewItem(listItemInfo, false);
            }
            foreach (var listItemInfo in listViewDatas2)
            {
                AddViewItem(listItemInfo);
            }
        }
    }
}
