using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows;

namespace AzureFacesDemo.Models
{
    public class Image
    {
        public BitmapImage Img { get; set; }
        public string ImageUrl { get; set; }
        public List<Face> FacesDetected { get; set; }

        public EventHandler FacesDetectedCompleted; 

        public Image(string imageUrl)
        {
            ImageUrl = imageUrl;
            FacesDetected = new List<Face>();

            WebRequest request = WebRequest.Create(imageUrl);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Img = new BitmapImage();
            Img.BeginInit();
            Img.StreamSource = responseStream;
            Img.CacheOption = BitmapCacheOption.Default;
            Img.EndInit();

            Img.DownloadCompleted += Img_DownloadCompleted;
        }

        private void Img_DownloadCompleted(object sender, EventArgs e)
        {
            DetectFaces();
            int i = 1;
            foreach(var face in FacesDetected)
            {
                var bitmap = BitmapImage2Bitmap(Img);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    var mediaColor = face.Color.Color;
                    var drawingcolor = System.Drawing.Color.FromArgb(
                            mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
                    var pen = new System.Drawing.Pen(drawingcolor);
                    pen.Width = 3;
                    g.DrawRectangle(pen, face.faceRectangle);

                    Font drawFont = new Font("Arial", 15, System.Drawing.FontStyle.Bold);
                    SolidBrush drawBrush = new SolidBrush(drawingcolor);
                    var x = face.faceRectangle.X - 3;
                    var y = face.faceRectangle.Y - 23;
                    g.DrawString(i.ToString(), drawFont, drawBrush, x, y);

                }

                Img = Bitmap2BitmapImage(bitmap);
                i++;
            }

            FacesDetectedCompleted?.Invoke(this, new EventArgs());
        }

        private void DetectFaces()
        {
            IList<DetectedFace> detectedFaces;

            // Detect faces with all attributes from image url.

            detectedFaces = App.FaceClient.Face.DetectWithUrlAsync(ImageUrl,
             returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
             // We specify detection model 1 because we are retrieving attributes.
             detectionModel: DetectionModel.Detection01,
             recognitionModel: App.RECOGNITION_MODEL4).GetAwaiter().GetResult();

            int index = 1;
            foreach (var face in detectedFaces)
            {
                var newFace = new Face();
                newFace.Identifier = "Face " + index;

                var colorKey = "Color" + (index % 5 + 1).ToString();
                newFace.Color = new SolidColorBrush((System.Windows.Media.Color)App.Current.FindResource(colorKey));

                index++;

                // Get bounding box of the faces
                newFace.Description += $"Rectangle : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}\n";

                newFace.faceRectangle = new Rectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);

                // Get accessories of the faces
                List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                int count = face.FaceAttributes.Accessories.Count;
                string accessory; string[] accessoryArray = new string[count];
                if (count == 0) { accessory = "NoAccessories"; }
                else
                {
                    for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                    accessory = string.Join(",", accessoryArray);
                }
                newFace.Description += $"Accessories : {accessory}\n";

                // Get face other attributes
                newFace.Description += $"Age : {face.FaceAttributes.Age}\n";
                newFace.Description += $"Blur : {face.FaceAttributes.Blur.BlurLevel}\n";

                // Get emotion on the face
                string emotionType = string.Empty;
                double emotionValue = 0.0;
                Emotion emotion = face.FaceAttributes.Emotion;
                if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                newFace.Description += $"Emotion : {emotionType}\n";

                // Get more face attributes
                newFace.Description += $"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}\n";
                newFace.Description += $"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}\n";
                newFace.Description += $"Gender : {face.FaceAttributes.Gender}\n";
                newFace.Description += $"Glasses : {face.FaceAttributes.Glasses}\n";

                // Get hair color
                Hair hair = face.FaceAttributes.Hair;
                string color = null;
                if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                HairColorType returnColor = HairColorType.Unknown;
                double maxConfidence = 0.0f;
                foreach (HairColor hairColor in hair.HairColor)
                {
                    if (hairColor.Confidence <= maxConfidence) { continue; }
                    maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                }
                newFace.Description += $"Hair : {color}\n";

                // Get more attributes
                newFace.Description += $"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}\n";
                newFace.Description += $"Noise : {face.FaceAttributes.Noise.NoiseLevel}\n";
                newFace.Description += $"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")}\n" +
                    $"{string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}\n{string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}\n";
                newFace.Description += $"Smile : {face.FaceAttributes.Smile}\n";

                FacesDetected.Add(newFace);
            }

            
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;
                
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

    }
}
