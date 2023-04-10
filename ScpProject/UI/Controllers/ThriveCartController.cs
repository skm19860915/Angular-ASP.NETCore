namespace Controllers.Controllers
{
    //[RoutePrefix("api/ThriveCart")]
    //public class ThriveCartController : ApiController
    //{
    //    //so to get this stupid shit to work, I had to remove the parameter, then submit it to the webhook integration page, then re-incorperate the paramters and voila it works
    //    [Route("Shred"), HttpGet, HttpHead, HttpOptions, HttpPost]
    //    public HttpResponseMessage ParseThriveCartWebHook([FromBody] ThriveCartWebHookData a)
    //    {
    //        if (a != null)
    //        {

    //           new DAL.Repositories.thiveCartDal(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).log(a.@event + " : " + a.customer_id + " : " + a.thrivecart_secret + " : " + a.organizationId + " : " + DateTime.Now);
    //        }
    //        else
    //        {
    //            new DAL.Repositories.thiveCartDal(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).log("got pingged" + Request.ToString());

    //        }
    //        return new HttpResponseMessage(HttpStatusCode.OK);
    //    }
    //    [Route("Log"), HttpPost]
    //    public void Log(genericLog log)
    //    {
    //        new DAL.Repositories.thiveCartDal(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).log("success url" +log.log);
    //    }
    //}
    //public class genericLog {
    //    public string log { get; set; }
    //}
    //public class ThriveCartWebHookData
    //{
    //    public string @event { get; set; }//have to use this reserved keyword because thrive cart thinks its ok to pass the word event  as a property name
    //    public int customer_id { get; set; }
    //    public string thrivecart_secret { get; set; }
    //    public string organizationId { get; set; }

    // }
}
