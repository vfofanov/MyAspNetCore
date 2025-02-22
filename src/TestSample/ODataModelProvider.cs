using System;
using AspNetCore.OData.Versioning;
using Microsoft.AspNetCore.Mvc;
using TestSample.Controllers.OData;
using TestSample.Controllers.OData.v1;
using TestSample.Models.OData;
using TestSample.Models.OData.v1;

namespace TestSample
{
    public class ODataModelProvider : ODataModelProvider<ApiVersion>
    {
        /// <inheritdoc />
        protected override ApiVersion GetNameConventionKey(ApiVersion version)
        {
            return version;
        }

        /// <inheritdoc />
        protected override ApiVersion GetKey(ApiVersion version, IServiceProvider provider)
        {
            return version;
        }

        

        /// <inheritdoc />
        protected override void FillEdmModel(AdvODataConventionModelBuilder builder, ApiVersion key)
        {
            builder.Namespace = "TestNs";
            
            switch (key)
            {
                case { MajorVersion: 1, MinorVersion: 0 }:
                    FillModelV1(builder);
                    break;
                case { MajorVersion: 2, MinorVersion: 0 }:
                    FillModelV2(builder);
                    break;
                default:
                    throw new NotSupportedException($"The input version '{key}' is not supported!");
            }
            builder.EnableLowerCamelCase();
        }

        private static void FillModelV1(AdvODataConventionModelBuilder builder)
        {
            builder.Add<Book, BooksController>();
            builder.EntitySet<Customer, CustomersController>();
        }

        private static void FillModelV2(AdvODataConventionModelBuilder builder)
        {
            builder.EntitySet<Book, BooksController>();
            builder.Add<Press, PressesController>(type =>
            {
                type.Collection
                    .Function(nameof(PressesController.EBooks))
                    .ReturnsCollectionFromEntitySet<Press, PressesController>();
            });

            builder.EntitySet<Models.OData.v2.Customer, Controllers.OData.v2.CustomersController>();
        }
    }
}