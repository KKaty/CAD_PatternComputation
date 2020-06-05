using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        //Update the list of MyMatrAdj: it delete all the relations stored in the various matrices
        //related to the centroids involved in the current newPatternPoint
        //(this function is used also in CheckAndUpdate_ComposedPatterns)
        public static void UpdateListOfMyMatrAdj(ref List<MyMatrAdj> listOfMatrAdj, 
            int indOfThisCentroid, string nameFile)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("     ---> UpdateListOfMyMatrAdj", nameFile);


            //KLdebug.Print("indOfThisCentroid nella lista di tutti i centroids in GS: " + indOfThisCentroid, nameFile);
            //KLdebug.Print(" AGGIORNO TUTTE LE MATRADJ mandando a 0 le entrate corrispondenti al centroid della current RE:", nameFile);
            //KLdebug.Print(" Numero di MatrAdj nella lista: " + listOfMatrAdj.Count, nameFile);
            if (listOfMatrAdj.Count > 0)
            {
                //Console.WriteLine("lista adiaenze " + listOfMatrAdj[0].matr.GetLength(1));

                var matrAdjDim = listOfMatrAdj[0].matr.GetLength(1);
                foreach (var matrAdj in listOfMatrAdj)
                {
                    //Console.WriteLine(" MatrAdj position: " + listOfMatrAdj.IndexOf(matrAdj));
                    //Console.WriteLine(" d della MatrAdj: " + indOfThisCentroid);
                    //Console.WriteLine(" nOccur prima dell'aggiornamento: " + matrAdj.matr.Length);


                    for (int i = 0; i < matrAdjDim; i++)
                    {
                        if (matrAdj.matr[indOfThisCentroid, i] != 0)
                        {
                            matrAdj.matr[indOfThisCentroid, i] = 0;
                            //Console.WriteLine(" porto a 0 l'entrata (" + indOfThisCentroid + "," + i + ")");
                            matrAdj.matr[i, indOfThisCentroid] = 0;
                            //Console.WriteLine(" porto a 0 l'entrata (" + i + "," + indOfThisCentroid + ")");
                            matrAdj.nOccur -= 1;
                        }
                    }

                    //Console.WriteLine(" nOccur dopo dell'aggiornamento: " + matrAdj.nOccur);
                    //KLdebug.Print(" ", nameFile);

                }

               // KLdebug.Print("Elimino le matrAdj con nOccur <2 ", nameFile);
                var listOfFound = listOfMatrAdj.FindAll(matrAdj => matrAdj.nOccur < 2);
                if (listOfFound.Any())
                {
                    
                    //KLdebug.Print(" Ci sono " + listOfFound.Count + " matrAdj da cancellare", nameFile);
                    foreach (var matrAdj in listOfFound)
                    {
                  //      KLdebug.Print(" calcello la MatrAdj con d=" + matrAdj.d, nameFile);
                        listOfMatrAdj.Remove(matrAdj);
                    }
                }
                //KLdebug.Print("Sono rimaste " + listOfMatrAdj.Count + " matrAdj.", nameFile);

                //   Successivamente si levano le stampe e basta fare questo:
                //listOfMatrAdj.RemoveAll(matrAdj => matrAdj.nOccur < 2);
            }
            //else
            //{
            //    KLdebug.Print("La Lista delle MatrAdj di questa GS è vuota!", nameFile);

            //}
            
            
        }

        //Update the list of paths of centroids found in the current MyMatrAdj
        public static void UpdateListOfMyPathOfCentroids(ref List<MyPathOfPoints> listOfPathOfCentroids,
            int indOfThisCentroid, string nameFile)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("     ---> UpdateListOfMyPathOfCentroids", nameFile);

          
            // I check if there are candidate MyPathOfPoints not geometrically verified yet:
            // for each of them I split them in 2 MyPathOfPoints sons
            // if the two sons are long enough I save them for a future Geometrical Verification.
            
            var listOfPathOfCentroidsToSplit = listOfPathOfCentroids.FindAll(
                    myPathOfCentroids => myPathOfCentroids.path.FindIndex(ind => ind == indOfThisCentroid) != -1);
            if (listOfPathOfCentroidsToSplit.Any())
            {
                //KLdebug.Print(" Trovati Path contenenti il centroid della RE corrente: ", nameFile);

                //KLdebug.Print(" Numero di path trovati: " + listOfPathOfCentroidsToSplit.Count, nameFile);
                //KLdebug.Print(" ", nameFile);

                foreach (var myPathOfCentroids in listOfPathOfCentroidsToSplit)
                {
                    //KLdebug.Print(" -considero il " + listOfPathOfCentroidsToSplit.IndexOf(myPathOfCentroids) + "path:", nameFile);
                    //for (int i = 0; i < myPathOfCentroids.path.Count; i++)
                    //{
                    //    KLdebug.Print("      " + myPathOfCentroids.path[i], nameFile);
                    //}

                    var indInPathOfThisCentroid = myPathOfCentroids.path.IndexOf(indOfThisCentroid);
                    //KLdebug.Print("  in questo path il centroid di Current RE è al posto " + indInPathOfThisCentroid, nameFile);

                    var firstPartOfThePath = myPathOfCentroids.path.GetRange(0, indInPathOfThisCentroid);
                    var firstPartOfThePathLength = firstPartOfThePath.Count;
                    //KLdebug.Print("Spezzo il path in un 1° path di lunghezza " + firstPartOfThePathLength, nameFile);

                    if (firstPartOfThePathLength > 2)
                    {
                        var newSonPath = new MyPathOfPoints(firstPartOfThePath,
                            myPathOfCentroids.pathGeometricObject);
                        listOfPathOfCentroids.Add(newSonPath);
                        //KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
                        //for (int i = 0; i < newSonPath.path.Count; i++)
                        //{
                        //    KLdebug.Print("      " + newSonPath.path[i], nameFile);
                        //}
                    }
                    //else
                    //{
                    //    KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);

                    //}

                    var secondPartOfThePath = myPathOfCentroids.path.GetRange(indInPathOfThisCentroid + 1,
                        myPathOfCentroids.path.Count - (indInPathOfThisCentroid + 1));
                    var secondPartOfThePathLength = secondPartOfThePath.Count;
                    //KLdebug.Print("Spezzo il path in un 2° path di lunghezza " + secondPartOfThePathLength, nameFile);

                    if (secondPartOfThePathLength > 2)
                    {
                        var newSonPath = new MyPathOfPoints(secondPartOfThePath,
                            myPathOfCentroids.pathGeometricObject);
                        listOfPathOfCentroids.Add(newSonPath);
                        //KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
                        //for (int i = 0; i < newSonPath.path.Count; i++)
                        //{
                        //    KLdebug.Print("      " + newSonPath.path[i], nameFile);
                        //}
                    }
                    //else
                    //{
                    //    KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);
                    //}

                    listOfPathOfCentroids.Remove(myPathOfCentroids);
                    //KLdebug.Print("RIMOSSO il Path spezzato in due.", nameFile);
                    //KLdebug.Print(" ", nameFile);

                }
                listOfPathOfCentroids = listOfPathOfCentroids.OrderByDescending(x => x.path.Count).ThenBy(y => y.pathGeometricObject.GetType() == typeof(MyLine) ? 0 : 1).ToList();
                //KLdebug.Print("Riordino la lista listOfPathOfCentroids ancora da geometricamente verificare.", nameFile);
            }
            else
            {
                //KLdebug.Print("Non ho trovato nessun path che intersechi la RE " + indOfThisCentroid, nameFile);
            }

           // KLdebug.Print(" ", nameFile);

        }
    }
}

