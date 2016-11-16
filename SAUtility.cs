//-----------------------------------------------------------------------
// <copyright file="SAUtility.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGis.Soe.Rest.SAUtility
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.GeoAnalyst;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using ESRI.ArcGIS.Server;
    using ESRI.ArcGIS.SOESupport;
    using ESRI.ArcGIS.SpatialAnalyst;

    /// <summary>
    /// class Spatial Analyst Utility
    /// </summary>
    [ComVisible(true)]
    [Guid("35949b83-d3d5-4d39-858a-07197f32b40b")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",
        AllCapabilities = "Watershed",
        DefaultCapabilities = "Watershed",
        Description = "Spatial Analyst Utility",
        DisplayName = "Spatial Analyst Utility",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Warning - Code ESRI - Capabilities")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Warning - Code ESRI - pSOH")]
    public class SAUtility : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        /// <summary>
        /// name of field Id Watershed
        /// </summary>
        private const string FieldNameIdWatershed = "IdWatershed";

        /// <summary>
        /// message code soe
        /// </summary>
        private const int MessageCodeSOE = 8467;

        /// <summary>
        /// name of soe
        /// </summary>
        private string soeName;

        /// <summary>
        /// config properties
        /// </summary>
        private IPropertySet configProps;

        /// <summary>
        /// server object Helper
        /// </summary>
        private IServerObjectHelper serverObjectHelper;

        /// <summary>
        /// logger soe
        /// </summary>
        private ServerLogger logger;

        /// <summary>
        /// REST Request Handler
        /// </summary>
        private IRESTRequestHandler reqHandler;

        /// <summary>
        /// Initializes a new instance of the SAUtility class
        /// </summary>
        public SAUtility()
        {
            this.soeName = this.GetType().Name;
            this.logger = new ServerLogger();
            this.reqHandler = new SoeRestImpl(this.soeName, this.CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        /// <summary>
        /// event initialization soe
        /// </summary>
        /// <param name="pSOH">Server Object Helper</param>
        public void Init(IServerObjectHelper pSOH)
        {
            this.serverObjectHelper = pSOH;
        }

        /// <summary>
        /// event shutdown soe
        /// </summary>
        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        /// <summary>
        /// event construct of soe
        /// </summary>
        /// <param name="props">properties of soe</param>
        public void Construct(IPropertySet props)
        {
            this.configProps = props;
        }

        #endregion

        #region IRESTRequestHandler Members

        /// <summary>
        /// get schema soe
        /// </summary>
        /// <returns>schema soe</returns>
        public string GetSchema()
        {
            return this.reqHandler.GetSchema();
        }

        /// <summary>
        /// return Handle REST Request
        /// </summary>
        /// <param name="Capabilities">Capabilities of soe</param>
        /// <param name="resourceName">resource Name</param>
        /// <param name="operationName">operation Name</param>
        /// <param name="operationInput">operation Input</param>
        /// <param name="outputFormat">output Format</param>
        /// <param name="requestProperties">request Properties</param>
        /// <param name="responseProperties">response Properties</param>
        /// <returns>Handle REST Request</returns>
        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return this.reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        /// <summary>
        /// create schema rest
        /// </summary>
        /// <returns>schema rest</returns>
        private RestResource CreateRestSchema()
        {
            RestResource rootResource = new RestResource(this.soeName, false, this.RootResHandler);

            RestOperation operationWatershed = new RestOperation("watershed", new string[] { "idWatershed", "location", "snapDistance", "idAccumulation", "idDirection" }, new string[] { "json" }, this.OperationWatershedHandler, "Watershed");

            rootResource.operations.Add(operationWatershed);

            return rootResource;
        }

        /// <summary>
        /// return root resource handler
        /// </summary>
        /// <param name="boundVariables">bound Variables</param>
        /// <param name="outputFormat">output Format</param>
        /// <param name="requestProperties">request Properties</param>
        /// <param name="responseProperties">response Properties</param>
        /// <returns>root resource handler</returns>
        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = "{\"Content-Type\" : \"application/json\"}";

            JsonObject result = new JsonObject();
            AddInPackageAttribute addInPackage = (AddInPackageAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AddInPackageAttribute), false)[0];
            result.AddString("agsVersion", addInPackage.TargetVersion);
            result.AddString("soeVersion", addInPackage.Version);
            result.AddString("author", addInPackage.Author);
            result.AddString("company", addInPackage.Company);

            return result.JsonByte();
        }

        /// <summary>
        /// return operation watershed handler 
        /// </summary>
        /// <param name="boundVariables">>bound Variables</param>
        /// <param name="operationInput">operation Input</param>
        /// <param name="outputFormat">output Format</param>
        /// <param name="requestProperties">request Properties</param>
        /// <param name="responseProperties">response Properties</param>
        /// <returns>operation watershed handler</returns>
        private byte[] OperationWatershedHandler(NameValueCollection boundVariables, JsonObject operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            long? idWatershed;
            if (operationInput.Exists("idWatershed"))
            {
                if (!operationInput.TryGetAsLong("idWatershed", out idWatershed))
                {
                    throw new ArgumentNullException("idWatershed");
                }
            }
            else
            {
                idWatershed = 0;
            }

            JsonObject jsonObjectPoint;
            if (!operationInput.TryGetJsonObject("location", out jsonObjectPoint))
            {
                throw new ArgumentNullException("location");
            }

            IPoint location = Conversion.ToGeometry(jsonObjectPoint, esriGeometryType.esriGeometryPoint) as IPoint;
            if (location == null)
            {
                throw new ArgumentException("Invalid location", "location");
            }

            double? snapDistance;
            if (!operationInput.TryGetAsDouble("snapDistance", out snapDistance))
            {
                throw new ArgumentNullException("snapDistance");
            }

            snapDistance = snapDistance ?? 0.0;

            long? idAccumulation;
            if (!operationInput.TryGetAsLong("idAccumulation", out idAccumulation) || !idAccumulation.HasValue)
            {
                throw new ArgumentNullException("idAccumulation");
            }

            long? idDirection;
            if (!operationInput.TryGetAsLong("idDirection", out idDirection) || !idDirection.HasValue)
            {
                throw new ArgumentNullException("idDirection");
            }

            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                IFeatureWorkspace featureWorkspace = Helper.CreateInMemoryWorkspace() as IFeatureWorkspace;
                IFeatureClass featureClass = this.CreateFeatureClass(location, featureWorkspace);

                IFeature feature = featureClass.CreateFeature();
                feature.Shape = location;
                feature.set_Value(featureClass.FindField(SAUtility.FieldNameIdWatershed), (int)idWatershed.Value);
                feature.Store();

                IHydrologyOp hydrologyOp = new RasterHydrologyOp() as IHydrologyOp;

                IGeoDataset accumulation = this.GetGeodataset((int)idAccumulation.Value);
                IGeoDataset direction = this.GetGeodataset((int)idDirection.Value);

                IFeatureClassDescriptor featureClassDescriptor = new FeatureClassDescriptorClass();
                featureClassDescriptor.Create(featureClass, null, SAUtility.FieldNameIdWatershed);
                IGeoDataset pourPoint = featureClassDescriptor as IGeoDataset;

                IRasterAnalysisEnvironment rasterAnalysisEnvironment = new RasterAnalysisClass();
                object extentProvider = Type.Missing;
                object snapRasterData = Type.Missing;
                rasterAnalysisEnvironment.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvMaxOf, ref extentProvider, ref snapRasterData);

                IGeoDataset snapRaster = hydrologyOp.SnapPourPoint(pourPoint, accumulation, snapDistance.Value);

                IGeoDataset watershed = hydrologyOp.Watershed(direction, snapRaster);

                IConversionOp conversionOp = new RasterConversionOpClass() as IConversionOp;
                IGeoDataset featureClassWatershed = conversionOp.RasterDataToPolygonFeatureData(watershed, featureWorkspace as IWorkspace, "WatershedPolygon", true);

                IRecordSetInit recordset = new RecordSetClass();
                recordset.SetSourceTable(featureClassWatershed as ITable, null);

                byte[] recorset = Conversion.ToJson(recordset as IRecordSet);
                this.logger.LogMessage(ServerLogger.msgType.infoDetailed, methodName, SAUtility.MessageCodeSOE, string.Format("Watershed created with succcess. IdWatershed {0}", (int)idWatershed.Value));
                return recorset;
            }
            catch (Exception ex)
            {
                this.logger.LogMessage(ServerLogger.msgType.error, methodName, SAUtility.MessageCodeSOE, ex.Message);
                return new ObjectError("error create watershed").ToJsonObject().JsonByte();
            }
        }

        /// <summary>
        /// Geodataset from id of layer
        /// </summary>
        /// <param name="layerID">unique id layer</param>
        /// <returns>object Geodataset</returns>
        private IGeoDataset GetGeodataset(int layerID)
        {
            IMapServer3 mapServer = this.GetMapServer();
            IMapServerDataAccess dataAccess = (IMapServerDataAccess)mapServer;
            return (IGeoDataset)dataAccess.GetDataSource(mapServer.DefaultMapName, layerID);
        }

        /// <summary>
        /// Get object MapServer of ServerObject 
        /// </summary>
        /// <returns>object MapServer</returns>
        private IMapServer3 GetMapServer()
        {
            IMapServer3 mapServer = this.serverObjectHelper.ServerObject as IMapServer3;
            if (mapServer == null)
            {
                throw new SpatialAnalystException("Unable to access the map server.");
            }

            return mapServer;
        }

        /// <summary>
        /// create a feature class in workspace with type geometry
        /// </summary>
        /// <param name="geometry">geometry of feature class</param>
        /// <param name="featureWorkspace">workspace for store feature class</param>
        /// <returns>feature class created</returns>
        private IFeatureClass CreateFeatureClass(IGeometry geometry, IFeatureWorkspace featureWorkspace)
        {
            // Create a fields collection for the feature class.  
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            // Add an object ID field to the fields collection. This is mandatory for feature classes.  
            IField oidField = new FieldClass();
            IFieldEdit oidFieldEdit = (IFieldEdit)oidField;
            oidFieldEdit.Name_2 = "OID";
            oidFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(oidField);

            IField idField = new FieldClass();
            IFieldEdit idFieldEdit = (IFieldEdit)idField;
            idFieldEdit.Name_2 = SAUtility.FieldNameIdWatershed;
            idFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
            fieldsEdit.AddField(idField);
            
            // Create a geometry definition (and spatial reference) for the feature class.  
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geometry.GeometryType;
            geometryDefEdit.SpatialReference_2 = geometry.SpatialReference;

            IFeatureClassDescription featureClassDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription objectClassDescription = (IObjectClassDescription)featureClassDescription;

            // Add a geometry field to the fields collection. This is where the geometry definition is applied.  
            IField geometryField = new FieldClass();
            IFieldEdit geometryFieldEdit = (IFieldEdit)geometryField;
            geometryFieldEdit.Name_2 = featureClassDescription.ShapeFieldName;
            geometryFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            geometryFieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(geometryField);

            // Use IFieldChecker to create a validated fields collection.  
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

            // The enumFieldError enumerator can be inspected at this point to determine   
            // which fields were modified during validation.
            // Create the feature class. Note that the CLSID parameter is null - this indicates to use the  
            // default CLSID, esriGeodatabase.Feature (acceptable in most cases for feature classes).  
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass("pourPoint", validatedFields, objectClassDescription.InstanceCLSID, objectClassDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, featureClassDescription.ShapeFieldName, string.Empty);
            return featureClass;
        }
    }
}
