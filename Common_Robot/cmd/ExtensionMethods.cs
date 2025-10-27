using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleControl
{
    public static class ExtensionMethods
    {
        public static void EnableContextMenu(this RichTextBox rtb,ConsoleControl control)
        {
            if (rtb.ContextMenuStrip == null)
            {
                // Create a ContextMenuStrip without icons
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.ShowImageMargin = false;

                //ToolStripMenuItem tsmiUndo = new ToolStripMenuItem("撤销");
                //tsmiUndo.Click += (sender, e) => rtb.Undo();
                //cms.Items.Add(tsmiUndo);

                //ToolStripMenuItem tsmiRedo = new ToolStripMenuItem("重做");
                //tsmiRedo.Click += (sender, e) => rtb.Redo();
                //cms.Items.Add(tsmiRedo);

                // Add a Separator
                cms.Items.Add(new ToolStripSeparator());

                //ToolStripMenuItem tsmiCut = new ToolStripMenuItem("剪切");
                //tsmiCut.Click += (sender, e) => rtb.Cut();
                //cms.Items.Add(tsmiCut);

                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("复制");
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);

                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("粘贴");
                tsmiPaste.Click += (sender, e) =>
                {
                    var isInReadOnlyZone = rtb.SelectionStart < control.inputStart;
                    if (isInReadOnlyZone==false) rtb.Paste();
                };
                cms.Items.Add(tsmiPaste);

                //ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("删除");
                //tsmiDelete.Click += (sender, e) => rtb.SelectedText = "";
                //cms.Items.Add(tsmiDelete);

                // Add a Separator
                cms.Items.Add(new ToolStripSeparator());

                // 7. Add the Select All Option (selects all the text inside the richtextbox)
                ToolStripMenuItem tsmiSelectAll = new ToolStripMenuItem("全选");
                tsmiSelectAll.Click += (sender, e) => rtb.SelectAll();
                cms.Items.Add(tsmiSelectAll);

                // When opening the menu, check if the condition is fulfilled 
                // in order to enable the action
                cms.Opening += (sender, e) =>
                {
                    //tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
                    //tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
                    //tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiCopy.Enabled = rtb.SelectionLength > 0;
                    //tsmiPaste.Enabled = !rtb.ReadOnly && Clipboard.ContainsText();
                    //tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
                };

                rtb.ContextMenuStrip = cms;
            }
        }
    }
}
