using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CRM.DAL.Models.DatabaseModels.Kontragents;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for CompanyProvileView.xaml
    /// </summary>
    public partial class CompanyProvileView : Window
    {
        private Kontragent kontragentViewModel = new Kontragent();
        readonly HttpClient client = new HttpClient();
        public CompanyProvileView()
        {
            InitializeComponent();
            this.DataContext = kontragentViewModel;
        }

        public async Task GetUserData()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Storage.Get()}");
            var response = await client.GetAsync($"https://localhost:5201/api/v1/Kontragent");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rs = await response.Content.ReadAsStringAsync();
                kontragentViewModel = JsonConvert.DeserializeObject<Kontragent>(rs);
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
        private void AddVacancy_OnClick(object sender, RoutedEventArgs e)
        {
            AddVacancyView view = new AddVacancyView();
            view.Show();
        }

        private async void CompanyProvileView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await GetUserData();
            TitleField.Text = kontragentViewModel.Title;
            //KontragentInfoField.Text = kontragentViewModel.KontragentInfo;
        }
    }
}
