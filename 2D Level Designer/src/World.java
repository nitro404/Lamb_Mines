import java.util.Vector;
import java.awt.*;
import java.io.*;

public class World {
	
	public Graph map;
	public Vector<Entity> entities;
	Vector<String> textureNames;
// set boundaries
// triggers?
// animated textures
	
	final public static String WORLD_TYPE = "2D World";
	final public static double WORLD_VERSION = 1.0;
	
	public World() {
		this.map = new Graph();
		this.entities = new Vector<Entity>();
		this.textureNames = new Vector<String>();
	}
	
	public void addEdge(Edge e) {
		this.map.addEdge(e);
	}
	
	public boolean containsEdge(Edge e) {
		return this.map.containsEdge(e);
	}
	
	public static World parseFrom(String fileName) {
		if(fileName == null || fileName.trim().length() == 0) {
			return null;
		}
		try {
			BufferedReader in = new BufferedReader(new FileReader(fileName.trim()));
			World world = parseFrom(in);
			if(in != null) {
				in.close();
			}
			return world;
		}
		catch(FileNotFoundException e) {
			System.out.println("ERROR: Unable to open file: \"" + fileName + "\"");
			e.printStackTrace();
		}
		catch(IOException e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		}
		catch(Exception e) {
			System.out.println("ERROR: Corrupted map file.");
			e.printStackTrace();
		}
		return null;
	}
	
	public static World parseFrom(BufferedReader in) throws IOException {
		if(in == null) {
			return null;
		}
		
		String input;
		World world = new World();
		
		// input the world header
		input = in.readLine();
		String worldType = input.substring(0, input.indexOf(':', 0)).trim();
		if(!WORLD_TYPE.equals(worldType)) {
			System.out.println("ERROR: Incompatible world type (" + worldType + "). Current editor only supports worlds of type " + WORLD_TYPE + ".");
			return null;
		}
		double worldVersion = Double.valueOf(input.substring(input.lastIndexOf(' ', input.length() - 1), input.length() - 1).trim());
		if(WORLD_VERSION != worldVersion) {
			System.out.println("ERROR: Incompatible map version (" + worldVersion + "). Current editor only supports version " + WORLD_VERSION + ".");
			return null;
		}
		
		// read in the texture names
		input = in.readLine();
		String textureHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!textureHeader.equals("Textures")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Textures\", found \"" + textureHeader + "\".");
			return null;
		}
		int numberOfTextures = Integer.valueOf(input.substring(input.lastIndexOf(':', input.length() - 1) + 1, input.length()).trim());
		for(int i=0;i<numberOfTextures;i++) {
			input = in.readLine().trim();
			if(input != null && input.length() != 0) {
				if(!world.textureNames.contains(input)) {
					world.textureNames.add(input);
				}
			}
		}
		
		// read in the edges
		input = in.readLine();
		String edgesHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!edgesHeader.equals("Edges")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Edges\", found \"" + edgesHeader + "\".");
			return null;
		}
		int numberOfEdges = Integer.valueOf(input.substring(input.lastIndexOf(':', input.length() - 1) + 1, input.length()).trim());
		for(int i=0;i<numberOfEdges;i++) {
			input = in.readLine().trim();
			world.addEdge(Edge.parseFrom(input));
		}
		
		// read in the entities
		input = in.readLine();
		String entitiesHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!entitiesHeader.equals("Entities")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Entities\", found \"" + entitiesHeader + "\".");
			return null;
		}
		int numberOfEntities = Integer.valueOf(input.substring(input.lastIndexOf(':', input.length() - 1) + 1, input.length()).trim());
		Entity newEntity;
		for(int i=0;i<numberOfEntities;i++) {
			input = in.readLine().trim();
//			newEntity = Entity.parseFrom(input);
//			if(!world.entities.contains(newEntity)) {
//				world.entities.add(newEntity);
//			}
		}
		
		return world;
	}
	
	public void writeTo(String fileName) {
		if(fileName == null || fileName.trim().length() == 0) {
			return;
		}
		try {
			PrintWriter out = new PrintWriter(new FileWriter(fileName));
			this.writeTo(out);
			if(out != null) {
				out.close();
			}
		}
		catch(FileNotFoundException e) {
			System.out.println("ERROR: Unable to open file: \"" + fileName + "\"");
			e.printStackTrace();
		}
		catch(IOException e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		}
		catch(Exception e) {
			System.out.println("ERROR: Corrupted map file.");
			e.printStackTrace();
		}
	}
	
	public void writeTo(PrintWriter out) throws IOException {
		// write the world header and version
		out.println(WORLD_TYPE + ": Version " + WORLD_VERSION);
		
		// write the textures
		out.println("Textures: " + this.textureNames.size());
		for(int i=0;i<this.textureNames.size();i++) {
			out.println("\t" + this.textureNames.elementAt(i));
		}
		
		// write the map
		this.map.writeTo(out);
		
		// write the entities
		out.println("Entities: " + this.entities.size());
		for(int i=0;i<this.entities.size();i++) {
//			out.println("\t" + this.entities.elementAt(i));
		}
	}

	public void paintOn(Graphics g) {
		g.setColor(new Color(0, 0, 0));
		for(int i=0;i<this.map.edges.size();i++) {
			this.map.edges.elementAt(i).paintOn(g);
		}
		int[] x = new int[this.map.verticies.size()];
		int[] y = new int[this.map.verticies.size()];
		for(int i=0;i<this.map.verticies.size();i++) {
			x[i] = (int) this.map.verticies.elementAt(i).x;
			y[i] = (int) this.map.verticies.elementAt(i).y;
		}
		Polygon p = new Polygon(x, y, this.map.verticies.size());
		g.fillPolygon(p);
	}
	
	public boolean equals(Object o) {
		if(o == null || !(o instanceof World)) {
			return false;
		}
		
		World w = (World) o;
		
		if(this.entities.size() != w.entities.size()) {
			return false;
		}
		for(int i=0;i<this.entities.size();i++) {
			if(!w.entities.contains(this.entities.elementAt(i))) {
				return false;
			}
		}
		
		return this.map.equals(w.map);
	}
	
	public String toString() {
		return "World";
	}
	
}