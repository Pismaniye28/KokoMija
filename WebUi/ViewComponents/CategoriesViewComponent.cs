using System.Collections.Generic;
using Bussines.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace KokoMija.WebUi.ViewComponents
{
    public class CategoriesViewComponent:ViewComponent
    {
        private ICategoryService _categoryService;
         public CategoriesViewComponent(ICategoryService categoryService)
        {
            this._categoryService= categoryService;
        }
         public async Task< IViewComponentResult> InvokeAsync()
        {
             if (RouteData.Values["category"] != null)
                 ViewBag.SelectedCategory = RouteData?.Values["category"];
             return View(await _categoryService.GetAll());
            
        }
    }
}