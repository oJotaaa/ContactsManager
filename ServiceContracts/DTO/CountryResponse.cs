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
    }

    public static class  CountryExtensions
    {
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
