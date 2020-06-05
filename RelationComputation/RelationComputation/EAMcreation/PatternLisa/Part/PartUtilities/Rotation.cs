using System;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {

        //The rotation matrix is referred to a counterclockwise rotation, so we must establish
        //which of the two possible direction of the outer pointing normals is the correct one
        public static double[] establishAxisDirection(MyVertex first, MyVertex second, MyVertex center, double[] planeNormal)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = string.Format("planenormal: ({0},{1},{2}) ", planeNormal[0], planeNormal[1], planeNormal[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            var tolerance = Math.Pow(10, -6);

            double[] vectorV1 = { first.x - center.x, first.y - center.y, first.z - center.z };
            double[] vectorV2 = { second.x - center.x, second.y - center.y, second.z - center.z };
            double[] crossProduct = FunctionsLC.CrossProduct(vectorV1, vectorV2);
            double[] crossProductNormalized = FunctionsLC.Normalize(crossProduct);
            //whatToWrite = string.Format("crossProduct normalized: ({0},{1},{2}) ", crossProductNormalized[0], crossProductNormalized[1], crossProductNormalized[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            if (FunctionsLC.MyEqualsArray(crossProductNormalized, planeNormal))
            {
                //KLdebug.Print("OK: tengo il versore così e lo arrotondo", nameFile);
                for (var i = 0; i < 3; i++)
                {
                    if (Math.Abs(planeNormal[i]) < tolerance)
                    {
                        planeNormal.SetValue(0, i);
                    }
                }
                //whatToWrite = string.Format("Diventa: ({0},{1},{2}) ", planeNormal[0], planeNormal[1], planeNormal[2]);
                //KLdebug.Print(whatToWrite, nameFile);
                return planeNormal;
            }
            else
            {
                //KLdebug.Print("Devo invertire il versore.", nameFile);
                double[] oppositePlaneNormal = {-planeNormal[0], -planeNormal[1], -planeNormal[2]};
                return oppositePlaneNormal;
            }

        }

    }
}