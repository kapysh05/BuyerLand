using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Application.Auth.DTOs;

/// <summary>Ответ на успешную авторизацию: пользователь + пара токенов.</summary>
/// <param name="UserId">Идентификатор пользователя.</param>
/// <param name="Email">Email пользователя (нормализованный, lowercase).</param>
/// <param name="DisplayName">Отображаемое имя.</param>
/// <param name="Role">Роль: Customer или Buyer.</param>
/// <param name="AccessToken">JWT access-токен (Bearer).</param>
/// <param name="RefreshToken">Refresh-токен (хранить безопасно).</param>
/// <param name="AccessTokenExpiresAt">UTC-время истечения access-токена.</param>
public record AuthResponseDto(
    Guid UserId,
    string Email,
    string DisplayName,
    UserRole Role,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);

/// <summary>Публичный профиль пользователя.</summary>
/// <param name="Id">Идентификатор.</param>
/// <param name="Email">Email.</param>
/// <param name="DisplayName">Отображаемое имя.</param>
/// <param name="PhoneNumber">Телефон в формате +7XXXXXXXXXX.</param>
/// <param name="Role">Роль.</param>
/// <param name="AvatarUrl">URL аватара (может быть null).</param>
/// <param name="CreatedAt">UTC-время регистрации.</param>
public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string PhoneNumber,
    UserRole Role,
    string? AvatarUrl,
    DateTime CreatedAt
);
