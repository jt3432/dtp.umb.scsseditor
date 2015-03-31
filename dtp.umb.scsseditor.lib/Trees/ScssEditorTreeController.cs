using dtp.umb.scsseditor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using umbraco;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace dtp.umb.scsseditor.lib.Trees
{
    [PluginController("DtpScssEditor")]
    [Tree("settings", "dtpScssEditorTree", "Scss Files", iconClosed: "icon-folder", iconOpen: "icon-folder-open")]
    public class ScssEditorTreeController : TreeController
    {
        private ScssFilesApiController _ctrl;

        public ScssEditorTreeController()
        {
            _ctrl = new ScssFilesApiController();
        }

        protected override Umbraco.Web.Models.Trees.TreeNode CreateRootNode(System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            return base.CreateRootNode(queryStrings);
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var guid = id;
            var nodes = new TreeNodeCollection();
            TreeNode node = null;

            string path = id == Constants.System.Root.ToInvariantString() ?  String.Empty : id;
            var files = _ctrl.GetFiles(path);

            foreach (var file in files)
            {
                if (file.IsFile)
                {
                    node = this.CreateTreeNode(file.PathRelative.TrimEnd("/"), id, queryStrings, file.Name, "icon-brackets", false);
                }
                else
                {
                    node = this.CreateTreeNode(file.PathRelative.TrimEnd("/"), id, queryStrings, file.Name, "icon-folder", file.HasChildren, "/settings");
                }
                nodes.Add(node);
            }

            
            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id.EndsWith(".scss"))
            {
                menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias), true);
            }
            else if (id == Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add<ActionNew>(ui.Text("actions", ActionNew.Instance.Alias), true);
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }
            else
            {
                menu.Items.Add<ActionNew>(ui.Text("actions", ActionNew.Instance.Alias), true);
                menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias), true);
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }

            return menu;
        }
    }
}
