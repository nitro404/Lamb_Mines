import java.util.Vector;
import java.io.*;

public class Graph {
	
	public Vector<Edge> edges;
	public Vector<Vertex> verticies;
	
	public Graph() {
		this.edges = new Vector<Edge>();
		this.verticies = new Vector<Vertex>();
	}
	
	public void addEdge(Edge e) {
		if(e != null && !this.edges.contains(e)) {
			this.edges.add(e);
			if(e.a != null && !this.verticies.contains(e.a)) {
				this.verticies.add(e.a);
			}
			if(e.b != null && !this.verticies.contains(e.b)) {
				this.verticies.add(e.b);
			}
		}
	}
	
	public boolean containsEdge(Edge e) {
		if(e == null) {
			return false;
		}
		
		for(int i=0;i<this.edges.size();i++) {
			if(this.edges.elementAt(i).equals(e)) {
				return true;
			}
		}
		return false;
	}
	
	public void writeTo(PrintWriter out) throws IOException {
		out.println("Edges: " + this.edges.size());
		for(int i=0;i<this.edges.size();i++) {
			out.print("\t");
			this.edges.elementAt(i).writeTo(out);
			out.println();
		}
	}
	
	public boolean equals(Object o) {
		if(o == null || !(o instanceof Graph)) {
			return false;
		}
		
		Graph g = (Graph) o;
		
		return this.edges.equals(g.edges);
	}
	
	public String toString() {
		String s = "[";
		for(int i=0;i<this.edges.size();i++) {
			s += this.edges.elementAt(i);
			if(i < this.edges.size() - 1) {
				s += ", ";
			}
		}
		s += "]";
		return s;
	}
}