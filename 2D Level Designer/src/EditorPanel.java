import java.awt.*;
import java.awt.event.*;

import javax.swing.*;

public class EditorPanel extends JPanel implements Scrollable, MouseListener, MouseMotionListener {
	
	private static final long serialVersionUID = 1L;
	
	private World world;
	
	public EditorPanel() {
		world = null;
		setLayout(null);
		addMouseListener(this);
		addMouseMotionListener(this);
	}
	
	public void setWorld(World world) {
		this.world = world;
	}
	
	public Dimension getPreferredSize() {
//TODO: Get map size
return new Dimension(1920, 1200);
	}
	
	public Dimension getPreferredScrollableViewportSize() {
		return getPreferredSize();
	}

	public int getScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction) {
		int currentPosition = 0;
		if(orientation == SwingConstants.HORIZONTAL) {
			currentPosition = visibleRect.x;
		}
		else {
			currentPosition = visibleRect.y;
		}
        
		int maxUnitIncrement = 1;
		if(direction < 0) {
			int newPosition = currentPosition -
							  (currentPosition / maxUnitIncrement)
                              * maxUnitIncrement;
            return (newPosition == 0) ? maxUnitIncrement : newPosition;
        }
		else {
            return ((currentPosition / maxUnitIncrement) + 1)
                   * maxUnitIncrement
                   - currentPosition;
        }
	}
	
	public int getScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction) {
		if(orientation == SwingConstants.HORIZONTAL) {
			return visibleRect.width - World.ISOMETRIC_GRID_WIDTH;
		}
		else {
			return visibleRect.height - World.ISOMETRIC_GRID_HEIGHT;
		}
	}
	
	public boolean getScrollableTracksViewportHeight() {
		return false;
	}
	
	public void mouseClicked(MouseEvent e) {
System.out.println("Mouse Clicked " + e.getX() + ", " + e.getY());
	}
	
	public void mouseEntered(MouseEvent e) {
System.out.println("Mouse Entered " + e.getX() + ", " + e.getY());
	}
	
	public void mouseExited(MouseEvent e) {
System.out.println("Mouse Exited " + e.getX() + ", " + e.getY());
	}
	
	public void mousePressed(MouseEvent e) {
System.out.println("Mouse Pressed " + e.getX() + ", " + e.getY());
	}
	
	public void mouseReleased(MouseEvent e) {
System.out.println("Mouse Released " + e.getX() + ", " + e.getY());
	}
	
	//motion
	public void mouseDragged(MouseEvent e) {
//System.out.println("Mouse Dragged " + e.getX() + ", " + e.getY());
	}
	
	//motion
	public void mouseMoved(MouseEvent e) {
//System.out.println("Mouse Moved " + e.getX() + ", " + e.getY());
	}
	
	public boolean getScrollableTracksViewportWidth() {
		return false;
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
		int x = World.ISOMETRIC_GRID_WIDTH;
		int y = World.ISOMETRIC_GRID_HEIGHT;
		for(int i=0;i<getWidth()+x;i+=x) {
			for(int j=0;j<getHeight()+y;j+=y) {
				g.drawLine(i - x, j - y, i, j);
				g.drawLine(i - x, j, i, j - y);
			}
		}
	}

	public void update() {
		this.repaint();
	}
	
}
