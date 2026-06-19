using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Shared.CategoryData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryBusiness _categoryBusiness;

        public CategoryController(ICategoryBusiness categoryBusiness)
        {
            _categoryBusiness = categoryBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var categoryList = await _categoryBusiness.GetList();
            return View(categoryList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryDetails category)
        {
            if (ModelState.IsValid)
            {
                category.User = "admin";
                bool isAdded = await _categoryBusiness.Add(category);
                if (isAdded)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Category added successfully";
                }
                else
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Failed to add category";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }


        public async Task<IActionResult> Edit(string id)
        {
            var categoryId = Convert.ToInt32(id);
            var categoryDetails = await _categoryBusiness.GetDetails(categoryId);
            return View(categoryDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDetails category)
        {
            if (ModelState.IsValid)
            {
                category.User = "admin";
                var details = await _categoryBusiness.Edit(category);
                if (details)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Category details updated successfully";
                }
                else
                {
                    TempData["isSuccess"] = "NO";
                    TempData["Message"] = "Failed to update category details";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }

        public async Task<IActionResult> UpdateStatus(string id)
        {
            var categoryId = Convert.ToInt32(id);
            var user = "admin";
            var isUpdated = await _categoryBusiness.UpdateStatus(categoryId,user);
            if (isUpdated)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Category status updated successfully";
            }
            else
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Failed to update category status";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _categoryBusiness.Delete(id);
            if (isDeleted)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Category deleted successfully";
            }
            else
            {
                TempData["isSuccess"] = "NO";
                TempData["Message"] = "Failed to delete category";
            }
            return RedirectToAction("Index");
        }


        
    }
}
