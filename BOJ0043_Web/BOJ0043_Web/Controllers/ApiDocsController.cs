using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net.Http;
using System.Threading.Tasks;

namespace BOJ0043_Web.Controllers
{
    /*
    Tento kontroler slouží k zobrazení uživatelského rozhraní pro 
    procházení API dokumentace na URL /api-docs.
    */
    [Route("api-docs")]
    public class ApiDocsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUrlHelper _urlHelper;

        public ApiDocsController(
            IHttpClientFactory httpClientFactory,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ApiDocUrl = _urlHelper.Action("GetApiDocumentation", "ApiDocumentation", null, Request.Scheme);
            return View();
        }
    }
}
