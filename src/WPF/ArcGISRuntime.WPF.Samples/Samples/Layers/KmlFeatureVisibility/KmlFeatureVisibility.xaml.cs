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
using System.Collections.Generic;
using System.Windows;

namespace ArcGISRuntime.WPF.Samples.KmlFeatureVisibility
{
    public partial class KmlFeatureVisibility
    {
        public KmlFeatureVisibility()
        {
            InitializeComponent();

            // Create the UI, setup the control references and execute initialization
            Initialize();
        }


        // BUGS: 
        // - Consume KML Layer Phase 2 (https://devtopia.esri.com/runtime/dotnet-api/issues/5443)
        // - Install Phase 2 of KML Reading (https://devtopia.esri.com/runtimecore/c_api/issues/8181)
        // - KML Api Design - Phase 2 (https://devtopia.esri.com/runtime/runtime-mapping-api-design/issues/1642)
        // - Need to create in-house .kml data file to showcase the 'KmlLayer toggle feature visibility' sample (https://devtopia.esri.com/runtime/common-samples/issues/439)
        //
        // TODO: 
        // ISSUE: The .kml data file does not always load (1/2 time), it crashes Visual Studio. The visibility of 
        // Kml features does not always turn/on off.

        private async void Initialize()
        {
            try
            {
                // Create a new scene
                Scene myScene = new Scene();

                // Create Uri to the map image layer
                var serviceUri = new Uri("http://sampleserver5.arcgisonline.com/arcgis/rest/services/Elevation/WorldElevations/MapServer");

                // Create new image layer from the url
                ArcGISMapImageLayer imageLayer = new ArcGISMapImageLayer(serviceUri);

                // Add created layer to the basemaps collection
                myScene.Basemap.BaseLayers.Add(imageLayer);

                // TODO: NEED GOOD DATA CREATE IN-HOUSE TO AVOID APPROVED BY ESRI LEGAL FOR 3RD PARTY DATA
                Uri myKmlUri = new Uri(@"\\apps-data\Data\issue-data\dotnet-api\KmlLayers\KmlKmzData\KML_Overlays_3.kml");

                // Create a Kml dataset from the Uri
                KmlDataset myKmlDataset = new KmlDataset(myKmlUri);

                // Create a new instance of a KmlLayer layer 
                KmlLayer myKmlLayer = new KmlLayer(myKmlDataset);

                // Load the KmlLayer
                await myKmlLayer.LoadAsync();

                // Add the KmLayer to the scene's operational layers
                myScene.OperationalLayers.Add(myKmlLayer);

                // Assign the scene to the SceneView
                MySceneView.Scene = myScene;

                // Define a map point to zoom to (in the case: Mount Etna in Italy)
                MapPoint myMapPoint = new MapPoint(14.728, 33.414, 317408, SpatialReferences.Wgs84);

                // Define a camera centered on the map point with the appropriate heading, pitch and roll
                Camera myCamera = new Camera(myMapPoint, 5.99, 57.62, 0);

                // Zoom to the extent
                await MySceneView.SetViewpointCameraAsync(myCamera);
            }
            catch (Exception ex)
            {
                // Something went wrong, display a message
                MessageBox.Show(ex.ToString());
            }
        }

        private IList<KmlNode> FindAllFeatures(KmlDataset dataset)
        {
            // This function gathers a list of all KML features in the given dataset, recursively

            // Create an empty list of Kml features
            var list = new List<KmlNode>();

            // Loop through all the Kml features in the root features node
            foreach (KmlNode rootNode in dataset.RootNodes)

                // Call the recursive function get the nested Kml features from all of the child features
                CollectFeaturesAndChildren(rootNode, list);

            // Returns a flat list of all Kml features in the Kml dataset
            return list;
        }

        private void CollectFeaturesAndChildren(KmlNode feature, ICollection<KmlNode> collection)
        {
            // Add the Kml feature to the collection
            collection.Add(feature);

            if (feature is KmlContainer)
            {
                KmlContainer myKmlContainer = (KmlContainer)feature;
                // Recursively loop through each child feature in the Kml dataset  
                foreach (KmlNode child in myKmlContainer.ChildNodes)
                    CollectFeaturesAndChildren(child, collection);
            };
        }

