using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages
{
    public class IngresarModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IngresarModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TokenRequest Input { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public ActionResult OnGet()
        {
            Message = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                return User.TokenIsReset() ? RedirectToPage(Constants.PageReiniciar) : RedirectToPage(Constants.PageIndex);
            }
            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("Users/RequestToken", Input);

                if (!response.IsSuccessStatusCode)
                {
                    Message = ResponseMessage.AnErrorHasOccurred;
                    return Page();
                }

                var content = await response.Content.ReadFromJsonAsync<Response<TokenResponse<User>>>();

                if (content.Code != ResponseCode.Ok && content.Code != ResponseCode.Conflict)
                {
                    Message = content.Message;
                    return Page();
                }

                var claims = new List<Claim>()
                {
                    new(ClaimTypes.Actor, content.Data.Data.PersonId.ToString()),
                    new(ClaimTypes.Authentication, content.Data.AccessToken),
                    new(ClaimTypes.Version, content.Data.TokenType.ToString())
                };
                foreach (var rol in content.Data.Data.Rols)
                {
                    claims.Add(new(ClaimTypes.Role, rol.Name));
                }

                var appIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties()
                {
                    ExpiresUtc = content.Data.ExpiresIn
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(appIdentity), authProperties);

                if (content.Code == ResponseCode.Conflict)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }
                return RedirectToPage(Constants.PageIndex);
            }
            catch (Exception)
            {
                Message = ResponseMessage.AnErrorHasOccurred;
                return Page();
            }
        }
    }
}
