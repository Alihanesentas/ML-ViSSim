import numpy as np
from algorithms.base_algorithm import BaseAlgorithm

# A different "Traveler" with a different strategy.
class BruteForce(BaseAlgorithm):
    
    def __init__(self, hyperparameters):
        super().__init__(hyperparameters)
        self.resolution = self.hyperparameters.get("resolution", 20)
        self.w0_range = np.linspace(-10, 10, self.resolution)
        self.w1_range = np.linspace(-10, 10, self.resolution)
        self.grid = np.array(np.meshgrid(self.w0_range, self.w1_range)).T.reshape(-1, 2)
        
        # BruteForce needs to know the *previous* step index
        # This state is managed by the algorithm instance.
        self.current_index = 0

    def step(self, model, data, current_w):
        
        # 1. Pick the *next* point on the grid, ignoring current_w
        if self.current_index >= len(self.grid):
            self.current_index = 0 # Loop back to start
            
        new_w = self.grid[self.current_index]
        
        # 2. Calculate its cost
        new_cost = model.calculate_cost(data, new_w)
        
        self.current_index += 1 # Move to the next index for the next call
        
        return (new_w.tolist(), new_cost)