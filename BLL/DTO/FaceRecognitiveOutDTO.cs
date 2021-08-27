using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class FaceRecognitiveOutDTO
    {
        public string ImageName { get; set; }
        public ICollection<string> FacesOnImage { get; set; }
        public ICollection<double> Distence { get; set; }
    }
}
