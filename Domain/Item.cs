using System.ComponentModel.DataAnnotations;


namespace Domain
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Unit { get; set; }

        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
