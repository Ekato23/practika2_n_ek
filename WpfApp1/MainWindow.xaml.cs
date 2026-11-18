using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Practika2;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(TxtIp.Text, int.Parse(TxtPort.Text));
                TxtLog.AppendText("Успешно подключено к серверу!\n");
            }
            catch (Exception ex)
            {
                TxtLog.AppendText($"Ошибка подключения: {ex.Message}\n");
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null || !_client.Connected)
            {
                MessageBox.Show("Сначала подключитесь к серверу!");
                return;
            }

            try
            {
                var msg = new ViewModelMessage(TxtCommand.Text, TxtData.Text);

                string jsonString = JsonConvert.SerializeObject(msg);
                byte[] data = Encoding.UTF8.GetBytes(jsonString);

                var stream = _client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var responseObj = JsonConvert.DeserializeObject<ViewModelSend>(responseJson);

                if (responseObj != null)
                {
                    TxtLog.AppendText($"Ответ [ID: {responseObj.Id}]: {responseObj.Message}\n");
                }
            }
            catch (Exception ex)
            {
                TxtLog.AppendText($"Ошибка отправки: {ex.Message}\n");
            }
        }
    }
}