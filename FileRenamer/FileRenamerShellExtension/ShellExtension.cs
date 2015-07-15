using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileRenamer;
using System.Threading;

namespace FileRenamerShellExtension
{
    /// <summary>
    /// The CountLinesExtensions is an example shell context menu extension,
    /// implemented with SharpShell. It adds the command 'Count Lines' to text
    /// files.
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class FileRenamerShellExtension : SharpContextMenu
    {
        private App application;

        /// <summary>
        /// Determines whether this instance can a shell
        /// context show menu, given the specified selected file list.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance should show a shell context
        /// menu for the specified file list; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanShowMenu()
        {
            //  We always show the menu.
            return true;
        }

        /// <summary>
        /// Creates the context menu. This can be a single menu item or a tree of them.
        /// </summary>
        /// <returns>
        /// The context menu for the shell context menu.
        /// </returns>
        protected override ContextMenuStrip CreateMenu()
        {
            //  Create the menu strip.
            var menu = new ContextMenuStrip();

            //  Create a 'count lines' item.
            var itemCountLines = new ToolStripMenuItem
            {
                Text = "Rename Files"
            };

            //  When we click, we'll call the 'CountLines' function.
            itemCountLines.Click += (sender, args) => LoadFileRenamer();

            //  Add the item to the context menu.
            menu.Items.Add(itemCountLines);

            //  Return the menu.
            return menu;
        }

        /// <summary>
        /// Counts the lines in the selected files.
        /// </summary>
        [STAThread()]   
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]  
        private void LoadFileRenamer()
        {
            //if (application == null)
           // {
           //     application = new FileRenamer.App { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
           // }

            //application.StartFromExtension(SelectedItemPaths);

            var domain1 = AppDomain.CreateDomain("FileRenamer");
            CrossAppDomainDelegate action = () =>
        {
            Thread thread = new Thread(() =>
            {
                App app = new App();
                app.StartFromExtension(SelectedItemPaths);
                //app.MainWindow = new Window1();
                //app.MainWindow.Show();
                app.Run();
            });
            thread.SetApartmentState(
             ApartmentState.STA);
            thread.Start();
        };

        }
    } 
}
