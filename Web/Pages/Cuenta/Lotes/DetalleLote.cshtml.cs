using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Cuenta.Lotes
{
    [Authorize]
    public class DetalleLoteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetalleLoteModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FromQuery]
        public int id { get; set; }

        public Agreement Agreement { get; set; } = new();

        public List<Payment> Payments { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetAsync($"Agreements/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<Agreement>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var response2 = await _httpClient.GetAsync($"Agreements/{id}/Payments");

                if (response2.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response2.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content2 = await response2.Content.ReadFromJsonAsync<Response<List<Payment>>>();

                if (content2.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content2.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Agreement = content.Data;
                Payments = content2.Data;
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
