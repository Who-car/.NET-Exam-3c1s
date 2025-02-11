namespace Backend.Application.Common.Strings;

public static class Errors
{
    public static class Authentication
    {
        public const string InvalidPassword = "Неверный пароль.";
        public const string UserNotFound = "Нет пользователя с таким именем.";
    }
    
    public static class Registration
    {
        public const string InvalidUsername = "Имя пользователя может состоять только из латинских букв, цифр и символа нижнего подчёркивания.";
        public const string InvalidPassword = "Длина пароля должна быть как минимум 8 символов.";
        public const string UserAlreadyExists = "Имя пользователя занято.";
    }
}