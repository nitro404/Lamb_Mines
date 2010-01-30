public class Main {
	
	public static void main(String[] args) {
		EditorWindow window = new EditorWindow();
		window.setVisible(true);
		
		int x = 16, y = 24;
		System.out.println("Initial: " + x + ", " + y);
		int a = (int) (x * Math.cos(0.46365));
		int b = (int) (y + (x * Math.sin(0.46365)));
		System.out.println("Isometric: " +  a +  ", " +  b);
		x = a;
		y = b;
//		int c = (int) (x * Math.cos(0.46365));
//		int d = (int) (- (y + (x * Math.sin(0.46365))));
		int c = (int) (x / Math.cos(0.46365));
		int d = (int) (y - (x / Math.sin(0.46365)));
		System.out.println("Reverse: " + c +  ", " + d);
	}
	
}
