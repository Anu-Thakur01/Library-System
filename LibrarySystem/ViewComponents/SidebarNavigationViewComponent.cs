using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Shared;
using Microsoft.AspNetCore.Mvc;
namespace LibrarySystem.ViewComponents;

public class SidebarNavigationViewComponent : ViewComponent
{
    private readonly IBookBusiness _bookBusiness;
    private readonly IAuthorBusiness _authorBusiness;

    public SidebarNavigationViewComponent(IBookBusiness bookBusiness, IAuthorBusiness authorBusiness)
    {
        _bookBusiness = bookBusiness;
        _authorBusiness = authorBusiness;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var bookDetailsList =await _bookBusiness.GetBookList();
        var authorDetailList = await _authorBusiness.GetList();
        NavigationDetails navigationDetails = new NavigationDetails
        {
            BookCount = bookDetailsList.Count,
            AuthorCount = authorDetailList.Count
        };
        return View(navigationDetails);
    }
}