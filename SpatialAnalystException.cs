//-----------------------------------------------------------------------
// <copyright file="SpatialAnalystException.cs" company="Studio A&T s.r.l.">
//  Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace Studioat.ArcGis.Soe.Rest
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// class Spatial Analyst Exception
    /// </summary>
    [Serializable]
    public class SpatialAnalystException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SpatialAnalystException class
        /// </summary>
        public SpatialAnalystException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpatialAnalystException class
        /// </summary>
        /// <param name="message">message error</param>
        public SpatialAnalystException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpatialAnalystException class
        /// </summary>
        /// <param name="message">message error</param>
        /// <param name="innerException">object Exception</param>
        public SpatialAnalystException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpatialAnalystException class
        /// </summary>
        /// <param name="info">object SerializationInfo</param>
        /// <param name="context">object StreamingContext</param>
        protected SpatialAnalystException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
