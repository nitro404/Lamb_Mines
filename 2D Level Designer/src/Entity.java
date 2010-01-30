public class Entity {

	public Vertex location;
//	public Texture texture;
	
	public Entity(Vertex location) {
		this.location = location;
	}
	
	public boolean equals(Object o) {
		if(o == null || !(o instanceof Entity)) {
			return false;
		}
		
		Entity e = (Entity) o;
		
		return this.location.equals(e.location);
	}
	
	public String toString() {
		return this.location.toString();
	}
	
}
