using Fingers10.ExcelExport.Attributes;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Attributes;
using System.ComponentModel.DataAnnotations;


namespace Shared.Models
{
    public class CategoryViewModel
    {
        [IncludeInReport(Order = 1)]
        [JqueryDataTableColumn(Order = 1)]
        public int Id { get; set; }

        [IncludeInReport(Order = 2)]
        [JqueryDataTableColumn(Order = 2)]
        [SearchableString(EntityProperty = "Name")]
        [Sortable(EntityProperty = "Name", Default = true)]
        [Required]
        public string Name { get; set; }

        [JqueryDataTableColumn(Order = 3)]
        public string Action { get; set; } = string.Empty;

    }
}


