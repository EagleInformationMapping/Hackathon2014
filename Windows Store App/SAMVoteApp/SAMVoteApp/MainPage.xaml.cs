using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Telerik.UI.Xaml.Controls.Grid;

using Windows.UI.Xaml.Media.Imaging;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Query; 
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SAMVoteApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _gfTimer;
        private List<RadDataGrid> _gfControls;
        private int _rotateRefresh; 
        private DateTime _lastRefresh;
        private Envelope _initialExt;
        private ArcGISDynamicMapServiceLayer _lyrWeather;
        private ArcGISDynamicMapServiceLayer _lyrTraffic;
        private Dictionary<string, Button> _mapButtons;
        private Dictionary<string, MapView> _maps;
        private TextBlock _idAddress;
        private TextBlock _idName;
        private TextBlock _idWait;
        private IList<VotingLocation> _VLWait;
        private DispatcherTimer _waitTimer;
        private RadCartesianChart _waitChart;
        private RadDataGrid _txtFeed;
        private DispatcherTimer _txtTimer;

        public MainPage()
        {
            this.InitializeComponent();

            //Testign service
            //https://api.geofeedia.com/v1/search/geofeed/31860?appId=ecd5c597&appKey=cf05e519bc603d0a72874855eb32fa42

            _gfTimer = new DispatcherTimer(); 

            _gfTimer.Interval = new TimeSpan(0, 0, 0,5,0); // 100 Milliseconds 
            _gfTimer.Tick += _gfTimer_Tick;
            _gfControls = new List<RadDataGrid>();

            _initialExt = new Envelope(-12979006.5520317, 3988064.57659092, -12948079.2642393, 4015899.13560402, new SpatialReference(102100));
            _mapButtons = new Dictionary<string, Button>();
            _maps = new Dictionary<string, MapView>();
            _VLWait = new List<VotingLocation>();

            _waitTimer = new DispatcherTimer();
            _waitTimer.Interval = new TimeSpan(0, 0, 0, 20, 0);
            _waitTimer.Tick += updateWait_Tick;

            _txtTimer = new DispatcherTimer();
            _txtTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            _txtTimer.Tick += _txtTimer_Tick;

        }

        async void _txtTimer_Tick(object sender, object e)
        {
            Uri url = new Uri("http://hackathon2014.azurewebsites.net/jamie");
            HttpClient httpClient = new HttpClient();

            IList<Twilio> messages = new List<Twilio>();
            try
            {
                var response = await httpClient.GetAsync(url);
                //data.Add(new Geofeedia() { CreatedOn = System.DateTime.Now, Title = "Testing", Source = new BitmapImage(new Uri("ms-appx:///Assets/geofeedia/instagram.png", UriKind.Absolute)), Url = "Test" });

                var content = response.Content.ReadAsStringAsync();
                var contentresult = await content;

                var result = JsonConvert.DeserializeObject(contentresult) as JArray;
                messages.Add(new Twilio() { Message = "LAST UPDATED: " + DateTime.Now.ToString() });

                if (result != null)
                {
                    foreach (var item in result)
                        {
                            try
                            {
                                messages.Add(new Twilio() { Message = (string)item["body"] });
                            }
                            catch (Exception ex2)
                            {
                            }
                        }
                    }

                }

            catch (Exception ex)
            {
            }
            if (messages.Count > 1)
            {
                _txtFeed.ItemsSource = messages; 
            }

        }

        async void updateWait_Tick(object sender, object e)
        {
            UpdateChart();
        }

        async void UpdateChart()
        {

            var uri = new Uri("http://ec2-54-193-191-197.us-west-1.compute.amazonaws.com/arcgis/rest/services/Voting_Locations/FeatureServer/0");
            var queryTask = new QueryTask(uri);
            MapView mv = _maps["m0"];


            try
            {
                var queryParams = new Esri.ArcGISRuntime.Tasks.Query.Query(mv.Extent, SpatialRelationship.Intersects);
                queryParams.ReturnGeometry = true;
                //--output spatial reference
                queryParams.OutSpatialReference = mv.SpatialReference;
                IList<string> fields = new List<string> { "Address", "Location_Name", "Wait_Time" };
                queryParams.OutFields = new OutFields(fields);
                // Execute the task and await the result
                QueryResult queryResult = await queryTask.ExecuteAsync(queryParams);

                // Get the list of features (graphics) from the result
                var results = queryResult.FeatureSet.Features;

                var resultsList = new List<VotingLocation>();
                foreach (var item in results)
                {
                    resultsList.Add(new VotingLocation() { Address = (string)item.Attributes["Address"], Name = (string)item.Attributes["Location_Name"], WaitTime = (int)item.Attributes["Wait_Time"], Location = item.Geometry });
                }
                if (resultsList.Count > 0)
                {
                    _VLWait = resultsList;
                    //create graph
                    _waitChart.DataContext = _VLWait;

                    int desiredBarWidth = 32;

                    _waitChart.Height = results.Count * desiredBarWidth;  
                }

            }
            catch (Exception ex)
            {
            }
        }
        async void _gfTimer_Tick(object sender, object e)
        {
            int iStart = 0;
            foreach (var gd in _gfControls)
            {
                    string id = gd.Name.Substring(1);
                    Uri url = new Uri("https://api.geofeedia.com/v1/search/geofeed/" + id + "?appId=ecd5c597&appKey=cf05e519bc603d0a72874855eb32fa42");
                    HttpClient httpClient = new HttpClient();


                    IList<Geofeedia> data = new List<Geofeedia>(); // gd.ItemsSource as IList<Geofeedia>;
                    //bool bClearData = false; 
                    //if (data == null)
                    //{
                    //   data =  new List<Geofeedia>();

                    //}
                    //else
                    //{
                    //    data.Clear();
                    //}

                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        //data.Add(new Geofeedia() { CreatedOn = System.DateTime.Now, Title = "Testing", Source = new BitmapImage(new Uri("ms-appx:///Assets/geofeedia/instagram.png", UriKind.Absolute)), Url = "Test" });

                        var content = response.Content.ReadAsStringAsync();
                        var contentresult = await content;

                        var result = JsonConvert.DeserializeObject(contentresult) as JObject;
                        data.Add(new Geofeedia() { CreatedOn = System.DateTime.Now, Title = "LAST UPDATED: " + DateTime.Now.ToString(), Source = new BitmapImage(new Uri("ms-appx:///Assets/geofeedia/black.png", UriKind.Absolute)), Url = "" });

                        if (result != null)
                        {
                            JArray ar = result["items"] as JArray;

                            if (ar != null)
                            {
                                int imax = 0;
                                foreach (var item in ar)
                                {
                                    try
                                    {
                                        string title = (string)item["title"];

                                        data.Add(new Geofeedia() { CreatedOn = DateTime.Parse((string)item["createdOn"]), Title = title , Source = new BitmapImage(new Uri("ms-appx:///Assets/geofeedia/" + (string)item["source"] + ".png", UriKind.Absolute)), Url = (string)item["url"] });
                                    }
                                    catch (Exception ex2)
                                    {
                                    }
                                }
                            }

                        }

                        //                response.EnsureSuccessStatusCode();

                        // object jOutput = JsonConvert.DeserializeObject(output);

                    }
                    catch (Exception ex)
                    {
                        // Need to convert int HResult to hex string
                        Debug.WriteLine("Error on " + gd.Name + " MESSAGE:" + ex.Message); 
                    }
                    if (data.Count > 1)
                    {
                        gd.ItemsSource = data;
                    }
            }
        }


        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        private void wb_Loaded(object sender, RoutedEventArgs e)
        {
            WebView wv = (WebView)sender; 
            wv.Navigate(new Uri("http://www.yahoo.com")); 
        }

        private void chart_Loaded(object sender, RoutedEventArgs e)
        {
            _waitChart = (RadCartesianChart)sender;
            _waitChart.Series[0].AllowSelect = true; 

            //ChartSelectionBehavior sb = (ChartSelectionBehavior)_waitChart.Behaviors.First();
            //sb.SelectionChanged += sb_SelectionChanged;

        }

        void sb_SelectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Selected");
        }

        private void btn_sync_loaded(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Click += b_SyncClick;
//            _mapButtons.Add(b.Name, b);
        }
        private void btn_extent_loaded(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Click += b_ExtentClick;
        }
        private void btn_beam_loaded(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Click += b_BeamClick;
        }

        void b_BeamClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string sourceName = b.Name;
            int source_id = int.Parse(sourceName.Substring(1, 1));
            MapView mv = _maps["m" + source_id];
            MapView mvMain = _maps["m0"];
            switch (source_id)
            {
                case 3:
                    mvMain.Map.Layers.Add(_lyrWeather); 
                    break;
                case 4:
                    mvMain.Map.Layers.Add(_lyrTraffic); 
                    break;
                default:
                    break;
            }
            
        }
        void b_ExtentClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender; 
            string sourceName = b.Name;
            int source_id = int.Parse(sourceName.Substring(1, 1));
            MapView mv = _maps["m" + source_id];
            mv.SetView(_initialExt);
        }


        void b_SyncClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            string sourceName = b.Name;
            int source_id = int.Parse(sourceName.Substring(1, 1));
            MapView mv = _maps["m" + source_id];
            Envelope extent = mv.Extent;

            foreach (KeyValuePair<string, MapView> item in _maps)
            {
                string map_id = "m" + source_id;
                if (item.Key != map_id)
                {
                    item.Value.SetView(extent);
                }
            }
        }
        async private void m4m_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MAP LOADED");
            Uri url = new Uri("https://www.arcgis.com/sharing/oauth2/token?client_id=DGYQP0wGM80uGWuP&client_secret=d6714cd01d754e01abb4d854e597146d&grant_type=client_credentials");
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync(url);
            //data.Add(new Geofeedia() { CreatedOn = System.DateTime.Now, Title = "Testing", Source = new BitmapImage(new Uri("ms-appx:///Assets/geofeedia/instagram.png", UriKind.Absolute)), Url = "Test" });

            var content = response.Content.ReadAsStringAsync();
            var contentresult = await content;

            var result = JsonConvert.DeserializeObject(contentresult) as JObject;
            string tk = (string)result["access_token"];
            Debug.WriteLine("TOKEN:" + tk);
            MapView mv = (MapView)sender;
            _lyrTraffic = new ArcGISDynamicMapServiceLayer(new Uri("http://traffic.arcgis.com/arcgis/rest/services/World/Traffic/MapServer"));
            _lyrTraffic.Token = tk;
            _lyrTraffic.ID = "BASE1";
            mv.Map.Layers.Add(_lyrTraffic);
            _maps.Add(mv.Name, mv); 
            
        }
        
        private void m3m_Loaded(object sender, RoutedEventArgs e)
        {
            _lyrWeather = new ArcGISDynamicMapServiceLayer(new Uri("http://gis.srh.noaa.gov/arcgis/rest/services/RIDGERadar/MapServer"));
            _lyrWeather.ID = "BASE1";

            MapView mv = (MapView)sender;
            mv.Map.Layers.Add(_lyrWeather);
            _maps.Add(mv.Name, mv);
        }
        private void m1m_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mv = (MapView)sender;
            _maps.Add(mv.Name, mv);
        }
        private void m2m_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mv = (MapView)sender;
            _maps.Add(mv.Name, mv);
        }
        private void m0m_Loaded(object sender, RoutedEventArgs e)
        {
            MapView mv = (MapView)sender;
            mv.Tapped += mv_Tapped;
            _maps.Add(mv.Name, mv);
            _waitTimer.Start();
            mv.ExtentChanged += mv_ExtentChanged;
        }

        private void btn_m0_loaded(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Click += b_Click;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            //Remove Layers
            MapView mv = _maps["m0"];
            mv.Map.Layers.Remove(_lyrWeather);
            mv.Map.Layers.Remove(_lyrTraffic);
        }

        void mv_ExtentChanged(object sender, EventArgs e)
        {
            MapView mv = (MapView)sender;
            Debug.WriteLine(mv.Extent.XMin + "," + mv.Extent.YMin + "," + mv.Extent.XMax + "," + mv.Extent.YMax);
            UpdateChart();
            
        }

        async void mv_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MapView mv = (MapView)sender;
            var screenPoint = e.GetPosition(mv);

            // Convert the screen point to a point in map coordinates
            var mapPoint = mv.ScreenToLocation(screenPoint);
            var uri = new Uri("http://ec2-54-193-191-197.us-west-1.compute.amazonaws.com/arcgis/rest/services/Voting_Locations/MapServer");
            var identifyTask = new IdentifyTask(uri);

            // Create variables to store identify parameter information   
            var extent = mv.Extent;            //--current map extent (Envelope)   
            var tolerance = 10;                    //--tolerance, in pixels, for finding features   
            var height = (int)mv.ActualHeight; //--current height, in pixels, of the map control   
            var width = (int)mv.ActualWidth;   //--current width, in pixels, of the map control

            // Create a new IdentifyParameter; pass the variables above to the constructor
            var identifyParams = new IdentifyParameter(mapPoint, extent, tolerance, height, width);

            // Identify only the top most visible layer in the service
            identifyParams.LayerOption = LayerOption.Top;

            // Set the spatial reference to match with the map's
            identifyParams.SpatialReference = mv.SpatialReference;

            // Execute the task and await the result
            IdentifyResult idResult = await identifyTask.ExecuteAsync(identifyParams);

            // See if a result was returned 
            if (idResult != null && idResult.Results.Count > 0)
            {
                // Get the feature for the first result
                var topLayerFeature = idResult.Results[0].Feature;

                // Display the feature as a graphic on the map
                //var resultLayer = MyMap.Layers["IdResultGraphics"] as GraphicsLayer;
                //resultLayer.Graphics.Clear();
                //resultLayer.Graphics.Add(topLayerFeature);
                Debug.WriteLine("MATCH:" + topLayerFeature.Attributes["Address"] + "|" + topLayerFeature.Attributes["Location_Name"]);
                _idAddress.Text = (string)topLayerFeature.Attributes["Address"];
                _idName.Text = (string)topLayerFeature.Attributes["Location_Name"];
                _idWait.Text = (string)topLayerFeature.Attributes["Wait_Time"] + " mins.";
                // Display the attributes in a data grid on the page
                //   this.ResultsDataGrid.ItemsSource = topLayerFeature.Attributes;

                var graphicsLayer = mv.Map.Layers["GL"] as Esri.ArcGISRuntime.Layers.GraphicsLayer;
                graphicsLayer.Graphics.Clear(); 

                var markerSym = new Esri.ArcGISRuntime.Symbology.PictureMarkerSymbol();
                await markerSym.SetSourceAsync(new Uri("ms-appx:///Assets/pushpin30.png", UriKind.Absolute));
                markerSym.Width = 30;
                markerSym.Height = 30;
                markerSym.YOffset = 15;
                var pointGraphic = new Esri.ArcGISRuntime.Layers.Graphic();
                pointGraphic.Geometry = topLayerFeature.Geometry;
                pointGraphic.Symbol = markerSym;
                graphicsLayer.Graphics.Add(pointGraphic);


            }


        }

        private void GF1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_gfTimer.IsEnabled)
            {
                _gfTimer.Start();
            }
            var g = (RadDataGrid)sender;
            _gfControls.Add(g);
        }

        private void idName_Loaded(object sender, RoutedEventArgs e)
        {
            _idName = (TextBlock)sender;
        }

        private void idAddress_Loaded(object sender, RoutedEventArgs e)
        {
            _idAddress = (TextBlock)sender;

        }

        private void idWait_Loaded(object sender, RoutedEventArgs e)
        {
            _idWait = (TextBlock)sender;

        }

        private void Txt_Loaded(object sender, RoutedEventArgs e)
        {
            _txtFeed = (RadDataGrid)sender;
            _txtTimer.Start();
        }
    }
}
