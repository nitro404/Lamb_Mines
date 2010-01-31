import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

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
	
	public void testPNG() {
	      File infile = new File("image.png");
	      BufferedImage im =
	      ImageIO.read(infile);

	      File outfile = new File("image.jpg");
	      ImageIO.write(im, "jpg", outfile);

	      String[] reader_names =
	      ImageIO.getReaderFormatNames();

	      String[] writer_names =
	      ImageIO.getWriterFormatNames();

	      Iterator iter =
	      getImageReadersBySuffix("png");
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
			//world.paintOn(g);
		}
		
		drawIsometricGrid(g);	
	}
	
	public void drawIsometricGrid(Graphics g) {
		Point gridLeft, gridRight, gridBottom, gridTop;
		
		g.setColor(new Color(0, 0, 0));
		int x = World.ISOMETRIC_GRID_WIDTH;
		int y = World.ISOMETRIC_GRID_HEIGHT;
		for(int i=(x/2);i<getWidth()+x;i+=x) {
			for(int j=(y/2);j<getHeight()+y;j+=y) {
				gridLeft = new Point(i, j - (y/2));
				gridRight = new Point(i, j + (y/2));
				gridBottom = new Point(i - (x/2), j);
				gridTop = new Point(i + (x/2), j);
				g.drawLine(gridLeft.x, gridLeft.y, gridTop.x, gridTop.y);
				g.drawLine(gridTop.x, gridTop.y, gridRight.x, gridRight.y);
				g.drawLine(gridRight.x, gridRight.y, gridBottom.x, gridBottom.y);
				g.drawLine(gridBottom.x, gridBottom.y, gridLeft.x, gridLeft.y);
			}
		}
	}
	
	public void update() {
		this.repaint();
	}
}
