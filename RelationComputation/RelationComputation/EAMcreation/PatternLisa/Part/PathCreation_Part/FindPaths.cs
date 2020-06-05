using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        //Given a MyMatrAdj, this function creates a list of paths of length >=3 (line or circumference).
        //Exception: when it finds a path of length n or n-1 (n = number of centroids on the current MyGroupingSurface)
        //.          the function early recalls the geometric check of MyRepeatedEntities corresponding to the centroids.

        // INPUT: a MyMatrAdj and a list of MyRepeatedEntity (on a MyGroupingSurface)
        // OUTPUT: it returns TRUE if it finds a pattern of length = n or length = n -1, FALSE otherwise 
        //         it returns a list of detected MyPathOfPoints (still to verify the geometry)
        //         a bool onlyShortPaths which is TRUE if there are not paths of length > 2, FALSE otherwise
        //         it updates the lists of final pattern (already geometrically verified), the list of MyMatrAdj
        //         of the current MyGroupingSurface, the list of MyGroupingSurface.
        //There is also a Tolerance check: if it is not satisfied it stops the procedure.


        //This function classifies the points involved in a given MyMatrAdj, subdividing them in the respective list
        public static void ClassifyComponents(MyMatrAdj matrAdjToSee, int n, ref List<int> listOfExtremePoints, ref List<int> listOfSimplePoints,
            ref List<int> listOfMBPoints)
        {
            for (var i = 0; i < n; i++)
            {
                var tot = 0;
                for (var j = 0; j < n; j++)
                {
                    tot += matrAdjToSee.matr[i, j];
                }

                if (tot == 1)
                {
                    listOfExtremePoints.Add(i);
                }
                if (tot == 2)
                {
                    listOfSimplePoints.Add(i);
                }
                if (tot > 2)
                {
                    listOfMBPoints.Add(i);
                }
            }
            
        }
        
    }
}
