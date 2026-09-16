using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BuyersMarket.Application.Common.Persistence;

/// <summary>
/// Хелперы для разбора <see cref="DbUpdateException"/> в постгресовые ошибки.
/// </summary>
public static class DbExceptionHelpers
{
    /// <summary>
    /// PostgreSQL SQLSTATE для нарушения UNIQUE-ограничения.
    /// </summary>
    public const string UniqueViolationSqlState = "23505";

    /// <summary>
    /// True, если <paramref name="ex"/> вызван нарушением UNIQUE-ограничения.
    /// Если задан <paramref name="constraint"/> — дополнительно сверяет имя ограничения.
    /// </summary>
    public static bool IsUniqueViolation(DbUpdateException ex, string? constraint = null)
        => ex.InnerException is PostgresException pg
           && pg.SqlState == UniqueViolationSqlState
           && (constraint is null || pg.ConstraintName == constraint);
}
