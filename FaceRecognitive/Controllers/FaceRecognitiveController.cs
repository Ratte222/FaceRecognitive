using BLL.Helpers;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognitive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitiveController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IFaceRecognitive _faceRecognitive;

        public FaceRecognitiveController(AppSettings appSettings, IFaceRecognitive faceRecognitive)
        {
            _appSettings = appSettings;
            _faceRecognitive = faceRecognitive;
        }

        [HttpGet("WhoIsInEachImage")]
        public IActionResult WhoIsInEachImage(bool? showDistance, double? tolerance)
        {
            
            return Ok(_faceRecognitive.WhoIsInEachImage(showDistance, tolerance));
        }
    }
}
