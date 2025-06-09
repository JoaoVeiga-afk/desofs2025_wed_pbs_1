using System;
using System.ComponentModel.DataAnnotations;

namespace ShopTex.Domain.Stores;
public class StoreAddress : IEquatable<StoreAddress>
{
    [Required]
    [StringLength(100)]
    public string Street { get; }

    [Required]
    [StringLength(50)]
    public string City { get; }

    [Required]
    [StringLength(50)]
    public string State { get; }

    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string ZipCode { get; }

    [Required]
    [StringLength(50)]
    public string Country { get; }

    public StoreAddress(string street, string city, string state, string zipCode, string country)
    {
        Street = street?.Trim() ?? throw new ArgumentNullException(nameof(street));
        City = city?.Trim() ?? throw new ArgumentNullException(nameof(city));
        State = state?.Trim() ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode?.Trim() ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country?.Trim() ?? throw new ArgumentNullException(nameof(country));

        Validate();
    }

    private void Validate()
    {
        Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
    }

    // Value object equality
    public override bool Equals(object? obj) => Equals(obj as StoreAddress);

    public bool Equals(StoreAddress? other)
    {
        if (other is null) return false;

        return Street == other.Street &&
                City == other.City &&
                State == other.State &&
                ZipCode == other.ZipCode &&
                Country == other.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, ZipCode, Country);
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}

