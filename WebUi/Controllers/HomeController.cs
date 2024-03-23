using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebUi.Models;
using Bussines.Abstract;
using Data.Abstract;
using KokoMija.Entity;
using WebUi.Identity;

namespace WebUi.Controllers;
public class HomeController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly ICourserService _courserService;
    private readonly IPhotoService _photoService;
    private readonly IRatingService _ratingService;
    private readonly IFavoriteService _favoriteService;
    private readonly ICommentService _commentService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IProductService productService
        ,ICourserService courserService
        ,IPhotoService photoService
        ,ICategoryService categoryService
        ,IFavoriteService favoriteService
        ,UserManager<User> userManager
        ,IRatingService ratingService
        ,ICommentService commentService
        ,ILogger<HomeController> logger)
    {
        this._productService=productService;
        this._courserService=courserService;
        this._photoService=photoService;
        this._categoryService=categoryService;
        this._ratingService=ratingService;
        this._favoriteService=favoriteService;
        this._userManager=userManager;
        this._commentService=commentService;
        this._logger = logger;
    }

            public async Task<IActionResult> Index()
            {
                var userId = _userManager.GetUserId(User);
                var categories = await _categoryService.GetAll();
                var courser = await _courserService.GetAll();

                // Retrieve the most favorited and most positively rated products
                var mostFavoritedProducts = await _ratingService.GetMostFavoritedProductsAsync();
                var mostPositivelyRatedProducts = await _ratingService.GetMostPositivelyRatedProductsAsync(); // 2 rows of 3 items

                var productViewModel = new ProductListViewModel()
                {
                    Products = _productService.GetHomePageProducts(),
                    Courser = courser,
                    Categories = categories,
                    MostFav = mostFavoritedProducts,
                    MostRated = mostPositivelyRatedProducts
                };

                // Fetch average rating, rating count, and favorite status for each product
                foreach (var product in productViewModel.Products.Concat(mostFavoritedProducts).Concat(mostPositivelyRatedProducts))
                {
                    product.AverageRating = await _ratingService.GetAverageRatingForProductAsync(product.ProductId);
                    product.RatingCount = await _ratingService.GetRatingCount(product.ProductId);
                    product.IsFavorite = await _favoriteService.IsFavoriteAsync(userId, product.ProductId);

                    // Handle the case when AverageRating is null
                    if (product.AverageRating == null)
                    {
                        // Set a default value or handle it accordingly
                        product.AverageRating = 0.0;
                    }
                }

                return View(productViewModel);
            }

        [HttpGet("wykaz")]
        public async Task<IActionResult> List(string category, int page = 1)
        {
            const int pageSize = 25;
            var productViewModel = new ProductListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category, page, pageSize)
            };
                var userId = _userManager.GetUserId(User); // Assuming userId is a string
            // Calculate average rating and rating count for each product
            foreach (var product in productViewModel.Products)
            {
                product.AverageRating = _ratingService.GetAverageRatingForProduct(product.ProductId);
                product.RatingCount = await _ratingService.GetRatingCount(product.ProductId);
                product.IsFavorite = await _favoriteService.IsFavoriteAsync(userId,product.ProductId);
            }

            return View(productViewModel);
        }

[HttpGet("wykaz/{url}")]
public async Task<IActionResult> Details(string url)
{
    if (url == null)
    {
        return NotFound();
    }

    Product product = _productService.GetProductDetails(url);
    product.AverageRating = _ratingService.GetAverageRatingForProduct(product.ProductId);
    product.RatingCount =await _ratingService.GetRatingCount(product.ProductId);

    if (product == null)
    {
        return NotFound();
    }
    
    var id = product.ProductId;
    var userId = _userManager.GetUserId(User); // Assuming userId is a string

    var recommentforhome = await _productService.HomePageRecomment(id);
    var mostFavProducts = await _ratingService.GetFavRecommentProduct(id);
    var mostRatedProducts = await _ratingService.GetRatedRecommentProduct(id);
    var comments = await _commentService.GetCommentsForProductAsync(id);

    // Calculate average rating and rating count for each product
    foreach (var pro in recommentforhome)
    {
        pro.IsFavorite = await _favoriteService.IsFavoriteAsync(userId, pro.ProductId);
        pro.AverageRating = _ratingService.GetAverageRatingForProduct(pro.ProductId);
        pro.RatingCount = await _ratingService.GetRatingCount(pro.ProductId);
    }

    return View(new ProductDetailModel
    {
        Product = product,
        Categories = product.ProductCategories.Select(i => i.Category).ToList(),
        HomePageRecomment = recommentforhome,
        MostFavProducts = mostFavProducts,
        MostRatedProducts = mostRatedProducts,
        Comments = comments.ToList(),
        IsFavorite = await _favoriteService.IsFavoriteAsync(userId, id)
    });
}



        [HttpGet("search/{q?}")]
        public async Task<IActionResult> Search(string q, int page = 1)
        {
            if (q == null)
            {
                var count = await _productService.GetAll();
                const int pageSize = 2;
                var allViewModel = new ProductListViewModel()
                {
                    PageInfo = new PageInfo()
                    {
                        TotalItems = count.Count(),
                        CurrentPage = page,
                        ItemsPerPage = pageSize,
                    },
                    Products = await _productService.GetAll(),
                };
                var userId = _userManager.GetUserId(User); // Assuming userId is a string
                // Calculate average rating and rating count for each product
                foreach (var product in allViewModel.Products)
                {
                    product.AverageRating = _ratingService.GetAverageRatingForProduct(product.ProductId);
                    product.RatingCount = await _ratingService.GetRatingCount(product.ProductId);
                    product.IsFavorite = await _favoriteService.IsFavoriteAsync(userId,product.ProductId);
                }

                return View(allViewModel);
            }
            var productViewModel = new ProductListViewModel()
            {
                Products = _productService.GetSearchResult(q)
            };
            if (productViewModel.Products.Count == 0)
            {
                return View("_noproduct");
            }

            // Calculate average rating and rating count for each product
            foreach (var product in productViewModel.Products)
            {
                product.AverageRating = _ratingService.GetAverageRatingForProduct(product.ProductId);
                product.RatingCount = await _ratingService.GetRatingCount(product.ProductId);
            }

            return View(productViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateProduct(int productId, int rating,string url)
        {
            // Get the currently logged-in user's ID
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Call the SubmitRatingAsync method from the RatingService to submit the user's rating
            await _ratingService.SubmitRatingAsync(productId, userId, rating);

            // Optionally, you can redirect the user back to the product details page after submitting the rating
            return RedirectToAction("Details", new { url = url });

        }

    [HttpGet("Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
