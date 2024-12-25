using DeliverySushi.Model;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeliverySushi.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private User user;
        private int userId;
        private string tempPassword; // Временное свойство для пароля
        private string adress;
        private long phone;
        private string login;

        public ICommand SaveCommand { get; }
        public UserViewModel()
        {
            SaveCommand  = new RelayCommand(
             param => SaveChanges(),
             param => CanSave()
         );
            User = new User(); 
        }

        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public string Adress
        {
            get => adress;
            set
            {
                adress = value;
                OnPropertyChanged();
            }
        }

        public long Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string TempPassword
        {
            get => tempPassword;
            set
            {
                tempPassword = value;
                OnPropertyChanged();
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(User.login) &&
                   !string.IsNullOrWhiteSpace(User.email) &&
                   !string.IsNullOrWhiteSpace(tempPassword) && // Используем tempPassword
                   User.phone > 0;
        }

        public void SetId(int id)
        {
            if (id != userId)
            {
                userId = id;
                LoadUser(userId); // Загружаем данные нового пользователя
            }
        }

        /// <summary>
        /// Сохраняет изменения в базу данных.
        /// </summary>
        private async void SaveChanges()
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var existingUser = await db.User.FindAsync(User.id);
                    if (existingUser != null)
                    {
                        existingUser.login = User.login;
                        existingUser.email = User.email;
                        existingUser.password = tempPassword; // Используем tempPassword
                        existingUser.phone = User.phone;
                        existingUser.adress = User.adress;

                        await db.SaveChangesAsync();
                        Console.WriteLine("Данные пользователя успешно обновлены.");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={User.id} не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Загружает пользователя по ID.
        /// </summary>
        public async void LoadUser(int id)
        {
            if (id <= 0)
            {
                Console.WriteLine("Некорректный ID пользователя.");
                return;
            }

            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedUser = await db.User.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedUser != null)
                    {
                        User = fetchedUser;
                        TempPassword = fetchedUser.password; // Устанавливаем tempPassword
                        Adress = fetchedUser.adress;
                        Phone = fetchedUser.phone;
                        Login = fetchedUser.login;
                        Console.WriteLine($"Пользователь с ID={id} успешно загружен: {User.login}");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={id} не найден.");
                        User = new User(); // Сбрасываем данные, если пользователь не найден
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователя: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Аутентификация пользователя по логину и паролю.
        /// </summary>
        public async Task<User> AuthenticateAsync(string login, string password)
        {
            try
            {
                using (var db = new sushiContext())
                {

                    var user = await db.User.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (user != null)
                    {
                        userId = user.id;
                        TempPassword = user.password; // Устанавливаем tempPassword
                        Console.WriteLine($"Пользователь найден: ID={user.id}, Логин={user.login}");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с логином '{login}' не найден или пароль неверен.");
                    }

                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        public async Task<int> RegisterAsync(string login, string password, string email, long phone)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    bool userExists = await db.User.AnyAsync(u => u.login == login || u.email == email);

                    if (userExists)
                    {
                        MessageBox.Show($"Пользователь с логином '{login}' или email '{email}' уже существует.");
                        return 0;
                    }

                    var newUser = new User
                    {
                        login = login,
                        password = password,
                        email = email,
                        phone = phone,
                        adress = ""
                    };

                    db.User.Add(newUser);
                    await db.SaveChangesAsync();

                    // После сохранения находим пользователя для получения ID
                    var createdUser = await db.User.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (createdUser != null)
                    {
                        userId = createdUser.id;
                        User = createdUser;
                        TempPassword = createdUser.password; // Устанавливаем tempPassword
                        Console.WriteLine($"Пользователь зарегистрирован: ID={userId}");
                        return userId;
                    }

                    Console.WriteLine("Ошибка при регистрации: пользователь не найден после создания.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        /// <summary>
        /// Получение пользователя по ID.
        /// </summary>
        public async Task<User> GetCustomerByIdAsync(int id)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedUser = await db.User.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedUser != null)
                    {
                        User = fetchedUser;
                        TempPassword = fetchedUser.password; // Устанавливаем tempPassword
                        Console.WriteLine($"Получены данные пользователя с ID={id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={id} не найден.");
                    }

                    return fetchedUser;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных пользователя: {ex.Message}");
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}