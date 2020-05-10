using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "2.0.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidEasyMotionPkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class EasyMotionPackage : AsyncPackage
    {
        private ExportProvider _exportProvider;

        public EasyMotionPackage() { }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await SetExportProvider();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                // Switch to main thread before calling AddCommand because it calls GetService
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidEasyMotionCmdSet, (int)PkgCmdIDList.CmdEasyMotionNavigate);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private async Task SetExportProvider()
        {
            if (await GetServiceAsync(typeof(SComponentModel)) is IComponentModel componentModel)
            {
                _exportProvider = componentModel.DefaultExportProvider;
            }
            else
            {
                throw new Exception("Unable to fetch ComponentModel");
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
                easyMotionUtil.ChangeToLookingForChar();
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
