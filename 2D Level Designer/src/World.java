import java.util.Vector;
import java.awt.*;
import java.io.*;

public class World {
	
	public Vector<Graph> barriers;
	public Vector<Entity> entities;
	Vector<String> textureNames;
// set boundaries
// triggers?
// animated textures
	
	final public static String WORLD_TYPE = "2D World";
	final public static double WORLD_VERSION = 1.1;
	
	final public static int GRID_WIDTH = 64;
	final public static int GRID_HEIGHT = 64;
	
	final public static int ISOMETRIC_GRID_WIDTH = World.getIsometricWidth(GRID_WIDTH, GRID_HEIGHT);
	final public static int ISOMETRIC_GRID_HEIGHT = World.getIsometricHeight(GRID_WIDTH, GRID_HEIGHT);
	final public static double ISOMETRIC_GRID_ANGLE = Math.atan(1.0 / 2.0) * (180.0 / Math.PI);
	final public static int ISOMETRIC_GRID_INCREMENT = 48;
	
	public World() {
		this.barriers = new Vector<Graph>();
		this.entities = new Vector<Entity>();
		this.textureNames = new Vector<String>();
	}
	
	public void addBarrier(Graph g) {
		if(!this.barriers.contains(g)) {
			this.barriers.add(g);
		}
	}
	
	public boolean containsBarrier(Graph g) {
		return this.barriers.contains(g);
	}
	
	public static int getIsometricWidth(int width, int height) {
		return (int) Math.sqrt(Math.pow(width, 2) + Math.pow(height, 2));
	}
	
	public static int getIsometricHeight(int width, int height) {
		return (int) (getIsometricWidth(width, height) / 2);
	}
	
	public static int getIsometricX(int x, int y) {
//		xCart = (x-z)*Math.cos(0.46365);
//		xI = xCart+xOrigin;
//		return (xI);
return -1;
	}
	
	public static int getIsometricY(int x, int y) {
//		yCart = y+(x+z)*Math.sin(0.46365);
//		yI = -yCart+yOrigin;
//		return (yI);
return -1;
	}
	
	public static int getCartesianX(int x, int y) {
return -1;
	}
	
	public static int getCartesianY(int x, int y) {
return -1;
	}
	
	/*
	// --- Functions ---
	// Convert isometric coordinates to Flash X coordinate
	xFla = function (x, y, z) {
		return ((x-z)*isoCos);
	};
	// Convert isometric coordinates to Flash Y coordinate
	yFla = function (x, y, z) {
		return (-(y+(x+z)*isoSin));
	};
	// Convert Flash X and Y and altitude coordinates to isometric X coordinate
	xIso = function (xF, yF, y) {
		return ((xF)/isoCos-(yF+y)/isoSin)/2;
	};
	// Convert Flash X and Y and altitude coordinates to isometric Y coordinate (actually Z, but I don't give a hoot)
	yIso = function (xF, yF, y) {
		return (-((xF)/isoCos+(yF+y)/isoSin))/2;
	};

	// --- Variables ---
	// isoCos and isoSin are defined in variables so they don't have to be calculated every time the functions are executed
	isoCos = Math.cos(0.46365);
	isoSin = Math.sin(0.46365);

	// --- Example Use ---
	// This example shows mostly why I wanted the new functions

	isoWorld_mc.onMouseUp = function() {

		// Let's put _xmouse and _ymouse into isometric coordinates
		isoMouseX = xIso(_xmouse, _ymouse, 0);
		isoMouseY = yIso(_xmouse, _ymouse, 0);

		// Let's round it to the nearest 100
		isoMouseSnapX = Math.round(isoMouseX/100)*100;
		isoMouseSnapY = Math.round(isoMousey/100)*100;

		// Let's convert them back to Flash coordinates
		flaMouseX = xFla(isoMouseSnapX, 0, isoMouseSnapY);
		flaMouseY = yFla(isoMouseSnapX, 0, isoMouseSnapY);

		// Now, let's change the position of this sucker
		this._x -= flaMouseX;
		this._y -= flaMouseY;

	};
	*/
	
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
		double worldVersion = Double.valueOf(input.substring(input.lastIndexOf(' ', input.length() - 1), input.length()).trim());
		if(WORLD_VERSION != worldVersion) {
			System.out.println("ERROR: Incompatible map version (" + worldVersion + "). Current editor only supports version " + WORLD_VERSION + ".");
			return null;
		}
		
		// read in the map dimensions
		input = in.readLine();
		String dimensionsHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!dimensionsHeader.equals("Dimensions")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Dimensions\", found \"" + dimensionsHeader + "\".");
			return null;
		}
		int mapWidth = Integer.valueOf(input.substring(input.indexOf(':', 0) + 1, input.indexOf(',', 0)).trim());
		int mapHeight = Integer.valueOf(input.substring(input.indexOf(',', 0) + 1, input.length()).trim());
		
		// read in the texture names
		input = in.readLine();
		String texturesHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!texturesHeader.equals("Textures")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Textures\", found \"" + texturesHeader + "\".");
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
		
		// read in the barriers
		input = in.readLine();
		String barriersHeader = input.substring(0, input.indexOf(':', 0)).trim();
		if(!barriersHeader.equals("Barriers")) {
			System.out.println("ERROR: Corrupted world file. Expected header \"Edges\", found \"" + barriersHeader + "\".");
			return null;
		}
		int numberOfBarriers = Integer.valueOf(input.substring(input.lastIndexOf(':', input.length() - 1) + 1, input.length()).trim());
		
		// read in the corresponding edges for each barrier
		for(int i=0;i<numberOfBarriers;i++) {
			input = in.readLine();
			String edgesHeader = input.substring(0, input.indexOf(':', 0)).trim();
			if(!edgesHeader.equals("Edges")) {
				System.out.println("ERROR: Corrupted world file. Expected header \"Edges\", found \"" + edgesHeader + "\".");
				return null;
			}
			int numberOfEdges = Integer.valueOf(input.substring(input.lastIndexOf(':', input.length() - 1) + 1, input.length()).trim());
			for(int j=0;j<numberOfEdges;j++) {
				input = in.readLine().trim();
				world.addEdge(Edge.parseFrom(input));
			}
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
