using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.MiCuenta.Clave
{
    [Authorize]
    public class ReiniciarModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ReiniciarModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public PasswordRequest Input { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public ActionResult OnGet()
        {
            Message = string.Empty;
            if (!User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageIndex);
            }
            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageIndex);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.PostAsJsonAsync("Users/ResetPassword", Input);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Message = ResponseMessage.TimeLimitExceeded;
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    Message = ResponseMessage.AnErrorHasOccurred;
                    return Page();
                }

                var content = await response.Content.ReadFromJsonAsync<Response<TokenResponse<User>>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (content.Code != ResponseCode.Ok)
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
