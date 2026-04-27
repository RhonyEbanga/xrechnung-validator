using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace validator_app.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;

    public ValidationController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }
[HttpPost("validate")]
public async Task<IActionResult> Validate(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest(new { message = "Keine Datei hochgeladen." });

    var client = _httpFactory.CreateClient("kosit");

    using var stream = file.OpenReadStream();
    var streamContent = new StreamContent(stream);
    streamContent.Headers.ContentType = 
        new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

    var response = await client.PostAsync("/", streamContent);
    var reportXml = await response.Content.ReadAsStringAsync();

    try
    {
        var result = ParseReport(reportXml);
        return Ok(new { reportXml, result });
    }
    catch (Exception ex)
    {
        return Ok(new { 
            reportXml, 
            result = new ValidationResult { 
                IsValid = false, 
                Steps = new List<ValidationStep>() 
            },
            parseError = ex.Message
        });
    }
}
    private ValidationResult ParseReport(string xml)
{
    var doc = System.Xml.Linq.XDocument.Parse(xml);
    XNamespace ns = "http://www.xoev.de/de/validator/varl/1";

    var isValid = doc.Root?.Attribute("valid")?.Value == "true";

    var steps = doc.Descendants(ns + "validationStepResult")
        .Select(s => new ValidationStep {
            Name = s.Attribute("id")?.Value ?? "",
            Valid = s.Attribute("valid")?.Value == "true",
            Errors = s.Descendants(ns + "error").Count(),
            Warnings = s.Descendants(ns + "warning").Count()
        }).ToList();

    var syntaxStep = steps.FirstOrDefault(s => 
        s.Name.Contains("val-xsd") || s.Name.Contains("xml"));
    
    var en16931Step = steps.FirstOrDefault(s => 
        s.Name.Contains("en16931") || s.Name.Contains("EN16931"));
    
    var xrechnungStep = steps.FirstOrDefault(s => 
        s.Name.Contains("xrechnung") || s.Name.Contains("XRechnung"));

    return new ValidationResult { 
        IsValid = isValid, 
        Steps = steps,
        SyntaxStep = syntaxStep,
        En16931Step = en16931Step,
        XrechnungStep = xrechnungStep
    };
}
public record ValidationResult {
    public bool IsValid { get; init; }
    public List<ValidationStep> Steps { get; init; } = new();
    public ValidationStep? SyntaxStep { get; init; }
    public ValidationStep? En16931Step { get; init; }
    public ValidationStep? XrechnungStep { get; init; }
}

public record ValidationStep {
    public string Name { get; init; } = "";
    public bool Valid { get; init; }
    public int Errors { get; init; }
    public int Warnings { get; init; }
}

}