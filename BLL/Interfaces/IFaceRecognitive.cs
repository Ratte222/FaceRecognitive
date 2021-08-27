using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IFaceRecognitive
    {
        List<FaceRecognitiveOutDTO> WhoIsInEachImage(bool? showDistance, double? tolerance);
    }
}
