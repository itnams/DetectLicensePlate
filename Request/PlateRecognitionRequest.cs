namespace DetectLicensePlate.Request
{
    public class PlateRecognitionRequest
    {
        public string Regions { get; set; }
        public IFormFile Upload { get; set; }
    }
}
