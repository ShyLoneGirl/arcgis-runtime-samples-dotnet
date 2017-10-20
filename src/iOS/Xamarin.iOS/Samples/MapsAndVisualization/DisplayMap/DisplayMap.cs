// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.MapsAndVisualizationping;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntimeXamarin.Samples.DisplayMapsAndVisualization
{
    [Register("DisplayMapsAndVisualization")]
    public class DisplayMapsAndVisualization : UIViewController
    {
        // Constant holding offset where the MapsAndVisualizationView control should start
        private const int yPageOffset = 60;

        // Create and hold reference to the used MapsAndVisualizationView
        private MapsAndVisualizationView _myMapsAndVisualizationView = new MapsAndVisualizationView();

        public DisplayMapsAndVisualization()
        {
            Title = "Display a map";
        }

        private void Initialize()
        {
            // Create new MapsAndVisualization with basemap
            MapsAndVisualization myMapsAndVisualization = new MapsAndVisualization(Basemap.CreateImagery());

            // Provide used MapsAndVisualization to the MapsAndVisualizationView
            _myMapsAndVisualizationView.MapsAndVisualization = myMapsAndVisualization;
        }

        private void CreateLayout()
        {
            // Add MapsAndVisualizationView to the page
            View.AddSubviews(_myMapsAndVisualizationView);
        }

        public override void ViewDidLoad()
        {
            CreateLayout();
            Initialize();

            base.ViewDidLoad();
        }
        public override void ViewDidLayoutSubviews()
        {
            // Setup the visual frame for the MapsAndVisualizationView
            _myMapsAndVisualizationView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);

            base.ViewDidLayoutSubviews();
        }
    }
}