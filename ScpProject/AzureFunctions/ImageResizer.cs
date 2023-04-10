
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;

namespace AzureFunctions
{
    public static class ImageResizer
    {

        [FunctionName("ResizeImage")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            ImageInformation data = JsonConvert.DeserializeObject<ImageInformation>(requestBody);

            var display = $"ContainerName : {data.ContainerName}  FileName : {data.FileName}  PictureId : {data.PictureId}";
            string profile = data.FileName + "_profile" + Path.GetExtension(data.FileName);
            string thumbnail = data.FileName + "_thumbnail" + Path.GetExtension(data.FileName);
            CloudStorageAccount storageAccount = AzureStorageCommon.CreateStorageAccountFromConnectionString("DefaultEndpointsProtocol=https;AccountName=scproimages;AccountKey=yuekKSu1u1wqpE8HiGSRFl9UkHC+9qBXrd9kw62bicX3cTBJsqK+KzU+kFkvbGWmLrgLlZNVFXK+BAwI0aiWBw==;EndpointSuffix=core.windows.net");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(data.ContainerName);
            var fuck = new MemoryStream();
            await container.GetBlobReference(data.FileName).DownloadToStreamAsync(fuck);
            fuck.Position = 0;

            var mmRepo = new DAL.Repositories.MultimediaRepo(Config.SqlConn);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(profile);
            blockBlob.Properties.ContentType = "image/" + Path.GetExtension(data.FileName);

            IImageEncoder imgEncoder;

            switch (Path.GetExtension(data.FileName).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    imgEncoder = new JpegEncoder();
                    break;
                case ".png":
                    imgEncoder = new PngEncoder();
                    break;
                case ".bmp":
                    imgEncoder = new BmpEncoder();
                    break;
                case ".gif":
                    imgEncoder = new GifEncoder();
                    break;
                default:
                    throw new System.ApplicationException("Bad Encoder");
                    break;
            }



            using (var image = Image.Load(fuck))
            {
                image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.BoxPad,
                            Size = new Size(0, 400)
                        }));
                var memStream = new MemoryStream();
                image.Save(memStream,imgEncoder);
                memStream.Position = 0;
                blockBlob.UploadFromStreamAsync(memStream).ContinueWith(x =>
            {
                mmRepo.UpdateProfile(profile, data.PictureId);
            });
            }
            fuck.Position = 0;

            using (var image = Image.Load(fuck))
            {
                image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.BoxPad,
                            Size = new Size(0, 250)
                        }));
                var memStream = new MemoryStream();
                image.Save(memStream, imgEncoder);
                memStream.Position = 0;
                blockBlob.UploadFromStreamAsync(memStream).ContinueWith(x =>
                {
                    mmRepo.UpdateThumbNailPicture(thumbnail, data.PictureId);
                });
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
    public class ImageInformation
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public int PictureId { get; set; }
    }
    public static class AzureStorageCommon
    {
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string ConnectionString)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(ConnectionString);
            return storageAccount;
        }
    }
}