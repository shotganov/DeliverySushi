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
    public class WorkerViewModel : INotifyPropertyChanged
    {
        private Worker worker;
        private int workerId;

        public Worker Worker
        {
            get => worker;
            set
            {
                worker = value;
                OnPropertyChanged();
            }
        }
        public void SetId(int id)
        {
            if (id != workerId)
            {
                workerId = id;
                //LoadUser(workerId); // Загружаем данные нового пользователя
            }
        }
        public async Task<Worker> AuthenticateAsync(string login, string password)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var worker = await db.Worker.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (worker != null)
                    {
                        workerId = worker.id;
                        Console.WriteLine($"Пользователь найден: ID={worker.id}, Логин={worker.login}, Роль={worker.position}");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с логином '{login}' не найден или пароль неверен.");
                    }

                    return worker;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка аутентификации: {ex.Message}");
                throw;
            }
        }

        public async Task<Worker> GetWorkerByIdAsync(int id)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedWorker = await db.Worker.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedWorker != null)
                    {
                        Worker = fetchedWorker;
                        Console.WriteLine($"Получены данные пользователя с ID={id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={id} не найден.");
                    }

                    return fetchedWorker;
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