using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.DAL.Models.DatabaseModels.City
{
    public class City
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public string Title { get; set; }

    }
}