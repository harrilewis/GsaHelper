using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Interop.gsa_8_7;

namespace GsaHelper

{    /// <summary>
     /// The jumping off point for automation. Lots of lovely methods. This is the comment for the object
     /// </summary>
    public class Controller
    {
        //the tolerance for finding nodes - used in a number of functions
        private double tolerance;
        private Interop.gsa_8_7.ComAuto gsaObject;

        /// <summary>
        /// A list of any error messages and suggestions returned by functions
        /// </summary>
        public List<string> messages = new List<string>();

        public Interop.gsa_8_7.ComAuto GsaObject {
            get {
                return gsaObject;
            }
        }
        
        /// <summary>
        /// Does this comment pop up in grasshopper this is the comment for the constructor
        /// </summary>
        /// <param name="tTolerance"></param>
        public Controller(double tTolerance)
        {
            tolerance = tTolerance;
            gsaObject = new Interop.gsa_8_7.ComAuto();
            if (gsaObject.NewFile() == 1)
            {
                return;
            }
        }

        //run gwa command directly from the helper.
        public bool Command(string gwaCommand)
        {
            gsaObject.GwaCommand(gwaCommand);
            return true;
        }
        

        #region step2Functions
        //create an element
        public bool CreateElement(List< Point3d> nodePositions, int elementNumber, out GsaElement element)
        {
            element = new GsaElement();
            element.NumTopo = nodePositions.Count();
            element.Ref = elementNumber;

            switch (element.NumTopo)
            {
                case 0:
                    //Print("element sent in with zero nodePositions - bad news!");
                    return false;
                case 1:
                    //Print("element sent in with 1 nodePosition - bad news!");
                    return false;
                case 2:
                    //looks like we have a beam!
                    element.eType = (int)ElementType.BEAM;
                    break;
                case 3:
                    //looks like we have a TRI3!
                    element.eType = (int)ElementType.TRI3;
                    break;
                case 4:
                    //looks like we have a QUAD!
                    element.eType = (int)ElementType.QUAD4;
                    break;
                default:
                    //Print("element sent in with an unrecognised number of node positions - bad news!");
                    return false;
            }

            List<int> topo = new List<int>();
            for (int i = 0; i < element.NumTopo; i++)
            {
                topo.Add(gsaObject.Gen_NodeAt(nodePositions[i].X, nodePositions[i].Y, nodePositions[i].Z, tolerance));
            }
            element.Topo = topo.ToArray();

            //these are defaults - best to change these after
            element.Property = 1;
            element.Group = 1;
            element.Color = (int) Colours.GREY;
            return true;
        }

        #region simplify set element
        //TODO: perhaps these can be set up to work with sections and nodes too?
        public bool SetElements(GsaElement singleElement, bool overwrite)
        {
            GsaElement[] arrayOfElements = new GsaElement[] { singleElement };
            return SetElements(arrayOfElements, overwrite);
        }

        public bool SetElements(IList<GsaElement> listOfElements, bool overwrite)
        {
            return SetElements(listOfElements.ToArray(), overwrite);
        }

        public bool SetElements(GsaElement[] arrayOfElements, bool overwrite)
        {
            int value = gsaObject.SetElements(arrayOfElements, overwrite);
            if (value == 0)
            {
                return true;
            } else
            {
                //no file open or invalid input
                return false;
            }
        }
        #endregion

        #endregion

        #region step3Functions

        //create a section
        public bool Create1dSection(string description, int sectionNumber, int material, out GsaSection section)
        {
            section = new GsaSection();
            section.SectDesc = description;
            section.Name = description;
            section.Material = material;
            section.Ref = sectionNumber;
            return true;
        }

        //create a 2d section
        public bool Create2dSection(double tThickness, int tSectionNumber, int tMaterial, out Gsa2dSection section)
        {
            section = new Gsa2dSection(tSectionNumber);
            section.thickness = tThickness;
            section.Material = tMaterial;
            return true;
        }

        //get elements of certain group
        public bool GetElementsOfGroup(string groupString, out GsaElement[] elementsOfGroup)
        {
            int[] elementsRefsOfGroup = new int[0];
            GsaEntity listType = GsaEntity.ELEMENT;
            gsaObject.EntitiesInList(groupString, ref listType, out elementsRefsOfGroup);
            elementsOfGroup = new GsaElement[0];
            gsaObject.Elements(elementsRefsOfGroup, out elementsOfGroup);
            return true;
        }

        //change all elements of a group to a certain property
        public bool SetGroupAsProperty(string groupString, int property)
        {
            GsaElement[] elementsOfGroup;
            if (GetElementsOfGroup(groupString, out elementsOfGroup))
            {
                for (int i = 0; i < elementsOfGroup.Count(); i++)
                {
                    elementsOfGroup[i].Property = property;
                }
                gsaObject.SetElements(elementsOfGroup, true);
                return true;
            }
            else {
                return false;
            }
        }
        #endregion

