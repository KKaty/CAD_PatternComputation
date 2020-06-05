using System.Collections.Generic;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a group on MyRepeatedEntity with a symmetric arrangement
    public class MyPattern
    {
        public int idMyPattern = -1;
        public List<MyRepeatedEntity> listOfMyREOfMyPattern = new List<MyRepeatedEntity>();
        public MyPathGeometricObject pathOfMyPattern;
        public string typeOfMyPattern;   // translation, reflection, rotation
        public double constStepOfMyPattern;
        public double angle = -1; //it is modified only in case of circular patterns
        public MyVertex patternCentroid = new MyVertex();

        public MyPattern()
        {
        }

        public MyPattern(List<MyRepeatedEntity> ListOfMyREOfMyPattern, MyPathGeometricObject PathOfMyPattern, 
            string TypeOfMyPattern)
        {
            this.listOfMyREOfMyPattern = ListOfMyREOfMyPattern;
            this.pathOfMyPattern = PathOfMyPattern;
            this.typeOfMyPattern = TypeOfMyPattern;
            this.constStepOfMyPattern = ListOfMyREOfMyPattern[0].centroid.Distance(ListOfMyREOfMyPattern[1].centroid);
        }
    }
}
