using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Cuenta
{
    [Authorize]
    public class LotesModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LotesModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<Agreement> Agreements { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var personId = User.FindFirst(ClaimTypes.Actor).Value;

                var response = await _httpClient.GetAsync($"People/{personId}/Agreements");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<List<Agreement>>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Agreements = content.Data;
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
