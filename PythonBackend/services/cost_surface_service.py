import numpy as np

# This is the "Heavy Load" service.
# Its only job is to generate the 3D mesh data (vertices and triangles).
class CostSurfaceService:
    
    def __init__(self, resolution=20):
        # A 20x20 grid means 400 vertices. Keep this low for the MVP.
        self.resolution = resolution 

    def get_surface(self, model, data):
        """
        Generates the vertices and triangles for the 3D cost surface.
        This is the "Python Zeki" (Option B) approach.
        """
        print(f"Python: Generating {self.resolution}x{self.resolution} cost surface ...")
        
        vertices = []
        triangles = []

        # TODO:
        # 1. Create two 1D arrays (w0_range, w1_range) from -10 to +10
        #    using np.linspace(-10, 10, self.resolution).
        # 2. Use np.meshgrid to create 2D grids (w0_grid, w1_grid).
        # 3. Loop through every (w0, w1) pair in the grid:
        #    a. Calculate cost = model.calculate_cost(data, [w0, w1])
        #    b. Append {"x": w0, "y": cost, "z": w1} to the 'vertices' list.
        # 4. Run the *second* set of loops (like in the meshing prototype)
        #    to calculate the 'triangles' (index) list here in Python .
        
        # --- MOCKUP DATA (FOR MVP STEP 1) ---
        # This is the 4-point, 2-triangle mock data from our previous chat
        if not vertices:
             vertices = [
                {"x": 0, "y": 0, "z": 0}, {"x": 1, "y": 0, "z": 0},
                {"x": 0, "y": 0, "z": 1}, {"x": 1, "y": 1, "z": 1}
             ]
             triangles = [0, 2, 1,   1, 2, 3] # (Sent as CCW, will be flipped in C#)
        # --- END MOCKUP DATA ---
        
        return {"vertices": vertices, "triangles": triangles}