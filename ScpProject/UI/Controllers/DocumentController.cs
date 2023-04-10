using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Controllers.ViewModels.Documents;

namespace Controllers
{
    [RoutePrefix("api/Document")]
    public class DocumentController : ApiController
    {

        [Route("CreateDocument")]
        public void CreateDocument(DocumentDTO targetDocument)
        {
            var x = string.Empty;
        }
    }
}
