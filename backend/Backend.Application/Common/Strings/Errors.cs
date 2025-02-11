namespace Backend.Application.Common.Strings;

public static class Errors
{
    public static class Authentication
    {
        public const string InvalidPassword = "Неверный пароль.";
        public const string UserNotFound = "Нет пользователя с таким именем.";
        public const string NotAuthorized = "Вы не авторизованы.";
    }
    
    public static class Registration
    {
        public const string InvalidUsername = "Имя пользователя может состоять только из латинских букв, цифр и символа нижнего подчёркивания.";
        public const string InvalidPassword = "Длина пароля должна быть как минимум 8 символов.";
        public const string UserAlreadyExists = "Имя пользователя занято.";
    }
    
    public static class Room
    {
        public const string NotFound = "Комната с таким Id не найдена.";
    }

    public static class Member
    {
        public const string UserIdIsInvalid = "Id пользователя не может быть пуст.";
        public const string RoomIdIsInvalid = "Id комнаты не может быть пуст.";
    }
}