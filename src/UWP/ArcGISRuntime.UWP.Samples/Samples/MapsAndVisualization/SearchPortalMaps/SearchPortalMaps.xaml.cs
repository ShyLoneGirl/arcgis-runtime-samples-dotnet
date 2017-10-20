// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.MapsAndVisualizationping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntime.UWP.Samples.SearchPortalMapsAndVisualizations
{
    public partial class SearchPortalMapsAndVisualizations
    {
        // OAuth configuration and default values ...
        // URL of the server to authenticate with (ArcGIS Online)
        private const string ArcGISOnlineUrl = "https://www.arcgis.com/sharing/rest";

        // Client ID for the app registered with the server (Portal MapsAndVisualizations)
        private string AppClientId = "2Gh53JRzkPtOENQq";

        // Redirect URL after a successful authorization (configured for the Portal MapsAndVisualizations application)
        private string OAuthRedirectUrl = "https://developers.arcgis.com";

        public SearchPortalMapsAndVisualizations()
        {
            InitializeComponent();

            // Show the default map
            DisplayDefaultMapsAndVisualization();

            // When the map view loads, show a dialog for entering OAuth settings
            MyMapsAndVisualizationView.Loaded += (s, e) => ShowOAuthSettingsDialog();
        }

        private void DisplayDefaultMapsAndVisualization()
        {
            // Create a new light gray canvas MapsAndVisualization
            MapsAndVisualization myMapsAndVisualization = new MapsAndVisualization(Basemap.CreateLightGrayCanvas());

            // Assign MapsAndVisualization to the MapsAndVisualizationView
            MyMapsAndVisualizationView.MapsAndVisualization = myMapsAndVisualization;
        }

        private async void ShowOAuthSettingsDialog()
        {
            // Show default settings for client ID and redirect URL
            ClientIdTextBox.Text = AppClientId;
            RedirectUrlTextBox.Text = OAuthRedirectUrl;

            // Display inputs for a client ID and redirect URL to use for OAuth authentication
            ContentDialogResult result = await OAuthSettingsDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Settings were provided, update the configuration settings for OAuth authorization
                AppClientId = ClientIdTextBox.Text.Trim();
                OAuthRedirectUrl = RedirectUrlTextBox.Text.Trim();

                // Update authentication manager with the OAuth settings
                UpdateAuthenticationManager();
            }
            else
            {
                // User canceled, warn that won't be able to save
                var messageDlg = new MessageDialog("No OAuth settings entered, you will not be able to browse maps from your ArcGIS Online account.");
                await messageDlg.ShowAsync();

                AppClientId = string.Empty;
                OAuthRedirectUrl = string.Empty;
            }
        }

        private void OnMapsAndVisualizationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When a web map is selected, update the map in the map view
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                // Make sure a portal item is selected
                var selectedMapsAndVisualization = e.AddedItems[0] as PortalItem;
                if (selectedMapsAndVisualization == null) { return; }

                // Create a new map and display it
                var webMapsAndVisualization = new MapsAndVisualization(selectedMapsAndVisualization);

                // Handle change in the load status (to report load errors)
                webMapsAndVisualization.LoadStatusChanged += WebMapsAndVisualizationLoadStatusChanged;

                MyMapsAndVisualizationView.MapsAndVisualization = webMapsAndVisualization;
            }

            // Hide the flyouts
            SearchMapsAndVisualizationsFlyout.Hide();
            MyMapsAndVisualizationsFlyout.Hide();

            // Unselect the map item
            var list = sender as ListView;
            list.SelectedItem = null;
        }

        private void WebMapsAndVisualizationLoadStatusChanged(object sender, Esri.ArcGISRuntime.LoadStatusEventArgs e)
        {
            // Get the current status
            var status = e.Status;

            // Report errors if map failed to load
            if (status == Esri.ArcGISRuntime.LoadStatus.FailedToLoad)
            {
                var map = sender as MapsAndVisualization;
                var err = map.LoadError;
                if (err != null)
                {
                    var dialog = new MessageDialog(err.Message, "MapsAndVisualization Load Error");
                    dialog.ShowAsync();
                }
            }
        }

        private async void MyMapsAndVisualizationsClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Get web map portal items in the current user's folder or from a keyword search
            IEnumerable<PortalItem> mapItems = null;
            ArcGISPortal portal;

            // If the list has already been populated, return
            if (MyMapsAndVisualizationsList.ItemsSource != null) { return; }

            // Call a sub that will force the user to log in to ArcGIS Online (if they haven't already)
            var loggedIn = await EnsureLoggedInAsync();
            if(!loggedIn) { return; }
            
            // Connect to the portal (will connect using the provided credentials)
            portal = await ArcGISPortal.CreateAsync(new Uri(ArcGISOnlineUrl));

            // Get the user's content (items in the root folder and a collection of sub-folders)
            PortalUserContent myContent = await portal.User.GetContentAsync();
            
            // Get the web map items in the root folder
            mapItems = from item in myContent.Items where item.Type == PortalItemType.WebMapsAndVisualization select item;

            // Loop through all sub-folders and get web map items, add them to the mapItems collection
            foreach (PortalFolder folder in myContent.Folders)
            {
                IEnumerable<PortalItem> folderItems = await portal.User.GetContentAsync(folder.FolderId);
                mapItems.Concat(from item in folderItems where item.Type == PortalItemType.WebMapsAndVisualization select item);
            }
            
            // Show the web maps in the list box
            MyMapsAndVisualizationsList.ItemsSource = mapItems;

            // Make sure the flyout is shown
            MyMapsAndVisualizationsFlyout.ShowAt(sender as Windows.UI.Xaml.FrameworkElement);
        }

        private async void SearchMapsAndVisualizationsClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Get web map portal items in the current user's folder or from a keyword search
            IEnumerable<PortalItem> mapItems = null;
            ArcGISPortal portal;

            // Connect to the portal (anonymously)
            portal = await ArcGISPortal.CreateAsync();

            // Create a query expression that will get public items of type 'web map' with the keyword(s) in the items tags
            var queryExpression = string.Format("tags:\"{0}\" access:public type: (\"web map\" NOT \"web mapping application\")", SearchText.Text);
            // Create a query parameters object with the expression and a limit of 10 results
            PortalQueryParameters queryParams = new PortalQueryParameters(queryExpression, 10);

            // Search the portal using the query parameters and await the results
            PortalQueryResultSet<PortalItem> findResult = await portal.FindItemsAsync(queryParams);
            // Get the items from the query results
            mapItems = findResult.Results;

            // Show the search result items in the list
            SearchMapsAndVisualizationsList.ItemsSource = mapItems;
        }

        private async Task<bool> EnsureLoggedInAsync()
        {
            bool loggedIn = false;
            try
            {
                // Create a challenge request for portal credentials (OAuth credential request for arcgis.com)
                CredentialRequestInfo challengeRequest = new CredentialRequestInfo();

                // Use the OAuth implicit grant flow
                challengeRequest.GenerateTokenOptions = new GenerateTokenOptions
                {
                    TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit
                };

                // Indicate the url (portal) to authenticate with (ArcGIS Online)
                challengeRequest.ServiceUri = new Uri(ArcGISOnlineUrl);

                // Call GetCredentialAsync on the AuthenticationManager to invoke the challenge handler
                var cred = await AuthenticationManager.Current.GetCredentialAsync(challengeRequest, false);
                loggedIn = cred != null;
            }
            catch (OperationCanceledException ex)
            {
                // TODO: handle login canceled
            }
            catch (Exception ex)
            {
                // TODO: handle login failure
            }

            return loggedIn;
        }        

        private void UpdateAuthenticationManager()
        {
            // Define the server information for ArcGIS Online
            ServerInfo portalServerInfo = new ServerInfo();

            // ArcGIS Online URI
            portalServerInfo.ServerUri = new Uri(ArcGISOnlineUrl);

            // Type of token authentication to use
            portalServerInfo.TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit;

            // Define the OAuth information
            OAuthClientInfo oAuthInfo = new OAuthClientInfo
            {
                ClientId = AppClientId,
                RedirectUri = new Uri(OAuthRedirectUrl)
            };
            portalServerInfo.OAuthClientInfo = oAuthInfo;

            // Get a reference to the (singleton) AuthenticationManager for the app
            AuthenticationManager thisAuthenticationManager = AuthenticationManager.Current;

            // Register the ArcGIS Online server information with the AuthenticationManager
            thisAuthenticationManager.RegisterServer(portalServerInfo);

            // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
            thisAuthenticationManager.ChallengeHandler = new ChallengeHandler(CreateCredentialAsync);
        }        

        // ChallengeHandler function that will be called whenever access to a secured resource is attempted
        public async Task<Credential> CreateCredentialAsync(CredentialRequestInfo info)
        {
            Credential credential = null;

            try
            {
                // User will be challenged for OAuth credentials
                credential = await AuthenticationManager.Current.GenerateCredentialAsync(info.ServiceUri);
            }
            catch (Exception ex)
            {
                // Exception will be reported in calling function
                throw (ex);
            }

            return credential;
        }
    }
}
