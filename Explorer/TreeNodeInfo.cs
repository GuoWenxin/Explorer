using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class TreeNodeInfo
    {
        public TreeNode TreeNode;
        public string FullPath;


        public TreeNodeInfo(TreeNode treeNode, string fullPath)
        {
            TreeNode = treeNode;
            FullPath = fullPath;
        }
    }
}
