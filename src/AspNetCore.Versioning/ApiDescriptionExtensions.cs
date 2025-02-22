﻿using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace AspNetCore.Versioning
{
    /// <summary>
    /// Provides extension methods for the <see cref="ApiDescription"/> class.
    /// </summary>
    public static class ApiDescriptionExtensions
    {
        /// <summary>
        /// Gets the API version associated with the API description, if any.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to get the API version for.</param>
        /// <returns>The associated <see cref="ApiVersion">API version</see> or <c>null</c>.</returns>
        public static ApiVersion GetApiVersion( this ApiDescription apiDescription ) => apiDescription.GetProperty<ApiVersion>();

        /// <summary>
        /// Gets a value indicating whether the associated API description is deprecated.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to evaluate.</param>
        /// <returns><c>True</c> if the <see cref="ApiDescription">API description</see> is deprecated; otherwise, <c>false</c>.</returns>
        public static bool IsDeprecated(this ApiDescription apiDescription)
        {
            if (apiDescription == null)
            {
                throw new ArgumentNullException(nameof(apiDescription));
            }

            var apiVersion = apiDescription.GetApiVersion();
            var model = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

            return model.DeprecatedApiVersions.Contains(apiVersion);
        }

        /// <summary>
        /// Sets the API version associated with the API description.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to set the API version for.</param>
        /// <param name="apiVersion">The associated <see cref="ApiVersion">API version</see>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetApiVersion( this ApiDescription apiDescription, ApiVersion apiVersion ) => apiDescription.SetProperty( apiVersion );

        /// <summary>
        /// Creates a shallow copy of the current API description.
        /// </summary>
        /// <param name="apiDescription">The <see cref="ApiDescription">API description</see> to create a copy of.</param>
        /// <returns>A new <see cref="ApiDescription">API description</see>.</returns>
        public static ApiDescription Clone( this ApiDescription apiDescription )
        {
            if ( apiDescription == null )
            {
                throw new ArgumentNullException( nameof( apiDescription ) );
            }

            var clone = new ApiDescription
            {
                ActionDescriptor = apiDescription.ActionDescriptor,
                GroupName = apiDescription.GroupName,
                HttpMethod = apiDescription.HttpMethod,
                RelativePath = apiDescription.RelativePath,
            };

            foreach ( var property in apiDescription.Properties )
            {
                clone.Properties.Add( property );
            }

            for ( var i = 0; i < apiDescription.ParameterDescriptions.Count; i++ )
            {
                clone.ParameterDescriptions.Add( apiDescription.ParameterDescriptions[i] );
            }

            for ( var i = 0; i < apiDescription.SupportedRequestFormats.Count; i++ )
            {
                clone.SupportedRequestFormats.Add( apiDescription.SupportedRequestFormats[i] );
            }

            for ( var i = 0; i < apiDescription.SupportedResponseTypes.Count; i++ )
            {
                clone.SupportedResponseTypes.Add( apiDescription.SupportedResponseTypes[i] );
            }

            return clone;
        }

        internal static ApiRequestFormat Clone( this ApiRequestFormat requestFormat )
        {
            return new ApiRequestFormat
            {
                Formatter = requestFormat.Formatter,
                MediaType = requestFormat.MediaType,
            };
        }

        internal static ApiResponseType Clone( this ApiResponseType responseType )
        {
            var clone = new ApiResponseType
            {
                ModelMetadata = responseType.ModelMetadata,
                StatusCode = responseType.StatusCode,
                Type = responseType.Type,
            };

            foreach ( var responseFormat in responseType.ApiResponseFormats )
            {
                clone.ApiResponseFormats.Add( responseFormat.Clone() );
            }

            return clone;
        }

        private static ApiResponseFormat Clone( this ApiResponseFormat responseFormat )
        {
            return new ApiResponseFormat
            {
                Formatter = responseFormat.Formatter,
                MediaType = responseFormat.MediaType,
            };
        }
    }
}