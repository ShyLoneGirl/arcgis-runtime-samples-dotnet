﻿// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using System.Drawing;
using UIKit;

namespace ArcGISRuntimeXamarin.Samples.SimpleRenderers
{
    [Register("SimpleRenderers")]
    public class SimpleRenderers : UIViewController
    {
        // Create and hold reference to the used MapView
        private MapView _myMapView = new MapView();

        public SimpleRenderers()
        {
            Title = "Simple renderer";
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
            // Setup the visual frame for the MapView
            _myMapView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);

            base.ViewDidLayoutSubviews();
        }

        private void Initialize()
        {
            // Create new map with basemap layer
            Map myMap = new Map(Basemap.CreateImageryWithLabels());

            // Add the map to the map view
            _myMapView.Map = myMap;

            // Create several map points using the WGS84 coordinates (latitude and longitude)
            MapPoint oldFaithfullPoint = new MapPoint(-110.828140, 44.460458, SpatialReferences.Wgs84);
            MapPoint cascadeGeyserPoint = new MapPoint(-110.829004, 44.462438, SpatialReferences.Wgs84);
            MapPoint plumeGeyserPoint = new MapPoint(-110.829381, 44.462735, SpatialReferences.Wgs84);

            // Use the two points farthest apart to create an envelope
            Envelope initialEnvelope = new Envelope(oldFaithfullPoint, plumeGeyserPoint);

            // Create a graphics overlay 
            GraphicsOverlay myGraphicOverlay = new GraphicsOverlay();

            // Create graphics based upon the map points
            Graphic oldFaithfullGraphic = new Graphic(oldFaithfullPoint);
            Graphic cascadeGeyserGraphic = new Graphic(cascadeGeyserPoint);
            Graphic plumeGeyserGraphic = new Graphic(plumeGeyserPoint);

            // Add the graphics to the graphics overlay
            myGraphicOverlay.Graphics.Add(oldFaithfullGraphic);
            myGraphicOverlay.Graphics.Add(cascadeGeyserGraphic);
            myGraphicOverlay.Graphics.Add(plumeGeyserGraphic);

            // Create a simple marker symbol - red, cross, size 12
            SimpleMarkerSymbol mySymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Cross, Color.Red, 12);

            // Create a simple renderer based on the simple marker symbol
            SimpleRenderer myRenderer = new SimpleRenderer(mySymbol);

            // Apply the renderer to the graphics overlay (all graphics use the same symbol)
            myGraphicOverlay.Renderer = myRenderer;

            // Add the graphics overlay to the map view
            _myMapView.GraphicsOverlays.Add(myGraphicOverlay);

            // Use the envelope to define the map views visible area (include some padding around the extent)
            _myMapView.SetViewpointGeometryAsync(initialEnvelope, 100);
        }

        private void CreateLayout()
        {  
            // Add MapView to the page
            View.AddSubviews(_myMapView);
        }
    }
}