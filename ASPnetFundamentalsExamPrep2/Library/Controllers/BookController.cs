using Library.Data;
using Library.Data.Models;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace Library.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly LibraryDbContext data;
        public BookController(LibraryDbContext context)
        {
            data = context;
        }

        public async Task<IActionResult> All()
        {
            var books = await data.Books
                .Select(b => new AllBooksModel()
                {
                    Title = b.Title,
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    Author = b.Author,
                    Rating = b.Rating,
                    Category = b.Category.Name,
                }).ToListAsync();

            return View(books);
        }

        public async Task<IActionResult> Mine()
        {
            var userId = GetUserId();

            var books = await data.Books
                .Include(b => b.UsersBooks)
                .Where(b => b.UsersBooks.Any(ub => ub.CollectorId == userId))
                .Select(b => new MineBookModel
                {
                    Title = b.Title,
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    Author = b.Author,
                    Description = b.Description,
                    Category = b.Category.Name,
                }).ToListAsync();

            return View(books);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCollection(int Id)
        {
            var book = await data.Books
                .Where(b => b.Id == Id)
                .Include(b => b.UsersBooks)
                .FirstOrDefaultAsync();

            if(book == null)
            {
                return RedirectToAction("All");
            }

            var userId = GetUserId();

            if(!book.UsersBooks.Any(ub => ub.CollectorId == userId))
            {
                book.UsersBooks.Add(new Data.Models.IdentityUserBook()
                {
                    BookId = Id,
                    CollectorId = userId,
                }); 
            }

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCollection(int Id)
        {
            var bookToRemove = await data.Books
                .Where(b => b.Id == Id)
                .Select(b => b.Id)
                .FirstOrDefaultAsync();

            if(bookToRemove == 0)
            {
                return RedirectToAction(nameof(Mine));
            }

            var userId = GetUserId();

            var identityToRemove = data.IdentityUsersBooks.FirstOrDefault(ub => ub.CollectorId == userId && ub.BookId == bookToRemove);

            if (identityToRemove != null)
            {
                data.IdentityUsersBooks.Remove(identityToRemove);
            }

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(Mine));

        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddBookModel();

            model.Categories = await GetCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookModel model)
        {
            var bookToAdd = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                ImageUrl = model.Url,
                Rating = model.Rating,
                CategoryId = model.CategoryId,
            };

            await data.Books.AddAsync(bookToAdd);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await data.Categories
                .AsNoTracking()
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                }).ToListAsync();
        }
    }
}
