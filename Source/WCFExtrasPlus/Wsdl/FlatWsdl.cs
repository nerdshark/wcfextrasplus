using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Xml.Schema;
using System.Linq;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;

namespace WCFExtrasPlus.Wsdl
{
	/// <summary>
	/// Originally developed by Thinktecture:
	/// http://weblogs.thinktecture.com/cweyer/2007/05/improving-wcf-interoperability-flattening-your-wsdl.html
	/// </summary>
    public class FlatWsdl
    {

		internal static void ExportEndpoint(WsdlExporter exporter)
        {
            XmlSchemaSet schemaSet = exporter.GeneratedXmlSchemas;

            foreach (ServiceDescription wsdl in exporter.GeneratedWsdlDocuments)
            {
				List<XmlSchema> importsList = new List<XmlSchema>();

                foreach (XmlSchema schema in wsdl.Types.Schemas)
                {
                    AddImportedSchemas(schema, schemaSet, importsList);
                }

                wsdl.Types.Schemas.Clear();

                foreach (XmlSchema schema in importsList)
                {
                    RemoveXsdImports(schema);
					wsdl.Types.Schemas.Add(schema);
                }
            }
        }

        private static void AddImportedSchemas(XmlSchema schema, XmlSchemaSet schemaSet, List<XmlSchema> importsList)
        {
            foreach (XmlSchemaImport import in schema.Includes)
            {
                ICollection realSchemas =
                    schemaSet.Schemas(import.Namespace);

                foreach (XmlSchema ixsd in realSchemas)
                {
                    if (!importsList.Contains(ixsd))
                    {
                        importsList.Add(ixsd);
                        AddImportedSchemas(ixsd, schemaSet, importsList);
                    }
                }
            }
        }

        private static void RemoveXsdImports(XmlSchema schema)
        {
            for (int i = 0; i < schema.Includes.Count; i++)
            {
                if (schema.Includes[i] is XmlSchemaImport)
                    schema.Includes.RemoveAt(i--);
            }
        }
        
    }
}