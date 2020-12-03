// Scripts Vertex.cs, PriorityQueue.cs, Map.cs, and Controller.cs 
// are adapted from:
// https://github.com/syonkers/Theta-star-Pathfinding
// by Shaun Yonkers.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;

// Script initializes a new map object, instantiates objects in
// game space to visualize the data structure, and applies a pathing
// algorithm to determine the shortest path between a start and end
// point provided by the map.
public class Controller : MonoBehaviour {

	Map map = new Map();

	private PriorityQueue openList;
	private List<Vertex> closedList;

	public Transform cube;
	public Transform sphere;
	public Transform connector;
	private Transform clone;

	// Function is called at the beginning of execution,
	// creating the objects in game space, and applying
	// a pathing algorithm.
	private void Start()
	{
		DrawMap();
		FindPath();
	}

	// Reads the xml document from the Map class and instantiates
	// a 3d game object at each coordinate that is marked as non-walkable.
	private void DrawMap()
	{
		string filename = Application.dataPath + "/City.xml";
		map.ReadMap(filename, File.Exists(filename));
		int x = 0;
		int z = 0;

		foreach (string s in map.fullMap)
		{
			foreach (char c in s)
			{
				if (c == map.closed)
					clone = (Transform)Instantiate(cube, new Vector3(x, 0, z), Quaternion.identity);
				x++;
			}
			x = 0;
			z++;
		}
	}

	// Using a priority queue and theta* pathing algorithm the function
	// starts at the end vertex and traces backwards to find a shortest
	// path (utilizing linecast to determine if the parent of the current
	// parent is accessible through line of sight and jumping to that vertex
	// instead).
	private void FindPath()
	{
		Vertex current;
		openList = new PriorityQueue();
		closedList = new List<Vertex>();

		// Locate and include end vertex to openList because
		// our pathing algorithm is working backwards
		foreach(Vertex v in map.vertices)
		{
			if (v.position == map.end)
			{
				v.cost = 0;
				v.parent = v;
				openList.Enqueue(v);
			}
		}

		// While shortest path has not been found.
		while (!openList.isEmpty())
		{
			// Pop vertex with lowest cost in open list to closed list.
			current = openList.Dequeue();
			closedList.Add(current);

			// Call for path to be drawn if final vertex has been found.
			if (current.position == map.start)
				DrawPath(current);
			else
			{
				foreach(Vertex nbr in current.neighbours)
				{
					if(!closedList.Contains(nbr))
					{
						if (!openList.Contains(nbr))
						{
							nbr.cost = float.PositiveInfinity;
							nbr.parent = null;
						}
						UpdateVertex(current, nbr);
					}
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="current"></param>
	/// <param name="nbr"></param>
	private void UpdateVertex(Vertex current, Vertex nbr)
	{
		float oldCost = nbr.cost;
		ComputeCost(current, nbr);
		if(nbr.cost < oldCost)
		{
			if (openList.Contains(nbr))
				openList.Remove(nbr);
			openList.Enqueue(nbr);
		}
	}

	// Evaluates the travel cost associated with reaching the
	// passed in neighbour vertex.  
	private void ComputeCost(Vertex current, Vertex nbr)
	{
		float localCost = GetLocalCost(current, nbr);
		if (!Physics.Linecast(current.parent.position, nbr.position))
		{
			if((current.cost +localCost) < nbr.cost)
			{
				nbr.parent = current.parent;
				nbr.cost = current.cost + localCost;
			}
		}
		else
		{
			if ((current.cost + localCost) < nbr.cost)
			{
				nbr.parent = current;
				nbr.cost = current.cost + localCost;
			}
		}
	}

	// Given two vertices, return the scalar distance between
	// their coordinates.
	private float GetLocalCost(Vertex current, Vertex nbr)
	{
		return Vector3.Distance(current.position, nbr.position);
	}

	// Starting with the map start point, iterate through the walkable map
	// following parent pointers to draw the shortest path using sphere prefabs
	// for intersection points, and connector prefabs for travel lines.
	private void DrawPath(Vertex currentVertex)
	{
		// Iterate through the the walkable list parent to parent.
		while (currentVertex.position != map.end)
		{
			Vertex parentVertex = currentVertex.parent;
			float temp = Vector3.Distance(currentVertex.position, parentVertex.position) / 2;

			// Instantiate intersection point.
			clone = (Transform)Instantiate(sphere, currentVertex.position, Quaternion.identity);

			// If vertex is identified as beginning, color with blue.
			if (currentVertex.position == map.start)
				clone.GetComponent<MeshRenderer>().material.color = Color.blue;

			// Determine scale, position, and orientation for travel line instantiated object.
			clone = (Transform)Instantiate(connector, new Vector3(0, 0, 0), Quaternion.identity);
			clone.localScale = new Vector3(clone.localScale.x, temp, clone.localScale.z);
			clone.position = Vector3.Lerp(currentVertex.position, parentVertex.position, 0.5f);
			clone.transform.up = parentVertex.position - currentVertex.position;

			currentVertex = parentVertex;
		}
		// Identify and color the end point coordinate red.
		clone = (Transform)Instantiate(sphere, currentVertex.position, Quaternion.identity);
		clone.GetComponent<MeshRenderer>().material.color = Color.red;
	}
}
