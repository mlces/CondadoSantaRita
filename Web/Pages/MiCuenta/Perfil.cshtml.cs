using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.MiCuenta
{
    public class PerfilModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public PerfilModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Person Person { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var personId = User.FindFirst(ClaimTypes.Actor).Value;

                var response = await _httpClient.GetFromJsonAsync<Response<Person>>($"People/{personId}");

                if (response.Code == 0)
                {
                    Person = response.Data;
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
