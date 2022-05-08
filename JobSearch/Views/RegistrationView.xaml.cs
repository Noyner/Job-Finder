using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;
using System.Threading.Tasks;
using CRM.IdentityServer.ViewModels.Account;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        readonly HttpClient client = new HttpClient();
        readonly RegisterViewModel registerViewModel = new RegisterViewModel();

        public RegistrationView()
        {
            InitializeComponent();
            DataContext = registerViewModel;
        }

        public async Task<HttpResponseMessage> PostData()
        {
            object data = new
            {
                registerViewModel.FirstName,
                registerViewModel.LastName,
                registerViewModel.Email,
                registerViewModel.Username,
                registerViewModel.Password,
                registerViewModel.PasswordConfirm
            };

            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var result = await client.PostAsync(new Uri("https://localhost:5101/api/v1/Account/Register"), content);

            return result;

        }

        private async void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PostData();
            this.Close();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoginView view = new LoginView();
            view.Show();
            this.Close();
        }
    }
}
