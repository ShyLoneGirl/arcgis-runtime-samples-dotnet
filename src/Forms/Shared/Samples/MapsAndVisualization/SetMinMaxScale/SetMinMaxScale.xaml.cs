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
using Xamarin.Forms;

namespace ArcGISRuntimeXamarin.Samples.SetMinMaxScale
{
    public partial class SetMinMaxScale : ContentPage
    {
        public SetMinMaxScale()
        {
            InitializeComponent ();

            Title = "Set Min & Max Scale";
            
            // Create the UI, setup the control references and execute initialization 
            Initialize();
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
            MyMapsAndVisualizationView.MapsAndVisualization = myMapsAndVisualization;
        }
    }
}
