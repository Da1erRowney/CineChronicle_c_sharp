using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace CineChronicle.Application.ViewModels
{
    public partial class ViewAuthPageModel : ObservableObject
    {
        #region [Private Fields]
        private static DatabaseServiceContent _databaseService;

        private static string _recoveryCode;
        #endregion

        #region [Ctor's]
        public ViewAuthPageModel() 
        {
            _databaseService = new DatabaseServiceContent();
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
            if (passwordEntry.Length < 8)
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

            if (_databaseService.GetUsereByNickName(nickName) != null)
            {
                return "Пользователь с таким ником уже существует";
            }
            CreatedAccount(passwordEntry, nickName, email);

            return "Аккаунт создан, вы успешно вошли в аккаунт";
        }

        private static void CreatedAccount(string passwordEntry, string nickName, string email)
        {
            // 1. Проверяем наличие никнейме
            if (string.IsNullOrEmpty(nickName))
            {
                int atIndex = email.IndexOf('@');
                if (atIndex != -1)
                {
                    nickName = email.Substring(0, atIndex);
                }
            }

            // Пример использования
            if (string.IsNullOrEmpty(passwordEntry))
            {
                passwordEntry = GenerateRandomPassword();
            }

            // 2. Создаем нового пользователя
            var user = new User
            {
                Email = email,
                Password = passwordEntry,
                NickName = nickName,
                NameIcon = "defaulticon.jpg"
            };
            _databaseService.InsertUser(user);

            // 3. Убираем с старого пользователя статус авторизованности
            if (_databaseService.GetAuthorizedByAuth(true) != null)
            {
                
                var authUser = _databaseService.GetAuthorizedByAuth(true);
                authUser.IsAuthenticated = false;
                _databaseService.UpdateAuth(authUser);
            }

            // 4.1. Проверяем был ли такой пользователь ранее и ставим статус авторизованности
            if (_databaseService.GetAuthorizedByEmail(email) != null)
            {
                var authUser = _databaseService.GetAuthorizedByEmail(email);
                authUser.IsAuthenticated = true;
                _databaseService.UpdateAuth(authUser);
            }
            else
            {
                // 4.2 Создаем нового авторизованного пользователя
                var authenticated = new Authorized
                {
                    Email = user.Email,
                    IsAuthenticated = true
                };
                _databaseService.InsertAuth(authenticated);
            }

            // 5. Добавляем пользователю непривязанный контент
            var unlinkedContentIds = _databaseService.GetUnlinkedContentIds();
            int userId = _databaseService.GetUserIdByEmail(email);
            DeviceInfo.UserId = userId;
            _databaseService.AddUserContent(userId, unlinkedContentIds);

            // 6. Создание пользовательских настроек
            var settings = new UserSettings
            {
                IsDarkTheme = true,
                IsVideoBackground = Preferences.Get("ShowVideos", true)
            };
            _databaseService.InsertUserSetting(settings);
        }
        private static string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(validChars.Length);
                password.Append(validChars[index]);
            }

            return password.ToString();
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

        #region [Auth Google User]
        public static string CheckGoogleAccount(string email)
        {
            if (_databaseService.GetUsereByEmail(email) != null)
            {

                // 3. Убираем с старого пользователя статус авторизованности
                if (_databaseService.GetAuthorizedByAuth(true) != null)
                {

                    var authUser = _databaseService.GetAuthorizedByAuth(true);
                    authUser.IsAuthenticated = false;
                    _databaseService.UpdateAuth(authUser);
                }

                // 4.1. Проверяем был ли такой пользователь ранее и ставим статус авторизованности
                if (_databaseService.GetAuthorizedByEmail(email) != null)
                {
                    var authUser = _databaseService.GetAuthorizedByEmail(email);
                    authUser.IsAuthenticated = true;
                    _databaseService.UpdateAuth(authUser);
                }
                else
                {
                    // 4.2 Создаем нового авторизованного пользователя
                    var authenticated = new Authorized
                    {
                        Email = email,
                        IsAuthenticated = true
                    };
                    _databaseService.InsertAuth(authenticated);
                }

                var unlinkedContentIds = _databaseService.GetUnlinkedContentIds();
                int userId = _databaseService.GetUserIdByEmail(email);
                DeviceInfo.UserId = userId;
                if (unlinkedContentIds.Count != 0)
                {

                    _databaseService.AddUserContent(userId, unlinkedContentIds);
                }
                return "Рады вас видеть!";
            }
            else
            {
                CreatedAccount("", "", email);
                return "Аккаунт успешно создан!";
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

            // Привязываем контент
            var unlinkedContentIds = _databaseService.GetUnlinkedContentIds();
            int userId = _databaseService.GetUserIdByEmail(email);
            DeviceInfo.UserId = userId;
            if (unlinkedContentIds.Count != 0)
            {
                _databaseService.AddUserContent(userId, unlinkedContentIds);
            }

            var setting = _databaseService.GetUserSettingById(DeviceInfo.UserId);
            if (setting.IsVideoBackground)
            {
                Preferences.Set("ShowVideos", true);
            }
            else
            {
                Preferences.Set("ShowVideos", false);
            }
                return "Вы успешно вошли в аккаунт";
        }
        #endregion

        #region [Recovery Account]
        public static string FindAccountUserForRecovery(string emailOrNick)
        {
            if (_databaseService.GetUsereByEmail(emailOrNick) == null && _databaseService.GetUsereByNickName(emailOrNick) == null)
            {
                return "Не удалось найти пользователя. Проверьте введенные данные.";
            }
            else
            {
                var user = _databaseService.GetUsereByEmail(emailOrNick);
                if (user == null) 
                {
                    user = _databaseService.GetUsereByNickName(emailOrNick);
                }

                string emailUser = user.Email;
                _recoveryCode = GenerateRandomCode();

                var emailService = new EmailService(
                    smtpServer: "smtp.gmail.com",
                    smtpPort: 587,
                    smtpUsername: "cine.chronicle.sup@gmail.com",
                    smtpPassword: "dhjt ejew piwg cnkr",
                    enableSsl: true
                );


                return emailService.SendRecoveryEmail(emailUser, _recoveryCode);
            }
        }

        private static string GenerateRandomCode(int length = 6)
        {
            Random random = new Random();
            string code = "";
        
            for (int i = 0; i < length; i++)
            {
                code += random.Next(0, 10).ToString(); // Генерирует случайную цифру от 0 до 9
            }
        
            return code;
        }

        public static string CheckRecoveryCode(string entryCode)
        {
            if(entryCode == _recoveryCode)
            {
                return "";
            }
            else
            {
                return "Код с почты указан неверно";
            }
        }

        public static string CheckPasswordAndUpdateser(string newPassword, string userData)
        {
            if (newPassword.Length < 8)
            {
                return "Пароль меньше 8 символов. Придумайте пароль длинее";
            }

            var user = _databaseService.GetUsereByEmail(userData);
            if (user == null)
            {
                user = _databaseService.GetUsereByNickName(userData);
            }
            user.Password = newPassword;
            _databaseService.UpdateUser(user);
            return "Пароль успешно обновлен.";
        }

        #endregion

        #region [Change Data Account]
        public static User GetUser()
        {
            var authUser = _databaseService.GetAuthorizedByAuth(true);
            return _databaseService.GetUsereByEmail(authUser.Email);
        }

        public static string CheckChangeFields(string email, string password, string nickName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickName)) return "Не все поля заполнены";

            email = email.ToLower().TrimEnd();
            if (!ValidateEmail(email))
            {
                return "Неправильный формат почты";
            }

            if (password.Length < 8)
            {
                return "Пароль меньше 8 символов. Придумайте пароль длинее";
            }

            var authUser = _databaseService.GetAuthorizedByAuth(true);
            var user =  _databaseService.GetUsereByEmail(authUser.Email);

            if(email != user.Email || password != user.Password)
            {
                var emailService = new EmailService(
                                   smtpServer: "smtp.gmail.com",
                                   smtpPort: 587,
                                   smtpUsername: "cine.chronicle.sup@gmail.com",
                                   smtpPassword: "dhjt ejew piwg cnkr",
                                   enableSsl: true
                               );

                _recoveryCode = GenerateRandomCode();
                emailService.SendRecoveryEmail(user.Email, _recoveryCode, true);
                return "На вашу изначальную почту был выслан код подтверждения для изменений";
            }
            else if(!string.IsNullOrEmpty(nickName) || _databaseService.GetUsereByNickName(nickName) == null)
            {
                user.NickName = nickName;
                _databaseService.UpdateUser(user);
                return "Пользователь успешно изменен";
            }
            else if(_databaseService.GetUsereByNickName(nickName) != null)
            {
                return "Такой ник нейм уже существует.";
            }
            else if (string.IsNullOrEmpty(nickName))
            {
                return "Поле с ник неймом пустое.";
            }
            else
            {
                return "";
            }
        }

        public static string SaveNewUserData(string email, string password, string nickName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickName)) return "Не все поля заполнены";

            email = email.ToLower().TrimEnd();
            if (!ValidateEmail(email))
            {
                return "Неправильный формат почты";
            }

            if (password.Length < 8)
            {
                return "Пароль меньше 8 символов. Придумайте пароль длинее";
            }

            var authUser = _databaseService.GetAuthorizedByAuth(true);
            var user = _databaseService.GetUsereByEmail(authUser.Email);
            user.NickName = nickName;
            user.Email = email;
            user.Password = password;
            _databaseService.UpdateUser(user);
            return "Данные успешно изменены";
        }
        #endregion
    }
}
