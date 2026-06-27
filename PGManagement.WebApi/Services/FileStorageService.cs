namespace PGManagement.WebApi.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        var webRoot = _env.WebRootPath ??
                      Path.Combine(_env.ContentRootPath, "wwwroot");

        var uploadsRoot = Path.Combine(webRoot, "uploads", subFolder);

        // Ensure folders exist
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // ************ DEBUG ************
        var debugFile = Path.Combine(webRoot, "debug-path.txt");

        await File.WriteAllTextAsync(debugFile,
    $"""
ContentRoot : {_env.ContentRootPath}
WebRoot     : {webRoot}
UploadsRoot : {uploadsRoot}
SavedFile   : {filePath}
FileExists  : {File.Exists(filePath)}
""");
        // ********************************

        var request = _httpContextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}/uploads/{subFolder}/{fileName}";
    }
}