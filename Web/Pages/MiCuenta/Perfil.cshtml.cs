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
            if (User.TokenIsReset())
            {
                return RedirectToPage("/MiCuenta/Clave/Reiniciar");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var personId = User.FindFirst(ClaimTypes.Actor).Value;

                var response = await _httpClient.GetAsync($"People/{personId}");

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Error");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage("/MiCuenta/Salir");
                }

                var content = await response.Content.ReadFromJsonAsync<Response<Person>>();

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage("/Error");
                }

                Person = content.Data;
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
