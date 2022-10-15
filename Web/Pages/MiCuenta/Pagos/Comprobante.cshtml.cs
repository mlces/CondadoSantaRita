using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.MiCuenta.Pagos
{
    public class ComprobanteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ComprobanteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FromQuery]
        public int id { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage("/MiCuenta/Clave/Reiniciar");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetFromJsonAsync<Response<PaymentReceipt>>($"PaymentReceipts/{id}");

                if (response.Code != ResponseCode.Ok)
                {
                    return RedirectToPage("/Error");
                }

                return File(response.Data.Data, "application/pdf");
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
