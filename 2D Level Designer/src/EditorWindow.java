import java.awt.*;
import javax.swing.*;

public class EditorWindow extends JFrame {
	
	private static final long serialVersionUID = 1L;
	
	public World world;
	
	public EditorWindow() {
		super("2D World Editor");
		world = World.parseFrom("level1.2d");
		world.writeTo("level1b.2d");
		setSize(640, 480);
	}
	
	public void paint(Graphics g) {
		g.clearRect(0, 0, this.getWidth() - 1, this.getHeight() - 1);
		world.paintOn(g);
	}
	
	
}
