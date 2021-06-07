using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using AzureFacesDemo.ViewModels;
using AzureFacesDemo.Views;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
namespace AzureFacesDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // From your Face subscription in the Azure portal, get your subscription key and endpoint.
        public const string SUBSCRIPTION_KEY = "ecd4551112154d2c88c0a9625913b69c";
        public const string ENDPOINT = "https://faceidentificationresource.cognitiveservices.azure.com/";

        // Used for all examples.
        // URL for the images.
        public const string IMAGE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";

        // Recognition model 4 was released in 2021 February.
        // It is recommended since its accuracy is improved
        // on faces wearing masks compared with model 3,
        // and its overall accuracy is improved compared
        // with models 1 and 2.
        public const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;

        public static IFaceClient FaceClient;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Authenticate.
            FaceClient = new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };

            //Open MainView
            MainView mainView = new MainView();
            MainViewModel mainViewModel = new MainViewModel();
            mainView.DataContext = mainViewModel;
            mainView.Show();
            

        }


    }
}
