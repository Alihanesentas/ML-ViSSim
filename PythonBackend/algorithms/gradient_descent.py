import numpy as np
from algorithms.base_algorithm import BaseAlgorithm

# This is the "Traveler" class that knows *how* to walk.
class GradientDescent(BaseAlgorithm):
    
    def __init__(self, hyperparameters):
        super().__init__(hyperparameters)
        self.learning_rate = self.hyperparameters.get("learning_rate", 0.01)

    def step(self, model, data, current_w):
        # 1. Ask the "Terrain Expert" (model) for the slope
        gradient = model.calculate_gradient_at_point(data, current_w)
        
        # 2. Apply its own strategy (Gradient Descent step)
        new_w = current_w - self.learning_rate * gradient
        
        # 3. Ask the model for the cost at the new location
        new_cost = model.calculate_cost(data, new_w)
        
        return (new_w.tolist(), new_cost) # .tolist() makes it JSON-friendly