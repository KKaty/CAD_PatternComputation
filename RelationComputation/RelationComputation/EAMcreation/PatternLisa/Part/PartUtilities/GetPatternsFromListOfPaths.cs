using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        public static void ReorderListOfPaths(ref List<MyPathOfPoints> listOfMyPathsOfCentroids)
        {
            //const string nameFile = "ReorderTheListOfFoundPaths.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("LISTA DEI PATH PRIMA DELLO SPOSTAMENTO DEI 'PATH DA 3 LINEA' PRIMA DEI 'PATH DA 4 CRF':", nameFile);
            //foreach (var path in listOfMyPathsOfCentroids)
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print(listOfMyPathsOfCentroids.IndexOf(path) + "° path:", nameFile);
            //    foreach (var centroid in path.path)
            //    {
            //        KLdebug.Print("-" + centroid, nameFile);
            //    }
            //}

            // I move the set of paths of length=3 type Line before the set of paths of length=4 type circum.
            var firstIndPathLineThree = listOfMyPathsOfCentroids.FindIndex(
                pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyLine) && pathObj.path.Count == 3));
            //KLdebug.Print("firstIndPathLineThree=" + firstIndPathLineThree, nameFile);

            var firstIndPathCircumFour = listOfMyPathsOfCentroids.FindIndex(
                pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyCircumForPath) && pathObj.path.Count == 4));
            //KLdebug.Print("firstIndPathCircumFour= " + firstIndPathCircumFour, nameFile);

            if (firstIndPathLineThree != -1 && firstIndPathCircumFour != -1)
            {
                var listOfPathLineThree =
                    listOfMyPathsOfCentroids.FindAll(
                        pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyLine) && pathObj.path.Count == 3));
                //KLdebug.Print("LISTA DEI PATH DA 3 LINEA:", nameFile);
                //foreach (var path in listOfPathLineThree)
                //{
                //    KLdebug.Print(" ", nameFile);
                //    foreach (var centroid in path.path)
                //    {
                //        KLdebug.Print("-" + centroid, nameFile);
                //    }

                //}
                listOfMyPathsOfCentroids.RemoveRange(firstIndPathLineThree, listOfPathLineThree.Count);
                //KLdebug.Print("RIMOSSI I PATH DA 3 LINEA, SONO RIMASTI:", nameFile);
                //foreach (var path in listOfMyPathsOfCentroids)
                //{
                //    KLdebug.Print(" ", nameFile);
                //    foreach (var centroid in path.path)
                //    {
                //        KLdebug.Print("-" + centroid, nameFile);
                //    }

                //}
                listOfMyPathsOfCentroids.InsertRange(firstIndPathCircumFour, listOfPathLineThree);
            }


            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("LISTA DEI PATH DOPO lo SPOSTAMENTO DEI 'PATH DA 3 LINEA' PRIMA DEI 'PATH DA 4 CRF':", nameFile);
            //foreach (var path in listOfMyPathsOfCentroids)
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print(listOfMyPathsOfCentroids.IndexOf(path) + "° path:", nameFile);
            //    foreach (var centroid in path.path)
            //    {
            //        KLdebug.Print("-" + centroid, nameFile);
            //    }

            //}
        }
    }
}
