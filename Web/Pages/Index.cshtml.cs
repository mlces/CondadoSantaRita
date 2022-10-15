using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        public ActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.TokenIsReset())
                {
                    return RedirectToPage(Constants.PageReiniciar);
                }
            }
            return Page();
        }
    }
}