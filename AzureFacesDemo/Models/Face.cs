using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;

namespace AzureFacesDemo.Models
{
    public class Face
    {
        public string Identifier { get; set; }
        public string Description { get; set; }
        public SolidColorBrush Color { get; set; }
        public Rectangle faceRectangle { get; set; }
    }
}
