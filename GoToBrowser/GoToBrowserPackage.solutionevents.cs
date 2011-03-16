using System.IO;
using EnvDTE;
using GoToBrowser.Options;
using GoToBrowser.Utils;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace GoToBrowser
{
    /// <summary>
    /// <see cref="IVsSolutionEvents"/>を実装する部分クラスです。
    /// </summary>
    partial class GoToBrowserPackage : IVsSolutionEvents
    {
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            var dte = this.GetService<DTE>();
            _config.SolutionName = Path.GetFileNameWithoutExtension(dte.Solution.FullName);

            var persistence = this.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
            persistence.LoadPackageUserOpts(this, GeneralConfig.URL_FORMAT_SUO_KEY);

            SetCommandVisible();

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}