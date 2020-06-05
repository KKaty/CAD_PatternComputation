using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        public static void UpdateListsOfPenultimateAndLast(List<int> listOfExtremePoints, 
            List<int> listOfMBPoints, ref List<int> listOfPenultimate, ref List<int> listOfLast, List<int> Path)
        {
            try
            {

            int Penultimate1 = Path[1];                     // 1 penultimate point of the Path
            int Penultimate2 = Path[Path.Count - 2];        // 2 penultimate point of the Path
            int LastOfPenultimate1 = Path[0];               // 1 last point of the Path
            int LastOfPenultimate2 = Path[Path.Count - 1];  // 2 last point of the Path

            //Un penultimo punto non può essere il penultimo punto di più path, perché:
            //-se avessero in comune il penultimo e non i comune l'ultimo, il penultimo sarebbe un MB
            //-se avessero in comune il penultimo e anche l'ultimo, o i due path si diramerebbero e il penultimo sarebbe MB
            //  oppure coinciderebbero
            //Inoltre l'ultimo punto non deve essere un estremo, quindi un simple
            if (!(listOfMBPoints.Contains(Penultimate1)) && !(listOfExtremePoints.Contains(LastOfPenultimate1)))
            {
                listOfPenultimate.Add(Penultimate1);
                listOfLast.Add(LastOfPenultimate1);
            }
            if (!(listOfMBPoints.Contains(Penultimate2)) && !(listOfExtremePoints.Contains(LastOfPenultimate2)))
            {
                listOfPenultimate.Add(Penultimate2);
                listOfLast.Add(LastOfPenultimate2);
            }

            }
            catch (Exception exception)
            {
            }
        }
    }
}
