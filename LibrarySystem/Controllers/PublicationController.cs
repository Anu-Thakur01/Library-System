using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Shared.PublicationData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class PublicationController : Controller
    {
        private readonly IPublicationBusiness _publicationBusiness;

        public PublicationController(IPublicationBusiness publicationBusiness)
        {
            _publicationBusiness = publicationBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var publicationList = await _publicationBusiness.GetList();
            return View(publicationList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PublicationDetails publication)
        {
            if (ModelState.IsValid)
            {
                publication.User = "admin";
                bool isAdded = await _publicationBusiness.Add(publication);
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
                return View(publication);
            }
        }


        public async Task<IActionResult> Edit(string id)
        {
            var publicationId = Convert.ToInt32(id);
            var publicationDetails = await _publicationBusiness.GetDetails(publicationId);
            return View(publicationDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PublicationDetails publication)
        {
            if (ModelState.IsValid)
            {
                publication.User = "admin";
                var details = await _publicationBusiness.Edit(publication);
                if (details)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Publication details updated successfully";
                }
                else
                {
                    TempData["isSuccess"] = "NO";
                    TempData["Message"] = "Failed to update publication details";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(publication);
            }
        }

        public async Task<IActionResult> UpdateStatus(string id)
        {
            var publicationId = Convert.ToInt32(id);
            var user = "admin";
            var isUpdated = await _publicationBusiness.UpdateStatus(publicationId,user);
            if (isUpdated)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Publication status updated successfully";
            }
            else
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Failed to update publication status";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _publicationBusiness.Delete(id);
            if (isDeleted)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Publication deleted successfully";
            }
            else
            {
                TempData["isSuccess"] = "NO";
                TempData["Message"] = "Failed to delete publication";
            }
            return RedirectToAction("Index");
        }


        
    }
}
