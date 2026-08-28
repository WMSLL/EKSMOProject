using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace ShutdownTimer
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private DateTime? scheduledTime;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Основная форма
            this.Text = "Таймер выключения";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Группа для времени
            var timeGroup = new GroupBox()
            {
                Text = "Выключение по времени",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 80)
            };

            var timePicker = new DateTimePicker()
            {
                Location = new System.Drawing.Point(20, 30),
                Size = new System.Drawing.Size(200, 20),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm",
                Value = DateTime.Now.AddHours(1)
            };

            var setTimeButton = new Button()
            {
                Text = "Установить время",
                Location = new System.Drawing.Point(230, 30),
                Size = new System.Drawing.Size(100, 23)
            };

            timeGroup.Controls.Add(timePicker);
            timeGroup.Controls.Add(setTimeButton);

            // Группа для таймера
            var timerGroup = new GroupBox()
            {
                Text = "Таймер обратного отсчета",
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(350, 80)
            };

            var hoursLabel = new Label() { Text = "Часы:", Location = new System.Drawing.Point(20, 30) };
            var hoursNumeric = new NumericUpDown()
            {
                Location = new System.Drawing.Point(70, 28),
                Size = new System.Drawing.Size(50, 20),
                Minimum = 0,
                Maximum = 24,
                Value = 0
            };

            var minutesLabel = new Label() { Text = "Минуты:", Location = new System.Drawing.Point(130, 30) };
            var minutesNumeric = new NumericUpDown()
            {
                Location = new System.Drawing.Point(190, 28),
                Size = new System.Drawing.Size(50, 20),
                Minimum = 0,
                Maximum = 59,
                Value = 10
            };

            var setTimerButton = new Button()
            {
                Text = "Запустить таймер",
                Location = new System.Drawing.Point(250, 28),
                Size = new System.Drawing.Size(80, 23)
            };

            timerGroup.Controls.AddRange(new Control[] { hoursLabel, hoursNumeric, minutesLabel, minutesNumeric, setTimerButton });

            // Статус
            var statusLabel = new Label()
            {
                Text = "Статус: Ожидание команды",
                Location = new System.Drawing.Point(20, 220),
                Size = new System.Drawing.Size(300, 20),
                Name = "statusLabel"
            };

            // Кнопка отмены
            var cancelButton = new Button()
            {
                Text = "Отменить выключение",
                Location = new System.Drawing.Point(20, 250),
                Size = new System.Drawing.Size(150, 30),
                Enabled = false
            };

            // Добавление элементов на форму
            this.Controls.AddRange(new Control[] { timeGroup, timerGroup, statusLabel, cancelButton });

            // Обработчики событий
            setTimeButton.Click += (s, e) => SetShutdownByTime(timePicker.Value);
            setTimerButton.Click += (s, e) => SetShutdownByTimer((int)hoursNumeric.Value, (int)minutesNumeric.Value);
            cancelButton.Click += (s, e) => CancelShutdown();

            // Сохранение ссылок для доступа из методов
            this.cancelButton = cancelButton;
            this.statusLabel = statusLabel;
        }

        private Button cancelButton;
        private Label statusLabel;

        private async void SetShutdownByTime(DateTime time)
        {
            if (time <= DateTime.Now)
            {
                MessageBox.Show("Выберите время в будущем!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            scheduledTime = time;
            var delay = time - DateTime.Now;

            UpdateStatus($"Выключение в {time:HH:mm} (через {delay:h\\:mm})");
            cancelButton.Enabled = true;

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(delay, cancellationTokenSource.Token);

                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ExecuteShutdown();
                }
            }
            catch (TaskCanceledException)
            {
                UpdateStatus("Выключение отменено");
            }
        }

        private async void SetShutdownByTimer(int hours, int minutes)
        {
            if (hours == 0 && minutes == 0)
            {
                MessageBox.Show("Установите время больше 0!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var totalMinutes = hours * 60 + minutes;
            var shutdownTime = DateTime.Now.AddMinutes(totalMinutes);
            scheduledTime = shutdownTime;

            UpdateStatus($"Выключение через {hours} ч. {minutes} мин. (в {shutdownTime:HH:mm})");
            cancelButton.Enabled = true;

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Обновление статуса каждую минуту
                for (int i = totalMinutes; i > 0; i--)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    var remainingHours = (i - 1) / 60;
                    var remainingMinutes = (i - 1) % 60;
                    UpdateStatus($"Выключение через {remainingHours} ч. {remainingMinutes} мин.");

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationTokenSource.Token);
                }

                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ExecuteShutdown();
                }
            }
            catch (TaskCanceledException)
            {
                UpdateStatus("Выключение отменено");
            }
        }

        private void CancelShutdown()
        {
            cancellationTokenSource?.Cancel();
            cancelButton.Enabled = false;
            scheduledTime = null;

            // Отмена выключения через командную строку
            ExecuteCommand("shutdown /a");
        }

        private void ExecuteShutdown()
        {
            try
            {
                UpdateStatus("Выключаем компьютер...");
                ExecuteCommand("shutdown /s /f /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выключении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Ошибка при выключении");
            }
        }

        private void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process.Start(processInfo);
        }

        private void UpdateStatus(string message)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action<string>(UpdateStatus), message);
            }
            else
            {
                statusLabel.Text = $"Статус: {message}";
            }
        }

       
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}