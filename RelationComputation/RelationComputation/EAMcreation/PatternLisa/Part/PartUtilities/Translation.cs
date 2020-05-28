﻿using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        //It detect the maximum translational relation in a set of MyRepeatedEntity, starting from index i of the list of MyRE
        //(INITIAL PATH TYPE: line or circumference)
        public static bool GetMaximumTranslation(List<MyRepeatedEntity> listOfREOnThePath, MyPathGeometricObject pathObject,
            ref int i, ref int numOfRE, ref bool noStop, ref MyPattern outputPattern, List<MyGroupingSurface> listOfInitialGroupingSurface)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("NUOVO AVVIO DI RICERCA TRASLAZIONE", nameFile);
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";

            #region Old version: computation of the candidate translational array computed as the MEAN VECTOR of all the connection arrays of the centroids
            //To compute the candidateTranslationArray I compute the mean vector 
            //of all the difference vector between the centroids:
            //double sumOfx = 0;
            //double sumOfy = 0;
            //double sumOfz = 0;
            //for (int i = 0; i < (numOfRE - 1); i++)
            //{
            //    sumOfx += listOfREOnThePath[i + 1].centroid.x - listOfREOnThePath[i].centroid.x;
            //    sumOfy += listOfREOnThePath[i + 1].centroid.y - listOfREOnThePath[i].centroid.y;
            //    sumOfz += listOfREOnThePath[i + 1].centroid.z - listOfREOnThePath[i].centroid.z;
            //}
            //double[] candidateTranslationArray =
            //{
            //    sumOfx/(numOfRE-1),
            //    sumOfy/(numOfRE-1),
            //    sumOfz/(numOfRE-1)
            //};
            //var whatToWrite = string.Format("Candidate translational array: ({0}, {1}, {2})",
            //    candidateTranslationArray[0], candidateTranslationArray[1], candidateTranslationArray[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            #endregion

            var listOfREOfNewMyPattern = new List<MyRepeatedEntity> { listOfREOnThePath[i] };
            var lengthOfCurrentPath = listOfREOfNewMyPattern.Count;   // = number of couple possibly related by translation (= number of repetition of distance d) 
            var exit = false;
            while (i < (numOfRE - 1) && exit == false) //fino alla penultima RE
            {
                KLdebug.Print(" ", nameFile);
                KLdebug.Print(" ", nameFile);
                whatToWrite = string.Format("         Confronto {0}^ RE e la {1}^ RE: ", i, i + 1);
                KLdebug.Print(whatToWrite, nameFile);

                if (IsTranslationTwoRE(listOfREOnThePath[i], listOfREOnThePath[i + 1]))
                {
                    listOfREOfNewMyPattern.Add(listOfREOnThePath[i + 1]);
                    lengthOfCurrentPath += 1;
                    KLdebug.Print("Aggiunta " + (i + 1) + "-esima RE al MYPattern. lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    KLdebug.Print(" ", nameFile);
                    KLdebug.Print("------>>> Interruzione alla posizione: " + i, nameFile);
                    KLdebug.Print(" ", nameFile);

                    i++;
                }
            }
            KLdebug.Print(" ", nameFile);

            if (lengthOfCurrentPath > 1)
            {
                outputPattern.listOfMyREOfMyPattern = listOfREOfNewMyPattern;
                outputPattern.pathOfMyPattern = pathObject;

                var listOfGroupingSurfaceForThisPattern = findGroupingSurfacesForThisPattern(listOfInitialGroupingSurface, 
                    listOfREOfNewMyPattern, lengthOfCurrentPath);

                outputPattern.listOfGroupingSurfaces = listOfGroupingSurfaceForThisPattern;
                KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
                if (pathObject.GetType() == typeof(MyLine))
                {
                    outputPattern.typeOfMyPattern = "linear TRANSLATION";
                    KLdebug.Print("CREATO PATTERN TRANS lineare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);
                }
                else
                {
                    outputPattern.typeOfMyPattern = "circular TRANSLATION";
                    KLdebug.Print("CREATO PATTERN TRANS circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);

                    var teta = FunctionsLC.FindAngle(listOfREOfNewMyPattern[0].centroid,
                        listOfREOfNewMyPattern[1].centroid, ((MyCircumForPath) pathObject).circumcenter);
                    outputPattern.angle = teta;
                }
                outputPattern.constStepOfMyPattern = listOfREOfNewMyPattern[0].centroid.Distance(listOfREOfNewMyPattern[1].centroid);
                return true;
            }

            KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
            KLdebug.Print("IL TENTATIVO DI PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            KLdebug.Print(" ", nameFile);

            return false;

        }

        public static List<Surface> findGroupingSurfacesForThisPattern(List<MyGroupingSurface> listOfInitialGroupingSurface, 
            List<MyRepeatedEntity> listOfREOfNewMyPattern, int lengthOfCurrentPath)
        {
            //const string nameFile = "AdjacentGS.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("Nuovo Pattern di lunghezza = " + lengthOfCurrentPath, nameFile);

            //Search all the grouping surface containing the pattern:
            int j;
            bool thisGSIsOk;
            var listOfGroupingSurfaceForThisPattern = new List<Surface>();
            foreach (var gs in listOfInitialGroupingSurface)
            {
                j = 0;
                thisGSIsOk = true;
                while (j < lengthOfCurrentPath && thisGSIsOk == true)
                {
                    if (gs.listOfREOfGS.FindIndex(re => re.idRE == listOfREOfNewMyPattern[j].idRE) == -1)
                    {
                        thisGSIsOk = false;
                    }
                    j++;
                }
                if (thisGSIsOk)
                {
                    //KLdebug.Print("Trovata GS di tipo " + gs.groupingSurface.GetType().ToString(), nameFile);
                    listOfGroupingSurfaceForThisPattern.Add(gs.groupingSurface);
                }
            }
            //KLdebug.Print("Numero di superfici adiacenti = " + listOfGroupingSurfaceForThisPattern.Count, nameFile);
            return listOfGroupingSurfaceForThisPattern;
        }

        
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

        public static bool CheckOfVerticesForTranslation(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double[] candidateTranslationArray, ref bool numberOfVerticesIsOk)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            var whatToWrite = "";

            var firstListOfVertices = firstMyRepeatedEntity.listOfVertices;
            var firstNumOfVerteces = firstListOfVertices.Count;
            var secondListOfVertices = secondMyRepeatedEntity.listOfVertices;
            var secondNumOfVerteces = secondListOfVertices.Count;

            #region STAMPA DEI VERTICI DELLE DUE RE
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
            #endregion

            if (firstNumOfVerteces == secondNumOfVerteces)
            {
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print("Numero di vertici prima RE: " + firstNumOfVerteces, nameFile);
                //KLdebug.Print("Numero di vertici seconda RE: " + secondNumOfVerteces, nameFile);
                //KLdebug.Print("Il numero di vertici corrisponde. Passo alla verifica della corrispondenza per traslazione:", nameFile);
                //KLdebug.Print(" ", nameFile);

                int i = 0;
                var checkIsOk = true;
                while (i < firstNumOfVerteces && checkIsOk)
                {
                    var found =
                           secondListOfVertices.Find(
                               vert => vert.IsTranslationOf(firstListOfVertices[i], candidateTranslationArray));
                    if (found != null)
                    {
                       //whatToWrite = string.Format("Ho trovato che {0}-esimo vertice: ({1}, {2}, {3})", i, firstListOfVertices[i].x,
                       //     firstListOfVertices[i].y, firstListOfVertices[i].z);
                       // KLdebug.Print(whatToWrite, nameFile);
                       // whatToWrite = string.Format("ha come suo traslato ({0}, {1}, {2})", found.x, found.y, found.z);
                       // var scarto =
                       //     new MyVertex(Math.Abs(firstListOfVertices[i].x + candidateTranslationArray[0] - found.x),
                       //         Math.Abs(firstListOfVertices[i].y + candidateTranslationArray[1] - found.y),
                       //         Math.Abs(firstListOfVertices[i].z + candidateTranslationArray[2] - found.z));
                       // whatToWrite = string.Format("scarto: ({0}, {1}, {2})", scarto.x, scarto.y, scarto.z);
                       // KLdebug.Print(whatToWrite, nameFile);
                       // KLdebug.Print(" ", nameFile);

                        i++;
                    }
                    else
                    {
                        //KLdebug.Print("TROVATO VERTICE NON CORRISPONDENTE ALLA CERCATA TRASLAZIONE!", nameFile);
                        //KLdebug.Print(" ", nameFile);
                        checkIsOk = false;
                    }
                }

                if (checkIsOk)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //KLdebug.Print("NUMERO DI VERTICI NELLE DUE RE NON CORRISPONDENTE. IMPOSSIBILE EFFETTUARE IL CHECK DEI VERTICI!", nameFile);
                //KLdebug.Print(" ", nameFile);
                numberOfVerticesIsOk = false;
                return false;
            }
        }

        public static bool CheckOfPlanesForTranslation(MyRepeatedEntity firstMyRepeatedEntity, MyRepeatedEntity secondMyRepeatedEntity)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("------>> CHECK DELLE NORMALI DELLE FACCE PLANARI:", nameFile);
            var whatToWrite = "";

            var numOfPlanarSurfacesFirst = firstMyRepeatedEntity.listOfPlanarSurfaces.Count;
            var numOfPlanarSurfacesSecond = secondMyRepeatedEntity.listOfPlanarSurfaces.Count;

            if (numOfPlanarSurfacesFirst == numOfPlanarSurfacesSecond)
            {
                //KLdebug.Print("Numero di facce planari prima RE: " + numOfPlanarSurfacesFirst, nameFile);
                //KLdebug.Print("Numero di facce planari seconda RE: " + numOfPlanarSurfacesSecond, nameFile);
                //KLdebug.Print("Il numero di facce planari corrisponde, passo alla verifica delle normali:", nameFile);
                //KLdebug.Print(" ", nameFile);

                //If there are planar faces I check the correct position of the normals
                if (numOfPlanarSurfacesFirst > 0)
                {
                    var listOfFirstNormals = new List<double[]>();
                    var listOfSecondNormals = new List<double[]>();

                    //KLdebug.Print("Lista delle normali delle facce planari 1^ RE:", nameFile);
                    foreach (var planarSurface in firstMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        double[] normalToAdd = { planarSurface.a, planarSurface.b, planarSurface.c };
                        listOfFirstNormals.Add(normalToAdd);

                       // whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd[0], normalToAdd[1], normalToAdd[2]);
                       // KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Lista delle normali delle facce planari 2^ RE:", nameFile);
                    foreach (var planarSurface in secondMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        double[] normalToAdd = { planarSurface.a, planarSurface.b, planarSurface.c };
                        listOfSecondNormals.Add(normalToAdd);

                        //whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd[0], normalToAdd[1], normalToAdd[2]);
                        //KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Inizio della verifica della corrispondenza delle normali", nameFile);
                    int i = 0;
                    while (listOfSecondNormals.Count != 0 && i < numOfPlanarSurfacesFirst)  // DOVREBBERO ESSERE FALSE O VERE CONTEMPORANEAMENTE!!!
                    {
                       // KLdebug.Print(i + "-esima normale della 1^RE:", nameFile);
                        var indexOfFound =
                            listOfSecondNormals.FindIndex(normal => FunctionsLC.KLMyEqualsArray(normal, listOfFirstNormals[i]));
                        if (indexOfFound != -1)
                        {
                            //whatToWrite = string.Format("Analizzo la normale: ({0},{1},{2})", 
                            //    listOfFirstNormals[i][0], listOfFirstNormals[i][1], listOfFirstNormals[i][2]);
                            //KLdebug.Print(whatToWrite, nameFile);
                            //KLdebug.Print(" -> Trovata corrispondenza con una faccia planare della 2^ RE;", nameFile);

                            listOfSecondNormals.RemoveAt(indexOfFound);
                            i++;
                        }
                        else
                        {
                            //KLdebug.Print(" -> Non è stata trovata corrispondenza con le normali della 2^ RE.", nameFile);
                            //KLdebug.Print("FINE", nameFile);
                            return false;
                        }
                        //KLdebug.Print(" ", nameFile);
                    }
                    //KLdebug.Print(" ", nameFile);

                    return true;
                }
                else
                {
                    //KLdebug.Print("NESSUNA FACCIA PLANARE.", nameFile);
                    //KLdebug.Print("FINE", nameFile);
                    return true;
                }
            }
            else
            {
                //KLdebug.Print("NUMERO DI FACCE PLANARI NON CORRISPONDENTE", nameFile);
                //KLdebug.Print("FINE", nameFile);
                return false;
            }


        }

        public static bool CheckOfCylindersForTranslation(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double[] candidateTranslationArray)
        {
                //controllo tutti gli assi per tutti i cilindri
                //poi per ogni edge di cilindro chiuso controllo i dati della curva

            const string nameFile = "GetTranslationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("------>> CHECK DELLE FACCE CILINDRICHE:", nameFile);

            var tolerance = Math.Pow(10, -5);
            var numOfCylinderSurfacesFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces.Count;
            var numOfCylinderSurfacesSecond = secondMyRepeatedEntity.listOfCylindricalSurfaces.Count;

            #region STAMPA DEGLI ASSI DELLE DUE RE
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("STAMPA DEGLI ASSI DELLA 1^ RE", nameFile);
            foreach (var myVert in firstMyRepeatedEntity.listOfCylindricalSurfaces)
            {
                var whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3}) raggio {4}", firstMyRepeatedEntity.listOfCylindricalSurfaces.IndexOf(myVert),
                    myVert.axisDirectionCylinder[0], myVert.axisDirectionCylinder[1], myVert.axisDirectionCylinder[2], myVert.radiusCylinder);
                KLdebug.Print(whatToWrite, nameFile);
            }
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("STAMPA DEGLI ASSI DELLA 2^ RE", nameFile);
            foreach (var myVert in secondMyRepeatedEntity.listOfCylindricalSurfaces)
            {
                var whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3}) raggio {4}", secondMyRepeatedEntity.listOfCylindricalSurfaces.IndexOf(myVert),
                    myVert.axisDirectionCylinder[0], myVert.axisDirectionCylinder[1], myVert.axisDirectionCylinder[2], myVert.radiusCylinder);
                KLdebug.Print(whatToWrite, nameFile);
            }
            KLdebug.Print(" ", nameFile);
            #endregion

            if (numOfCylinderSurfacesFirst == numOfCylinderSurfacesSecond)
            {
                KLdebug.Print("Numero di facce CILINDRICHE prima RE: " + numOfCylinderSurfacesFirst, nameFile);
                KLdebug.Print("Numero di facce CILINDRICHE seconda RE: " + numOfCylinderSurfacesSecond, nameFile);
                KLdebug.Print("Il numero di facce cilidriche corrisponde, passo alla verifica geometrica:", nameFile);
                KLdebug.Print(" ", nameFile);

                if (numOfCylinderSurfacesFirst > 0)
                {
                    var listOfCylindersToLookIn = new List<MyCylinder>(secondMyRepeatedEntity.listOfCylindricalSurfaces);
                    int i = 0;
                    while (listOfCylindersToLookIn.Count != 0 && i < numOfCylinderSurfacesFirst)
                    {
                        var cylinderOfFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces[i];
                        KLdebug.Print("Sto analizzando lo " + i + "-esimo cilindro: ", nameFile);

                        if (!CylinderOfFirstAtPosition_i_IsOkTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateTranslationArray, cylinderOfFirst, tolerance, ref i)) return false;
                    }
                    return true;
                }
                return true;
            }
            return false;
        }

        public static bool CylinderOfFirstAtPosition_i_IsOkTranslation(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double[] candidateTranslationArray, MyCylinder cylinderOfFirst,
            double tolerance, ref int i)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            //I compute the distance vector v between the centroidOfFirst and the Origin of the cylinderOfFirst (which pass through the axis)
            //The axis of the second MyRepeatedEntity should have the same axis direction and
            //should pass through the point = centroidOfSecond + v
            double[] originOfFirst = cylinderOfFirst.originCylinder;
            var whatToWrite = string.Format("Origin of first: ({0},{1},{2})", originOfFirst[0], originOfFirst[1], originOfFirst[2]);
            KLdebug.Print(whatToWrite, nameFile);

            double[] centroidOfFirst =
            {
                firstMyRepeatedEntity.centroid.x, firstMyRepeatedEntity.centroid.y,
                firstMyRepeatedEntity.centroid.z
            };
            whatToWrite = string.Format("Centroid of first: ({0},{1},{2})", centroidOfFirst[0], centroidOfFirst[1], centroidOfFirst[2]);
            KLdebug.Print(whatToWrite, nameFile);

            // if centroid and origin of the cylinder are coinciding, I find another point on the cylinder axis to substitute the origin:
            if (FunctionsLC.MyEqualsArray(originOfFirst, centroidOfFirst))
            {
                originOfFirst[0] = originOfFirst[0] + cylinderOfFirst.axisDirectionCylinder[0];
                originOfFirst[1] = originOfFirst[1] + cylinderOfFirst.axisDirectionCylinder[1];
                originOfFirst[2] = originOfFirst[2] + cylinderOfFirst.axisDirectionCylinder[2];
                KLdebug.Print("Ricalcolato origin perché coincideva con centroid.", nameFile);
                whatToWrite = string.Format("Nuovo origin: ({0},{1},{2})", originOfFirst[0], originOfFirst[1], originOfFirst[2]);
                KLdebug.Print(whatToWrite, nameFile);
                KLdebug.Print(" ", nameFile);
            }
            else
            {
                KLdebug.Print("Tutto ok, centroid e origin sono diversi tra di loro.", nameFile);
                KLdebug.Print(" ", nameFile);
            }
            double[] vectorToFindPointOnAxis =
            {
                originOfFirst[0] - centroidOfFirst[0],
                originOfFirst[1] - centroidOfFirst[1],
                originOfFirst[2] - centroidOfFirst[2]
            };
            whatToWrite = string.Format("vectorToFindPointOnAxis: ({0},{1},{2})", vectorToFindPointOnAxis[0], vectorToFindPointOnAxis[1], vectorToFindPointOnAxis[2]);
            KLdebug.Print(whatToWrite, nameFile);
            KLdebug.Print("", nameFile);

            double[] centroidOfSecond =
            {
                secondMyRepeatedEntity.centroid.x, secondMyRepeatedEntity.centroid.y,
                secondMyRepeatedEntity.centroid.z
            };
            whatToWrite = string.Format("centroidOfSecond: ({0},{1},{2})", centroidOfSecond[0], centroidOfSecond[1], centroidOfSecond[2]);
            KLdebug.Print(whatToWrite, nameFile);
            KLdebug.Print("", nameFile);

            double[] pointOnAxis =
            {
                centroidOfSecond[0] + vectorToFindPointOnAxis[0],
                centroidOfSecond[1] + vectorToFindPointOnAxis[1],
                centroidOfSecond[2] + vectorToFindPointOnAxis[2]
            };
            whatToWrite = string.Format("pointOnAxis: ({0},{1},{2})", pointOnAxis[0], pointOnAxis[1], pointOnAxis[2]);
            KLdebug.Print(whatToWrite, nameFile);
            KLdebug.Print("", nameFile);

            var axisToFind = FunctionsLC.ConvertPointPlusDirectionInMyLine(pointOnAxis,
                cylinderOfFirst.axisDirectionCylinder);

             var listOfPossibleCylinders =
                     secondMyRepeatedEntity.listOfCylindricalSurfaces.FindAll(
                     cyl => cyl.axisCylinder.Equals(axisToFind));

             if (listOfPossibleCylinders.Any())
            {
                KLdebug.Print("----> Insieme dei cilindri con asse corrispondente della 2^ RE NON VUOTO: ", nameFile);

                
                var numOfPossibleCylinders = listOfPossibleCylinders.Count;
                KLdebug.Print("sono " + numOfPossibleCylinders + "cilindri candidati. Verifico le altre caratteristiche:", nameFile);
                KLdebug.Print("", nameFile);

                //If I find cylinders with right axis line, I check the radius correspondence,
                //the center of bases correspondence.
                //For elliptical bases I make a further control on radius axis.
                var notFound = true;
                int j = 0;
                while (notFound && j < numOfPossibleCylinders)
                {
                    var possibleCylinder = listOfPossibleCylinders[j];
                    KLdebug.Print(j + "-esimo cilindro candidato:", nameFile);
                    if (Math.Abs(cylinderOfFirst.radiusCylinder - possibleCylinder.radiusCylinder) < tolerance)
                    {
                        KLdebug.Print("----raggio OK", nameFile);

                        if (CheckOfClosedEdgesCorrespondenceTransl(cylinderOfFirst, possibleCylinder,
                           candidateTranslationArray))
                        {
                            KLdebug.Print("TROVATO CILINDRO GIUSTO. FINE", nameFile);
                            notFound = false;
                        }
                    }
                    j++;
                }
                if (notFound)
                {
                    KLdebug.Print("", nameFile);
                    KLdebug.Print("NESSUN CILINDRO CORRISPONDENTE. FINE", nameFile);
                    return false;
                }

                //if the previous cylinder is ok, i pass to the next cylinder:
                i++;
            }
            else
            {
                KLdebug.Print("----> Insieme dei cilindri con asse corrispondente della 2^ RE VUOTO. FINE", nameFile);
                return false;
            }
            return true;
        }

        //Given 2 cylinder with corresponding radius and axis (MyLine) corresponding to the expected position for translation, 
        //given the translation vector, it returns TRUE if the two base edges corresponds to the ones of the second cylinder,
        //it return FALSE otherwise.
        //For circles: a comparison of the centers is sufficient
        //For ellipses: a comparison of the centers, of the major and minor radius and of the axis (MyLine) is necessary
        public static bool CheckOfClosedEdgesCorrespondenceTransl(MyCylinder cylinderOfFirst, MyCylinder possibleCylinder, double[] candidateTranslationArray)
        {
            const string nameFile = "GetTranslationalPatterns.txt";

            var listOfCircleFirst = cylinderOfFirst.listOfBaseCircle;
            var listOfCircleSecond = new List<MyCircle>(possibleCylinder.listOfBaseCircle);
            var listOfEllipseFirst = cylinderOfFirst.listOfBaseEllipse;
            var listOfEllipseSecond = new List<MyEllipse>(possibleCylinder.listOfBaseEllipse);

            int numOfCircleFirst = listOfCircleFirst.Count;
            int numOfEllipseFirst = listOfEllipseFirst.Count;
            int numOfCircleSecond = listOfCircleSecond.Count;
            int numOfEllipseSecond = listOfEllipseSecond.Count;

            KLdebug.Print("Numero circle chiuse della 1^ RE: " + numOfCircleFirst, nameFile);
            KLdebug.Print("Numero circle chiuse della 2^ RE: " + numOfCircleSecond, nameFile);
            KLdebug.Print("Numero ellipse chiuse della 1^ RE: " + numOfEllipseFirst, nameFile);
            KLdebug.Print("Numero ellipse chiuse della 2^ RE: " + numOfEllipseSecond, nameFile);

            if (numOfCircleFirst == numOfCircleSecond && numOfEllipseFirst == numOfEllipseSecond)
            {               
                KLdebug.Print("Il numero di CIRCLE e ELLIPSE corrisponde.", nameFile);
                KLdebug.Print(" ", nameFile);

                if (numOfCircleFirst + numOfEllipseFirst == 0)  //cylinders with baseFace not planar
                {
                    KLdebug.Print("Non ci sono circle o ellipse di base. FINE DELLA VERIFICA DELLA CORRISPONDENZA DEGLI EDGE DI BASE DEL CILINDRO.", nameFile);
                    return true;
                }
                if (!CorrespondenceOfCirclesInCylinderTransl(candidateTranslationArray, listOfCircleFirst, listOfCircleSecond)) return false;

                if (!CorrespondenceOfEllipsesInCylinderTransl(candidateTranslationArray, listOfEllipseFirst, listOfEllipseSecond)) return false;

                return true;
            }
            return false;

            #region prova
            //bool firstCenterOk = false;
            //if (cylinderOfFirst.firstBaseCurve.Identity() == possibleCylinder.firstBaseCurve.Identity())
            //{
            //    if (cylinderOfFirst.firstBaseCurve.IsCircle())
            //    {
            //        double[] firstCircleParam = cylinderOfFirst.firstBaseCurve.CircleParams;
            //        var firstCenter = new MyVertex(firstCircleParam[0], firstCircleParam[1], firstCircleParam[2]);

            //        double[] secondCircleParam = possibleCylinder.firstBaseCurve.CircleParams;
            //        var secondCenter = new MyVertex(secondCircleParam[0], secondCircleParam[1], secondCircleParam[2]);

            //        firstCenterOk = secondCenter.IsTranslationOf(firstCenter, candidateTranslationArray);

            //    }
            //    else
            //    {
            //        double[] firstEllipseParam = cylinderOfFirst.firstBaseCurve.GetEllipseParams();
            //        var firstCenter = new MyVertex(firstEllipseParam[0], firstEllipseParam[1], firstEllipseParam[2]);
            //        var firstMajorRad = firstEllipseParam[3];
            //        double[] firstMajorRadAxis = { firstEllipseParam[4], firstEllipseParam[5], firstEllipseParam[6] };
            //        var firstMinorRad = firstEllipseParam[7];
            //        double[] firstMinorRadAxis = { firstEllipseParam[8], firstEllipseParam[9], firstEllipseParam[10] };

            //        double[] secondEllipseParam = possibleCylinder.secondBaseCurve.GetEllipseParams();
            //        var secondCenter = new MyVertex(secondEllipseParam[0], secondEllipseParam[1], secondEllipseParam[2]);
            //        var secondMajorRad = secondEllipseParam[3];
            //        double[] secondMajorRadAxis = { secondEllipseParam[4], secondEllipseParam[5], secondEllipseParam[6] };
            //        var secondMinorRad = secondEllipseParam[7];
            //        double[] secondMinorRadAxis = { secondEllipseParam[8], secondEllipseParam[9], secondEllipseParam[10] };

            //        firstCenterOk = secondCenter.IsTranslationOf(firstCenter, candidateTranslationArray);
            //    }




            //if (cylinderOfFirst.firstBaseCurve.IsCircle())
            //{
            //    double[] firstCircleParam = cylinderOfFirst.firstBaseCurve.CircleParams;
            //    var firstCenter = new MyVertex(firstCircleParam[0], firstCircleParam[1], firstCircleParam[2]);
            //    if (possibleCylinder.firstBaseCurve.IsCircle())
            //    {
            //        double[] secondCircleParam = possibleCylinder.firstBaseCurve.CircleParams;
            //        var secondCenter = new MyVertex(secondCircleParam[0], secondCircleParam[1], secondCircleParam[2]);
            //        bool firstCenterOk = secondCenter.IsTranslationOf(firstCenter, candidateTranslationArray);
            //    }
            //    else
            //    {
            //        if (possibleCylinder.secondBaseCurve.IsCircle())
            //        {
            //            double[] secondCircleParam = possibleCylinder.secondBaseCurve.CircleParams;
            //            var secondCenter = new MyVertex(secondCircleParam[0], secondCircleParam[1],
            //                secondCircleParam[2]);
            //            bool firstCenterOk = secondCenter.IsTranslationOf(firstCenter, candidateTranslationArray);
            //        }
            //    }
            //}
            //else //obviously cylinderOfFirst is an ellipse
            //{
            //    double[] firstEllipseParam = cylinderOfFirst.firstBaseCurve.GetEllipseParams();
            //    var firstCenter = new MyVertex(firstEllipseParam[0], firstEllipseParam[1], firstEllipseParam[2]);
            //    var firstMajorRad = firstEllipseParam[3];
            //    double[] firstMajorRadAxis = { firstEllipseParam[4], firstEllipseParam[5], firstEllipseParam[6] };
            //    var firstMinorRad = firstEllipseParam[7];
            //    double[] firstMinorRadAxis = { firstEllipseParam[8], firstEllipseParam[9], firstEllipseParam[10] };
            //    if (possibleCylinder.firstBaseCurve.IsEllipse())
            //    {

            //    }
            //}
            #endregion                                   
        }

        public static bool CorrespondenceOfCirclesInCylinderTransl(double[] candidateTranslationArray,
            List<MyCircle> listOfCircleFirst, List<MyCircle> listOfCircleSecond)
        {
            const string nameFile = "GetTranslationalPatterns.txt";

            var numOfCircleFirst = listOfCircleFirst.Count;
            var numOfCircleSecond = listOfCircleSecond.Count;

            // OBSERVATION: int i, j are such that 0<=i,j<=2
            if (numOfCircleFirst > 0)
            {
                int i = 0;
                // for every circle of first cylinder
                while (i < numOfCircleFirst)
                {
                    KLdebug.Print(i + "-esimo Circle del 1° cilindro:", nameFile);

                    var firstCenter = listOfCircleFirst[i].centerCircle;
                    var whatToWrite = string.Format("Centro della prima crf ({0},{1},{2})", firstCenter.x, firstCenter.y, firstCenter.z);
                    KLdebug.Print(whatToWrite, nameFile);

                    int j = 0;
                    var thisCircleIsOk = false;

                    while (thisCircleIsOk == false && j < numOfCircleSecond)
                    {
                        KLdebug.Print(j + "-esimo Circle del 2° cilindro:", nameFile);

                        var secondCenter = listOfCircleSecond[j].centerCircle;

                        whatToWrite = string.Format("Centro della seconda crf ({0},{1},{2})", secondCenter.x, secondCenter.y, secondCenter.z);
                        KLdebug.Print(whatToWrite, nameFile);

                        thisCircleIsOk = secondCenter.IsTranslationOf(firstCenter, candidateTranslationArray);
                        KLdebug.Print("Corrispondono i centri? " + thisCircleIsOk, nameFile);
                        KLdebug.Print(" ", nameFile);

                        if (thisCircleIsOk)
                        {
                            listOfCircleSecond.RemoveAt(j);
                        }
                        j++;
                    }
                    if (!thisCircleIsOk)
                    {
                        KLdebug.Print("Non ho trovato nessun Circle che vada bene nel 2° cilindro. Fine.", nameFile);
                        return false;
                    }
                    else
                    {
                        numOfCircleSecond = listOfCircleSecond.Count;
                        i++;
                    }
                }
            }
            else
            {
                KLdebug.Print("Non ci sono Circle in questo cilindro della 1^ RE. Passo agli ellissi.", nameFile);
            }
            KLdebug.Print("VERIFICA DEI CIRCLE é OK.", nameFile);
            KLdebug.Print(" ", nameFile);

            return true;
        }

        public static bool CorrespondenceOfEllipsesInCylinderTransl(double[] candidateTranslationArray,
            List<MyEllipse> listOfEllipseFirst, List<MyEllipse> listOfEllipseSecond)
        {
            const string nameFile = "GetTranslationalPatterns.txt";

            var numOfEllipseFirst = listOfEllipseFirst.Count;
            var numOfEllipseSecond = listOfEllipseSecond.Count;
            var tolerance = Math.Pow(10, -5);

            // OBSERVATION: int i, j are such that 0<=i,j<=2
            if (numOfEllipseFirst > 0)
            {
                int i = 0;
                // for every ellipse of first cylinder
                while (i < numOfEllipseFirst)
                {
                    var firstEllipse = listOfEllipseFirst[i];
                    KLdebug.Print(i + "-esimo Ellipse del 1° cilindro:", nameFile);
                    var whatToWrite = string.Format("Centro della prima ellisse ({0},{1},{2})", firstEllipse.centerEllipse.x, firstEllipse.centerEllipse.y, firstEllipse.centerEllipse.z);
                    KLdebug.Print(whatToWrite, nameFile);

                    int j = 0;
                    var thisEllipseIsOk = false;

                    while (thisEllipseIsOk == false && j < numOfEllipseSecond)
                    {
                        var secondEllipse = listOfEllipseSecond[j]; 
                        KLdebug.Print(j + "-esimo Ellipse del 2° cilindro:", nameFile);
                        whatToWrite = string.Format("Centro della seconda ellisse ({0},{1},{2})", secondEllipse.centerEllipse.x, secondEllipse.centerEllipse.y, secondEllipse.centerEllipse.z);
                        KLdebug.Print(whatToWrite, nameFile);

                        //comparison of the ellipse centers:
                        thisEllipseIsOk = secondEllipse.centerEllipse.IsTranslationOf(firstEllipse.centerEllipse,
                            candidateTranslationArray);
                        if (thisEllipseIsOk)
                        {
                            KLdebug.Print("Corrispondono i centri? " + thisEllipseIsOk, nameFile);
                            //if the radius correspond:
                            if (Math.Abs(firstEllipse.majorRadEllipse - secondEllipse.majorRadEllipse) < tolerance &&
                                Math.Abs(firstEllipse.minorRadEllipse - secondEllipse.minorRadEllipse) < tolerance)
                            {
                                KLdebug.Print("Corrispondono i major minor radius? True", nameFile);

                                //check if the directions corresponding to the axis are ok:

                                double[] firstMajorAxisDirectionEllipseOpposite =
                                {
                                    -firstEllipse.majorAxisDirectionEllipse[0], 
                                    -firstEllipse.majorAxisDirectionEllipse[1],
                                    -firstEllipse.majorAxisDirectionEllipse[2]
                                };
                                double[] firstMinorAxisDirectionEllipseOpposite =
                                {
                                    -firstEllipse.minorAxisDirectionEllipse[0], 
                                    -firstEllipse.minorAxisDirectionEllipse[1],
                                    -firstEllipse.minorAxisDirectionEllipse[2]
                                };
                                if (FunctionsLC.MyEqualsArray(firstEllipse.majorAxisDirectionEllipse, secondEllipse.majorAxisDirectionEllipse) ||
                                    FunctionsLC.MyEqualsArray(firstMajorAxisDirectionEllipseOpposite, secondEllipse.majorAxisDirectionEllipse))
                                {
                                    if (
                                        FunctionsLC.MyEqualsArray(firstEllipse.minorAxisDirectionEllipse,
                                            secondEllipse.minorAxisDirectionEllipse) ||
                                        FunctionsLC.MyEqualsArray(firstMinorAxisDirectionEllipseOpposite,
                                            secondEllipse.minorAxisDirectionEllipse))
                                    {
                                        KLdebug.Print("Corrispondono i major minor axis direction? True", nameFile);
                                        listOfEllipseSecond.RemoveAt(j);
                                    }
                                    else
                                    {
                                        KLdebug.Print("Corrispondono i major minor axis direction? False", nameFile);
                                        thisEllipseIsOk = false;
                                    }
                                    
                                }
                                else
                                {
                                    KLdebug.Print("Corrispondono i major minor axis direction? False", nameFile);
                                    thisEllipseIsOk = false;
                                }
                            }
                            else
                            {
                                KLdebug.Print("Corrispondono i major minor radius? False", nameFile);
                                thisEllipseIsOk = false;
                            }
                        }
                        j++;
                    }
                    if (!thisEllipseIsOk)
                    {
                        KLdebug.Print("Non ho trovato nessun Ellipse che vada bene nel 2° cilindro. Fine.", nameFile);
                        return false;
                    }
                    else
                    {
                        numOfEllipseSecond = listOfEllipseSecond.Count;
                        i++;
                    }
                }
            }
            else
            {
                KLdebug.Print("Non ci sono Ellipse in questo cilindro della 1^ RE.", nameFile);
            }
            KLdebug.Print("VERIFICA DEGLI ELLIPSE é OK.", nameFile);
            KLdebug.Print(" ", nameFile);
            return true;
        }      

        // SBAGLIATO: VEDI FIGURA TIPO CARAMELLA TIC-TAC CON UNA DELLE DUE SUPERFICI SFERICHE AGLI CONCAVA ANZICHè CONVESSA
        public static bool CheckOfSpheresForTranslation(MyRepeatedEntity firstMyRepeatedEntity, MyRepeatedEntity secondMyRepeatedEntity, double[] candidateTranslationArray)
        {
            var tolerance = Math.Pow(10, -5);

            var numOfSphereSurfacesFirst = firstMyRepeatedEntity.listOfSphericalSurfaces.Count;
            var numOfSphereSurfacesSecond = secondMyRepeatedEntity.listOfSphericalSurfaces.Count;

            if (numOfSphereSurfacesFirst == numOfSphereSurfacesSecond)
            {
                if (numOfSphereSurfacesFirst > 0)
                {
                    var listOfSphereToDelete = new List<MySphere>(secondMyRepeatedEntity.listOfSphericalSurfaces);
                    int i = 0;
                    while (listOfSphereToDelete.Count != 0 && i < numOfSphereSurfacesFirst)
                        // DOVREBBERO ESSERE FALSE O VERE CONTEMPORANEAMENTE!!!
                    {
                        var sphereOfFirst = firstMyRepeatedEntity.listOfSphericalSurfaces[i];

                        if (sphereOfFirst.centerSphere != null)
                        {
                            double[] expectedPosition =
                            {
                                sphereOfFirst.centerSphere[0] + candidateTranslationArray[0],
                                sphereOfFirst.centerSphere[1] + candidateTranslationArray[1],
                                sphereOfFirst.centerSphere[2] + candidateTranslationArray[2]
                            };
                        var indexOfFound =
                            listOfSphereToDelete.FindIndex(
                                sphere => FunctionsLC.MyEqualsArray(sphere.centerSphere, expectedPosition));
                        if (indexOfFound != -1)
                        {
                            if (Math.Abs(listOfSphereToDelete[indexOfFound].radiusSphere -
                                         sphereOfFirst.radiusSphere) < tolerance)
                            {
                                listOfSphereToDelete.RemoveAt(indexOfFound);
                                i++;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }


        //(THE FOLLOWING FUNCTION IS CREATED FOR COMPOSED PATTERN SEARCH)
        //This function is the analogous of IsTranslationTwoRE, but in this situation the candidate
        //translational vector is given as input.
        public static bool IsTranslationTwoREGivenCandidateVector(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double[] candidateTranslationArray)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";

            whatToWrite = string.Format("Candidate translational array: ({0}, {1}, {2})", candidateTranslationArray[0],
                            candidateTranslationArray[1], candidateTranslationArray[2]);
            KLdebug.Print(whatToWrite, nameFile);
        
            var numberOfVerticesIsOk = true;
            var checkOfVertices = CheckOfVerticesForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity,
                candidateTranslationArray, ref numberOfVerticesIsOk);
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

            KLdebug.Print("   ANDATO A BUON FINE IL CHECK DEI VERTICI: PASSO AL CHECK DELLE FACCE.", nameFile);
            KLdebug.Print(" ", nameFile);

            //Check of correct position of normals of all Planar face:
            if (!CheckOfPlanesForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity))
            {
                return false;
            }

            ////Check of correct position of cylinder faces:
            if (!CheckOfCylindersForTranslation(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateTranslationArray))
            {
                return false;
            }

            //CONTINUARE CON GLI ALTRI TIPI DI SUPERFICI............
            KLdebug.Print("   ====>>> TRASLAZIONE TRA QUESTE DUE re VERIFICATA!", nameFile);
            return true;
        }
    }
}
