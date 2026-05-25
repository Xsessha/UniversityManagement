namespace UniversityManagement.Web.Helpers;

public static class FileUploadHelper
{
    public static async Task<string> SaveFile(IFormFile file, string path)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var fullPath = Path.Combine(path, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }
}