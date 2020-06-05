using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {
        public static void KLGetPatternsFromListOfPaths_Assembly(List<MyPathOfPoints> listOfMyPathsOfPoints,
            List<MyRepeatedComponent> listOfComponents, List<MyVertex> listCentroid, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo)
        {
            Part.PartUtilities.GeometryAnalysis.ReorderListOfPaths(ref listOfMyPathsOfPoints);
            while (listOfMyPathsOfPoints.Count > 0)
            {
                var firstIndex = listOfMyPathsOfPoints.IndexOf(listOfMyPathsOfPoints.First());
                var currentPathOfPoints = new MyPathOfPoints(listOfMyPathsOfPoints[firstIndex].path,
                    listOfMyPathsOfPoints[firstIndex].pathGeometricObject);
                listOfMyPathsOfPoints.RemoveAt(firstIndex);
                //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                //listOfOutputPatternTwo.Clear();
                //listOfOutputPattern.Clear();

                var maxLength = KLGetPatternsFromPath_Assembly(currentPathOfPoints,
                    listOfComponents, listCentroid, ref listOfMyPathsOfPoints, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo);

            }
        }
    }
}
