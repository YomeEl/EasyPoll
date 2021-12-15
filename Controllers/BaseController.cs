using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using EasyPoll.Models;
using EasyPoll.Data;

namespace EasyPoll.Controllers
{
    public class BaseController : Controller
    {
        protected ServiceDBContext dbcontext;
        protected UserModel user;

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            dbcontext = ServiceDBContext.GetDBContext();
            user = AuthentificationController.GetUserByToken(HttpContext.Request.Cookies["token"]);
        }
    }
}