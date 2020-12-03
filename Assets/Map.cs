using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

// Map creates an object used to read an xml datasheet and convert
// the information into a usable data structure of interconnected
// vertices and general information.
// The Map may be used in conjunction with a pathing algorithm to find
// a walkable path between two provided points.
public class Map {

	public int size;
	public char open;
	public char closed;

	public Vector3 start;
	public Vector3 end;

	public List<Vertex> vertices;
	public List<string> fullMap = new List<string>();

	// Interprets the provided file path to locate the desired
	// xml file and initializes an xml stream reader.
	public void ReadMap(string fName, bool isFile)
	{
		if (isFile)
		{
			using (XmlReader reader = XmlReader.Create(new StreamReader(fName)))
				ParseXML(reader);
		}
		else
		{
			TextAsset data = Resources.Load(fName) as TextAsset;
			using (XmlReader reader = XmlReader.Create(new StringReader(data.text)))
				ParseXML(reader);
		}
	}

	// Accepting the xml stream reader, the function iterates through the
	// xml document parsing the information into usable data structures
	// in the Map class.
	private void ParseXML(XmlReader reader)
	{
		while (reader.Read())
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					switch (reader.Name)
					{
						// Parses the general data associated with creating a city of game objects.
						case "City":
							size = (int)System.Convert.ToDouble(reader.GetAttribute("Size"));
							open = System.Convert.ToChar(reader.GetAttribute("Open"));
							closed = System.Convert.ToChar(reader.GetAttribute("Closed"));

							string temp = reader.GetAttribute("Start");
							start = ConvertToVector(temp);

							temp = reader.GetAttribute("End");
							end = ConvertToVector(temp);

							// Initializes a list of walkable vertices in the city.
							vertices = new List<Vertex>();
							break;

						// Parses a line of data that associates with walkable and non-walkable
						// game space coordinates.  
						case "Line":
							int lineNum = (int)System.Convert.ToDouble(reader.GetAttribute("Id"));
							int i = 0;
							string info = reader.ReadElementContentAsString();
							fullMap.Add(info);

							// Iterates through each line and determines whether the coordinate is a 
							// walkable type.
							foreach(char c in info)
							{
								if(c == open)
								{
									// If vertices is an open type, it is added to the walkable list.
									Vertex newVertex = new Vertex(new Vector3(i, 0, lineNum));
									vertices.Add(newVertex);
								}
								i++;
							}
							break;
					}
					break;
			}
		}
		// Find neighbouring vertices for each walkable vertex found.
		foreach(Vertex v in vertices)
		{
			GetNeighbours(v);
		}
	}
	
	// Function accepts a vertex and locates all neighbours +/- 1 spaces
	// along it's X and Z axis. 
	private void GetNeighbours(Vertex newVertex)
	{
		// Iterates the walkable vertex list and adds the current element as a neighbour
		// if its position is one unit away.
		foreach(Vertex v in vertices)
		{
			if(v.position == new Vector3(newVertex.position.x + 1, newVertex.position.y, newVertex.position.z))
				newVertex.neighbours.Add(v);
			if (v.position == new Vector3(newVertex.position.x - 1, newVertex.position.y, newVertex.position.z))
				newVertex.neighbours.Add(v);
			if (v.position == new Vector3(newVertex.position.x, newVertex.position.y, newVertex.position.z + 1))
				newVertex.neighbours.Add(v);
			if (v.position == new Vector3(newVertex.position.x, newVertex.position.y, newVertex.position.z - 1))
				newVertex.neighbours.Add(v);
		}
	}

	// Converts a string parameter to a Vector3 by parsing the coordinates
	// and applying the elements as floats along the X and Z axis.
	private Vector3 ConvertToVector(string s)
	{
		char[] delimiter = { ',' };
		string[] coordinates = s.Split(delimiter);
		float xVal = (float)System.Convert.ToDouble(coordinates[0]);
		float zVal = (float)System.Convert.ToDouble(coordinates[1]);
		Vector3 newVector = new Vector3(xVal, 0, zVal);
		return newVector;
	}
}
