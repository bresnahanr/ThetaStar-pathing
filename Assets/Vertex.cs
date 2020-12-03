using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Data type that holds coordinates, cost, and connections
// to other vertices.
public class Vertex  {
	private Vector3 _position;
	private Vertex _parent;
	private float _cost;
	private List<Vertex> _neighbours;

	// Coordinate of vertex
	public Vector3 position
	{
		get { return _position; }
		set { _position = value; }
	}

	// Pointer to parent vertex
	public Vertex parent
	{
		get { return _parent; }
		set { _parent = value; }
	}

	// Associated travel cost
	public float cost
	{
		get { return _cost; }
		set { _cost = value; }
	}

	// List of adjacent, walkable vertices
	public List<Vertex> neighbours
	{
		get { return _neighbours; }
		set { _neighbours = value; }
	}

	// Creator of new vertex object
	public Vertex(Vector3 where)
	{
		position = where;
		parent = null;
		neighbours = new List<Vertex>();
		cost = 0;
	}
}
