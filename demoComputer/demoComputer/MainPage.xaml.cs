using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace demoComputer
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            imgBanner.Source = ImageSource.FromResource("XamarinComputerVision.images.banner.png");
            imgChoosed.Source = ImageSource.FromResource("XamarinComputerVision.images.thumbnail.jpg");
        }
        private async void btnPick_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                imgChoosed.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                var result = await GetImageDescription(file.GetStream());
                lblResult.Text = null;
                file.Dispose();
                if (result.Description.Captions[0].Confidence > 0.8)
                    lblResult.Text = result.Description.Captions[0].Text;
                foreach (string tag in result.Description.Tags)
                {
                    lblResult.Text = lblResult.Text + "\n" + tag;
                }
            }
            catch
            (Exception ex)
            {
                string test = ex.Message;
            }
        }
        public async Task<AnalysisResult> GetImageDescription(Stream imageStream)
        {
            VisionServiceClient visionClient = new VisionServiceClient("12e698784ae8424c9ce25a5e2a69124a", "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0");
            VisualFeature[] features = { VisualFeature.Tags, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description };
            return await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), null);
        }
    
    }
}