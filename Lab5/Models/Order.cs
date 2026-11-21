using System;
using System.Collections.Generic;

namespace Lab5.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public int? PickupPointId { get; set; }

    public int? UserId { get; set; }

    public int Code { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderArticle> OrderArticles { get; set; } = new List<OrderArticle>();

    public virtual PickupPoint? PickupPoint { get; set; }

    public virtual OrderStatus? Status { get; set; }

    public virtual User? User { get; set; }
}
