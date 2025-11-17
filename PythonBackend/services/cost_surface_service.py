import numpy as np

# "Heavy Load" service
class CostSurfaceService:
    
    def __init__(self, resolution=20):
        self.resolution = resolution 

    def get_surface(self, model, data):
        print(f"Python: Generating {self.resolution}x{self.resolution} cost surface [cite: 170-221]...")
        
        # 1. Create the (w0, w1) grid
        w0_range = np.linspace(-10, 10, self.resolution)
        w1_range = np.linspace(-10, 10, self.resolution)
        w0_grid, w1_grid = np.meshgrid(w0_range, w1_range)

        # 2. Calculate cost for every point on the grid
        # We use a vectorized operation for speed if the model supports it,
        # otherwise, we loop.
        # For our LinearRegression, we'll loop for clarity.
        
        vertices = []
        cost_grid = np.zeros(w0_grid.shape)
        
        for i in range(self.resolution):
            for j in range(self.resolution):
                w_pair = [w0_grid[i, j], w1_grid[i, j]]
                cost = model.calculate_cost(data, w_pair)
                cost_grid[i, j] = cost
                # Add (w0, cost, w1) to vertices
                vertices.append({"x": w_pair[0], "y": cost, "z": w_pair[1]})

        # 3. Calculate triangles (Python side)
        triangles = []
        for i in range(self.resolution - 1):
            for j in range(self.resolution - 1):
                # Get indices for the 4 corners of a quad
                vi = (i * self.resolution) + j
                
                # Triangle 1
                triangles.extend([vi, vi + self.resolution, vi + 1])
                # Triangle 2
                triangles.extend([vi + 1, vi + self.resolution, vi + self.resolution + 1])

        return {"vertices": vertices, "triangles": triangles}