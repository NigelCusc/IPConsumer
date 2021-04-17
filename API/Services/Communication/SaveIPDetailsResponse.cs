using API.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Communication
{
    public class SaveIPDetailsResponse : BaseResponse
    {
        public IPDetails IPDetails { get; private set; }

        private SaveIPDetailsResponse(bool success, string message, IPDetails details) : base(success, message)
        {
            IPDetails = details;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="details">Saved detailsEntity.</param>
        /// <returns>Response.</returns>
        public SaveIPDetailsResponse(IPDetails details) : this(true, string.Empty, details)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveIPDetailsResponse(string message) : this(false, message, null)
        { }
    }
}
