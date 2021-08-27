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

        //always return one face location in one image
        public List<FaceDetecrionDTO> GetFaceLocation()
        {
            string pathKnownImage = Path.Combine(Directory.GetCurrentDirectory(), "FaceRecognitive", "unknown_face");
            return ParseFaceDetection(ProcessingImage(_appSettings.PathTo_face_detection, pathKnownImage));
        }

        public List<FaceDetecrionDTO> ParseFaceDetection(string content)
        {
            string[] contentSplit = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<FaceDetecrionDTO> result = new List<FaceDetecrionDTO>();
            foreach (string value in contentSplit)
            {
                string[] valueSplit = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                valueSplit[0] = Path.GetFileName(valueSplit[0]);
                valueSplit[4] = valueSplit[4].TrimEnd();
                FaceDetecrionDTO faceDetecrionDTO = result.FirstOrDefault(i => i.ImageName.ToLower() == valueSplit[0].ToLower());
                bool imageExist = false;
                if (faceDetecrionDTO != null)
                {
                    imageExist = true;
                }
                else
                {
                    faceDetecrionDTO = new FaceDetecrionDTO()
                    {
                            ImageName = valueSplit[0]
                    };
                    faceDetecrionDTO.Top = new List<int>();
                    faceDetecrionDTO.Right = new List<int>();
                    faceDetecrionDTO.Bottom = new List<int>();
                    faceDetecrionDTO.Left = new List<int>();
                }
                faceDetecrionDTO.Top.Add(Convert.ToInt32(valueSplit[1]));
                faceDetecrionDTO.Right.Add(Convert.ToInt32(valueSplit[2]));
                faceDetecrionDTO.Bottom.Add(Convert.ToInt32(valueSplit[3]));
                faceDetecrionDTO.Left.Add(Convert.ToInt32(valueSplit[4]));
                if (!imageExist)
                { 
                    result.Add(faceDetecrionDTO);
                }
            }
            return result;
        }

        public List<FaceRecognitiveOutDTO> WhoIsInEachImage(bool? showDistance, double? tolerance)
        {
            string pathUnknownImage = Path.Combine(Directory.GetCurrentDirectory(), "FaceRecognitive", "unknown_face");
            string pathKnownImage = Path.Combine(Directory.GetCurrentDirectory(), "FaceRecognitive", "known_face");
            string sd = null, tol = null;
            if (showDistance.HasValue)
            {
                sd = $"--show-distance {showDistance.Value.ToString().ToLower()} ";
            }
            if (tolerance.HasValue)
            {
                tol = $"--tolerance {tolerance.Value.ToString(CultureInfo.InvariantCulture)} ";
            }
            return ParseOut_face_recognition(ProcessingImage(
                _appSettings.PathTo_face_recognition,$"{sd}{tol}{pathKnownImage} {pathUnknownImage}"));
        }

        private string ProcessingImage(string fileName, string argument)
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.FileName = fileName;                
                myProcess.StartInfo.Arguments = argument;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.Start();
                myProcess.WaitForExit();
                return myProcess.StandardOutput.ReadToEnd();
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
