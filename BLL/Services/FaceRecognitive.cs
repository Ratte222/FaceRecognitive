using BLL.DTO;
using BLL.Helpers;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BLL.Services
{
    public class FaceRecognitive: IFaceRecognitive
    {
        private readonly AppSettings _appSettings;
        public FaceRecognitive(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public List<FaceRecognitiveOutDTO> WhoIsInEachImage(bool? showDistance, double? tolerance)
        {
            string pathUnknownImage = Path.Combine(Directory.GetCurrentDirectory(), "FaceRecognitive", "unknown_face");
            string pathKnownImage = Path.Combine(Directory.GetCurrentDirectory(), "FaceRecognitive", "known_face");
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.FileName = _appSettings.PathTo_face_recognition;
                string sd = null, tol = null;
                if(showDistance.HasValue)
                {
                    sd = $"--show-distance {showDistance.Value.ToString().ToLower()} ";
                }
                if(tolerance.HasValue)
                {
                    tol = $"--tolerance {tolerance.Value.ToString(CultureInfo.InvariantCulture)} ";
                }
                myProcess.StartInfo.Arguments = $"{sd}{tol}{pathKnownImage} {pathUnknownImage}";
                //myProcess.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                myProcess.WaitForExit();
                return ParseOut_face_recognition(myProcess.StandardOutput.ReadToEnd());
            }           
        }

        private List<FaceRecognitiveOutDTO> ParseOut_face_recognition(string content)
        {
            string[] contentSplit = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<FaceRecognitiveOutDTO> result = new List<FaceRecognitiveOutDTO>();
            foreach(string value in contentSplit)
            {
                string[] valueSplit = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                valueSplit[0] = Path.GetFileName(valueSplit[0]);
                valueSplit[1] = valueSplit[1].TrimEnd();
                
                FaceRecognitiveOutDTO faceRecognitiveOutDTO = result.FirstOrDefault(i => i.ImageName.ToLower() == valueSplit[0].ToLower());
                bool imageExist = false;
                if (faceRecognitiveOutDTO != null)
                {
                    imageExist = true;
                }
                else
                {
                    faceRecognitiveOutDTO = new FaceRecognitiveOutDTO();
                    faceRecognitiveOutDTO.ImageName = valueSplit[0];
                    faceRecognitiveOutDTO.FacesOnImage = new List<string>();
                    faceRecognitiveOutDTO.Distence = new List<double>();
                }                
                faceRecognitiveOutDTO.FacesOnImage.Add(valueSplit[1]);
                if (valueSplit.Length == 3)
                {
                    double temp;
                    NumberStyles numberStyles = NumberStyles.AllowDecimalPoint;

                    valueSplit[2] = valueSplit[2].TrimEnd();
                        //.Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                    if (Double.TryParse(valueSplit[2], numberStyles, CultureInfo.InvariantCulture, out temp))
                    {
                        faceRecognitiveOutDTO.Distence.Add(temp);
                    }
                    else
                    {
                        faceRecognitiveOutDTO.Distence.Add(-1.0);
                    }
                }
                if(!imageExist)
                {
                    result.Add(faceRecognitiveOutDTO);
                }
                

            }
            return result;
        }
    }
}
