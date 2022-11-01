using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Pages
{
    public class ConveniosModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Property> Properties { get; set; } = new();

        public ConveniosModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetAsync("Properties/WithoutAgreement");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<List<Property>>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Properties = content.Data;
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
