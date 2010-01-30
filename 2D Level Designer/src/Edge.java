import java.io.*;
import java.awt.*;

public class Edge {
	
	public Vertex a, b;
	
	public Edge(Vertex a, Vertex b) {
		this.a = a;
		this.b = b;
	}
	
	public Vertex getPointA() {
		return this.a;
	}
	
	public Vertex getPointB() {
		return this.b;
	}
	
	public boolean containsVertex(Vertex p) {
		return this.a == p || this.b == p;
	}
	
	public double getDeltaX() {
		return this.b.x - this.a.x;
	}

	public double getDeltaY() {
		return this.b.x - this.a.x;
	}
	
	public static Edge parseFrom(String input) {
		if(input == null || input.trim().length() == 0) {
			return null;
		}
		
		String data = input.trim();
		
		Vertex a = Vertex.parseFrom((data.substring(data.indexOf('(', 0), data.indexOf(')', 0) + 1)));
		Vertex b = Vertex.parseFrom(data.substring(data.lastIndexOf('(', data.length() - 1), data.lastIndexOf(')', data.length() - 1) + 1));
		
		return new Edge(a, b);
	}
	
	public void writeTo(PrintWriter out) throws IOException {
		a.writeTo(out);
		out.print(" ");
		b.writeTo(out);
	}
	
	public void paintOn(Graphics g) {
		g.drawLine((int)a.x, (int)a.y, (int)b.x, (int)b.y);
	}
	
	public boolean equals(Object o) {
		if(o == null || !(o instanceof Edge)) {
			return false;
		}
		
		Edge e = (Edge) o;
		
		return this.a == e.a && this.b == e.b;
	}
	
	public String toString() {
		return "(" + this.a.x + ", " + this.a.y + ") (" + this.b.x + ", " + this.b.y + ")";
	}
	
}