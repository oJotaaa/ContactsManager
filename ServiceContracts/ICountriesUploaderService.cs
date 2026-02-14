using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country Entity.
    /// </summary>
    public interface ICountriesUploaderService
    {
        /// <summary>
        /// Uploads country data from the specified Excel file and adds the countries to the system asynchronously.
        /// </summary>
        /// <param name="formFile">The Excel file containing country data to upload. The file must be a valid Excel document and cannot be
        /// null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of countries
        /// successfully uploaded.</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
