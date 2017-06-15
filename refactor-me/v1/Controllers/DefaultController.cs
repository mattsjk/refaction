using System.Collections.Generic;
using System.Web.Http;
using System.Net;

namespace refactor_me.v1.Controllers
{
    /// <summary>
    /// This controller does not use attribute routing and is accessible by
    /// conventional routes only. Conventional and attribute based controllers
    /// can work in the same project and this controller has been included
    /// to illustrate this case.
    /// </summary>
    public class DefaultController : ApiController
    {
        private string[] companies = new string[] { "Microsoft", "Google", "Amazon", "Oracle", "OpenText", "Adobe" };
        // GET: api/Default
        public IEnumerable<string> Get()
        {
            return companies;
        }

        // GET: api/Default/5
        public string Get(int id)
        {
            return companies[id];
        }

        // POST: api/Default
        public IHttpActionResult Post([FromBody]string value)
        {
            return StatusCode(HttpStatusCode.Created);
        }

        // PUT: api/Default/5
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
            return StatusCode(HttpStatusCode.Created);
        }

        // DELETE: api/Default/5
        public IHttpActionResult Delete(int id)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
