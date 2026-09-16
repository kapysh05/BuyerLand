using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Вычисляется в C#. Использовать ТОЛЬКО для проверок в памяти / в DTO.
    /// В LINQ-to-SQL не транслируется — для запросов к БД использовать <see cref="IsActiveExpr"/>.
    /// </summary>
    [NotMapped]
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public User User { get; set; } = null!;

    /// <summary>
    /// Спецификация активности токена в терминах колонок БД.
    /// EF Core корректно транслирует её в SQL: <c>"RevokedAt" IS NULL AND "ExpiresAt" &gt; @now</c>.
    /// </summary>
    public static Expression<Func<RefreshToken, bool>> IsActiveExpr(DateTime now)
        => t => t.RevokedAt == null && t.ExpiresAt > now;
}
