using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CRM.DAL.Models.DatabaseModels.Users;
using CRM.DAL.Models.RequestModels.Auth;
using CRM.DAL.Models.ResponseModels.Auth;
using CRM.IdentityServer.ViewModels.Account;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        readonly LoginRequest loginViewModel = new LoginRequest();
        readonly HttpClient client = new HttpClient();

        public LoginView()
        {
            InitializeComponent();
            DataContext = loginViewModel;
        }

        public async Task<HttpResponseMessage> PostData()
        {
            object data = new
            {
                loginViewModel.Login,
                loginViewModel.Password
            };

            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var result = await client.PostAsync(new Uri("https://localhost:5201/api/v1/Account/Login"), content);

            if(result.StatusCode==HttpStatusCode.OK)
            { 
                var tr = JsonConvert.DeserializeObject<TokenResponse>( await result.Content.ReadAsStringAsync());
                Storage.Set(tr.Token);
                UserProfileView user = new UserProfileView();
                user.Show();
            }
            else
            {
                
            }
            return result;
        }


        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PostData();
            this.Close();
        }

        private void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            RegistrationView regView = new RegistrationView();
            regView.Show();
        }
    }
}
