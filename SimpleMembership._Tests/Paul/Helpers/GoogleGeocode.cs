using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MXM.API.Common
{
    public class GoogleGeocode
    {
        /// <summary>
        /// ResultSet
        /// </summary>
        public Results[] Results;
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string status { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum StatusCodes
    {
        /// <summary>
        /// OK
        /// </summary>
        OK,
        /// <summary>
        /// ZERO_RESULTS
        /// </summary>
        ZERO_RESULTS,
        /// <summary>
        /// OVER_QUERY_LIMIT
        /// </summary>
        OVER_QUERY_LIMIT,
        /// <summary>
        /// REQUEST_DENIED
        /// </summary>
        REQUEST_DENIED,
        /// <summary>
        /// INVALID_REQUEST
        /// </summary>
        INVALID_REQUEST
    }

    /// <summary>
    /// 
    /// </summary>
    public class Results
    {
        /// <summary>
        /// Gets or sets the formatted_address.
        /// </summary>
        /// <value>
        /// The formatted_address.
        /// </value>
        public string formatted_address { get; set; }
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public geometry geometry { get; set; }
        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        public string[] types { get; set; }
        /// <summary>
        /// Gets or sets the address_components.
        /// </summary>
        /// <value>
        /// The address_components.
        /// </value>
        public address_component[] address_components { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class geometry
    {
        /// <summary>
        /// Gets or sets the location_type.
        /// </summary>
        /// <value>
        /// The location_type.
        /// </value>
        public string location_type { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public location location { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class location
    {
        /// <summary>
        /// Gets or sets the lat.
        /// </summary>
        /// <value>
        /// The lat.
        /// </value>
        public double lat { get; set; }
        /// <summary>
        /// Gets or sets the LNG.
        /// </summary>
        /// <value>
        /// The LNG.
        /// </value>
        public double lng { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class address_component
    {
        /// <summary>
        /// Gets or sets the long_name.
        /// </summary>
        /// <value>
        /// The long_name.
        /// </value>
        public string long_name { get; set; }
        /// <summary>
        /// Gets or sets the short_name.
        /// </summary>
        /// <value>
        /// The short_name.
        /// </value>
        public string short_name { get; set; }
        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        public string[] types { get; set; }
    }
}