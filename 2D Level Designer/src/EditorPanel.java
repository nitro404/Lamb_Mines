import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

import java.io.*;
import java.awt.image.BufferedImage;
import javax.imageio.ImageIO;

public class EditorPanel extends JPanel implements Scrollable, ActionListener, MouseListener, MouseMotionListener {
	
	private static final long serialVersionUID = 1L;
	
	private World world;
	
	private JPopupMenu popupMenu;
	private JMenuItem popupMenuNewVertex;
	private JMenuItem popupMenuDeleteVertex;
	private JMenuItem popupMenuCancel;
	
	public EditorPanel() {
		world = null;
		setLayout(null);
		addMouseListener(this);
		addMouseMotionListener(this);
		
		createPopupMenu();
		
		testPNG();
	}
	
	private BufferedImage image;
	
	public void testPNG() {
		try {
			File imageFile = new File(EditorWindow.SPRITE_DIRECTORY + "\\grass_base01.png");
			image = ImageIO.read(imageFile);
		}
		catch(Exception e) { }
	}
	
	public void drawPNG(Graphics g) {
		Graphics2D g2 = (Graphics2D) g;
		g2.drawImage(image, null, 0, 20);
	}
	
	public void createPopupMenu() {
		popupMenu = new JPopupMenu();
		popupMenuNewVertex = new JMenuItem("Create Vertex");
		popupMenuDeleteVertex = new JMenuItem("Delete Vertex");
		popupMenuCancel = new JMenuItem("Cancel");
		
		popupMenu.add(popupMenuNewVertex);
		popupMenu.add(popupMenuDeleteVertex);
		popupMenu.addSeparator();
		popupMenu.add(popupMenuCancel);
	}
	
	public void setWorld(World world) {
		this.world = world;
	}
	
	public Dimension getPreferredSize() {
		if(world != null) {
			return world.dimensions;
		}
		else {
			return new Dimension(16 * World.ISOMETRIC_GRID_WIDTH, 16 * World.ISOMETRIC_GRID_HEIGHT);
		}
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
        
		int maxUnitIncrement = 7;
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

	public boolean getScrollableTracksViewportWidth() {
		return false;
	}
	
	public void mouseClicked(MouseEvent e) { }
	
	public void mouseEntered(MouseEvent e) { }
	public void mouseExited(MouseEvent e) { }
	public void mousePressed(MouseEvent e) {
		
	}
	public void mouseReleased(MouseEvent e) {
		if(e.getButton() == MouseEvent.BUTTON3) {
			popupMenu.show(this, e.getX(), e.getY());
		}
		else if(e.getButton() == MouseEvent.BUTTON1) {
			// find closest object, set it as selected
			//double click to select polygon
			//ensure verticies to NOT intersect
			//System.out.println("Mouse Clicked " + e.getX() + ", " + e.getY());
			
		}
	}
	public void mouseDragged(MouseEvent e) { }
	public void mouseMoved(MouseEvent e) { }
	
	public void actionPerformed(ActionEvent e) {
		
	}
	
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		
		if(world != null) {
			g.clearRect(0, 0, this.getWidth(), this.getHeight());
			world.paintOn(g);
		}
		
		drawPNG(g);
		
		drawIsometricGrid(g);
	}
	
	public void drawIsometricGrid(Graphics g) {
		if(world != null) {
			g.setColor(new Color(0, 0, 0));
			Point topLeft, topRight, bottomRight, bottomLeft;
			
			int w = World.CARTESIAN_GRID_INCREMENT;
			int offset = world.dimensions.width / 2;
			for(int i=0;i<world.gridSize.x;i++) {
				for(int j=0;j<world.gridSize.y;j++) {
					topLeft =     World.getIsometricPoint(new Point( i*w,     j*w));
					topRight =    World.getIsometricPoint(new Point((i*w)+w,  j*w));
					bottomRight = World.getIsometricPoint(new Point((i*w)+w, (j*w)+w));
					bottomLeft =  World.getIsometricPoint(new Point( i*w,    (j*w)+w));
					
					g.drawLine(offset + topLeft.x,     topLeft.y,     offset + topRight.x,    topRight.y);
					g.drawLine(offset + topRight.x,    topRight.y,    offset + bottomRight.x, bottomRight.y);
					g.drawLine(offset + bottomRight.x, bottomRight.y, offset + bottomLeft.x,  bottomLeft.y);
					g.drawLine(offset + bottomLeft.x,  bottomLeft.y,  offset + topLeft.x,     topLeft.y);
				}
			}
		}
	}
	
	public void update() {
		this.repaint();
	}
}
