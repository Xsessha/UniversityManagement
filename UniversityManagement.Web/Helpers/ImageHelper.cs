namespace UniversityManagement.Web.Helpers;

public static class ImageHelper
{
    public static string GetImagePath(string fileName)
    {
        return $"/uploads/{fileName}";
    }
}