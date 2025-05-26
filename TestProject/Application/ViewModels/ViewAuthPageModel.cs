using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;

namespace CineChronicle.Application.ViewModels
{
    public partial class ViewAuthPageModel : ObservableObject
    {
        #region [Private Fields]
        private static DatabaseServiceContent _databaseService;
        #endregion

        #region [Ctor's]
        public ViewAuthPageModel() 
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
        }
        #endregion

        #region [Create Account]
        public static string AddAccount(string emailEntryl, string passwordEntry, string nickName)
        {
            if (string.IsNullOrEmpty(emailEntryl) || string.IsNullOrEmpty(passwordEntry))
            {
                return "Не все поля заполнены";
            }

            if (emailEntryl.Length == 0 && passwordEntry.Length == 0)
            {
                return "Не все поля заполнены";
            }
            if (passwordEntry.Length <8)
            {
                return "Пароль меньше 8 символов. Придумайте пароль длинее";
            }

            string email = emailEntryl.ToLower().TrimEnd();
            if (!ValidateEmail(email))
            {
                return "Неправильный формат почты";
            }

            if (_databaseService.GetUsereByEmail(email) != null)
            {
                return "Такой пользователь уже существует";
            }


            /// Создание аккаунта


            if (string.IsNullOrEmpty(nickName))
            {
                int atIndex = email.IndexOf('@'); 
                if (atIndex != -1)
                {
                    nickName = email.Substring(0, atIndex);
                } 
            }

            // Создаем нового пользователя
            var user = new User
            {
                Email = email,
                Password = passwordEntry,
                NickName = nickName,
                NameIcon = "defaulticon.jpg"
            };
            _databaseService.InsertUser(user);

            if (_databaseService.GetAuthorizedByAuth(true) != null)
            {
                // Убираем с старого пользователя статус авторизованности
                var authUser = _databaseService.GetAuthorizedByAuth(true);
                authUser.IsAuthenticated = false;
                _databaseService.UpdateAuth(authUser);
            }

            var authenticated = new Authorized
            {
                Email = user.Email,
                IsAuthenticated = true
            };

            _databaseService.InsertAuth(authenticated);
            return "Аккаунт создан, вы успешно вошли в аккаунт";
        }
        public static bool ValidateEmail(string email)
        {
            string emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

            if (Regex.IsMatch(email, emailRegex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region [Login in Account]
        public static string CheckValidator(string emailEntry, string passwordEntry)
        {
            if(string.IsNullOrEmpty(emailEntry) || string.IsNullOrEmpty(passwordEntry))
            {
                return "Не все поля заполнены";
            }

            if (emailEntry.Length == 0 && passwordEntry.Length == 0)
            {
                return "Не все поля заполнены";
            }

            string email = emailEntry.ToLower().TrimEnd();

            if (_databaseService.GetUsereByEmail(email) == null)
            {
                return "Введенной почты не существует";
            }

            if (_databaseService.GetUsereByEmail(email).Password != passwordEntry)
            {
                return "Пароли не совпадают";
            }

            if (_databaseService.GetAuthorizedByAuth(true) != null)
            {
                // Ставим прошлому пользователю false если был до этого аккаунт
                var authUser = _databaseService.GetAuthorizedByAuth(true);
                authUser.IsAuthenticated = false;
                _databaseService.UpdateAuth(authUser);

                //Добавляем нового пользователя в авторизованные
                var newAuthUser = _databaseService.GetAuthorizedByEmail(email);
                newAuthUser.IsAuthenticated = true;
                _databaseService.UpdateAuth(newAuthUser);
            }
            else
            {
                // Создаем нового авторизованного пользователя
                var newAuthUser = _databaseService.GetAuthorizedByEmail(email);
                newAuthUser.IsAuthenticated = true;
                _databaseService.UpdateAuth(newAuthUser);
            }
            return "Вы успешно вошли в аккаунт";
        }
        #endregion
    }
}
