//-----------------------------------------------------------------------
// <copyright file="Helper.cs" company="Studio A&T s.r.l.">
//  Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGis.Soe.Rest.SAUtility
{
    using System;
    using ESRI.ArcGIS.DataSourcesRaster;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;

    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// return value of pixel
        /// </summary>
        /// <param name="raster">object raster</param>
        /// <param name="x">coordinate x</param>
        /// <param name="y">coordinate y</param>
        /// <returns>value of pixel</returns>
        public static double IdentifyPixelValue(IRaster2 raster, double x, double y)
        {
            // Get the column and row by giving x,y coordinates in a map space.
            int col = raster.ToPixelColumn(x);
            int row = raster.ToPixelRow(y);

            // Get the value at a given band.
            object o = raster.GetPixelValue(0, col, row);
            return (o == null) ? double.NaN : Convert.ToDouble(o);
        }

        /// <summary>
        /// Open a raster dataset in a geodatabase (PGDB, FGDB, or ArcSDE).
        /// </summary>
        /// <param name="rasterWorkspaceEx">raster workspace</param>
        /// <param name="datasetName">name dataset</param>
        /// <returns>object raster dataset</returns>
        public static IRasterDataset OpenRasterDataset(IRasterWorkspaceEx rasterWorkspaceEx, string datasetName)
        {
            return rasterWorkspaceEx.OpenRasterDataset(datasetName);
        }

        /// <summary>
        /// open geodatabase from path filename
        /// </summary>
        /// <param name="path">path file name of file geodatabase</param>
        /// <returns>object Workspace</returns>
        public static IWorkspace OpenFGDB(string path)
        {
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory2 workspaceFactory = (IWorkspaceFactory2)Activator.CreateInstance(factoryType);
            return workspaceFactory.OpenFromFile(path, 0);
        }

        /// <summary>
        /// create a memory workspace
        /// </summary>
        /// <returns>memory workspace</returns>
        public static IWorkspace CreateInMemoryWorkspace()
        {
            // Create an in-memory workspace factory.
            Type factoryType = Type.GetTypeFromProgID(
              "esriDataSourcesGDB.InMemoryWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)
              Activator.CreateInstance(factoryType);

            // Create an in-memory workspace.
            IWorkspaceName workspaceName = workspaceFactory.Create(string.Empty, "memoryWorkspace", null, 0);

            // Cast for IName and open a reference to the in-memory workspace through the name object. Guid.NewGuid().ToString()
            IName name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)name.Open();
            return workspace;
        }

        /// <summary>
        /// open file Scratch workspace
        /// </summary>
        /// <returns>workspace scratch</returns>
        public static IWorkspace OpenFileGdbScratchWorkspace()
        {
            // Create a file scratch workspace factory.
            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.FileGDBScratchWorkspaceFactory");
            IScratchWorkspaceFactory scratchWorkspaceFactory = (IScratchWorkspaceFactory)
                Activator.CreateInstance(factoryType);

            // Get the default scratch workspace.
            IWorkspace scratchWorkspace = scratchWorkspaceFactory.DefaultScratchWorkspace;
            return scratchWorkspace;
        }
    }
}
