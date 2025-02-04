﻿using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using V1Iso20022 = Zent.Iso20022.ModelGeneration.Model.V1.Iso20022;
using V2Iso20022 = Zent.Iso20022.ModelGeneration.Model.V2.Iso20022;
using V1 = Zent.Iso20022.ModelGeneration.Parsers.V1;
using V2 = Zent.Iso20022.ModelGeneration.Parsers.V2;
using Zent.Iso20022.ModelGeneration.Models.V2;


namespace Zent.Iso20022.ModelGeneration;

public sealed class Architect
{
    internal static string GetAssemblyVersion()
    {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.FileVersion;
    }

    public static IList<V2Iso20022.MasterData> BuildModel(string[] schemas)
    {
        return (new Architect()).BuildModelV2(schemas);
    }

    public static V1Iso20022.MasterData BuildModelV1(string schema)
    {
        XDocument doc;
        XmlNamespaceManager xnm;

        var names = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames();

        foreach (var name in names)
        {
            Console.WriteLine(name);
        }

        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("Zent.Iso20022.ModelGeneration.SourceFiles.20240411_ISO20022_2013_eRepository.iso20022")!;
        using (var streamReader = new StreamReader(stream, Encoding.UTF8, true))
        {
            var reader = new XmlTextReader(streamReader);
            doc = XDocument.Load(reader);
            XmlNameTable table = reader.NameTable;
            xnm = new XmlNamespaceManager(table);

            XNamespace xmlns = xnm.LookupNamespace("xmlns");
            xnm.AddNamespace("iso20022", doc.Elements().First().Attribute(xmlns + "iso20022").Value);
            xnm.AddNamespace("xmi", doc.Elements().First().Attribute(xmlns + "xmi").Value);
            xnm.AddNamespace("xsi", doc.Elements().First().Attribute(xmlns + "xsi").Value);
        }

        var masterData = new V1Iso20022.MasterData { Doc = doc, Xnm = xnm, ModelVersion = GetAssemblyVersion() };

        V1.Parser.Parse(masterData, schema);


        var md = masterData;

        return masterData;
    }
    public IList<V2Iso20022.MasterData> BuildModelV2(string[] schemas)
    {
        var names = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames();

        foreach (var name in names)
        {
            Console.WriteLine(name);
        }

        var cancellationToken = new CancellationToken();

        var modelBuilder = new V2.Parser(LocateERepository(), LocateExternalCodeSets(), cancellationToken);
        

        return modelBuilder.Parse(() => GetAssemblyVersion(), schemas);
    }

    internal BluePrint LocateERepository()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("Zent.Iso20022.ModelGeneration.SourceFiles.20240411_ISO20022_2013_eRepository.iso20022")!;
        using (var streamReader = new StreamReader(stream, Encoding.UTF8, true))
        {
            var reader = new XmlTextReader(streamReader);
            var doc = XDocument.Load(reader);
            
            XmlNameTable table = reader.NameTable;
            var xnm = new XmlNamespaceManager(table);

            XNamespace xmlns = xnm.LookupNamespace("xmlns");
            xnm.AddNamespace("iso20022", doc.Elements().First().Attribute(xmlns + "iso20022").Value);
            xnm.AddNamespace("xmi", doc.Elements().First().Attribute(xmlns + "xmi").Value);
            xnm.AddNamespace("xsi", doc.Elements().First().Attribute(xmlns + "xsi").Value);

            return new BluePrint(doc, xnm);
        }
    }

    internal BluePrint LocateExternalCodeSets()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("Zent.Iso20022.ModelGeneration.SourceFiles.1Q2024_ExternalCodeSets_v2.xsd")!;
        using (var streamReader = new StreamReader(stream, Encoding.UTF8, true))
        {
            var reader = new XmlTextReader(streamReader);
            var doc = XDocument.Load(reader);
            XmlNameTable table = reader.NameTable;
            var xnm = new XmlNamespaceManager(table);

            XNamespace xmlns = xnm.LookupNamespace("xmlns");
            xnm.AddNamespace("xs", doc.Elements().First().Attribute(xmlns + "xs").Value);

            return new BluePrint(doc, xnm);
        }
    }
}
