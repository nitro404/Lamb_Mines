import java.awt.*;
//import java.awt.event.*;
import javax.swing.*;

public class EditorPanel extends JPanel {
	
	private static final long serialVersionUID = 1L;
	
	private World world;
	
	public EditorPanel() {
		world = null;
		System.out.println(World.ISOMETRIC_GRID_ANGLE);
//		this.setSize(1920, 1200);
	}
	
	public void setWorld(World world) {
		this.world = world;
	}
	
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		
		if(world != null) {
			g.clearRect(0, 0, this.getWidth(), this.getHeight());
			world.paintOn(g);
		}
		
		drawIsometricGrid(g);
	}
	
	public void drawIsometricGrid(Graphics g) {
		g.setColor(new Color(0, 0, 0));
		for(int i=0;i<getWidth();i+=World.ISOMETRIC_GRID_WIDTH) {
			for(int j=0;j<getHeight();j+=World.ISOMETRIC_GRID_HEIGHT) {
				for(int k=getWidth();k>=0;k-=World.ISOMETRIC_GRID_WIDTH) {
					for(int l=getHeight();k>=0;k-=World.ISOMETRIC_GRID_HEIGHT) {
						g.drawLine(i, j, k, l);
					}
				}
			}
		}
	}
	
	public void update() {
		this.repaint();
	}
	
}
