using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for UserProfileView.xaml
    /// </summary>
    public partial class UserProfileView : Window, INotifyPropertyChanged
    {
        public User user = new User();
        readonly HttpClient client = new HttpClient();

        public UserProfileView()
        {
            InitializeComponent();
            this.DataContext = user;
        }

        public async Task GetUserData()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Storage.Get()}");
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

        private async void UserProfileView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await GetUserData();
            NameField.Text = $"{user.FirstName} {user.LastName}";
            EmailField.Text = user.Email;
            //CityField.Text = user.City.Title;
            GenderField.Text = user.Gender.ToString();
            PhoneField.Text = user.PhoneNumber;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void FillInfoButton_OnClick(object sender, RoutedEventArgs e)
        {
            FillInfoView f = new FillInfoView();
            f.Show();
            this.Close();
        }
    }
}
