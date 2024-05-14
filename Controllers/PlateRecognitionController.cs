using DetectLicensePlate.Data;
using DetectLicensePlate.Entities;
using DetectLicensePlate.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DetectLicensePlate.Controllers
{
    [Route("api/plate-recognition")]
    [ApiController]
    public class PlateRecognitionController : ControllerBase
    {
        private readonly DataContext _context;
        public PlateRecognitionController(DataContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<PlateRecognition>> Post([FromForm] PlateRecognitionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Sử dụng HttpClient để gọi API https://api.platerecognizer.com/v1/plate-reader
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Token 1254320020c337933480bc4fc22da578a072915c");
                var imageData = new byte[256];
                using (var formData = new MultipartFormDataContent())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await request.Upload.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        imageData = fileBytes;
                        formData.Add(new ByteArrayContent(fileBytes), "upload", request.Upload.FileName);
                    }
                    formData.Add(new StringContent(request.Regions), "regions");

                    var response = await httpClient.PostAsync("https://api.platerecognizer.com/v1/plate-reader", formData);
                    var t = request.Upload;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<PlateRecognition>();
                        var plateRecognition = new PlateRecognition
                        {
                            Filename = result.Filename,
                            Image = imageData,
                            Version = result.Version,
                            Timestamp = result.Timestamp 
                        };
                        await _context.PlateRecognition.AddAsync(plateRecognition);
                        await _context.SaveChangesAsync();
                        int recognitionId = plateRecognition.Id;
                        foreach (var item in result.Results)
                        {
                            var plateResults = new PlateResults
                            {
                                PlateRecognitionId = recognitionId,
                                Plate = item.Plate
                            };
                            await _context.PlateResults.AddAsync(plateResults);
                            await _context.SaveChangesAsync();
                            int plateResultsId = plateResults.Id;
                            var plateBox = new PlateBox
                            {
                                PlateResultsId = plateResultsId,
                                Xmin = item.Box?.Xmin,
                                Ymin = item.Box?.Ymin,
                                Xmax = item.Box?.Xmax,
                                Ymax = item.Box?.Ymax
                            };
                            await _context.PlateBox.AddAsync(plateBox);
                            await _context.SaveChangesAsync();
                        }
                        var joinForPlateResults = _context.PlateResults.Join(_context.PlateBox, result => result.Id, box => box.PlateResultsId, (result, box) => new
           PlateResults()
                        {
                            Box = box,
                            Id = result.Id,
                            PlateRecognitionId = result.PlateRecognitionId,
                            Plate = result.Plate
                        });
                        var data = await _context.PlateRecognition
                      .GroupJoin(
                          joinForPlateResults,
                          plateRecognition => plateRecognition.Id,
                          plateResults => plateResults.PlateRecognitionId,
                          (plateRecognition, plateResults) => new PlateRecognition()
                          {
                              Id = plateRecognition.Id,
                              Results = plateResults.ToList(),
                              Filename = plateRecognition.Filename,
                              Image = plateRecognition.Image,
                              Version = plateRecognition.Version,
                              Timestamp = plateRecognition.Timestamp,
                          }).Where(item => item.Id == recognitionId).FirstOrDefaultAsync();
                        return Ok(data);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, response);
                    }
                }
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<PlateRecognition>>> GetAllProduct()
        {
            var joinForPlateResults = _context.PlateResults.Join(_context.PlateBox, result => result.Id, box => box.PlateResultsId, (result, box) => new
            PlateResults() {
                Box = box,
                Id = result.Id,
                PlateRecognitionId = result.PlateRecognitionId,
                Plate = result.Plate
            });
            var result = await _context.PlateRecognition
          .GroupJoin(
              joinForPlateResults,
              plateRecognition => plateRecognition.Id,
              plateResults => plateResults.PlateRecognitionId,
              (plateRecognition, plateResults) => new PlateRecognition()
              {
                  Id = plateRecognition.Id,
                  Results = plateResults.ToList(),
                  Filename = plateRecognition.Filename,
                  Image = plateRecognition.Image,
                  Version = plateRecognition.Version,
                  Timestamp = plateRecognition.Timestamp,
              })
          .OrderBy(item => item.Id).ToListAsync();
            return Ok(result);
        }
    }
}
