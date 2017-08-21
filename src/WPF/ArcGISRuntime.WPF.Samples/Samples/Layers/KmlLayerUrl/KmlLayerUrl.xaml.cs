// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using System;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Ogc;

namespace ArcGISRuntime.WPF.Samples.KmlLayerUrl
{
    public partial class KmlLayerUrl
    {
        public KmlLayerUrl()
        {
            InitializeComponent();

            // Create the UI, setup the control references and execute initialization
            Initialize();
        }

        private async void Initialize()
        {
            // Create a new map
            Map myMap = new Map();

            // Define the basemap
            myMap.Basemap = Basemap.CreateDarkGrayCanvasVector();

            // Create a Uri for the Kml data on the web server
            Uri myKmlUri = new Uri("http://www.wpc.ncep.noaa.gov/kml/noaa_chart/WPC_Day1_SigWx.kml");

            // Create a Kml dataset from the Uri
            KmlDataset myKmlDataset = new KmlDataset(myKmlUri);

            // Create a new instance of a KmlLayer layer 
            KmlLayer myKmlLayer = new KmlLayer(myKmlDataset);

            // Add the KmLayer to the map's operational layers
            myMap.OperationalLayers.Add(myKmlLayer);

            // Assign the map to the MapView
            MyMapView.Map = myMap;

            // Define and extent to zoom to (in the case: the continental United States)
            Envelope myExtent = new Envelope(-14029640.7178862, 2641221.12421063, -7285597.58055383, 6520185.12465264, SpatialReferences.WebMercator);

            // Zoom to the extent
            await MyMapView.SetViewpointGeometryAsync(myExtent);
        }
    }
}