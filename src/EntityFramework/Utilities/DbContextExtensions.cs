﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Utilities
{
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    internal static class DbContextExtensions
    {
        public static DbModel GetDynamicUpdateModel(this DbContext context, DbProviderInfo providerInfo)
        {
            DebugCheck.NotNull(context);
            DebugCheck.NotNull(providerInfo);

            var modelBuilder
                = context.InternalContext.CodeFirstModel.CachedModelBuilder.Clone();

            modelBuilder.Entities().Configure(c => c.MapToTable());

            return modelBuilder.Build(providerInfo);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static XDocument GetModel(this DbContext context)
        {
            DebugCheck.NotNull(context);

            return GetModel(w => EdmxWriter.WriteEdmx(context, w));
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static XDocument GetModel(Action<XmlWriter> writeXml)
        {
            DebugCheck.NotNull(writeXml);

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(
                    memoryStream, new XmlWriterSettings
                                      {
                                          Indent = true
                                      }))
                {
                    writeXml(xmlWriter);
                }

                memoryStream.Position = 0;

                return XDocument.Load(memoryStream);
            }
        }
    }
}
