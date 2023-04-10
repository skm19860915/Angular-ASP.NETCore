using DAL.Repositories;
using System;
using System.Text;
using Models.MultiMedia;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http;
using Newtonsoft.Json;
using AzureFunctions;

namespace BL
{
    public interface IMultiMediaManager
    {
        int CreatePicture(byte[] content, string fileName, Guid createdUserGuid, string azureMediaBaseUrl);
    }

    public class MultiMediaManager : IMultiMediaManager
    {
        private string _azureConnectionEndpoint { get; set; }
        private IMultimediaRepo _multimediaRepo { get; set; }
        private ITagRepo<MultiMediaTag> _multiMediaTagRepo { get; set; }
        private IUserRepo _userRepo { get; set; }

        public MultiMediaManager( string azureConnectionEndpoint, IUserRepo userRepo, ITagRepo<MultiMediaTag> mmtagRepo, IMultimediaRepo mmrepo)
        {

            _azureConnectionEndpoint = azureConnectionEndpoint;
            _userRepo = userRepo;
            _multiMediaTagRepo = mmtagRepo;
            _multimediaRepo = mmrepo;
        }

        public int CreatePicture(byte[] content, string fileName, Guid createdUserGuid, string azureMediaBaseUrl)
        {
            var user = _userRepo.Get(createdUserGuid);
            var url = azureMediaBaseUrl + user.ImageContainerName + @"/";

            var pictureId = _multimediaRepo.CreatePicture(url, fileName, createdUserGuid);
            StoreImage(content, "", fileName, user.ImageContainerName, pictureId);
            return pictureId;
        }


        private void StoreImage(byte[] imageBytes, string contentType, string fileName, string containerName, int pictureId)
        {
            CloudStorageAccount storageAccount = AzureStorageCommon.CreateStorageAccountFromConnectionString(_azureConnectionEndpoint);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);


            var ret = container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext()).Result;
            var fileMask = System.Guid.NewGuid() + System.IO.Path.GetExtension(fileName);
            //prevents multiplefiles with same name from being uploaded and stomping on eachother
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileMask);
            blockBlob.Properties.ContentType = "image/" + System.IO.Path.GetExtension(fileName);
            blockBlob.UploadFromByteArray(imageBytes, 0, imageBytes.Length);

            var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/ResizeImage?code={Config.ResizeImageCode}",
              new StringContent(JsonConvert.SerializeObject(
               new ImageInformation() { ContainerName = containerName, FileName = fileMask, PictureId = pictureId }), Encoding.UTF8, "application/json"));
            //now its asynch should we wait for an answer to say that picture isnt good or what?
        }
    }
}
