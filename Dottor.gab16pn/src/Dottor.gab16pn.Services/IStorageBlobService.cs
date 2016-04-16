using System;
using System.Collections.Generic;
namespace Dottor.gab16pn.Services
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Blob;

    public interface IStorageBlobService
    {
        /// <summary>
        /// Restituisce tutti i file contenuti in una cartella all'interno di un preciso container di Azure
        /// </summary>
        /// <param name="containerName">Nome del container</param>
        /// <param name="folderName">Nome di una cartella</param>
        Task<IEnumerable<IListBlobItem>> GetAllFilesAsync(string containerName, string folderName);

        /// <summary>
        /// Caricamento di un file nel blob storage di Azure
        /// </summary>
        /// <param name="containerName">Nome del container</param>
        /// <param name="name">Nome del file</param>
        /// <param name="contentType">Content-type del file</param>
        /// <param name="content">Contenuto del file</param>
        Task UploadFromStreamAsync(string containerName, string name, string contentType, byte[] content);
    }
}