        private void ToggleKmlFeatureVisibility(KmlLayer myKmlLayer, string overlayType)
        {
            // Get the Kml dataset from the Kml layer
            KmlDataset myKmlDataSet = myKmlLayer.Dataset;

            // Get the root features from the Kml dataset
            IReadOnlyList<KmlNode> myRootFeatures = myKmlDataSet.RootNodes;

            // Get all of the Kml features in the Kml dataset
            var myKmlFeatures = FindAllFeatures(myKmlDataSet);

            // Loop through each Kml feature
            foreach (KmlNode oneKmlFeature in myKmlFeatures)
            {
                // Toggle the visibility of Kml GroundOverlay types
                if (overlayType == "aKmlGroundOverlay")
                {
                    if (oneKmlFeature is KmlGroundOverlay)
                    {
                        oneKmlFeature.IsVisible = !oneKmlFeature.IsVisible;
                    }
                }

                // Toggle the visibility of Kml ScreenOverlay types 
                if (overlayType == "aKmlScreenOverlay")
                {
                    if (oneKmlFeature is KmlScreenOverlay)
                    {
                        oneKmlFeature.IsVisible = !oneKmlFeature.IsVisible;
                    }
                }

                // Toggle the visibility of Kml Placemark types 
                if (overlayType == "aKmlPlacemark")
                {
                    if (oneKmlFeature is KmlPlacemark)
                    {
                        oneKmlFeature.IsVisible = !oneKmlFeature.IsVisible;
                    }
                }
            }
        }

        private void ScreenOverlaysOnOff_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the layer collection from the scene view
            LayerCollection myOperationLayersCollectionSV = MySceneView.Scene.OperationalLayers;

            // Make sure there is at least on operational layer present 
            if (myOperationLayersCollectionSV.Count > 0)
            {
                // Get the first operational layer 
                Layer myLayerSV = myOperationLayersCollectionSV[0];

                // Make sure the operational layer is a KmlLayer
                if (myLayerSV is KmlLayer)
                {
                    // Cast the operational layer to a KmLayer
                    KmlLayer myKmlLayerSV = (KmlLayer)myLayerSV;

                    // Call the function to toggle on/off Kml screen overlays
                    ToggleKmlFeatureVisibility(myKmlLayerSV, "aKmlScreenOverlay");
                }
            }

        }

        private void GroundOverlaysOnOff_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the layer collection from the scene view
            LayerCollection myOperationLayersCollectionSV = MySceneView.Scene.OperationalLayers;

            // Make sure there is at least on operational layer present 
            if (myOperationLayersCollectionSV.Count > 0)
            {
                // Get the first operational layer 
                Layer myLayerSV = myOperationLayersCollectionSV[0];

                // Make sure the operational layer is a KmlLayer
                if (myLayerSV is KmlLayer)
                {
                    // Cast the operational layer to a KmLayer
                    KmlLayer myKmlLayerSV = (KmlLayer)myLayerSV;

                    // Call the function to toggle on/off Kml ground overlays
                    ToggleKmlFeatureVisibility(myKmlLayerSV, "aKmlGroundOverlay");
                }
            }
        }

        private void PLacemarksOnOff_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the layer collection from the scene view
            LayerCollection myOperationLayersCollectionSV = MySceneView.Scene.OperationalLayers;

            // Make sure there is at least on operational layer present 
            if (myOperationLayersCollectionSV.Count > 0)
            {
                // Get the first operational layer 
                Layer myLayerSV = myOperationLayersCollectionSV[0];

                // Make sure the operational layer is a KmlLayer
                if (myLayerSV is KmlLayer)
                {
                    // Cast the operational layer to a KmLayer
                    KmlLayer myKmlLayerSV = (KmlLayer)myLayerSV;

                    // Call the function to toggle on/off Kml placemarks
                    ToggleKmlFeatureVisibility(myKmlLayerSV, "aKmlPlacemark");
                }
            }
        }

    }
}