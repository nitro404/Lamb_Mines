import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

import java.io.*;
import java.util.Vector;
import java.awt.image.BufferedImage;
import javax.imageio.ImageIO;

public class EditorPanel extends JPanel implements Scrollable, ActionListener, MouseListener, MouseMotionListener {
	
	private static final long serialVersionUID = 1L;
	
	private World world;
	
	private Vertex selectedPoint;
	private Vertex selectedVertex;
	
	private JPopupMenu popupMenu;
	private JMenuItem popupMenuNewVertex;
	private JMenuItem popupMenuDeleteVertex;
	private JMenuItem popupMenuCancel;
	
	public static Point gridTop;
	public static Point gridRight;
	public static Point gridBottom;
	public static Point gridLeft;
	
	public EditorPanel() {
		world = null;
		setLayout(null);
		addMouseListener(this);
		addMouseMotionListener(this);
		
		createPopupMenu();
		
		testPNG();
		
		update();
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
		
		popupMenuNewVertex.addActionListener(this);
		popupMenuDeleteVertex.addActionListener(this);
		
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
			selectedPoint = new Vertex(World.removeOffsetFrom(World.getCartesianPoint(e.getPoint())));
			selectVertex();
			popupMenuDeleteVertex.setEnabled(selectedVertex != null);
			popupMenu.show(this, e.getX(), e.getY());
		}
		else if(e.getButton() == MouseEvent.BUTTON1) {
			Vertex previousVertex = null;
			if(selectedVertex != null) {
				previousVertex = selectedVertex; 
			}
			selectedPoint = new Vertex(World.removeOffsetFrom(World.getCartesianPoint(e.getPoint())));
			selectVertex();
			
			if(previousVertex != null && selectedVertex != null && !previousVertex.equals(selectedVertex)) {
				int result = JOptionPane.showConfirmDialog(this, "Create edge?", "Edge Creation", JOptionPane.YES_NO_OPTION);
				if(result == JOptionPane.YES_OPTION) {
					this.world.addEdge(new Edge(new Vertex(previousVertex.x, previousVertex.y),
												new Vertex(selectedVertex.x, selectedVertex.y)));
				}
			}
		}
		this.update();
	}
	public void mouseDragged(MouseEvent e) { }
	public void mouseMoved(MouseEvent e) {
System.out.println((e.getX() - + World.HORIZONTAL_OFFSET) + ", " + e.getY());
	}
	
	public void actionPerformed(ActionEvent e) {
		if(world != null) {
			if(e.getSource().equals(popupMenuNewVertex)) {
				world.addVertex(new Vertex(selectedPoint.x, selectedPoint.y));
				selectedVertex = null;
			}
			else if(e.getSource().equals(popupMenuDeleteVertex)) {
				world.removeVertex(selectedVertex);
				selectedVertex = null;
			}
		}
		this.update();
	}
	
	public void selectVertex() {
		double maxRadius = 6;
		selectedVertex = null;
		if(world != null) {
			for(int i=0;i<world.edges.verticies.size();i++) {
				Vertex v = world.edges.verticies.elementAt(i);
				if(Math.sqrt( Math.pow( (selectedPoint.x - v.x) , 2) + Math.pow( (selectedPoint.y - v.y) , 2) ) <= maxRadius) {
					selectedVertex = v;
				}
			}
		}
		update();
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
			g.setColor(new Color(64, 64, 64));
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
					
					if(i == 0 && j == 0) {
						gridTop = new Point(offset + topLeft.x, topLeft.y);
					}
					else if(i == 0 && j == world.gridSize.y - 1) {
						gridLeft = new Point(offset + bottomLeft.x, bottomLeft.y);
					}
					else if(i == world.gridSize.x - 1 && j == 0) {
						gridRight = new Point(offset + topRight.x, topRight.y);
					}
					else if(i == world.gridSize.x - 1 && j == world.gridSize.y - 1) {
						gridBottom = new Point(offset + bottomRight.x, bottomRight.y);
						
					}
				}
			}
		}
		
		g.setColor(new Color(255, 0, 0));
		if(selectedVertex != null) { selectedVertex.paintOn(g); }
	}
	
	public void update() {
		this.repaint();
	}
}
