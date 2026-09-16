namespace BuyersMarket.Domain.Errors;

/// <summary>
/// Реестр доменных ошибок. Использовать вместо инлайн-конструирования
/// <see cref="Error"/> — даёт единые коды и сообщения, легко искать ссылки.
/// </summary>
public static class Errors
{
    public static class User
    {
        public static readonly Error EmailAlreadyExists =
            Error.Conflict("user.email_already_exists", "Email is already in use.");
    }
}
