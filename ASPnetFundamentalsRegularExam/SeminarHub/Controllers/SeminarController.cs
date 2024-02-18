using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Data.DataConstants;
using SeminarHub.Data.Models;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext data;

        public SeminarController(SeminarHubDbContext _context)
        {
            data = _context;
        }

        public async Task<IActionResult> All()
        {
            var model = await data.Seminars
                .Select(s => new AllViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime.ToString(SeminarConstants.DateFormat),
                    Organizer = s.Organizer.UserName
                }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddViewModel();

            model.Categories = await GetCategories();
            
                return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddViewModel model)
        {
            DateTime dateTime = DateTime.Now;

            if (!DateTime.TryParseExact(model.DateAndTime,
                SeminarConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! format must be {SeminarConstants.DateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return RedirectToAction(nameof(Add));
            }
            var userId = GetUserId();

            var seminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                CategoryId = model.CategoryId,
                DateAndTime = dateTime,
                OrganizerId = userId,
                Duration = model.Duration
            };

            await data.Seminars.AddAsync(seminar);
            await data.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }

        public async Task<IActionResult> Joined()
        {

            var userId = GetUserId();

            var result = await data.Seminars
                .Include(x => x.SeminarsParticipants)
                .Where(x => x.SeminarsParticipants.Any(sp => sp.ParticipantId == userId))
                .Select(x => new AllViewModel()
                {
                    Id = x.Id,
                    DateAndTime = x.DateAndTime.ToString(SeminarConstants.DateFormat),
                    Lecturer = x.Lecturer,
                    Organizer = x.Organizer.UserName,
                    Topic = x.Topic,

                })
                .ToListAsync();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var seminar = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            var model = new DeleteViewModel()
            {
                Id = seminar.Id,
                Topic = seminar.Topic,
                DateAndTime = seminar.DateAndTime
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteViewModel model)
        {
            var seminar = await data.Seminars
                .Where(s => s.Id == model.Id)
                .FirstOrDefaultAsync();

            if(seminar == null)
            {
                return BadRequest();
            }

            var userId = GetUserId();

            if(userId == seminar.OrganizerId)
            {
                data.Seminars.Remove(seminar);

                await data.SaveChangesAsync();
            }
            

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var seminar = await data.Seminars
                .Include(x => x.Category)
                .Include(x => x.Organizer)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(seminar == null)
            {
                throw new InvalidOperationException("There is no such seminar!");
            }

            var model = new DetailsViewModel()
            {
                Id = seminar.Id,
                Category = seminar.Category.Name,
                DateAndTime = seminar.DateAndTime.ToString(SeminarConstants.DateFormat),
                Lecturer = seminar.Lecturer,
                Organizer = seminar.Organizer.UserName,
                Topic = seminar.Topic,
                Details = seminar.Details,
                Duration = seminar.Duration
            };
                
            return View(model);
        }

        public async Task<IActionResult> Join(int id)
        {
            var userId = GetUserId();

            var seminar = await data.Seminars
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if(data.SeminarsParticipants.FirstOrDefault(sp => sp.SeminarId == id && sp.ParticipantId == userId) != null)
            {
                return RedirectToAction(nameof(All));
            }

            if (seminar == null)
            {
                return BadRequest();
            }

            if(seminar.OrganizerId != userId)
            {
                data.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    ParticipantId = userId,
                    SeminarId = seminar.Id
                });
            }
            await data.SaveChangesAsync();
            return RedirectToAction(nameof(Joined));
        }

        public async Task<IActionResult> Leave(int id)
        {
            var userId = GetUserId();

            var seminars = await data.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if(seminars == null)
            {
                return BadRequest();
            }

            var seminarToRemove = seminars.SeminarsParticipants
                .FirstOrDefault(sp => sp.ParticipantId == userId);

            if(seminarToRemove == null)
            {
                return BadRequest();
            }

            seminars.SeminarsParticipants.Remove(seminarToRemove);

            await data.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await data.Seminars.FindAsync(id);

            if(seminar == null)
            {
                return BadRequest();
            }

            if(seminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }
            
            
            var editedSeminar = new EditViewModel()
            {
                Id = id,
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                CategoryId = seminar.CategoryId,
                DateAndTime = seminar.DateAndTime.ToString(SeminarConstants.DateFormat),
                Duration = seminar.Duration,
            };
            editedSeminar.Categories = await GetCategories();
            return View(editedSeminar);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model, int id)
        {
            var seminar = await data.Seminars.FindAsync(id);

            DateTime dateTime = DateTime.Now;

            if (!DateTime.TryParseExact(model.DateAndTime,
                SeminarConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! format must be {SeminarConstants.DateFormat}");
            }

            if (seminar == null)
            {
                return BadRequest();
            }

            if(seminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                RedirectToAction(nameof(Edit));
            }

            seminar.Topic = model.Topic;
            seminar.Lecturer = model.Lecturer;
            seminar.Details = model.Details;
            seminar.CategoryId = model.CategoryId;
            seminar.DateAndTime = dateTime;
            seminar.Duration = model.Duration;

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
