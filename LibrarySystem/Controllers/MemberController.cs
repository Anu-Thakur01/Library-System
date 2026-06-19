using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Shared.MemberData;
using LibrarySystem.Shared.PublicationData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberBusiness _memberBusiness;

        public MemberController(IMemberBusiness memberBusiness)
        {
            _memberBusiness = memberBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var memberList = await _memberBusiness.GetList();
            return View(memberList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MemberDetails member)
        {
            if (ModelState.IsValid)
            {
                member.User = "admin";
                bool isAdded = await _memberBusiness.Add(member);
                if (isAdded)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Member added successfully";
                }
                else
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Failed to add member";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(member);
            }
        }


        public async Task<IActionResult> Edit(string id)
        {
            var memberId = Convert.ToInt32(id);
            var memberDetails = await _memberBusiness.GetDetails(memberId);
            return View(memberDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberDetails member)
        {
            if (ModelState.IsValid)
            {
                member.User = "admin";
                var details = await _memberBusiness.Edit(member);
                if (details)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Member details updated successfully";
                }
                else
                {
                    TempData["isSuccess"] = "NO";
                    TempData["Message"] = "Failed to update member details";
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(member);
            }
        }

        public async Task<IActionResult> UpdateStatus(string id)
        {
            var memberId = Convert.ToInt32(id);
            var user = "admin";
            var isUpdated = await _memberBusiness.UpdateStatus(memberId,user);
            if (isUpdated)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Member status updated successfully";
            }
            else
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Failed to update member status";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _memberBusiness.Delete(id);
            if (isDeleted)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Member deleted successfully";
            }
            else
            {
                TempData["isSuccess"] = "NO";
                TempData["Message"] = "Failed to delete member";
            }
            return RedirectToAction("Index");
        }


        
    }
}
