import java.awt.*;
import java.awt.event.*;
import java.io.*;

import javax.swing.*;

public class EditorWindow extends JFrame implements ActionListener {
	
	private static final long serialVersionUID = 1L;
	
	final public static String MAP_DIRECTORY = "\\Maps"; 
	
	public World world;
	
	private JMenuBar menu;
	private JMenu menuFile;
	private JMenuItem menuFileNewMap;
	private JMenuItem menuFileOpenMap;
	private JMenuItem menuFileSaveMap;
	private JMenuItem menuFileExit;
	
	private EditorPanel editorPanel;
	JScrollPane editorPanelScrollPane;
	
	private JFileChooser fileChooser; 
	
	public EditorWindow() {
		super("2D World Editor");
		setSize(800, 600);
		setDefaultCloseOperation(EXIT_ON_CLOSE);
		
		createMenu();
		
		fileChooser = new JFileChooser();
		fileChooser.setCurrentDirectory(new File(System.getProperty("user.dir") + MAP_DIRECTORY));
		
		editorPanel = new EditorPanel();
		editorPanelScrollPane = new JScrollPane(editorPanel);
//		editorPanelScrollPane.setPreferredSize(new Dimension(editorPanel.getWidth(), editorPanel.getHeight()));
		editorPanelScrollPane.setPreferredSize(new Dimension(320, 240));
//		Rule columnView = new Rule(Rule.HORIZONTAL, true);
//        Rule rowView = new Rule(Rule.VERTICAL, true);
//        columnView.setPreferredWidth(320);
//        Rule rowView.setPreferredHeight(480);
//		editorPanelScrollPane.setColumnHeaderView(columnView);
//		editorPanelScrollPane.setRowHeaderView(rowView);
		add(editorPanel);
	}
	
	public void createMenu() {
		menu = new JMenuBar();
		
		menuFile = new JMenu("File");
		menuFileNewMap = new JMenuItem("New Map");
		menuFileOpenMap = new JMenuItem("Open Map");
		menuFileSaveMap = new JMenuItem("Save Map");
		menuFileExit = new JMenuItem("Exit");
		
		menuFileNewMap.addActionListener(this);
		menuFileOpenMap.addActionListener(this);
		menuFileSaveMap.addActionListener(this);
		menuFileExit.addActionListener(this);
		
		menuFile.add(menuFileNewMap);
		menuFile.add(menuFileOpenMap);
		menuFile.add(menuFileSaveMap);
		menuFile.addSeparator();
		menuFile.add(menuFileExit);
		
		menu.add(menuFile);
		
		setJMenuBar(menu);
	}
	
	public void actionPerformed(ActionEvent e) {
		if(e.getSource().equals(menuFileNewMap)) {
			editorPanel.setWorld(null);
		}
		else if(e.getSource().equals(menuFileOpenMap)) {
			if(fileChooser.showDialog(this, "Open Map") == JFileChooser.APPROVE_OPTION) {
				world = World.parseFrom(fileChooser.getSelectedFile().getAbsolutePath());
				editorPanel.setWorld(world);
			}
		}
		else if(e.getSource().equals(menuFileSaveMap)) {
			if(fileChooser.showDialog(this, "Open Map") == JFileChooser.APPROVE_OPTION) {
				world.writeTo(fileChooser.getSelectedFile().getAbsolutePath());
			}
		}
		else if(e.getSource().equals(menuFileExit)) {
			System.exit(0);
		}
		
		update();
	}
	
	public void update() {
		this.repaint();
		editorPanel.update();
	}
	
}
