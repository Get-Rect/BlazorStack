using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobs;

        public BlobService(BlobServiceClient blobs)
        {
            _blobs = blobs;
        }

        
    }
}
