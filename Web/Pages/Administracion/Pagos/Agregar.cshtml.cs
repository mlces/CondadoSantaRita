using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Web.Pages.Administracion.Pagos
{
    [Authorize(Roles = "Administrador")]
    public class AgregarModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AgregarModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public SelectList? Banks { get; set; }

        public SelectList? PaymentMethods { get; set; }

        [BindProperty]
        public PaymentRequest Input { get; set; } = new();

        [FromQuery]
        public int id { get; set; }

        public string Code { get; set; }

        public string Client { get; set; }

        public string BalancePaid { get; set; }

        public string BalancePayable { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (User.TokenIsReset())
            {
                return RedirectToPage(Constants.PageReiniciar);
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.GetAsync($"Catalogs/Banks");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<List<Bank>>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Banks = new(content.Data, nameof(Bank.BankId), nameof(Bank.Name));

                var response2 = await _httpClient.GetAsync($"Agreements/{id}");

                if (response2.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response2.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content2 = await response2.Content.ReadFromJsonAsync<Response<Agreement>>();

                if (content2.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content2.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                Input.PropertyId = content2.Data.PropertyId;
                Input.PayerId = content2.Data.PersonId;
                Input.PaymentDetails = new PaymentDetailRequest[4]
                {
                    new(){ PaymentMethodId = PaymentMethod.Efectivo.PaymentMethodId},
                    new(){ PaymentMethodId = PaymentMethod.Transferencia.PaymentMethodId},
                    new(){ PaymentMethodId = PaymentMethod.Cheque.PaymentMethodId},
                    new(){ PaymentMethodId = PaymentMethod.Deposito.PaymentMethodId},
                };

                Code = content2.Data.Property.Code;
                Client = content2.Data.Person.FirstName + " " + content2.Data.Person.LastName;
                BalancePaid = content2.Data.BalancePaid.ToCurrency();
                BalancePayable = content2.Data.BalancePayable.ToCurrency();

                PaymentMethods = new(new List<PaymentMethod>() {
                    PaymentMethod.Efectivo,
                    PaymentMethod.Transferencia,
                    PaymentMethod.Cheque,
                    PaymentMethod.Deposito
                }, nameof(PaymentMethod.PaymentMethodId), nameof(PaymentMethod.Name));

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
                Input.PaymentDetails[0].PaymentMethodId = PaymentMethod.Efectivo.PaymentMethodId;
                Input.PaymentDetails[1].PaymentMethodId = PaymentMethod.Transferencia.PaymentMethodId;
                Input.PaymentDetails[2].PaymentMethodId = PaymentMethod.Cheque.PaymentMethodId;
                Input.PaymentDetails[3].PaymentMethodId = PaymentMethod.Deposito.PaymentMethodId;

                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", User.FindFirst(ClaimTypes.Authentication).Value);

                var response = await _httpClient.PostAsJsonAsync($"Payments", Input);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageSalir);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage(Constants.PageError);
                }

                var content = await response.Content.ReadFromJsonAsync<Response<Payment>>();

                if (content.Code == ResponseCode.Unauthorized)
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }

                if (content.Code != ResponseCode.Ok)
                {
                    return RedirectToPage(Constants.PageError);
                }

                return RedirectToPage(Constants.PageAdminComprobante, new { id = content.Data.PaymentId });
            }
            catch (Exception)
            {
                return RedirectToPage(Constants.PageError);
            }
        }
    }
}
