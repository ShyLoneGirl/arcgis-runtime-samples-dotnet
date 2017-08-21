// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using System;
using Esri.ArcGISRuntime.Geometry;
using System.Windows;
using ArcGISRuntime.Samples.Managers;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Reflection;

namespace ArcGISRuntime.WPF.Samples.KmlLayerTOC
{
    public partial class KmlLayerTOC
    {
        public KmlLayerTOC()
        {
            InitializeComponent();

            // Create the UI, setup the control references and execute initialization
            Initialize();
        }

        private async void Initialize()
        {
            try
            {

                // BUGS: 
                // - Table of Contents Implementation (https://devtopia.esri.com/runtime/common-toolkit/issues/47)
                //
                // TODO: 
                // ISSUE: The Table of Contents (TOC) is not yet implemented for Quartz Update 2. Until this is done we 
                // cannot create a sample that demonstrates using this functionality.

                // ******************************************************************************************************
                // TODO: THIS IS USING A MOCK IMAGE FOR THE TOC. THIS IS NOT WHAT THE REAL CODE WOULD BE. 
                // NEED TO USE THE ARCGIS RUNTIME TOC CONTROL AND BIND IT TO THE MAP/MAPVIEW.
                var currentAssembly = Assembly.GetExecutingAssembly();
                var resourceStream = currentAssembly.GetManifestResourceStream("ArcGISRuntime.WPF.Resources.MOCK_TOC.jpg");
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.StreamSource = resourceStream;
                myBitmapImage.EndInit();
                Mock_TOC.Stretch = Stretch.Fill;
                Mock_TOC.Source = myBitmapImage;
                // ******************************************************************************************************

                // The Kml file will be downloaded from ArcGIS Online
                // Note: The DataManager is a helper class of the sample viewer application that handles 
                // downloading data files to the local disk of the device, it is *NOT* a class of the ArcGIS Runtime

                // The desired Kml file is expected to be called US_State_Capitals.kml
                string filename = "US_State_Capitals.kml";

                // The data manager provides a method to get the folder
                string folder = DataManager.GetDataFolder();

                // Create the file and path location (aka. filepath variable) to the .kml file on the local disk of the device
                string filepath = Path.Combine(folder, "SampleData", "LoadKmlFromDisk", filename);

                // Check if the file exists
                if (!File.Exists(filepath))
                {
                    // Download the Kml file
                    await DataManager.GetData("324e4742820e46cfbe5029ff2c32cb1f", "LoadKmlFromDisk");
                }

                // Create a new map
                Map myMap = new Map();

                // Define the basemap
                //myMap.Basemap = Basemap.CreateDarkGrayCanvasVector();

                // Create a Uri for the Kml data on the local device
                Uri myKmlUri = new Uri(filepath);

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
            catch (Exception ex)
            {
                // Something went wrong, display a message
                MessageBox.Show(ex.ToString());
            }
        }

    }
}