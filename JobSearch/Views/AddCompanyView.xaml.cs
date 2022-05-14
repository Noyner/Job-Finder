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
using CRM.DAL.Models.DatabaseModels.Kontragents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JobSearch.Views
{
    /// <summary>
    /// Interaction logic for AddCompanyView.xaml
    /// </summary>
    public partial class AddCompanyView : Window
    {
        private Kontragent kontragentViewModel = new Kontragent();
        readonly HttpClient client = new HttpClient();
        public AddCompanyView()
        {
            InitializeComponent();
            this.DataContext = kontragentViewModel;
        }
        public async Task<HttpResponseMessage> PostData()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Storage.Get()}");
            object data = new
            {
                title = kontragentViewModel.Title,
                kontragentInfo = kontragentViewModel.KontragentInfo,
            };

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var content = new StringContent(JsonConvert.SerializeObject(data, serializerSettings));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var result = await client.PostAsync(new Uri("https://localhost:5201/api/v1/Kontragent"), content);

            return result;

        }
        private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PostData();
            this.Close();
        }
    }
}
