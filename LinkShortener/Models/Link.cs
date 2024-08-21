using Microsoft.AspNetCore.Identity;

namespace LinkShortener.Models;

public class Link
{
    public int Id { get; set; } // Уникальный идентификатор
    public string OriginalUrl { get; set; } // Оригинальная ссылка
    public string ShortCode { get; set; } // Сгенерированный короткий код
    public DateTime CreatedAt { get; set; } // Дата создания
    public int ClickCount { get; set; } // Количество переходов
                                        // Внешний ключ для пользователя
    public string UserId { get; set; }
    public IdentityUser User { get; set; } // Связь с пользователем
}
