﻿namespace ShopTex.Domain.Products;

public class PartialProductUpdateDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public string Category { get; set; }

    public string Status { get; set; }

    public string StoreId { get; set; }

    public PartialProductUpdateDto() { }
}