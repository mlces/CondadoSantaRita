using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.MiCuenta.Lotes
{
    public class DetalleLoteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetalleLoteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FromQuery]
        public int id { get; set; }

        public Contract Contract { get; set; } = new();

        public List<Payment> Payments { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage("/MiCuenta/Clave/Reiniciar");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetFromJsonAsync<Response<Contract>>($"Contracts/{id}");

                if (response.Code == 0)
                {
                    Contract = response.Data;
                }

                var response2 = await _httpClient.GetFromJsonAsync<Response<List<Payment>>>($"Contracts/{id}/Payments");

                if (response2.Code == 0)
                {
                    Payments = response2.Data;
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
