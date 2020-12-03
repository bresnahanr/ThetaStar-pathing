using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A priority queue is a data type that functions like
// a queue but with priority value associated to each
// element.  The elements with high priority are served
// before those with low priority.
public class PriorityQueue {

	List<Vertex> heap;

	// Creates new priority queue list.
	public PriorityQueue()
	{
		heap = new List<Vertex>();
	}

	// Appends a vertex object to the end of the priority
	// queue, then iterates through the heap to organize 
	// vertices by cost.
	public void Enqueue (Vertex newVertex)
	{
		heap.Add(newVertex);
		int i = heap.Count - 1;
		while (i != 0)
		{
			int p = (i - 1) / 2;
			if (heap[p].cost > heap[i].cost)
			{
				Swap(i, p);
				i = p;
			}
			else
				return;
		}
	}

	// Returns a vertex from the priority queue, removing
	//it then iterates through the heap to reorganize vertices
	// according to cost.
	public Vertex Dequeue()
	{
		int i = heap.Count - 1;
		Vertex first = heap[0];
		heap[0] = heap[i];
		heap.RemoveAt(i);
		--i;
		int p = 0;

		while (true)
		{
			int left = p * 2 + 1;
			if (left > i)
				break;
			int right = left + 1;
			if ((right < p) && (heap[right].cost < heap[left].cost))
				left = right;
			if (heap[p].cost <= heap[left].cost)
				break;
			Swap(p, left);
			p = left;
		}
		return first;
	}

	// Returns true if vertex is contained within the heap.
	// Returns false if not.
	public bool Contains(Vertex v)
	{
		return heap.Contains(v);
	}

	// Returns true if number of vertices in heap is zero.
	// Returns false if not.
	public bool isEmpty()
	{
		return heap.Count == 0;
	}

	// Removes the parameter vertex from the heap.
	public void Remove(Vertex v)
	{
		heap.Remove(v);
	}

	// Swaps the position of two vertices on the heap at
	// the indexes a and b.
	private void Swap(int a, int b)
	{
		Vertex temp = heap[a];
		heap[a] = heap[b];
		heap[b] = temp;
	}
}

