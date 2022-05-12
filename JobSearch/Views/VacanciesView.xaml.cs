using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using CRM.DAL.Models.DatabaseModels.Vacancies;
using Newtonsoft.Json;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for VacanciesView.xaml
    /// </summary>
    public partial class VacanciesView : Window
    {

        readonly HttpClient client = new HttpClient();
        public VacanciesView()
        {
            InitializeComponent();
            AddVacancy();
        }

        public async Task GetVacancies()
        {
            client.DefaultRequestHeaders.Add("Vacancy", $"Bearer {Storage.Get()}");
            var response = await client.GetAsync($"https://localhost:5201/api/v1/Vacancy?$expand=kontragent($expand=icon($select=path)),vacancyskills($expand=skill),city,language&$top=10&$skip=0&$count=true");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rs = await response.Content.ReadAsStringAsync();
                var vacancy = JsonConvert.DeserializeObject<Vacancy>(rs);

                if (vacancy != null)
                {
                    VacancyTitle.Text = vacancy.Title;

                    VacancyDescription.Text = vacancy.FullDescription;

                    VacancySkills.Text = String.Join(", ", vacancy.VacancySkills);

                    VacancyCity.Text = vacancy.City.Title;
                }
            }
        }

        public void AddVacancy()
        {
            var nestedStackPanel = new StackPanel();
            var motherStackPanel = new StackPanel();
            var textBlock1 = new TextBlock();
            var textBlock2 = new TextBlock();
            var textBlock3 = new TextBlock();
            var textBlock4 = new TextBlock();

            nestedStackPanel.Style = new Style(typeof(StackPanel), this.FindResource("VacancyStackPanel") as Style);
            textBlock1.Style = new Style(typeof(TextBlock), this.FindResource("TitleStyle") as Style);
            textBlock2.Style = new Style(typeof(TextBlock), this.FindResource("DescriptionStyle") as Style);
            textBlock3.Style = new Style(typeof(TextBlock), this.FindResource("CityStyle") as Style);
            textBlock4.Style = new Style(typeof(TextBlock), this.FindResource("SkillsStyle") as Style);

            nestedStackPanel.Children.Add(textBlock1);
            nestedStackPanel.Children.Add(textBlock2);
            nestedStackPanel.Children.Add(textBlock3);
            nestedStackPanel.Children.Add(textBlock4);
            motherStackPanel.Children.Add(nestedStackPanel);
            this.VacanciesStackPanel.Children.Add(motherStackPanel);
        }

        private async void VacanciesView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await GetVacancies();
        }
    }
}

