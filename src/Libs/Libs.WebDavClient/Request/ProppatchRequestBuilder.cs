﻿//<auto-generated/>
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class ProppatchRequestBuilder
    {
        public static string BuildRequestBody(IDictionary<XName, string> propertiesToSet,
            IReadOnlyCollection<XName> propertiesToRemove,
            IReadOnlyCollection<NamespaceAttr> namespaces)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propertyupdate = new XElement("{DAV:}propertyupdate", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            foreach (var ns in namespaces)
            {
                var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                propertyupdate.SetAttributeValue(nsAttr, ns.Namespace);
            }
            if (propertiesToSet.Any())
            {
                var set = new XElement(
                    "{DAV:}set",
                    new XElement(
                        "{DAV:}prop",
                        propertiesToSet.Select(prop =>
                        {
                            var el = new XElement(prop.Key);
                            el.SetInnerXml(prop.Value);
                            return el;
                        }).ToArray()
                    )
                );
                propertyupdate.Add(set);
            }

            if (propertiesToRemove.Any())
            {
                var remove = new XElement(
                    "{DAV:}remove",
                    new XElement(
                        "{DAV:}prop",
                        propertiesToRemove.Select(prop => new XElement(prop)).ToArray()
                    )
                );
                propertyupdate.Add(remove);
            }

            doc.Add(propertyupdate);
            return doc.ToStringWithDeclaration();
        }
    }
}
