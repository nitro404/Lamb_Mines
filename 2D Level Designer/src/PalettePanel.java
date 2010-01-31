import java.awt.*;
import java.awt.event.*;
import javax.swing.*;
import java.io.*;
import java.util.Vector;
import java.awt.image.BufferedImage;
import javax.imageio.ImageIO;

public class PalettePanel extends JPanel implements Scrollable, ActionListener, MouseListener, MouseMotionListener {
	
	private static final long serialVersionUID = 1L;
	
	private EditorWindow editorWindow;
	
	private BufferedImage image;
	
	public PalettePanel(EditorWindow editorWindow) {
		setLayout(null);
		addMouseListener(this);
		addMouseMotionListener(this);
		
		this.editorWindow = editorWindow;
		
		testPNG();
		
		update();
	}
	
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
	
	public Dimension getPreferredSize() {
		return new Dimension(240, 640);
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
			return visibleRect.width - 5;
		}
		else {
			return visibleRect.height - 5;
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
	public void mousePressed(MouseEvent e) { }
	
	public void mouseReleased(MouseEvent e) {
		if(e.getButton() == MouseEvent.BUTTON3) {
			
		}
		else if(e.getButton() == MouseEvent.BUTTON1) {
			
		}
		this.update();
	}
	
	public void mouseDragged(MouseEvent e) { }
	public void mouseMoved(MouseEvent e) { }
	
	public void actionPerformed(ActionEvent e) {
		
		this.update();
	}
		
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		
		g.clearRect(0, 0, this.getWidth(), this.getHeight());
		
		drawPNG(g);
	}
	
	public void update() {
		this.repaint();
	}
}
