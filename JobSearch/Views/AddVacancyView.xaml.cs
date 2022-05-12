using System;
using System.Collections.Generic;
using System.Linq;
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
using CRM.DAL.Models.DatabaseModels.City;
using CRM.DAL.Models.DatabaseModels.Skill;
using CRM.DAL.Models.DatabaseModels.Vacancies;
using CRM.DAL.Models.DatabaseModels.VacancySkills;
using CRM.IdentityServer.ViewModels.Account;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for AddVacancyView.xaml
    /// </summary>
    public partial class AddVacancyView : Window
    {
        readonly HttpClient client = new HttpClient();
        readonly Vacancy vacancyViewModel = new Vacancy();
        public AddVacancyView()
        {
            InitializeComponent();
            this.DataContext = vacancyViewModel;
        }

        public async Task<HttpResponseMessage> PostData()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Storage.Get()}");
            object data = new
            {
                title = vacancyViewModel.Title,
                fullDescription = vacancyViewModel.FullDescription,
                city = new City(){Title = CityBox.Text}
                //vacancySkills = SkillsBox.Text.Replace(" ","").Split(',').Select(x => new VacancySkill() { Skill = new Skill() { Title = x, Id = new Guid()}, Id = new Guid() }).ToList()
            };

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var content = new StringContent(JsonConvert.SerializeObject(data, serializerSettings));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var result = await client.PostAsync(new Uri("https://localhost:5201/api/v1/Vacancy"), content);

            return result;

        }

        private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PostData();
            this.Close();
        }
    }
}
