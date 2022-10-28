using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Attributes
{
    public class MyAuthorizeAttribute : ActionFilterAttribute
    {
        public string TokenTypeName { get; set; }

        public MyAuthorizeAttribute(string tokenTypeName = nameof(TokenType.Access))
        {
            TokenTypeName = tokenTypeName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var response = new Response<string>();
            try
            {
                var user = context.HttpContext.User;

                if (context.Controller is IController controller)
                {
                    Claim? claim = user.FindFirst(ClaimTypes.Actor);
                    controller.PersonId = claim != null ? int.Parse(claim.Value) : default;

                    claim = user.FindFirst(ClaimTypes.Sid);
                    controller.TokenId = claim != null ? new(claim.Value) : default;

                    bool tokenDisabled = controller.DbContext.Tokens
                        .AsNoTracking()
                        .Where(o => o.TokenId == controller.TokenId)
                        .Select(o => o.Disabled)
                        .SingleOrDefault();

                    if (tokenDisabled)
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Result = new OkObjectResult(response.GenerateError(ex));
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
