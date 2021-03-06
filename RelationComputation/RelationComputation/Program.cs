﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuickGraph;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities;

namespace RelationComputation
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Starting..");

            //var swApp = new SldWorks();

            //var swAPIWarnings = 0;
            //var swAPIErrors = 0;

            //var importData = (ImportStepData)swApp.GetImportFileData(args[0]);
            //var swActiveDoc = (ModelDoc2)swApp.LoadFile4(args[0], "r", importData, swAPIErrors);

            //if (swActiveDoc == null)
            //{
            //    Console.WriteLine("swActiveDoc nullo " + swAPIWarnings + " " + swAPIErrors);
            //    return;
            //}

            //var swAssemblyDoc = (AssemblyDoc)swActiveDoc;

            //var SwConfiguration = swActiveDoc.GetActiveConfiguration();
            //var rootComponent = (Component2)SwConfiguration.GetRootComponent();
            //var fileName = rootComponent.Name2;

            //Console.WriteLine("Modello attivato " + fileName);

            //var transforation =
            //    (Array)AssemblyTraverse.KL_GetTransformsOfAssemblyComponents(rootComponent, swApp);
            //const string fatherName = KLgraph.RootLabel;
            //var pathComponent = rootComponent.Name2;
            //var componentName = pathComponent.Split('/').Last();
            //var componentPath = rootComponent.GetPathName();
            //var id = rootComponent.GetHashCode();

            //var childrenNumber = rootComponent.IGetChildrenCount();
            //var shape = AssemblyTraverse.KL_GetShapeAssembly(rootComponent, swApp);
            //var statistic = AssemblyTraverse.KL_GetStatisticAssembly(rootComponent, swApp);
            //var nodeAssembly = new KLgraph.KLnodeAssembly(id, transforation, fatherName, -1, rootComponent, pathComponent, componentName, componentPath, -1,
            //    childrenNumber, statistic, shape);

            //Console.WriteLine("Creato il nodo assemblato");

            //var vertexList = new List<KLgraph.KLnode>();
            //var edgeList = new List<KLgraph.KLedge>();
            //vertexList.Add(nodeAssembly);
            //var listOfMyListOfInstances = new List<MyListOfInstances>();

            //AssemblyTraverse.KL_GetGraphOfAssemblyComponents(rootComponent, nodeAssembly,
            //   ref vertexList, edgeList, ref listOfMyListOfInstances, swApp);


            //var partList = new List<KLgraph.KLnode>(vertexList);
            //partList.RemoveAll(v => v.GetType() != typeof(KLgraph.KLnodePart));

            //nodeAssembly.KLstatistic.PrincipalPartNumber = partList.Count();

            //nodeAssembly.Instances.AddRange(listOfMyListOfInstances);


            List<MyPatternOfComponents> listPattern = new List<MyPatternOfComponents>();
            List<MyPatternOfComponents> listPattern2 = new List<MyPatternOfComponents>();
            StringBuilder fileOutput = new StringBuilder();
            fileOutput.AppendLine("RICERCA PATH");

            var compName = "DIN";
            var listCentroidWordRF = new List<MyVertex>();
            var v1 = new MyVertex(-0.00302203528372547, -0.0657609306477435, 0.0657609306501929);
            var v2 = new MyVertex(-0.00302203528372547, 0.0930000000026054, 0.0);
            var v3 = new MyVertex(-0.00302203528372547, 0.0, -0.093000000000156);
            var v4 = new MyVertex(-0.00302203528372547, -0.0657609306477435, -0.0657609306505048);
            var v5 = new MyVertex(-0.00302203528372547, -0.0929999999973946, -0.0);
            var v6 = new MyVertex(-0.00302203528372547, 0.0657609306529544, 0.0657609306501929);
            var v7 = new MyVertex(-0.00302203528372547, 0.0657609306529543, -0.0657609306505049);
            var v8 = new MyVertex(-0.00302203528372547, 0.0, 0.0929999999998441);
            
            listCentroidWordRF.Add(v1);
            listCentroidWordRF.Add(v2);
            listCentroidWordRF.Add(v3);
            listCentroidWordRF.Add(v4);
            listCentroidWordRF.Add(v5);
            listCentroidWordRF.Add(v6);
            listCentroidWordRF.Add(v7);
            listCentroidWordRF.Add(v8);


            var newListOfComponetsNoInfo = new List<MyRepeatedComponent>();

            foreach (var comp in listCentroidWordRF)
            {
                var newComp = new MyRepeatedComponent();
                var newRE = new MyRepeatedEntity();
                newRE.centroid = comp;
                newComp.Origin = comp;
                newComp.Name = compName;
                newComp.IdCorrespondingNode = 0;
                newComp.RepeatedEntity = newRE;


                newListOfComponetsNoInfo.Add(newComp);
            }


            AssemblyPatterns.KLFindPatternsOfComponents(newListOfComponetsNoInfo, listCentroidWordRF,
                        ref listPattern, ref listPattern2, ref fileOutput);
            //PatternComputationFunctions.GetAssemblyPatternsOfRepeatedElements(listOfMyListOfInstances, nodeAssembly,
            //    out listPattern, out listPattern2);


            //Console.WriteLine("Istanze trovate " + listOfMyListOfInstances.Count);
            //foreach (var inst in listOfMyListOfInstances)
            //{
            //    Console.WriteLine("Istanza di " + inst.Name + " trovata " + inst.ListOfMyComponent.Count + " volte");
            //}

            Console.WriteLine("Pattern trovati " + listPattern.Count);
            foreach (var patt in listPattern)
            {
                Console.WriteLine("Pattern di tipo " + patt.typeOfMyPattern + " formato da " + patt.listOfMyRCOfMyPattern.First().Name);
            }


            //Console.WriteLine("Ending.. nodi " + graph.VertexCount + " e archi " + graph.EdgeCount);

            /*
            var fileName = Path.GetFileNameWithoutExtension(args[0]);
            var storageGraph = new KLgraph.Graph(graph.Vertices.ToList(), graph.Edges.ToList());

            JsonSerializerSettings setting = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };

            var directoryOutput = Path.GetDirectoryName(args[1]);
            var fileNameOutput = Path.GetFileNameWithoutExtension(args[1]);
            //Console.WriteLine(directoryOutput);
            //Console.WriteLine(fileNameOutput);

            SaveModel(JsonConvert.SerializeObject(storageGraph, setting), directoryOutput, fileNameOutput + ".json");
            Console.WriteLine("JSON saved!");
            */
            //swApp.ExitApp();
        }

        public static void SaveModel(string line, string pathfolder, string fileName)
        {

            using (StreamWriter w = File.CreateText(pathfolder + "\\" + fileName))
            {
                w.WriteLine(line);
            }
        }

    }
}
