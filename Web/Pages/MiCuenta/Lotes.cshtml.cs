using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.MiCuenta
{
    public class LotesModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LotesModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<Contract> Contracts { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var personId = User.FindFirst(ClaimTypes.Actor).Value;

                var response = await _httpClient.GetFromJsonAsync<Response<List<Contract>>>($"People/{personId}/Contracts");

                if (response.Code == 0)
                {
                    Contracts = response.Data;
                }

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
