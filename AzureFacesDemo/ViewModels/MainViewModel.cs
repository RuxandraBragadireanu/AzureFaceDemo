using AzureFacesDemo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;

namespace AzureFacesDemo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        
        private BitmapImage _currentImage { get; set; }       
        private int _currentImageIndex { get; set; }
        private readonly List<AzureFacesDemo.Models.Image> _imagesList;
        private string _imageCounter { get; set; }

        #endregion

        #region Properties
        public BitmapImage CurrentImage
        {
            get => _currentImage;
            set
            {
                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        public int CurrentImageIndex
        {
            get => _currentImageIndex;
            set
            {
                if (value >= _imagesList.Count)
                {
                    _currentImageIndex = 0;
                }
                else if (value < 0)
                {
                    _currentImageIndex = _imagesList.Count - 1;
                }
                else
                {
                    _currentImageIndex = value;
                }

                ImageCounter = (_currentImageIndex + 1).ToString() + "/" + _imagesList.Count.ToString();
                DetectedFaces.Clear();

                CurrentImage = _imagesList[_currentImageIndex].Img;
                DetectedFaces = new ObservableCollection<Face>(_imagesList[_currentImageIndex].FacesDetected);
   
                OnPropertyChanged(nameof(DetectedFaces));
                OnPropertyChanged(nameof(CurrentImageIndex));                
            }
        }

        public string ImageCounter 
        {
            get => _imageCounter;
            set
            {
                _imageCounter = value;
                OnPropertyChanged(nameof(ImageCounter));
            }
        }

        public ObservableCollection<Face> DetectedFaces { get; set; }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            DetectedFaces = new ObservableCollection<Face>();
            //create images list
            var imagesList = new List<AzureFacesDemo.Models.Image>();
            for(int i = 1; i<=6; i++)
            {
                var image = new AzureFacesDemo.Models.Image(App.IMAGE_BASE_URL + "detection" + i.ToString() + ".jpg");
                image.FacesDetectedCompleted += OnImagesDetectedCompleted;
                imagesList.Add(image);
            }

            List<string> extraImages = new List<string>() {
                "https://tylaz.net/wp-content/uploads/2021/03/Heres-how-facial-expressions-can-make-Apple-Face-ID-more.jpeg",
                "https://www.englishlessonviaskype.com/wp-content/uploads/2015/06/Idioms-describing-character-and-personality.jpg",
                "https://s3.amazonaws.com/freestock-prod/450/freestock_65325331.jpg",
                "https://www.japanitalybridge.com/wordpress/wp-content/uploads/2017/02/2017-feb13-harajukugirl-3.jpg",
                "https://mediad.publicbroadcasting.net/p/shared/npr/styles/x_large/nprshared/202105/997996099.jpg",
                "https://img.grouponcdn.com/seocms/3Kt6fX8W4atnx7BSuBFJL835rjK7/hero__jpg-1080x648",
                "https://cdn5.vectorstock.com/i/1000x1000/87/34/satisfied-people-faces-happy-laughing-people-vector-15488734.jpg",                
                "http://st.depositphotos.com/1303589/3196/i/450/depositphotos_31964369-monkey-family.jpg",
                "https://cdn.toxel.ro/img/contents/Fotografii-urangutan-Mali-pui01.jpg",
                "https://10mosttoday.com/wp-content/uploads/2013/09/The_Nightwatch_by_Rembrandt-1024x853.jpg",
                "https://i.ebayimg.com/images/g/GBEAAOSw1llcShv4/s-l400.jpg",
                "https://i.pinimg.com/236x/48/e7/b2/48e7b2cf2ed4531888971472bb0c58de--fun-recipes-vegan-recipes.jpg",

            };

            foreach(var url in extraImages)
            {
                var image = new AzureFacesDemo.Models.Image(url);
                image.FacesDetectedCompleted += OnImagesDetectedCompleted;
                imagesList.Add(image);
            }

            _imagesList = imagesList;
            CurrentImageIndex = 0;

        }
        #endregion

        #region Methods
        private void OnImagesDetectedCompleted(object sender, EventArgs e)
        {
            var image = sender as Models.Image;
            if (sender == null)
            {
                return;
            }

            if(image == _imagesList[CurrentImageIndex])
            {
                CurrentImage = _imagesList[_currentImageIndex].Img;
                DetectedFaces = new ObservableCollection<Face>(_imagesList[_currentImageIndex].FacesDetected);

                OnPropertyChanged(nameof(DetectedFaces));

            }
        }

        #endregion

    }
}
