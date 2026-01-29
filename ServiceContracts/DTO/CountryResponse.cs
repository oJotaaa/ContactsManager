using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the response returned from a country-related API operation.
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        // It compares the current object with another CountryResponse object for equality.
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not CountryResponse)
            {
                return false;
            }

            CountryResponse country_to_compare = (CountryResponse)obj;

            return CountryID == country_to_compare.CountryID &&
                   CountryName == country_to_compare.CountryName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class  CountryExtensions
    {
        // Extension method to convert Country entity to CountryResponse DTO
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.Id,
                CountryName = country.CountryName
            };
        }
    }
}
