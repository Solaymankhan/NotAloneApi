using NotAlone.Services.Abstract;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace NotAlone.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobServiceClient BlobServiceClient => _blobServiceClient;

        public FileService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> uploadPostFiles(List<IFormFile>? files, string message_post)
        {
            if (files!.Count==0 || files == null) return "File is Empty";

            try
            {
                string image_blob="",video_blob="";
                if (message_post == "Message")
                {
                    image_blob = "messageimages";
                    video_blob = "messagevideos";
                }
                else if(message_post=="Post")
                {
                    image_blob = "postimages";
                    video_blob = "postvideos";
                }
                string imgUrl = "";
                var postImageContainerInstance = BlobServiceClient.GetBlobContainerClient(image_blob);
                var postVideoContainerInstance = BlobServiceClient.GetBlobContainerClient(video_blob);
                var uploadTasks = files.Select(async file =>
                {
                    BlobClient? blobInstance = null;
                    if (IsVideoFile(file))
                    {
                        var uniqueId = Guid.NewGuid().ToString();
                        var timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        var fileExtension = Path.GetExtension(file.FileName);
                        var newFileName = $"{timestamp}_{uniqueId}{fileExtension}";
                        var tempDirectory = Path.GetTempPath();
                        var originalFilePath = Path.Combine(tempDirectory, file.FileName);
                        var newFilePath = Path.Combine(tempDirectory, newFileName);
                        using (var stream = new FileStream(originalFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        blobInstance = postVideoContainerInstance.GetBlobClient(newFileName);
                        using (var stream = File.OpenRead(originalFilePath))
                        {
                            await blobInstance.UploadAsync(stream);
                        }
                        File.Delete(originalFilePath);
                    }
                    else if (IsImageFile(file))
                    {
                        file = await ResizeImage(file, 200); // Resize image to 200kb
                        blobInstance = postImageContainerInstance.GetBlobClient(file.FileName);
                        await blobInstance.UploadAsync(file.OpenReadStream());
                    }
                    return blobInstance.Uri.ToString();

                }).ToArray();
                var uploadedUrls = await Task.WhenAll(uploadTasks);
                imgUrl = string.Join(" ", uploadedUrls);

                return imgUrl.Trim();

            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<string?> uploadSingleFile(IFormFile? singleFile,string blobName)
        {
            if (singleFile == null) return "File is Empty";

            try
            {
                var fileInstance = BlobServiceClient.GetBlobContainerClient(blobName);
                BlobClient? blobInstance = null;
                if (IsImageFile(singleFile))
                {
                    var resizedFile = await ResizeImage(singleFile, 80); // Resize image to 200kb
                    blobInstance = fileInstance.GetBlobClient(resizedFile.FileName);
                    await blobInstance.UploadAsync(resizedFile.OpenReadStream());
                }
                
                return blobInstance.Uri.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private bool IsVideoFile(IFormFile file)
        {
            // Define the allowed file extensions for  videos
            string[] allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv",".3gp" };

            // Get the file extension and convert it to lowercase for case-insensitive comparison
            var extension = Path.GetExtension(file.FileName)?.ToLower();

            // Check if the extension is valid for either images or videos
            return allowedVideoExtensions.Contains(extension);
        }


        private bool IsImageFile(IFormFile file)
        {
            // Define image file extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            // Get the file extension and convert it to lowercase for case-insensitive comparison
            var extension = Path.GetExtension(file.FileName)?.ToLower();

            // Check if the extension is in the list of image extensions
            return imageExtensions.Contains(extension);
        }



        private async Task<IFormFile?> ResizeImage(IFormFile? file, int targetSizeInKb)
        {
            if (file.Length > 10 * 1024 * 1024) return null;
            
                using (var originalStream = file.OpenReadStream())
            {
                using (var image = await Image.LoadAsync(originalStream))
                {
                    var percentage=1.0;
                    if (file.Length <= 10 * 1024 * 1024 && file.Length > 9 * 1024 * 1024)
                        percentage = 0.18;
                    else if (file.Length <= 9 * 1024 * 1024 && file.Length > 8 * 1024 * 1024)
                        percentage = 0.2;
                    else if (file.Length <= 8 * 1024 * 1024 && file.Length > 7 * 1024 * 1024)
                        percentage = 0.205;
                    else if (file.Length <= 7 * 1024 * 1024 && file.Length > 6 * 1024 * 1024)
                        percentage = 0.21;
                    else if (file.Length <= 6 * 1024 * 1024 && file.Length > 5 * 1024 * 1024)
                        percentage = 0.22;
                    else if (file.Length <= 5 * 1024 * 1024 && file.Length > 4 * 1024 * 1024)
                        percentage = 0.24;
                    else if (file.Length <= 4 * 1024 * 1024 && file.Length > 3 * 1024 * 1024)
                        percentage = 0.26;
                    else if (file.Length <= 3 * 1024 * 1024 && file.Length > 2 * 1024 * 1024)
                        percentage = 0.28;
                    else if (file.Length <= 2 * 1024 * 1024 && file.Length > 1 * 1024 * 1024)
                        percentage = 0.3;
                    else if (file.Length <= 1 * 1024 * 1024 && file.Length > 0.6 * 1024 * 1024)
                        percentage = 0.35;
                    else if (file.Length <= 0.6 * 1024 * 1024 && file.Length > 0.3 * 1024 * 1024)
                        percentage = 0.4;

                    var newWidth = (int)(image.Width * percentage);
                    var newHeight = (int)(image.Height * percentage);

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(newWidth, newHeight),
                        Mode = ResizeMode.Max
                    }));

                    var timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    var newFileName = $"{timestamp}.jpeg";

                    var tempFilePath = Path.GetTempFileName();
                    await image.SaveAsync(tempFilePath, new JpegEncoder());

                    return new FormFile(new FileStream(tempFilePath, FileMode.Open), 0, new FileInfo(tempFilePath).Length, file.Name, newFileName)
                    {
                        Headers = file.Headers,
                        ContentType = "image/jpeg"
                    };
                }
            }
        }


        //finishing line



    }
}
