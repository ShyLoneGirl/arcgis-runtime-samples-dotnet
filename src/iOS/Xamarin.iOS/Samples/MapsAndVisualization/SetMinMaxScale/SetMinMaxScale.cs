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
using UIKit;

namespace ArcGISRuntimeXamarin.Samples.SetMinMaxScale
{
    [Register("SetMinMaxScale")]
    public class SetMinMaxScale : UIViewController
    {
        // Constant holding offset where the MapsAndVisualizationView control should start
        private const int yPageOffset = 60;

        // Create and hold reference to the used MapsAndVisualizationView
        private MapsAndVisualizationView _myMapsAndVisualizationView = new MapsAndVisualizationView();

        public SetMinMaxScale()
        {
            Title = "Set Min & Max Scale";
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
            // Create new MapsAndVisualization with Streets basemap 
            MapsAndVisualization myMapsAndVisualization = new MapsAndVisualization(Basemap.CreateStreets());

            // Set the scale at which this layer can be viewed
            // MinScale defines how far 'out' you can zoom where
            // MaxScale defines how far 'in' you can zoom.
            myMapsAndVisualization.MinScale = 8000;
            myMapsAndVisualization.MaxScale = 2000;

            // Create central point where map is centered
            MapsAndVisualizationPoint centralPoint = new MapsAndVisualizationPoint(-355453, 7548720, SpatialReferences.WebMercator);

            // Create starting viewpoint
            Viewpoint startingViewpoint = new Viewpoint(
                centralPoint,
                3000);
            // Set starting viewpoint
            myMapsAndVisualization.InitialViewpoint = startingViewpoint;

            // Set map to mapview
            _myMapsAndVisualizationView.MapsAndVisualization = myMapsAndVisualization;
        }

        private void CreateLayout()
        {
            // Add MapsAndVisualizationView to the page
            View.AddSubviews(_myMapsAndVisualizationView);
        }
    }
}