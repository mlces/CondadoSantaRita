using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Web.Pages.Administracion.Contratos
{
    public class AgregarModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [FromQuery]
        public int id { get; set; }

        [BindProperty]
        public ContractRequest Input { get; set; } = new();

        public AgregarModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public SelectList? PaymentPlans { get; set; }

        public SelectList? People { get; set; }

        public Property? Property { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetAsync($"People");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<List<Person>>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                People = new(content.Data, nameof(Person.PersonId), nameof(Person.FirstName));

                var response2 = await _httpClient.GetAsync($"Catalogs/PaymentPlans");

                if (response2.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response2.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content2 = await response2.Content.ReadFromJsonAsync<Response<List<PaymentPlan>>>();

                if (content2.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content2.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                PaymentPlans = new(content2.Data, nameof(PaymentPlan.PaymentPlanId), nameof(PaymentPlan.Name));

                var response3 = await _httpClient.GetAsync($"Properties/WithoutContract/{id}");

                if (response3.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response3.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content3 = await response3.Content.ReadFromJsonAsync<Response<Property>>();

                if (content3.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content3.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Property = content3.Data;

                Input.PropertyId = Property.PropertyId;

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }

        public async Task<ActionResult> OnPost()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.PostAsJsonAsync($"Contracts", Input);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<Contract>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                return RedirectToPage(Constants.PageAdminDetalleLote, new { id = content.Data.ContractId });
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
