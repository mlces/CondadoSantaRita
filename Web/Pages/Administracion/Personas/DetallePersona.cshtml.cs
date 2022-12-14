using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages.Administracion.Personas
{
    public class DetallePersonaModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetallePersonaModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FromQuery]
        public int id { get; set; }

        public Person Person { get; set; } = new();

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetAsync($"People/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<Person>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Person = content.Data;
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
