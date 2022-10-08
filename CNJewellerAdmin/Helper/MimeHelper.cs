using Microsoft.AspNetCore.StaticFiles;

namespace CNJewellerAdmin.Helper
{
    public class MimeHelper
    {
        public static string GetMimeMapping(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}
