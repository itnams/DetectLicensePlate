namespace DetectLicensePlate.Entities
{
    public class PlateRecognition
    {
        public int Id { get; set; }
        public List<PlateResults>? Results { get; set; }
        public string? Filename { get; set; }

        public byte[]? Image { get; set; }
        public int? Version { get; set; }
        public string? Timestamp { get; set; }
    }
    public class PlateResults
    {
        public int Id { get; set; }
        public int PlateRecognitionId { get; set; }
        public PlateBox? Box { get; set; }
        public string? Plate { get; set; }
    }
    public class PlateBox
    {
        public int Id { get; set; }
        public int PlateResultsId { get; set; }
        public int? Xmin { get; set; }
        public int? Ymin { get; set; }
        public int? Xmax { get; set; }
        public int? Ymax { get; set; }

    }
}
