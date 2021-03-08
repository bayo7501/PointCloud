using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Grapefruit {
    class XMLReader {

        private readonly string path = "";

        private List<Pnt> tinPnts = new List<Pnt>();
        private List<Face> tinFaces = new List<Face>();

        public XMLReader(string path) {
            this.path = path;
        }

        public void ReadXML(string element) {
            string retWord = string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlElement rootElement = xmlDoc.DocumentElement;
            // Surfaces
            XmlNodeList surfaces = rootElement.GetElementsByTagName(element);

            if (surfaces.Count < 1 | surfaces.Count > 1) {
                tinPnts = null;
                tinFaces = null;
                return;
            }

            //Console.WriteLine(surfaces[0].Name);

            // Surface
            XmlNodeList surface = surfaces[0].ChildNodes;

            if (surface.Count < 2 | surface.Count > 2) {
                tinPnts = null;
                tinFaces = null;
                return;
            }

            // [0]はアスファルトで敷き均した深さ
            // [1]が計画の深さに当たる

            //Console.WriteLine(surface[1].Name);

            // Definition
            XmlNodeList definition = surface[1].ChildNodes;
            if (definition.Count < 1 | definition.Count > 1) {
                tinPnts = null;
                tinFaces = null;
                return;
            }

            //Console.WriteLine(definition[0].Name);

            // Pnts Faces
            XmlNodeList xmlNodeList = definition[0].ChildNodes;
            if (xmlNodeList.Count < 2 | xmlNodeList.Count > 2) {
                tinPnts = null;
                tinFaces = null;
                return;
            }

            // Pnt
            XmlNode pnts = xmlNodeList[0];
            foreach (XmlNode p in pnts.ChildNodes) {
                //Console.WriteLine(p.Name);
                // P
                //Console.WriteLine(p.Attributes[0].InnerText);
                //Console.WriteLine(p.InnerText);
                Pnt pnt = new Pnt(
                    p.Attributes[0].InnerText,
                    double.Parse(p.InnerText.Split(' ')[0]),
                    double.Parse(p.InnerText.Split(' ')[1]),
                    double.Parse(p.InnerText.Split(' ')[2]));
                tinPnts.Add(pnt);
            }
            // faces
            XmlNode faces = xmlNodeList[1];
            foreach (XmlNode f in faces.ChildNodes) {
                //Console.WriteLine(f.Name);
                // F
                //Console.WriteLine(f.InnerText);

                string pntAID = f.InnerText.Split(' ')[0];
                string pntBID = f.InnerText.Split(' ')[1];
                string pntCID = f.InnerText.Split(' ')[2];
                Pnt a = tinPnts.Where(p => p.ID.Equals(pntAID)).Single();
                Pnt b = tinPnts.Where(p => p.ID.Equals(pntBID)).Single();
                Pnt c = tinPnts.Where(p => p.ID.Equals(pntCID)).Single();

                Face face = new Face(a, b, c);
                tinFaces.Add(face);
            }
        }

        public List<Pnt> Pnts {
            get => tinPnts;
        }

        public List<Face> Faces {
            get => tinFaces;
        }
    }
}
