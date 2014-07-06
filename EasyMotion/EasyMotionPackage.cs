using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;

namespace EasyMotion
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidEasyMotionPkgString)]
    public sealed class EasyMotionPackage : Package
    {
        private IComponentModel _componentModel;
        private ExportProvider _exportProvider;

        public EasyMotionPackage()
        {

        }

        protected override void Initialize()
        {
            base.Initialize();

            _componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            _exportProvider = _componentModel.DefaultExportProvider;

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidEasyMotionCmdSet, (int)PkgCmdIDList.CmdEasyMotionNavigate);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            ITextView textView;
            if (!TryGetActiveTextView(out textView))
            {
                return;
            }

            var easyMotionUtilProvider = _exportProvider.GetExportedValue<IEasyMotionUtilProvider>();
            var easyMotionUtil = easyMotionUtilProvider.GetEasyMotionUtil(textView);

            // HACK: just doing this to drive the example right now, need a real implementation
            if (easyMotionUtil.State == EasyMotionState.LookingForDecision)
            {
                easyMotionUtil.ChangeToDisabled();
            }
            else
            {
                easyMotionUtil.ChangeToLookingForDecision('a');
            }
        }

        private bool TryGetActiveTextView(out ITextView textView)
        {
            var vsTextManager = (IVsTextManager)GetService(typeof(SVsTextManager));

            IVsTextView vsTextView;
            if (ErrorHandler.Failed(vsTextManager.GetActiveView(0, null, out vsTextView)))
            {
                textView = null;
                return false;
            }

            try
            {
                var vsEditorAdaptersFactoryService = _exportProvider.GetExportedValue<IVsEditorAdaptersFactoryService>();
                textView = vsEditorAdaptersFactoryService.GetWpfTextView(vsTextView);
                return textView != null;
            }
            catch
            {
                // GetWpfTextView can throw an exception
                textView = null;
                return false;
            }
        }
    }
}
