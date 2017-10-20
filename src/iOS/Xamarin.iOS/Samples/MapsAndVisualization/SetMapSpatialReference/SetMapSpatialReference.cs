// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.MapsAndVisualizationping;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using System;
using UIKit;

namespace ArcGISRuntimeXamarin.Samples.SetMapsAndVisualizationSpatialReference
{
    [Register("SetMapsAndVisualizationSpatialReference")]
    public class SetMapsAndVisualizationSpatialReference : UIViewController
    {
        // Constant holding offset where the MapsAndVisualizationView control should start
        private const int yPageOffset = 60;

        // Create and hold reference to the used MapsAndVisualizationView
        private MapsAndVisualizationView _myMapsAndVisualizationView = new MapsAndVisualizationView();

        public SetMapsAndVisualizationSpatialReference()
        {
            Title = "Set map spatial reference";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the UI, setup the control references and execute initialization 
            CreateLayout();
            Initialize();
        }

        public override void ViewDidLayoutSubviews()
        {
            // Setup the visual frame for the MapsAndVisualizationView
            _myMapsAndVisualizationView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);

            base.ViewDidLayoutSubviews();
        }

        private void Initialize()
        {
            // Create new MapsAndVisualization using spatial reference as world bonne (54024)
            MapsAndVisualization myMapsAndVisualization = new MapsAndVisualization(SpatialReference.Create(54024));

            // Adding a map image layer which can reproject itself to the map's spatial reference
            // Note: Some layer such as tiled layer cannot reproject and will fail to draw if their spatial 
            // reference is not the same as the map's spatial reference
            ArcGISMapsAndVisualizationImageLayer operationalLayer = new ArcGISMapsAndVisualizationImageLayer(new Uri(
                "https://sampleserver6.arcgisonline.com/arcgis/rest/services/SampleWorldCities/MapsAndVisualizationServer"));

            // Add operational layer to the MapsAndVisualization
            myMapsAndVisualization.OperationalLayers.Add(operationalLayer);

            // Assign the map to the MapsAndVisualizationView
            _myMapsAndVisualizationView.MapsAndVisualization = myMapsAndVisualization;
        }

        private void CreateLayout()
        {
           // Add MapsAndVisualizationView to the page
            View.AddSubviews(_myMapsAndVisualizationView);
        }
    }
}