        #region step4Functions
        //create a node
        public bool CreateNode(Point3d position, int restraint, out GsaNode node)
        {
            node = new GsaNode();
            long nodeRef = gsaObject.Gen_NodeAt(position.X, position.Y, position.Z, tolerance);
            double[] pos = { (double)position.X, (double)position.Y, (double)position.Z };
            double[] stiff = { 0, 0, 0, 0, 0, 0 };
            node.Coor = pos;
            node.Stiffness = stiff;
            node.Restraint = restraint;
            node.Ref = (int)nodeRef;
            return true;
        }
        #endregion

        #region step7Functions
        public void TryAnalyse()
        {
            int status = gsaObject.Analyse(0);
            switch (status)
            {
                //TODO: return useful messages
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region step8Functions
        public int FetchNodeResults(string outputCase, Point3d nodePosition, Interop.gsa_8_7.ResHeader dataRef, out NodeResult nodeResults)
        {
            int nodeComp;
            Interop.gsa_8_7.GsaResults[] gsaNodeResults;
            long nodeRef = gsaObject.Gen_NodeAt(nodePosition.X, nodePosition.Y, nodePosition.Z, 0.1);
            int status = gsaObject.Output_Init_Arr(0, "default", outputCase, dataRef, 0);
            if (status == 0 && gsaObject.Output_DataExist((int)nodeRef) == 1)
            {
                gsaObject.Output_Extract_Arr((int)nodeRef, out gsaNodeResults, out nodeComp);
                //create the NodeResult struct
                nodeResults = new NodeResult(gsaNodeResults[0].dynaResults, gsaObject.Output_UnitString(), gsaObject.Output_DataTitle(1));
            }
            else {
                //need to initialise to complete the out input requirements
                nodeResults = new NodeResult();
            }
            return status;
        }



        public int Fetch1dResults(string outputCase, double elementRef, Interop.gsa_8_7.ResHeader dataRef, int numOneDPos, out OneDResult oneDResult)
        {
            int numResultComponents;
            Interop.gsa_8_7.GsaResults[] gsaOneDResults;
            int status = gsaObject.Output_Init_Arr(0, "default", outputCase, dataRef, numOneDPos);
            if (status == 0 && gsaObject.Output_DataExist((int)elementRef) == 1)
            {
                gsaObject.Output_Extract_Arr((int)elementRef, out gsaOneDResults, out numResultComponents);

                //create the staggered array of data
                int numDataPositions = gsaOneDResults.Count();
                double[][] values = new double[numDataPositions][];
                for (int i = 0; i < numDataPositions; i++)
                {
                    values[i] = gsaOneDResults[i].dynaResults;
                }
                oneDResult = new OneDResult(values, gsaObject.Output_UnitString(), gsaObject.Output_DataTitle(1));
            }
            else {
                double[][] results = new double[1][];
                oneDResult = new OneDResult(results, "results were not created properly", "results were not created properly");
            }
            return status;
        }
        #endregion

        #region littleHelpers
        //to convert a list of three rbg to a int
        public int ConvertRGBToGsaInt(int[] rgb)
        {
            return rgb[0] + (rgb[1] * 256) + (rgb[2] * 256 * 256);
        }


        #region listHelpers
        //There are many instances in GSA where there is a need to work with collections of nodes, elements, members or cases.
        //Lists are used where the user is required to specify a collection of entities. A list is expressed as a string of text in a specific syntax.

        public string CreateList(double recordNumber, string name, string listType, string listSyntax)
        {
            string command = "SET,LIST,";
            command += recordNumber;
            command += ",";
            command += name;
            command += ",";
            command += listType;
            command += ",";
            command += listSyntax;
            //create the list
            Command(command);
            //return the command in case we want to review
            return command;
        }

        //generate a list syntax for list of node numbers
        public string GenerateNodeList(List<int> nodeNumbers)
        {
            string list = "N";
            for (int i=0;i<nodeNumbers.Count;i++)
            {
                list += nodeNumbers[i].ToString();
                list += " ";
            }
            return list;
        }
        
        //generate a list syntax for list of nodes
        public string GenerateNodeList(List<GsaNode> nodes)
        {
            List<int> nodeNumbers = new List<int>();
            for (int i=0;i<nodes.Count;i++)
            {
                nodeNumbers.Add(nodes[i].Ref);
            }
            return GenerateNodeList(nodeNumbers);
        }

        //generate a list syntax for list of element numbers
        public string GenerateElementOrMemberList(List<int> elementNumbers)
        {
            string list = "E";
            for (int i = 0; i < elementNumbers.Count; i++)
            {
                list += elementNumbers[i].ToString();
                list += " ";
            }
            return list;
        }

        //generate a list syntax for list of elements
        public string GenerateElementOrMemberList(List<GsaElement> elements)
        {
            List<int> elementNumbers = new List<int>();
            for (int i = 0; i < elements.Count; i++)
            {
                elementNumbers.Add(elements[i].Ref);
            }
            return GenerateElementOrMemberList(elementNumbers);
        }

        //generate a list syntax for list of cases
        public string GenerateCaseList(List<int> caseNumbers)
        {
            string list = "L";
            for (int i = 0; i < caseNumbers.Count; i++)
            {
                list += caseNumbers[i].ToString();
                list += " ";
            }
            return list;
        }

        #endregion
        #endregion
    }
}