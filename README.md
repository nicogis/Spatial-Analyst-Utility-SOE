# Spatial Analyst Utility SOE Rest

This solution (developed in c#) creates a SOE Rest for arcgis server 10.7 for these operations:

- Watershed

Installation:
 
a) upload file Studioat.ArcGis.Soe.Rest.SAUtility.soe (see http://resources.arcgis.com/en/help/main/10.1/0154/0154000004sm000000.htm)

b) create a service map and enable the extension 'Spatial Analyst Utility' in capabilities.

c) Watershed operation
http://hostname/instanceags/rest/services/yourservice/MapServer/exts/SAUtility/watershed

Parameters:

1. idWatershed: integer (id of watershed created. It's returned from soe in field GRIDCODE of featureset) (optional)
2. location: Geometry Point (see rest api esri)
3. snapDistance: number (snap distance for pour points to the cell of highest flow accumulation)
4. idAccumulation: id layer of raster accumulation in service
5. idDirection: id layer of raster direction in service

In capabilities (operations allowed) you can allow these operations (Watershed).

I have added in file zip an example in api esri javascript to see how to use it.
You only need edit config.js. I have added data used for demo

The solutions are checked 100% with stylecop


[Live demo](https://sit.sistemigis.it/Samples/Watershed/) 
