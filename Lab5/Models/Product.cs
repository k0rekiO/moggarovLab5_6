using System;
using System.Collections.Generic;

namespace Lab5.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal Price { get; set; }

    public int? SupplierId { get; set; }

    public int? ManufacturerId { get; set; }

    public int? CategoryId { get; set; }

    public decimal Discount { get; set; }

    public int Count { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<OrderArticle> OrderArticles { get; set; } = new List<OrderArticle>();

    public virtual Supplier? Supplier { get; set; }
}
