﻿using System.IO;
using EnvDTE;
using GoToBrowser.Configs;
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
        /// <inheritdoc />
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            var persistence = this.GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
            persistence.LoadPackageUserOpts(this, ConfigContents.CONFIG_SUO_KEY);

            SetCommandVisible();

            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }
    }
}