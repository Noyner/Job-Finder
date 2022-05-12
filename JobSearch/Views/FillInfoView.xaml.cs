using System;
using System.Collections.Generic;
using System.Linq;
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
using CRM.IdentityServer.ViewModels.Account;
using Microsoft.AspNet.OData;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for FillInfoView.xaml
    /// </summary>
    public partial class FillInfoView : Window
    {
        readonly HttpClient client = new HttpClient();
        private User user = new User();
        public FillInfoView()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Storage.Get()}");
            InitializeComponent();
            this.DataContext = user;
        }

        public async Task GetUserData()
        {
            var response = await client.GetAsync($"https://localhost:5201/api/v1/User/Profile");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rs = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(rs);
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task<HttpResponseMessage> PostData()
        {
            
            var data = new
            {
                dateOfBirth = DateOfBirthBox.SelectedDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")+"+03:00",
                gender = GenderBox.Text,
                phoneNumber = PhoneBox.Text
            };

            var d = new Delta<User>(typeof(User));
            d.TrySetPropertyValue("PhoneNumber", PhoneBox.Text);

            var content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata.metadata=minimal;odata.streaming=true");

            var result = await client.PatchAsync(new Uri($"https://localhost:5201/api/v1/User(\'{this.user.Id}\')"), content);

            return result;
        }


        private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PostData();
            UserProfileView view = new UserProfileView();
            view.Show();
            this.Close();
        }

        private async void FillInfoView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await GetUserData();
        }
    }
}
