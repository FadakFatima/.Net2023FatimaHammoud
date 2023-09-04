using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {


        private readonly UserManager<AppUser> userManager;
        private readonly CmsShoppingCartContext context;

        public CartController(CmsShoppingCartContext context)
        {
            this.context = context;
        }

        //Get /cart  
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.
                GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new CartViewModel()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Price * x.Quantity)
            };

            return View(cartVM);
        }


        //Get /cart/ add/S
        public async Task<IActionResult> Add(int id)
        {

            Product product = await context.Products.FindAsync(id);

            List<CartItem> cart = HttpContext.Session.
                GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartItem cartItem = cart.Where(x =>
            x.ProductId == id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                return RedirectToAction("Index");
            }

            return ViewComponent("SmallCart");


        }



        //Get /cart/ decrease/S
        public IActionResult Decrease(int id)
        {


            List<CartItem> cart = HttpContext.Session.
                GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.Where(x =>
            x.ProductId == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(x => x.ProductId == id);
            }

            HttpContext.Session.SetJson("Cart", cart);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }

            return RedirectToAction("Index");

        }



        //Get /cart/ remove/S
        public IActionResult Remove(int id)
        {

            List<CartItem> cart = HttpContext.Session.
                GetJson<List<CartItem>>("Cart");

            cart.RemoveAll(x => x.ProductId == id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }

            return RedirectToAction("Index");

        }


        //Get /cart/ clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");

            //return RedirectToAction("Page", "Pages");
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect(Request.Headers["Referer"].ToString());

            return Ok();

        }
       /*  [HttpPost]
       public IActionResult RateContent(int contentId, int rating)
        {
            var userId = @ViewBag.id.AppUser; // Implement a way to get the current user's ID
            var existingRating = context.Ratings.FirstOrDefault(r => r.ContentId == contentId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.RatingValue = rating;
            }
            else
            {
                var newRating = new Rating
                {
                    UserId = userId,
                    ContentId = contentId,
                    RatingValue = rating
                };
                context.Ratings.Add(newRating);
            }

            context.SaveChanges();

            return RedirectToAction("index");
        }*/



    }
}
