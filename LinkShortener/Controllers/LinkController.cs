using LinkShortener.Data;
using LinkShortener.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LinkShortener.Controllers
{
    [Authorize]
    public class LinkController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LinkController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Link/Index
        // Отображение списка ссылок пользователя
        public async Task<IActionResult> Index()
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);

            // Извлекаем список ссылок, связанных с текущим пользователем
            var links = await _context.Links
                                      .Where(l => l.UserId == user.Id)
                                      .ToListAsync();

            return View(links);
        }

        // POST: /Link/Shorten
        // Сокращение ссылки
        [HttpPost]
        public async Task<IActionResult> Shorten(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
            {
                ModelState.AddModelError("", "Please enter a valid URL.");
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User); // Получаем текущего пользователя

            // Генерируем уникальный код для ссылки
            var shortCode = GenerateUniqueCode();

            // Создаем объект Link и заполняем его данными
            var link = new Link
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.Now,
                UserId = user.Id // Привязываем ссылку к текущему пользователю
            };

            // Сохраняем ссылку в базе данных
            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /{code}
        // Перенаправление по короткой ссылке
        [HttpGet("/{code}")]
        [AllowAnonymous] // Разрешаем доступ к перенаправлению всем пользователям
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid URL code.");
            }

            // Ищем ссылку по короткому коду
            var link = await _context.Links.FirstOrDefaultAsync(l => l.ShortCode == code);

            if (link == null)
            {
                return NotFound("Link not found.");
            }

            // Увеличиваем счетчик кликов
            link.ClickCount++;
            await _context.SaveChangesAsync();

            // Перенаправляем пользователя на оригинальный URL
            return Redirect(link.OriginalUrl);
        }

        // Метод для генерации уникального кода для ссылки
        private string GenerateUniqueCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string code;

            // Генерируем код, пока не получим уникальный
            do
            {
                code = new string(Enumerable.Repeat(chars, 6)
                                            .Select(s => s[random.Next(s.Length)])
                                            .ToArray());
            }
            while (_context.Links.Any(l => l.ShortCode == code));

            return code;
        }
    }
}

