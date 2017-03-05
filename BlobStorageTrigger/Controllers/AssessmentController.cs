using App.DataManupulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BlobStorageTrigger.Controllers
{
    public class AssessmentController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public IHttpActionResult Get()
        {
            DataParser oParser = new DataParser();
            var s = oParser.ReadJsonValues("http://v2qaservice.tapestrykpi.com/admin/getreportviews?client=twc&emailAddress=pinakik@softcrylic.co.in").Result;
            oParser.ConvertToType(s);
            return Ok();
        }

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}