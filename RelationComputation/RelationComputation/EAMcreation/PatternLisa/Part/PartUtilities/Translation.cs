using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        
        //It verifies if a symmetry TRANSLATIONAL relation of translational vector between two MyRepeatedEntity
        public static bool IsTranslationTwoRE(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            double[] candidateTranslationArray =
            {
                secondMyRepeatedEntity.centroid.x-firstMyRepeatedEntity.centroid.x,
                secondMyRepeatedEntity.centroid.y-firstMyRepeatedEntity.centroid.y,
                secondMyRepeatedEntity.centroid.z-firstMyRepeatedEntity.centroid.z,
            };
            //whatToWrite = string.Format("Candidate translational array: ({0}, {1}, {2})", candidateTranslationArray[0],
            //                candidateTranslationArray[1], candidateTranslationArray[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            #region levato per il momento
            //var firstListOfVertices = firstMyRepeatedEntity.listOfVertices;
            //var firstNumOfVerteces = firstListOfVertices.Count;
            //var secondListOfVertices = secondMyRepeatedEntity.listOfVertices;
            //var secondNumOfVerteces = secondListOfVertices.Count;

            //#region STAMPA DEI VERTICI DELLE DUE RE
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 1^ RE", nameFile);
            //foreach (var myVert in firstListOfVertices)
            //{
            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", firstListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 2^ RE", nameFile);
            //foreach (var myVert in secondListOfVertices)
            //{
            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", secondListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);
            //#endregion

            //if (firstNumOfVerteces == secondNumOfVerteces)
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print("Numero di vertici prima RE: " + firstNumOfVerteces, nameFile);
            //    KLdebug.Print("Numero di vertici seconda RE: " + secondNumOfVerteces, nameFile);
            //    KLdebug.Print("Il numero di vertici corrisponde. Passo alla verifica della corrispondenza per traslazione:", nameFile);
            //    KLdebug.Print(" ", nameFile);

            //    int i = 0;
            //    while (i < firstNumOfVerteces)
            //    {
            //        if (secondListOfVertices.FindIndex(
            //                vert => vert.IsTranslationOf(firstListOfVertices[i], candidateTranslationArray)) != -1)
            //        {
            //            var found =
            //                secondListOfVertices.Find(
            //                    vert => vert.IsTranslationOf(firstListOfVertices[i], candidateTranslationArray));
            //            whatToWrite = string.Format("Ho trovato che {0}-esimo vertice: ({1}, {2}, {3})", i, firstListOfVertices[i].x,
            //                firstListOfVertices[i].y, firstListOfVertices[i].z);
            //            KLdebug.Print(whatToWrite, nameFile);
            //            whatToWrite = string.Format("ha come suo traslato ({0}, {1}, {2})", found.x, found.y, found.z);
            //            var scarto =
            //                new MyVertex(Math.Abs(firstListOfVertices[i].x + candidateTranslationArray[0] - found.x),
            //                    Math.Abs(firstListOfVertices[i].y + candidateTranslationArray[1] - found.y),
            //                    Math.Abs(firstListOfVertices[i].z + candidateTranslationArray[2] - found.z));
            //            whatToWrite = string.Format("scarto: ({0}, {1}, {2})", scarto.x, scarto.y, scarto.z);
            //            KLdebug.Print(whatToWrite, nameFile);
            //            KLdebug.Print(" ", nameFile);

            //            i++;
            //        }
            //        else
            //        {
            //            KLdebug.Print("TROVATO VERTICE NON CORRISPONDENTE ALLA CERCATA TRASLAZIONE!", nameFile);
            //            KLdebug.Print(" ", nameFile);

            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    KLdebug.Print("NUMERO DI VERTICI NELLE DUE RE NON CORRISPONDENTE. IMPOSSIBILE EFFETTUARE IL CHECK DEI VERTICI!", nameFile);
            //    KLdebug.Print(" ", nameFile);

            //    return false;
            //}
            #endregion

            var numberOfVerticesIsOk = true;
            //var checkOfVertices = CheckOfVerticesForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity,
            //    candidateTranslationArray, ref numberOfVerticesIsOk);
            var checkOfVertices = true;
            if (numberOfVerticesIsOk)
            {
                if (checkOfVertices == false)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            //KLdebug.Print("   ANDATO A BUON FINE IL CHECK DEI VERTICI: PASSO AL CHECK DELLE FACCE.", nameFile);
            //KLdebug.Print(" ", nameFile);

            //Check of correct position of normals of all Planar face:
            if(true)
            //if (!CheckOfPlanesForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity))
            {
                return false;
            }
            
            //////Check of correct position of cylinder faces:
            //if (!CheckOfCylindersForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateTranslationArray))
            //{
            //    return false;
            //}

            //CONTINUARE CON GLI ALTRI TIPI DI SUPERFICI............
            //KLdebug.Print("   ====>>> TRASLAZIONE TRA QUESTE DUE re VERIFICATA!", nameFile);
            return true;
        }

    }
}
