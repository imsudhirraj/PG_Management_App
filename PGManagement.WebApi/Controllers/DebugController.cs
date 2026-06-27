using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public DebugController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var webRoot = _env.WebRootPath;

        return Ok(new
        {
            ContentRoot = _env.ContentRootPath,
            WebRoot = webRoot,
            WebRootExists = Directory.Exists(webRoot),
            UploadFolderExists = Directory.Exists(Path.Combine(webRoot, "uploads")),
            UploadKycExists = Directory.Exists(Path.Combine(webRoot, "uploads", "kyc")),
            Files = Directory.Exists(webRoot)
                ? Directory.GetFiles(webRoot, "*", SearchOption.AllDirectories)
                : Array.Empty<string>()
        });
    }
    [HttpGet("file")]
    public IActionResult File()
    {
        var path = Path.Combine(
            _env.WebRootPath,
            "uploads",
            "kyc",
            "669945bc-6a3f-4f90-afc6-4f695e5f5569.pdf");

        if (!System.IO.File.Exists(path))
            return NotFound(path);

        return PhysicalFile(path, "application/pdf");
    }
}
